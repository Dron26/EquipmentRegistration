using System;
using System.Collections.Generic;
using System.IO;
using CodeBase.Infrastracture.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.AdditionalPanels
{
    public class HistoryPanel : MonoBehaviour, IWindow
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _showData;
        [SerializeField] private Button _showLog;
        [SerializeField] private Button _showEmployees;
        [SerializeField] private Button _clear;
        [SerializeField] private Button _backButton;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private TMP_Text _infoText;
        [SerializeField] private TMP_Text _info;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _priviousButton;
        [SerializeField] private int _startLine;
        [SerializeField] private int _endLine;
        
        private string _selectedFile;
        public Action OnBackButtonCLick;
        private string _equipmentName = " - рохля №";
        private string textBase = "Текущее состояние ";
        private SaveLoadService _saveLoadService;
        private int _maxLength = 400;
        private List<string> _logs = new List<string>();

        public void Init(SaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
            AddListeners();
            _info.gameObject.SetActive(false);
            _startLine = 0;
            _endLine = _maxLength;
        }

        public void Reset()
        {
            _infoText.text = "";
            _showData.gameObject.SetActive(true);
            _showLog.gameObject.SetActive(true);
            _showData.interactable = true;
            _showLog.interactable = true;
            _startLine = 0;
            _endLine = _maxLength;
        }

        public void ShowEmployees()
        {
            Reset();

            _showData.interactable = true;
            _showLog.interactable = true;
            _showEmployees.interactable = true;

            SentLogMessage("-> Сотрудники");

            _infoText.text = "";

            _panel.SetActive(true);
            _info.gameObject.SetActive(true);
            _info.text = textBase;
            List<Employee> _employees = _saveLoadService.Database.GetEmployees();

            foreach (var employee in _employees)
            {
                string employeeInfo = "";

                if (employee.HaveBox)
                {
                    employeeInfo = employee.Login + "  ключ " + employee.Box.Key + "  " + " ТСД " +
                                   employee.Equipment.SerialNumber[^4..];
                }

                if (employee.HaveTrolley)
                {
                    if (employee.HaveBox)
                    {
                        employeeInfo += _equipmentName + employee.Trolley.Number;
                    }
                    else
                    {
                        employeeInfo = employee.Login + _equipmentName + employee.Trolley.Number;
                    }
                }

                _infoText.text += employeeInfo + "\n";
            }

            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }

        public string[] SubArray<T>(string data, int index, int length)
        {
            string[] result = new string[length];
            Array.Copy(data.ToCharArray(), index, result, 0, length);
            return result;
        }


        private void OnClickNextLog()
        {
            _startLine = _endLine;
            _endLine += _maxLength;

            if (_selectedFile != null)
            {
                ShowInfo(_selectedFile);
            }
        }

        private void OnClickPreviousLog()
        {
            if (_startLine == 0)
            {
                _endLine = _startLine + 1;
            }
            else
            {
                _endLine = _startLine;
            }

            _startLine -= _maxLength;

            if (_startLine < 0)
            {
                _startLine = 0;
            }

            if (_selectedFile != null)
            {
                ShowInfo(_selectedFile);
            }
        }

        public void ShowInfo(string dataFileName)
        {
            _selectedFile = dataFileName;
            SentLogMessage("->  " + dataFileName);
            _infoText.text = "";
            _panel.SetActive(true);
            _info.gameObject.SetActive(false);
            string filePath = Path.Combine(Application.persistentDataPath, dataFileName);

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                if (_endLine > lines.Length)
                {
                    _endLine = lines.Length;
                }

                if (_startLine >= _endLine)
                {
                    _startLine = _endLine - 1;
                }

                for (int i = _startLine; i < _endLine; i++)
                {
                    _infoText.text += lines[i] + "\n";
                }

                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            }
            else
            {
                Debug.LogWarning("Log file does not exist.");
            }
        }

        private void HideButtonNext()
        {
            _nextButton.interactable = false;
        }

        private void ShowButtonNext()
        {
            _nextButton.interactable = true;
        }

        public void SwithAdminState(bool state)
        {
            _panel.SetActive(state);
        }

        public void SwithManagerState(bool state)
        {
            _showData.gameObject.SetActive(false);
            _showLog.gameObject.SetActive(false);
            _panel.SetActive(state);
        }

        public void OnCLickBackButton()
        {
            SentLogMessage("<- Назад");
            Reset();
            SwithAdminState(false);
            _info.gameObject.SetActive(false);
            OnBackButtonCLick?.Invoke();
            _panel.SetActive(false);
        }

        private void SentLogMessage(string message)
        {
            _saveLoadService.SentLogInfo(message, "");
        }

        private void AddListeners()
        {
            _showData.onClick.AddListener(() => ShowInfo(Const.DataInfo));
            _showLog.onClick.AddListener(() => ShowInfo(Const.LogInfo));
            _showEmployees.onClick.AddListener(ShowEmployees);
            _clear.onClick.AddListener(Reset);
            _backButton.onClick.AddListener(OnCLickBackButton);
            _nextButton.onClick.AddListener(OnClickNextLog);
            _priviousButton.onClick.AddListener(OnClickPreviousLog);
        }

        private void RemuveListeners()
        {
            _showData.onClick.RemoveListener(() => ShowInfo(Const.DataInfo));
            _showLog.onClick.RemoveListener(() => ShowInfo(Const.LogInfo));
            _showEmployees.onClick.RemoveListener(ShowEmployees);
            _clear.onClick.RemoveListener(Reset);
            _backButton.onClick.RemoveListener(OnCLickBackButton);
            _nextButton.onClick.AddListener(OnClickNextLog);
            _priviousButton.onClick.AddListener(OnClickPreviousLog);
        }

        private void OnDisable()
        {
            RemuveListeners();
        }
    }
}