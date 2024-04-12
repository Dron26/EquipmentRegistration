using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Infrastracture.Datas;
using CodeBase.Infrastracture.EquipmentGroup;
using CodeBase.Infrastracture.TrolleyGroup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.AdditionalPanels
{
    public class EquipmentAddPanel : MonoBehaviour, IWindow
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private GameObject _equipmentPanel;
        [SerializeField] private GameObject _trolleyPanel;
        [SerializeField] private GameObject _equipmentList;
        [SerializeField] private GameObject _trolleyList;
        [SerializeField] private GameObject _viweport;
        [SerializeField] private GameObject _mainItem;
        [SerializeField] private GameObject _mainTrolleyItem;
        [SerializeField] private Button _equipmentAddButton;
        [SerializeField] private Button _trolleyAddButton;
        [SerializeField] private Button _applyEquipmentButton;
        [SerializeField] private Button _applyTrolleyButton;
        [SerializeField] private Button _resetEquipmentInputTextButton;
        [SerializeField] private Button _resetTrolleyInputTextButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _okButton;
        [SerializeField] private Button _nokButton;
        [SerializeField] private ScrollRect _scroll;
        [SerializeField] private TMP_InputField _inputEquipmentField;
        [SerializeField] private TMP_InputField _inputBoxField;
        [SerializeField] private TMP_InputField _inputTrolleyField;
        [SerializeField] private TMP_InputField _boxText;
        [SerializeField] private TMP_InputField _trolleyText;
        [SerializeField] private GameObject _trolleyTextPanel;
        [SerializeField] private GameObject _equipmentTextPanel;

        public Action OnBackButtonCLick;
        private SaveLoadService _saveLoadService;
        private List<Box> _boxes = new();
        private List<Box> _busyBoxes = new();
        private List<Trolley> _trolleys = new();
        private Employee _selectedEmployee;
        private GameObject _tempEquipmentPanel;
        private GameObject _tempTrolleyPanel;
        private WarningPanel _warningPanel;
        private bool _isReseted;
        private bool _isBoxInputed;
        private bool _isEquipmentInputed;
        private bool _isTrolleyInputed;
        private Dictionary<int, Equipment> _freeBoxes = new Dictionary<int, Equipment>();

        public delegate void ActionWithTextNumber(string textNumber);

        private List<int> _freeTrolleys = new();
        private List<Trolley> _busyTrolleys = new();

        public void Init(SaveLoadService saveLoadService, WarningPanel warningPanel)
        {
            _saveLoadService = saveLoadService;
            _warningPanel = warningPanel;
            AddListeners();
        }

        public void Work()
        {
            FillEquipmentList();
            FillTrolleyList();

            _equipmentPanel.SetActive(false);
            _trolleyPanel.SetActive(false);

            _okButton.gameObject.SetActive(false);
            _nokButton.gameObject.SetActive(false);
            _equipmentAddButton.gameObject.SetActive(true);
            _trolleyAddButton.gameObject.SetActive(true);
            _equipmentAddButton.interactable = true;
            _trolleyAddButton.interactable = true;
            _trolleyTextPanel.SetActive(false);
            _equipmentTextPanel.SetActive(false);
        }

        private void FillEquipmentList()
        {
            _tempEquipmentPanel = Instantiate(_equipmentList, _viweport.transform);
            _tempEquipmentPanel.SetActive(true);

            _boxes = _saveLoadService.GetBoxes();
            SortButtons();

            foreach (var info in _freeBoxes)
            {
                GameObject newBox = Instantiate(_mainItem, _tempEquipmentPanel.transform);
                TMP_Text textKey = newBox.GetComponent<ItemMain>().GetBox().GetComponentInChildren<TMP_Text>();
                TMP_Text textEquipment =
                    newBox.GetComponent<ItemMain>().GetEquipment().GetComponentInChildren<TMP_Text>();
                textKey.text = info.Key.ToString();
                textEquipment.text = info.Value.SerialNumber[^4..];
            }

            _tempEquipmentPanel.SetActive(false);
        }

        private void SortButtons()
        {
            foreach (Box box in _boxes)
            {
                if (!box.Busy)
                {
                    _freeBoxes.Add(Convert.ToInt32(box.Key), box.Equipment);
                }
                else
                {
                    _busyBoxes.Add(box);
                }
            }

            _freeBoxes = _freeBoxes.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        private void FillTrolleyList()
        {
            _tempTrolleyPanel = Instantiate(_trolleyList, _viweport.transform);
            _tempTrolleyPanel.SetActive(true);
            _scroll.content = _tempTrolleyPanel.GetComponent<RectTransform>();
            _trolleys = _saveLoadService.GetTrolleys();
            SortTrolleys();

            foreach (int number in _freeTrolleys)
            {
                GameObject newTrolley = Instantiate(_mainTrolleyItem, _tempTrolleyPanel.transform);
                TMP_Text textNumber = newTrolley.GetComponentInChildren<TMP_Text>();
                textNumber.text = number.ToString();
            }

            _tempTrolleyPanel.SetActive(false);
        }

        private void SortTrolleys()
        {
            foreach (Trolley trolley in _trolleys)
            {
                if (!trolley.Busy)
                {
                    _freeTrolleys.Add(Convert.ToInt32(trolley.Number));
                }
                else
                {
                    _busyTrolleys.Add(trolley);
                }
            }
        }

        public void Reset()
        {
            _isReseted = true;
            _inputEquipmentField.text = "";
            _inputBoxField.text = "";
            _inputTrolleyField.text = "";
            _applyEquipmentButton.interactable = false;
            _applyTrolleyButton.interactable = false;
            _freeBoxes.Clear();
            _freeTrolleys.Clear();
            _busyBoxes.Clear();
            _busyTrolleys.Clear();
            Destroy(_tempEquipmentPanel);
            Destroy(_tempTrolleyPanel);
            _isReseted = false;
        }

        private void OnCLickTrolleyAddButton()
        {
            SentLogMessage("->Добавить рохлю ", "");
            _equipmentAddButton.interactable = true;
            _trolleyAddButton.interactable = false;
            _trolleyTextPanel.SetActive(true);
            _equipmentTextPanel.SetActive(false);
            _scroll.content = _tempTrolleyPanel.GetComponent<RectTransform>();
            _tempTrolleyPanel.SetActive(true);
            _tempEquipmentPanel.SetActive(false);
            _trolleyPanel.SetActive(true);
            _equipmentPanel.SetActive(false);
            _inputTrolleyField.interactable = true;
            _inputTrolleyField.Select();
            _inputTrolleyField.ActivateInputField();
            _applyTrolleyButton.interactable = true;

            _okButton.gameObject.SetActive(false);
            _nokButton.gameObject.SetActive(false);
        }

        private void OnCLickEquipmentAddButton()
        {
            SentLogMessage("->Добавить оборудование ", "");
            _equipmentAddButton.interactable = false;
            _trolleyAddButton.interactable = true;
            _trolleyTextPanel.SetActive(false);
            _equipmentTextPanel.SetActive(true);
            _scroll.content = _tempEquipmentPanel.GetComponent<RectTransform>();
            _tempEquipmentPanel.SetActive(true);
            _tempTrolleyPanel.SetActive(false);
            _equipmentPanel.SetActive(true);
            _trolleyPanel.SetActive(false);
            _inputBoxField.Select();
            _inputBoxField.ActivateInputField();
            _applyEquipmentButton.interactable = true;
            _okButton.gameObject.SetActive(false);
            _nokButton.gameObject.SetActive(false);
        }

        public void ValidateEquipmentInput()
        {
            _isBoxInputed = IsBoxValidate();
            _isEquipmentInputed = IsEquipmentValidate();

            if (_isBoxInputed && _isEquipmentInputed)
            {
                _equipmentPanel.SetActive(false);
                AddedBox();
            }
            else
            {
                _nokButton.gameObject.SetActive(true);
            }
        }

        public bool IsBoxValidate()
        {
            string text = _inputBoxField.text;

            foreach (var box in _boxes)
            {
                if (text == box.Key)
                {
                    _warningPanel.ShowWindow(WindowNames.OnBoxAlreadyExist.ToString());
                    SentLogMessage("Ящик уже существует", "");
                    return false;
                }
            }


            if (IsAllDigits(text) && !text.Contains(" "))
            {
                SentLogMessage("Введен новый ящик:  " + _inputBoxField.text, "");
                _isBoxInputed = true;
                _inputEquipmentField.interactable = true;
                _inputEquipmentField.Select();
                _inputEquipmentField.ActivateInputField();

                _okButton.gameObject.SetActive(true);
                _nokButton.gameObject.SetActive(false);
            }
            else
            {
                return false;
            }


            return true;
        }

        private bool IsEquipmentValidate()
        {
            foreach (var box in _boxes)
            {
                if (_inputEquipmentField.text == box.Equipment.SerialNumber)
                {
                    _warningPanel.ShowWindow(WindowNames.OnEquipmentAlreadyExist.ToString());
                    SentLogMessage("Сканер уже существует", "");
                    return false;
                }
            }

            if (_inputEquipmentField.text.Length > 3)
            {
                SentLogMessage("Введен новый сканер:  " + _inputEquipmentField.text, "");
                _isEquipmentInputed = true;
                return true;
            }

            return false;
        }

        private void AddedBox()
        {
            _selectedEmployee = _saveLoadService.Employee;
            SentLogMessage("Выполнено добавление оборудования", "Ящик и сканер");

            string action = "Выполнил добавление оборудования" + " " + _inputBoxField.text + " " +
                            _inputEquipmentField.text;
            string Login = _selectedEmployee.Login;
            string Pass = _selectedEmployee.Password;
            string Key = "*";
            string ShortNumber = "*";
            string Time = DateTime.Now.ToString();

            SentData sentData = new SentData(action, Login, Pass, Key, ShortNumber, Time, "");
            SentDataMessage(sentData);

            _equipmentPanel.SetActive(false);

            Equipment equipment = new Equipment(_inputEquipmentField.text);
            Box box = new Box(_inputBoxField.text, equipment);

            _saveLoadService.SetBox(box);
            _saveLoadService.SaveDatabase();

            _inputBoxField.text = "";
            _inputEquipmentField.text = "";
            _okButton.gameObject.SetActive(true);
            _equipmentAddButton.interactable = true;
            Reset();
            Work();
        }

        public void ValidateTrolleyInput()
        {
            _isTrolleyInputed = IsTrolleyValidate();

            if (_isTrolleyInputed)
            {
                _trolleyPanel.SetActive(false);
                AddedTrolley();
            }
            else
            {
                _nokButton.gameObject.SetActive(true);
            }
        }

        bool IsAllDigits(string s) => s.All(char.IsDigit);

        public bool IsTrolleyValidate()
        {
            foreach (var trolley in _trolleys)
            {
                if (_inputTrolleyField.text == trolley.Number)
                {
                    _warningPanel.ShowWindow(WindowNames.OnTrolleyAlreadyExist.ToString());
                    SentLogMessage("Рохля уже существует", "");
                    return false;
                }
            }


            if (IsAllDigits(_inputTrolleyField.text))
            {
                SentLogMessage("Введена новая рохля:  " + _inputBoxField.text, "");
                _isTrolleyInputed = true;

                _okButton.gameObject.SetActive(false);
                _nokButton.gameObject.SetActive(false);
            }
            else
            {
                return false;
            }

            return true;
        }

        private void AddedTrolley()
        {
            _selectedEmployee = _saveLoadService.Employee;
            SentLogMessage("Выполнено добавление рохли ", "");

            string action = "Выполнил добавление рохли " + " " + _inputTrolleyField.text;
            string Login = _selectedEmployee.Login;
            string Pass = _selectedEmployee.Password;
            string Key = "*";
            string ShortNumber = "*";
            string Time = DateTime.Now.ToString();

            SentData sentData = new SentData(action, Login, Pass, Key, ShortNumber, Time, _inputTrolleyField.text);
            SentDataMessage(sentData);

            _trolleyPanel.SetActive(false);

            Trolley trolley = new Trolley(_inputTrolleyField.text);


            _saveLoadService.SetTrolley(trolley);
            _saveLoadService.SaveDatabase();

            _inputTrolleyField.text = "";
            _okButton.gameObject.SetActive(true);
        }

        private void OnApplyAddedEmployee()
        {
            _okButton.gameObject.SetActive(false);
            _nokButton.gameObject.SetActive(false);
            Reset();
            Work();
        }

        private void ResetEquipmentInput()
        {
            SentLogMessage("Выполнен сброс при вводе оборудования", "сброс ящик/cканер");
            Reset();
            Work();
        }

        private void ResetTrolleyInput()
        {
            SentLogMessage("Выполнен сброс рохли", "сброс рохли");
            Reset();
            Work();
        }

        public void SwithState(bool state)
        {
            _panel.gameObject.SetActive(state);

            if (state)
            {
                SentLogMessage("-> Добавить", "");
            }
        }


        private void SentLogMessage(string message, string comment)
        {
            _saveLoadService.SentLogInfo(message, comment);
        }

        private void SentDataMessage(SentData message)
        {
            _saveLoadService.SentDataInfo(message);
        }

        private void OnDisable()
        {
            RemuveListeners();
        }

        private void AddListeners()
        {
            _equipmentAddButton.onClick.AddListener(OnCLickEquipmentAddButton);
            _trolleyAddButton.onClick.AddListener(OnCLickTrolleyAddButton);
            _backButton.onClick.AddListener(OnCLickBackButton);
            _resetEquipmentInputTextButton.onClick.AddListener(ResetEquipmentInput);
            _resetTrolleyInputTextButton.onClick.AddListener(ResetTrolleyInput);
            _applyEquipmentButton.onClick.AddListener(ValidateEquipmentInput);
            _applyTrolleyButton.onClick.AddListener(ValidateTrolleyInput);
            _okButton.onClick.AddListener(OnApplyAddedEmployee);
            _nokButton.onClick.AddListener(OnApplyAddedEmployee);
        }


        private void RemuveListeners()
        {
            _equipmentAddButton.onClick.RemoveListener(OnCLickEquipmentAddButton);
            _trolleyAddButton.onClick.RemoveListener(OnCLickTrolleyAddButton);
            _backButton.onClick.RemoveListener(OnCLickBackButton);
            _resetEquipmentInputTextButton.onClick.RemoveListener(ResetEquipmentInput);
            _resetTrolleyInputTextButton.onClick.RemoveListener(ResetTrolleyInput);
            _applyEquipmentButton.onClick.RemoveListener(ValidateEquipmentInput);
            _okButton.onClick.RemoveListener(OnApplyAddedEmployee);
            _nokButton.onClick.RemoveListener(OnApplyAddedEmployee);
            _applyTrolleyButton.onClick.RemoveListener(ValidateTrolleyInput);
        }


        public void OnCLickBackButton()
        {
            SentLogMessage("<- Назад ", "");
            SwithState(false);
            OnBackButtonCLick.Invoke();
        }
    }
}