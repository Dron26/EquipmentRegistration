using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeBase.Infrastracture.Datas;
using CodeBase.Infrastracture.EquipmentGroup;
using CodeBase.Infrastracture.TrolleyGroup;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.AdditionalPanels
{
    public class AdminPanel : MonoBehaviour, IWindow
    {
        
        [SerializeField] private CVSLoader _cvsLoader;
        [SerializeField] private GoogleSheetLoader _googleSheetLoader;
        [SerializeField] private Button _apply;
        [SerializeField] private GameObject _panelOk;
        [SerializeField] private GameObject _panelNok;

        [SerializeField] private Button _swithPlatform;
        
        [SerializeField] private TMP_InputField _inputLoggPass;
        [SerializeField] private TMP_InputField _inputBoxes;
        [SerializeField] private TMP_InputField _inputTrolleys;
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _backButton;

        [SerializeField] private Toggle _toggleEquipment;
        [SerializeField] private Toggle _toggleTrolleys;
        
        [SerializeField] private TMP_InputField _inputUrlId;
        [SerializeField] private Button _setUrlId;
        [SerializeField] private Button _setWebData;
        public Action<List<Employee>, List<Box>, List<Trolley>> Loaded;
        public Action OnBackButtonCLick;
        public bool isLoadedEmployees = false;
        public bool isLoadedEquipment = false;
        public bool isLoadedTrolleys = false;
        public bool _isSwithPlatform = false;
        private bool _isInternetConnection;
        private SaveLoadService _saveLoadService;
        private List<Employee> _employees = new();
        private List<Equipment> _equipments = new();
        private List<Box> _boxes = new();
        private List<Trolley> _trolleys = new();
        private string[] _linesInputLoggPass;
        private string[] _linesInputBoxes;
        private string[] _linesInputTrolleys;
        private List<string> _textLoggPass= new();
        private List<string> _textBoxes= new();
        private List<string> _textTrolleys= new();
        List<List<string>> textInFields = new();
        
        
        private void OnCLickSwithPlatform()
        {
            _isSwithPlatform = !_isSwithPlatform;
        }
        private void SetUrlId()
        {
            SentLogMessage(" ->Установка   UrlId ");
            
            if (_inputUrlId.text!="")
            {
                _cvsLoader.SetUrlId(_inputUrlId.text);
            }
        }
        
        private void LoadWebData()
        {
            SentLogMessage(" ->Загрузка WebData ");
            _googleSheetLoader.StartDownload();
        }

        private void OnSheetDownloaded(WebData webData)
        {
            Loaded?.Invoke(webData.Employees, webData.Boxes, webData.Trolleys);
            
            
            SentLogMessage(" Успешная Загрузка WebData ");
            SussesAreLoaded();
        }

        public void LoadData()
        {
            _apply.interactable = false;
            SentLogMessage(" ->Сохранить Базу");

            if (_inputBoxes.text != "" && _inputLoggPass.text != "" && _inputTrolleys.text != "")
            {
                _panelOk.gameObject.SetActive(false);
                _panelNok.gameObject.SetActive(false);
                
                if (_isSwithPlatform )
                {
                    SetTextFromInput();
                }
                else 
                {
                   SetTextFromPath();
                }
                
                try
                {
                    foreach (string line in textInFields[0])
                    {
                        string[] values = line.Split('-');

                        if (values.Length >= 3)
                        {
                            Employee employee = new Employee(values[0], values[1], values[2]);
                            _employees.Add(employee);
                        }
                    }

                    SentLogMessage("Данные по сотрудникам загружены");
                    isLoadedEmployees = true;
                }
                catch (Exception ex)
                {
                    _panelNok.gameObject.SetActive(true);
                    SentLogMessage("Ошибка загрузки данных по сотрудникам : " + ex.Message);
                    isLoadedEmployees = false;
                }

                try
                {
                    foreach (string line in textInFields[1])
                    {
                        string[] values = line.Split('-');

                        if (values.Length >= 2)
                        {
                            Equipment _equipment = new Equipment(values[0].Trim());
                            _equipments.Add(_equipment);
                            Box box = new Box(values[1], _equipment);
                            _boxes.Add(box);
                        }
                    }

                    SentLogMessage("Данные о оборудовании загружены");
                    isLoadedEquipment = true;
                }
                catch (Exception ex)
                {
                    SentLogMessage("Ошибка загрузки данных о оборудовании: " + ex.Message);
                    _panelNok.gameObject.SetActive(true);
                    isLoadedEquipment = false;
                }

                try
                {
                    foreach (string line in textInFields[2])
                    {
                        if (line.Length > 0)
                        {
                            Trolley trolley = new Trolley(line);
                            _trolleys.Add(trolley);
                        }
                    }

                    SentLogMessage("Данные о рохлях загружены");
                    isLoadedTrolleys = true;
                }
                catch (Exception ex)
                {
                    SentLogMessage("Ошибка загрузки данных о рохлях: " + ex.Message);
                    _panelNok.gameObject.SetActive(true);
                    isLoadedTrolleys = false;
                }

                if (isLoadedEmployees && isLoadedEquipment && isLoadedTrolleys)
                {
                    SussesAreLoaded();
                    Loaded?.Invoke(_employees, _boxes, _trolleys);
                }
            }
            else
            {
                if (_isSwithPlatform == false)
                {
                    SetPath();

                    SetTextFromPath();
                }
            }
        }
        
        private void SussesAreLoaded()
        {
            _inputBoxes.text = "";
            _inputLoggPass.text = "";
            _inputTrolleys.text = "";
            _inputBoxes.interactable = false;
            _inputLoggPass.interactable = false;
            _inputTrolleys.interactable = false;
            _panelNok.gameObject.SetActive(false);
            _panelOk.gameObject.SetActive(true);
        }
        
        private void SetTextFromPath()
        {
            _textLoggPass.AddRange(File.ReadAllLines(_inputLoggPass.text));
            _textBoxes.AddRange(File.ReadAllLines(_inputBoxes.text));
            _textTrolleys.AddRange(File.ReadAllLines(_inputTrolleys.text));
                           
            textInFields.Clear();
            textInFields.Add(_textLoggPass);
            textInFields.Add(_textBoxes);
            textInFields.Add(_textTrolleys);
        }

        private void SetTextFromInput()
        {
            int countInputField = 3;
            List<TMP_InputField> inputFields=new();
            inputFields.Add(_inputLoggPass);
            inputFields.Add(_inputBoxes);
            inputFields.Add(_inputTrolleys);
                    
            for (int i = 0; i < inputFields.Count; i++)
            {
                int index = 0;
                string line = "";

                if (inputFields[i].text!="")
                {
                    foreach (char simbol in inputFields[i].text)
                    {
                        if (simbol!='\r'&&simbol!='\n')
                        {
                            if (simbol!='%')
                            {
                                line += simbol;
                            }
                            else
                            {
                                textInFields[i].Add(line);
                                index++;
                                line = "";
                            }
                        }
                    }
                }
            }
        }

        private void SetPath()
        {
            if (_inputBoxes.text == "")
            {
                _inputBoxes.text = Path.Combine(Application.persistentDataPath, Const.BoxPath);
            }

            if (_inputLoggPass.text == "")
            {
                _inputLoggPass.text = Path.Combine(Application.persistentDataPath, Const.EmployeesPath);
            }

            if (_inputTrolleys.text == "")
            {
                _inputTrolleys.text = Path.Combine(Application.persistentDataPath, Const.TrolleysPath);
            }
        }
        
        
        public void Init(SaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
            AddListeners();
        }

        public void Reset()
        {
            isLoadedEmployees = false;
            isLoadedEquipment = false;
            _panelOk.gameObject.SetActive(false);
            _panelNok.gameObject.SetActive(false);
            _apply.interactable = true;
         _employees = new();
         _equipments = new();
         _boxes = new();
         _trolleys = new();
         _linesInputLoggPass=null;
         _linesInputBoxes=null;
         _linesInputTrolleys=null;
         _textLoggPass= new();
         _textBoxes= new();
         _textTrolleys= new();
         textInFields = new();
        }

        private void AddListeners()
        {
            _apply.onClick.AddListener(LoadData);
            _backButton.onClick.AddListener(OnCLickBackButton);
            _toggleEquipment.onValueChanged.AddListener(SetEquipmentStatus);
            _toggleTrolleys.onValueChanged.AddListener(SetTrolleyStatus);
            _swithPlatform.onClick.AddListener(OnCLickSwithPlatform);
            _setUrlId.onClick.AddListener(SetUrlId);
            _setWebData.onClick.AddListener(LoadWebData);
            _googleSheetLoader.OnProcessData += OnSheetDownloaded;
        }
        
        private void SetEquipmentStatus(bool isSelected)
        {
            _toggleTrolleys.isOn=!isSelected;
                SentLogMessage("-> Выбрана работа с оборудованием ");
            _saveLoadService.SetSwithEquipmentState(isSelected);
        }

        private void SetTrolleyStatus(bool isSelected)
        {
            _toggleEquipment.isOn = !isSelected;
            SentLogMessage("-> Выбрана работа с рохлями ");
            _saveLoadService.SetSwithTrolleyState(isSelected);
        }

        private void RemuveListeners()
        {
            _apply.onClick.RemoveListener(LoadData);
            _backButton.onClick.RemoveListener(OnCLickBackButton);
            _toggleEquipment.onValueChanged.RemoveListener(SetEquipmentStatus);
            _toggleTrolleys.onValueChanged.RemoveListener(SetTrolleyStatus);
            _swithPlatform.onClick.RemoveListener(OnCLickSwithPlatform);
            _setUrlId.onClick.RemoveListener(SetUrlId);
            _setWebData.onClick.RemoveListener(LoadWebData);
            _googleSheetLoader.OnProcessData -= OnSheetDownloaded;
        }

        private void SentLogMessage(string message)
        {
            _saveLoadService.SentLogInfo(message, "");
        }

        private void OnDisable()
        {
            RemuveListeners();
        }

        public void SwithState(bool state)
        {
            _panel.SetActive(state);
            
            if (state)
            {
                _isInternetConnection=_saveLoadService.IsDatabaseLoaded;
                textInFields.Add(_textLoggPass);
                textInFields.Add(_textBoxes);
                textInFields.Add(_textTrolleys);
            }   
        }

        public void OnCLickBackButton()
        {
            SentLogMessage("<- Назад");
            _apply.interactable = false;
            SwithState(false);
            isLoadedEmployees = false;
            isLoadedEquipment = false;
            OnBackButtonCLick?.Invoke();
            _inputLoggPass.text = "";
            _inputBoxes.text = "";
            _inputTrolleys.text = "";
        }
    }
}