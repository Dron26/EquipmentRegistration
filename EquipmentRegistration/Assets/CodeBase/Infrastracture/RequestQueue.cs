using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeBase.Infrastracture.Datas;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace CodeBase.Infrastracture
{
    public class RequestQueue : MonoBehaviour
    {
        private SaveLoadService _saveLoadService;
        private List<SentData> _offlineQueue = new List<SentData>();
        private const int MAX_QUEUE_SIZE = 500;
        private List<SentData> _offlineQueueLog = new List<SentData>();
        private List<SentData> _offlineQueueData = new List<SentData>();
        private List<SentData> _offlineQueueTrolley = new List<SentData>();
        private const int MAX_QUEUE_SIZE_LOG = 500;
        private const int MAX_QUEUE_SIZE_DATA_TROLLEY = 500;
        private const int MAX_QUEUE_SIZE_DATA = 500;
        private const int MAX_QUEUE_SIZE_TROLLEY = 500;
        private string _filePathOfflineData = "path_to_your_data_file.json";
        private string _filePathOfflineLog = "path_to_your_file.json";
        private string _filePathOfflineTrolley = "path_to_your_trolley_file.json";

        private Queue<SentData> _queueData = new Queue<SentData>();
        private Queue<SentData> _queueTrolleyData = new Queue<SentData>();
        private Queue<SentData> _queueLog = new Queue<SentData>();
        private bool _isProcessingData = false;
        private bool _isProcessingTrolleyData = false;
        private bool _isProcessingLog = false;
        private bool _isQueueProcessed = false;
        private bool isCheckingInternet = false;
        private bool _isEquipmentSelected = false;
        private bool _isTrolleySelected = false;
        private bool isSended = false;

        private string BASE_URL;
        private string LOG_URL;

        private string EquipmentBaseUrl =
            "https://docs.google.com/forms/u/0/d/e/1FAIpQLScENY48wHtzHZwrfs5EldSC3tZTcg0B-BNXBmfO31XXpziM4w/formResponse";

        private string EquipmentLogUrl =
            "https://docs.google.com/forms/u/0/d/e/1FAIpQLSe3Cc86niLM2J0vhsw-4NQd8AnD4QcZLcCRehpgEAYPS1iibw/formResponse";

        private string TrollyBaseUrl =
            "https://docs.google.com/forms/u/0/d/e/1FAIpQLSd6TEzgv5598bWKc0-wuXFtTNHdjqebwPf3t6EaZyNPeh6PyQ/formResponse";

        private string Trolly_URL =
            "https://docs.google.com/forms/u/0/d/e/1FAIpQLSc9S0Bpj6hXYWS_TKl4-IU2J8LBGdw6CjkXKBsaqtGcGZc7BQ/formResponse";

        private string TrollyLogUrl =
            "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfhCOEMphSxAcFgvpVLifdoxS70z4ziCtzBXwdXnG9yOSmuIg/formResponse";


        private void SetURL()
        {
            _isEquipmentSelected = false;
            _isTrolleySelected = false;

            if (_saveLoadService.SwithStatus.IsEquipmentSelected)
            {
                _isEquipmentSelected = true;
                BASE_URL = EquipmentBaseUrl;
                LOG_URL = EquipmentLogUrl;
            }
            else
            {
                _isTrolleySelected = true;
                BASE_URL = TrollyBaseUrl;
                LOG_URL = TrollyLogUrl;
            }
        }


        public void Init(SaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
            SetURL();
            _filePathOfflineData = Path.Combine(Application.persistentDataPath, _filePathOfflineData);
            _filePathOfflineLog = Path.Combine(Application.persistentDataPath, _filePathOfflineLog);
            _filePathOfflineTrolley = Path.Combine(Application.persistentDataPath, _filePathOfflineTrolley);
            AddListeners();

            if (File.Exists(_filePathOfflineLog))
            {
                string json = File.ReadAllText(_filePathOfflineLog);
                Debug.Log("JSON content: " + json);

                _offlineQueueLog = JsonConvert.DeserializeObject<List<SentData>>(json);
            }

            if (File.Exists(_filePathOfflineData))
            {
                string json = File.ReadAllText(_filePathOfflineData);
                Debug.Log("JSON content for Data: " + json);

                _offlineQueueData = JsonConvert.DeserializeObject<List<SentData>>(json);
            }

            if (File.Exists(_filePathOfflineTrolley))
            {
                string json = File.ReadAllText(_filePathOfflineTrolley);
                Debug.Log("JSON content for Trolley: " + json);


                _offlineQueueTrolley = JsonConvert.DeserializeObject<List<SentData>>(json);
            }

            StartCoroutine(CheckInternetConnection());
        }

        private IEnumerator CheckInternetConnection()
        {
            WaitForSeconds wait = new WaitForSeconds(2f);

            while (true)
            {
                yield return wait;

                bool internetAvailable = InternetIsAvailable();

                if (internetAvailable)
                {
                    OnInternetAvailable();
                }
            }
        }


        private void OnInternetAvailable()
        {
            StartCoroutine(ProcessDataQueue());
            StartCoroutine(ProcessLogQueue());
            StartCoroutine(ProcessTrolleyDataQueue());
        }


        public void EnqueueData(SentData data)
        {
            try
            {
                if (!InternetIsAvailable())
                {
                    if (_offlineQueueData.Count < MAX_QUEUE_SIZE_DATA)
                    {
                        _offlineQueueData.Add(data);
                        SaveOfflineQueueToJSONData(_offlineQueueData, _filePathOfflineData, false);
                    }
                    else
                    {
                        SaveOfflineQueueToJSONData(_offlineQueueData, _filePathOfflineData, false);
                        _offlineQueueData.Clear();
                    }
                }
                else
                {
                    _queueData.Enqueue(data);

                    if (!_isProcessingData)
                    {
                        StartCoroutine(ProcessDataQueue());
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public void EnqueueLog(SentData data)
        {
            try
            {
                if (!InternetIsAvailable())
                {
                    if (_offlineQueueLog.Count < MAX_QUEUE_SIZE_LOG)
                    {
                        _offlineQueueLog.Add(data);
                        SaveOfflineQueueToJSONData(_offlineQueueLog, _filePathOfflineLog, false);
                    }
                    else
                    {
                        SaveOfflineQueueToJSONData(_offlineQueueLog, _filePathOfflineLog, false);
                        _offlineQueueLog.Clear();
                    }
                }
                else
                {
                    _queueLog.Enqueue(data);

                    if (!_isProcessingLog)
                    {
                        StartCoroutine(ProcessLogQueue());
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public void EnqueueTrolleyData(SentData data)
        {
            try
            {
                if (!InternetIsAvailable())
                {
                    if (_offlineQueueTrolley.Count < MAX_QUEUE_SIZE_TROLLEY)
                    {
                        _offlineQueueTrolley.Add(data);
                        SaveOfflineQueueToJSONData(_offlineQueueTrolley, _filePathOfflineTrolley, false);
                    }
                    else
                    {
                        SaveOfflineQueueToJSONData(_offlineQueueTrolley, _filePathOfflineTrolley, false);
                        _offlineQueueTrolley.Clear();
                    }
                }
                else
                {
                    _queueTrolleyData.Enqueue(data);

                    if (!_isProcessingTrolleyData)
                    {
                        StartCoroutine(ProcessTrolleyDataQueue());
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }


        private IEnumerator ProcessDataQueue()
        {
            _isProcessingData = true;

            if (_offlineQueueData.Count > 0)
            {
                SaveOfflineQueueToJSONData(_offlineQueueData, _filePathOfflineData, false);
                yield return StartCoroutine(ProcessOfflineQueueLocally(_offlineQueueData, _filePathOfflineData));
            }

            while (_queueData.Count > 0 && _offlineQueueData.Count == 0)
            {
                SentData data = _queueData.Dequeue();
                yield return StartCoroutine(PostData(data));

                yield return new WaitForSeconds(1f);
            }

            _isProcessingData = false;
        }

        private IEnumerator ProcessLogQueue()
        {
            _isProcessingLog = true;

            if (_offlineQueueLog.Count > 0)
            {
                SaveOfflineQueueToJSONData(_offlineQueueLog, _filePathOfflineLog, false);
                yield return StartCoroutine(ProcessOfflineQueueLocally(_offlineQueueLog, _filePathOfflineLog));
            }

            while (_queueLog.Count > 0 && _offlineQueueLog.Count == 0)
            {
                SentData data = _queueLog.Dequeue();
                yield return StartCoroutine(PostLog(data));

                yield return new WaitForSeconds(1f);
            }

            _isProcessingLog = false;
        }


        private IEnumerator ProcessTrolleyDataQueue()
        {
            _isProcessingTrolleyData = true;

            if (_offlineQueueTrolley.Count > 0)
            {
                SaveOfflineQueueToJSONData(_offlineQueueTrolley, _filePathOfflineTrolley, false);
                yield return StartCoroutine(ProcessOfflineQueueLocally(_offlineQueueTrolley, _filePathOfflineTrolley));
            }

            while (_queueTrolleyData.Count > 0 && _offlineQueueTrolley.Count == 0)
            {
                SentData data = _queueTrolleyData.Dequeue();
                yield return StartCoroutine(PostDataTrolley(data));

                yield return new WaitForSeconds(1f);
            }

            _isProcessingTrolleyData = false;
        }


        private IEnumerator<RequestResult> PostData(SentData data)
        {
            string action = data.Action;
            string login = data.Login;
            string pass = data.Pass;
            string key = data.Key;
            string shortNumber = data.ShortNumber;
            string trolley = "";
            string additionalFirst = "";
            string additionalSecond = "";
            string initiatorLogin = "";
            string comment = data.Comment;
            string time = GetCurrentTime();

            using (UnityWebRequest webRequest = UnityWebRequest.Post(BASE_URL, new WWWForm()))
            {
                WWWForm form = new WWWForm();

                if (_isEquipmentSelected)
                {
                    form.AddField("entry.671585176", time);
                    form.AddField("entry.1357908308", action);
                    form.AddField("entry.1332130277", login);
                    form.AddField("entry.441880881", pass);
                    form.AddField("entry.600451713", key);
                    form.AddField("entry.1893219231", shortNumber);
                    form.AddField("entry.1086723980", trolley);
                    form.AddField("entry.90345619", additionalFirst);
                    form.AddField("entry.1279552655", additionalSecond);
                    form.AddField("entry.405360371", initiatorLogin);
                    form.AddField("entry.1208956316", comment);
                }
                else
                {
                    form.AddField("entry.2093698949", time);
                    form.AddField("entry.910602564", action);
                    form.AddField("entry.145576940", login);
                    form.AddField("entry.1592464882", pass);
                    form.AddField("entry.923460414", trolley);
                    form.AddField("entry.456702159", additionalFirst);
                    form.AddField("entry.480217626", additionalSecond);
                    form.AddField("entry.1434778015", initiatorLogin);
                    form.AddField("entry.2124610392", comment);
                }


                webRequest.uploadHandler = new UploadHandlerRaw(form.data);
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                var asyncOperation = webRequest.SendWebRequest();

                while (!asyncOperation.isDone)
                {
                    yield return new RequestResult();
                }

                RequestResult result =
                    new RequestResult(webRequest.result == UnityWebRequest.Result.Success, webRequest);

                yield return result;
            }
        }


        IEnumerator<RequestResult> PostLog(SentData data)
        {
            string action = data.Action;
            string additionalFirst = "";
            string additionalSecond = "";
            string time = GetCurrentTime();

            using (UnityWebRequest webRequest = UnityWebRequest.Post(LOG_URL, new WWWForm()))
            {
                WWWForm form = new WWWForm();
                if (_isEquipmentSelected)
                {
                    form.AddField("entry.83066782", time);
                    form.AddField("entry.1718735305", action);
                    form.AddField("entry.1998124105", additionalFirst);
                    form.AddField("entry.1669892790", additionalSecond);
                }
                else
                {
                    form.AddField("entry.536923513", time);
                    form.AddField("entry.266401696", action);
                    form.AddField("entry.1899267090", additionalFirst);
                    form.AddField("entry.280856764", additionalSecond);
                }


                webRequest.uploadHandler = new UploadHandlerRaw(form.data);
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                var asyncOperation = webRequest.SendWebRequest();

                while (!asyncOperation.isDone)
                {
                    yield return new RequestResult();
                }

                RequestResult result =
                    new RequestResult(webRequest.result == UnityWebRequest.Result.Success, webRequest);

                yield return result;
            }
        }

        IEnumerator<RequestResult> PostDataTrolley(SentData data)
        {
            string action = data.Action;
            string login = data.Login;
            string trolleyNumber = data.TrolleyNumber;
            string additionalFirst = "";
            string additionalSecond = "";
            string initiatorLogin = "";
            string comment = "";
            string time = GetCurrentTime();

            using (UnityWebRequest webRequest = UnityWebRequest.Post(Trolly_URL, new WWWForm()))
            {
                WWWForm form = new WWWForm();

                form.AddField("entry.1738994279", time);
                form.AddField("entry.1600380077", action);
                form.AddField("entry.211483959", login);
                form.AddField("entry.643116357", trolleyNumber);
                form.AddField("entry.1655139404", comment);

                webRequest.uploadHandler = new UploadHandlerRaw(form.data);
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                var asyncOperation = webRequest.SendWebRequest();
                while (!asyncOperation.isDone)
                {
                    yield return new RequestResult();
                }

                RequestResult result =
                    new RequestResult(webRequest.result == UnityWebRequest.Result.Success, webRequest);

                yield return result;
            }
        }

        private void SaveOfflineQueueToJSONData(List<SentData> offlineQueue, string filePath, bool isRemove)
        {
            try
            {
                string json = JsonConvert.SerializeObject(offlineQueue);
                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving offline data to JSON: {e}");
            }
        }


        private void LoadOfflineQueueLogFromJSON()
        {
            string filePath = Path.Combine(Application.persistentDataPath, _filePathOfflineLog);

            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    List<SentData> offlineQueueLog = JsonConvert.DeserializeObject<List<SentData>>(json);

                    _offlineQueueLog.AddRange(offlineQueueLog);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load offline queue log from JSON. Error: {e.Message}");
                }
            }
        }

        private IEnumerator ProcessOfflineQueueLocally(List<SentData> offlineQueue, string filePath)
        {
            Func<SentData, IEnumerator<RequestResult>> action = null;
            SentData data;

            if (filePath == _filePathOfflineData)
            {
                action = PostData;
            }
            else if (filePath == _filePathOfflineTrolley)
            {
                action = PostDataTrolley;
            }
            else
            {
                action = PostLog;
            }

            while (offlineQueue.Count > 0)
            {
                data = offlineQueue[0];

                if (InternetIsAvailable() && isSended == false)
                {
                    IEnumerator<RequestResult> request = action(data);
                    isSended = true;
                    yield return StartCoroutine(ProcessRequestResult(request, offlineQueue, filePath));
                }
                else
                {
                    yield break;
                }
            }
        }

        private IEnumerator ProcessRequestResult(IEnumerator<RequestResult> request, List<SentData> offlineQueue,
            string filePath)
        {
            while (request.MoveNext())
            {
                yield return request.Current;

                if (request.Current.Success)
                {
                    bool isRemove = true;
                    offlineQueue.RemoveAt(0);
                    isSended = false;
                    SaveOfflineQueueToJSONData(offlineQueue, filePath, isRemove);
                }
            }
        }

        public struct RequestResult
        {
            public bool Success;
            public UnityWebRequest WebRequest;

            public RequestResult(bool success, UnityWebRequest webRequest)
            {
                Success = success;
                WebRequest = webRequest;
            }
        }

        private bool InternetIsAvailable()
        {
            NetworkReachability reachability = Application.internetReachability;

            switch (reachability)
            {
                case NetworkReachability.NotReachable:
                    return false;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    return true;
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    return true;
                default:
                    return false;
            }
        }

        private string GetCurrentTime()
        {
            DateTime now = DateTime.Now;
            return now.ToString("dd.MM.yyyy") + " " + now.ToString("hh:mm:ss");
        }

        private void OnApplicationQuit()
        {
            ClearEmptyJsonFile(_filePathOfflineLog);
            ClearEmptyJsonFile(_filePathOfflineData);
            ClearEmptyJsonFile(_filePathOfflineTrolley);
        }

        private void ClearEmptyJsonFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);

                if (json.All(c => c == '{' || c == '}'))
                {
                    File.WriteAllText(filePath, "");
                }
            }
        }

        private void AddListeners()
        {
            _saveLoadService.OnSelectEquipment += SetURL;
            _saveLoadService.OnSelectTrolley += SetURL;
        }


        private void RemuveListeners()
        {
            _saveLoadService.OnSelectEquipment -= SetURL;
            _saveLoadService.OnSelectTrolley -= SetURL;
        }
    }
}