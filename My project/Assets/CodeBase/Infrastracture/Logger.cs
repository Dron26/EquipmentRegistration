using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeBase.Infrastracture.Datas;
using UnityEngine;

namespace CodeBase.Infrastracture
{
    public class Logger : MonoBehaviour
    {
        [SerializeField] private RequestQueue _requestQueue;
        
        private Queue<SentData> _queue = new Queue<SentData>();
        private Uri uri = null;
        private int _maxCountDay = 8;
        private bool _isAdditionalLogOn = false;
        private bool _isProcessing = false;

        public void Init(SaveLoadService saveLoadService)
        {
            _requestQueue.Init(saveLoadService);
            DontDestroyOnLoad(_requestQueue);
            DontDestroyOnLoad(this);
        }

        public void WriteData(SentData data)
        {
            DateTime Time = DateTime.Now;
            string info =
                $"Time: {Time}, {data.Action},Login: {data.Login}, Password: {data.Pass}, Tsd: {data.ShortNumber}, Key: {data.Key} ";

            AppendDataToFile(Const.DataInfo, info);
        }

        public void WriteLog(SentData data)
        {
            DateTime currentTime = DateTime.Now;
            string info = $"{currentTime} :  {data.Action}";

            AppendDataToFile(Const.LogInfo, info);
        }

        public void SendLog(SentData log)
        {
            WriteLog(log);
            _requestQueue.EnqueueLog(log);
        }

        public void SendData(SentData data)
        {
            WriteData(data);
            _requestQueue.EnqueueData(data);
        }

        public void SendTrolleyData(SentData data)
        {
            
             _requestQueue.EnqueueTrolleyData(data);
        }
        public void CheckTime()
        {
            List<string> consts = new List<string>();

            consts.Add(Const.DataInfo);
            consts.Add(Const.LogInfo);

            for (int i = 0; i < consts.Count; i++)
            {
                string filename = consts[i].ToString();
                string path = Path.Combine(Application.persistentDataPath, filename);

                if (File.Exists(path))
                {
                    try
                    {
                        string[] pempText = File.ReadAllLines(path);
                        string text = "";
                        DateTime date;

                        if (pempText.Length != 0)
                        {
                            text = File.GetCreationTime(path).ToString("dd.MM.yyyy");
                        }

                        if (DateTime.TryParse(text, out date))
                        {
                            DateTime thresholdDate = date.AddDays(_maxCountDay);

                            if (thresholdDate <= DateTime.Now)
                            {
                                string newPath = Path.Combine(Application.persistentDataPath, date.ToString("dd.MM.yyyy"), filename);
                                File.Copy(path, newPath);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                if (!File.Exists(path))
                {
                    string text = DateTime.Now.Date.ToString("dd.MM.yyyy");

                    using (StreamWriter writer = new StreamWriter(path, false))
                    {
                        writer.WriteLineAsync(text);
                    }
                }
            }
        }

        public void AppendDataToFile(string filename, string content)
        {
            string path = Path.Combine(Application.persistentDataPath, filename);

            try
            {
                using (StreamWriter
                       writer = new StreamWriter(path, true)) // Второй параметр true для дописывания в файл
                {
                    writer.WriteLine(content); // Дописываем данные в новую строку
                }

            }
            catch (Exception e)
            {
            }
        }
    }
}