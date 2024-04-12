using System;
using System.Collections.Generic;
using CodeBase.Infrastracture;
using CodeBase.Infrastracture.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.AdditionalPanels
{
    public class ResetPanel : MonoBehaviour, IWindow
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _resetEquipmentButton;
        [SerializeField] private Button _resetEmployeePassButton;
        [SerializeField] private Button _resetTrolleyButton;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _applyEquimpmentButton;
        [SerializeField] private Button _applyTrolleyButton;
        [SerializeField] private Button _resetInputPassTextButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _okPanel;
        [SerializeField] private Button _nokPanel;

        [SerializeField] private GameObject _passPanel;
        [SerializeField] private GameObject _equipmentPanel;
        [SerializeField] private GameObject _trolleyPanel;
        [SerializeField] private GameObject _employeesList;
        [SerializeField] private GameObject _employeeItem;
        [SerializeField] private GameObject _viewport;

        [SerializeField] private TMP_InputField _newPassTextInput;
        [SerializeField] private TMP_Text _textlogin;
        [SerializeField] private TMP_Text _textloginTrolley;
        [SerializeField] private TMP_Text _textEquipment;
        [SerializeField] private TMP_Text _textTrolley;
        [SerializeField] private TMP_Text _hideText;
        [SerializeField] private ScrollRect _scroll;

        Dictionary<GameObject, Employee> _data = new Dictionary<GameObject, Employee>();

        public Action OnBackButtonCLick;

        private SaveLoadService _saveLoadService;
        private List<Employee> _employees = new();
        private Employee _selectedEmployee;
        private GameObject _tempPanel;
        private WarningPanel _warningPanel;
        private char _simbol = '*';
        private bool _isResetEquipmentSelected;
        private bool _isResetEmployeeSelected;
        private bool _isResetTrolleySelected;

        public void Init(SaveLoadService saveLoadService, WarningPanel warningPanel)
        {
            _saveLoadService = saveLoadService;
            _warningPanel = warningPanel;
            AddListeners();
        }

        public void Work()
        {
            FillEmployeesList();
        }

        public void Reset()
        {
            _resetEquipmentButton.interactable = false;
            _resetEmployeePassButton.interactable = false;
            _applyButton.interactable = false;
            _applyEquimpmentButton.interactable = false;
            _resetInputPassTextButton.interactable = false;
            _passPanel.SetActive(false);
            _equipmentPanel.SetActive(false);
            _trolleyPanel.SetActive(false);
            _newPassTextInput.interactable = false;
            _newPassTextInput.text = "";
            _data = new Dictionary<GameObject, Employee>();
            _okPanel.gameObject.SetActive(false);
            _nokPanel.gameObject.SetActive(false);
            Destroy(_tempPanel);
        }

        private void ResetInput()
        {
            ResetInputPass();
        }

        private void ResetInputPass()
        {
            _okPanel.gameObject.SetActive(false);
            _nokPanel.gameObject.SetActive(false);
            
            if (_isResetEquipmentSelected)
            {
                SentLogMessage("Отмена выбора сотрудника для сброса оборудования", "");
                _resetInputPassTextButton.interactable = false;
                _resetEquipmentButton.interactable = true;
                _resetEmployeePassButton.interactable = true;
                _equipmentPanel.SetActive(false);
                _textlogin.text = "";
                _textEquipment.text = "";
                _applyEquimpmentButton.interactable = false;
                _isResetEquipmentSelected = !_isResetEquipmentSelected;
            }

            if (_isResetEmployeeSelected)
            {
                SentLogMessage("Отмена выбора сотрудника для сброса пароля", "");
                _newPassTextInput.text = "";
                _newPassTextInput.Select();
                _newPassTextInput.ActivateInputField();
            }

            if (_isResetTrolleySelected)
            {
                SentLogMessage("Отмена выбора сотрудника для сброса рохли", "");
                _textTrolley.text = "";
                _applyTrolleyButton.interactable = false;
                _isResetTrolleySelected = !_isResetTrolleySelected;
                _newPassTextInput.Select();
                _newPassTextInput.ActivateInputField();
            }
        }

        private void FillEmployeesList()
        {
            _tempPanel = Instantiate(_employeesList, _viewport.transform);
            _tempPanel.SetActive(true);
            _scroll.content = _tempPanel.GetComponent<RectTransform>();

            _employees = _saveLoadService.GetEmployees();

            foreach (Employee employee in _employees)
            {
                GameObject button = Instantiate(_employeeItem, _tempPanel.transform);
                TMP_Text text = button.GetComponentInChildren<TMP_Text>();
                text.text = employee.Login;
                button.GetComponent<Button>().onClick.AddListener(() => GetEmployee(button));
                _data.Add(button, employee);
            }
        }

        private void GetEmployee(GameObject button)
        {
            _selectedEmployee = _data[button];
            SentLogMessage("Выбран сотрудник " + _selectedEmployee.Login, "");
            ResetPanels();
            
            _okPanel.gameObject.SetActive(false);
            _nokPanel.gameObject.SetActive(false);
            
            if (_selectedEmployee.HaveBox)
            {
                _resetEquipmentButton.interactable = true;
            }
            else
            {
                _resetEquipmentButton.interactable = false;
                _equipmentPanel.SetActive(false);
            }

            if (_selectedEmployee.HaveTrolley)
            {
                _resetTrolleyButton.interactable = true;
            }
            else
            {
                _resetTrolleyButton.interactable = false;
                _trolleyPanel.SetActive(false);
            }

            _resetEmployeePassButton.interactable = true;
        }

        private void SwithPanel(bool isSelectedEquipment, bool isSelectedEmployee, bool _isResetTrolleySelected)
        {
            if (isSelectedEquipment)
            {
                _equipmentPanel.SetActive(true);

                _resetEquipmentButton.interactable = false;
                _resetEmployeePassButton.interactable = true;

                if (_selectedEmployee.HaveTrolley)
                {
                    _resetTrolleyButton.interactable = true;
                }

                _textlogin.text = _selectedEmployee.Login;
                _textEquipment.text = _selectedEmployee.Box.Key + "/ " +
                                      _selectedEmployee.Box.Equipment.SerialNumber[^4..];
                _applyEquimpmentButton.interactable = true;

                _passPanel.SetActive(false);
                _trolleyPanel.SetActive(false);

                _applyTrolleyButton.interactable = false;
                _applyButton.interactable = false;
                _resetInputPassTextButton.interactable = false;
                _newPassTextInput.interactable = false;
            }

            if (isSelectedEmployee)
            {
                _passPanel.SetActive(true);
                _resetEmployeePassButton.interactable = false;
                _applyButton.interactable = true;
                _resetInputPassTextButton.interactable = true;
                _newPassTextInput.interactable = true;
                _newPassTextInput.Select();
                _newPassTextInput.ActivateInputField();
                _resetTrolleyButton.interactable = false;
                _resetEquipmentButton.interactable = false;
                _applyEquimpmentButton.interactable = false;

                if (_selectedEmployee.HaveBox)
                {
                    _resetEquipmentButton.interactable = true;
                }

                if (_selectedEmployee.HaveTrolley)
                {
                    _resetTrolleyButton.interactable = true;
                }

                _equipmentPanel.SetActive(false);
                _trolleyPanel.SetActive(false);

                _textlogin.text = "";
                _textEquipment.text = "";
                _applyTrolleyButton.interactable = false;
            }

            if (_isResetTrolleySelected)
            {
                _resetTrolleyButton.interactable = false;

                _resetEquipmentButton.interactable = false;

                if (_selectedEmployee.HaveBox)
                {
                    _resetEquipmentButton.interactable = true;
                }

                _resetEmployeePassButton.interactable = true;

                _textTrolley.text = _selectedEmployee.Trolley.Number;
                _applyTrolleyButton.interactable = true;
                _trolleyPanel.SetActive(true);
                _textloginTrolley.text = _selectedEmployee.Login;
                _textTrolley.text = _selectedEmployee.Trolley.Number;


                _applyButton.interactable = false;
                _resetInputPassTextButton.interactable = false;
                _newPassTextInput.interactable = false;
                _applyEquimpmentButton.interactable = false;
            }
        }
        
        private void ResetPanels()
        {
            _resetEquipmentButton.interactable = false;
            _equipmentPanel.SetActive(false);
            _textlogin.text = "";
            _textEquipment.text = "";
            _applyEquimpmentButton.interactable = false;
            _passPanel.SetActive(false);
            _resetEmployeePassButton.interactable = false;
            _applyButton.interactable = false;
            _resetInputPassTextButton.interactable = false;
            _newPassTextInput.interactable = false;
            _resetTrolleyButton.interactable = false;
            _textTrolley.text = "";
            _applyTrolleyButton.interactable = false;
            _trolleyPanel.SetActive(false);
        }

        private void ResetEmployeePass()
        {
            SentLogMessage("-> Сбросить пароль ", "");
            _okPanel.gameObject.SetActive(false);
            _nokPanel.gameObject.SetActive(false);
            SwithPanel(false, true, false);
        }

        private void ResetEquipment()
        {
            SentLogMessage("-> Сбросить оборудование ", "");
            _okPanel.gameObject.SetActive(false);
            _nokPanel.gameObject.SetActive(false);
            SwithPanel(true, false, false);
        }

        private void ResetTrolley()
        {
            SentLogMessage("-> Сбросить рохлю ", "");
            _okPanel.gameObject.SetActive(false);
            _nokPanel.gameObject.SetActive(false);
            SwithPanel(false, false, true);
        }

        private void ApplyAction()
        {
            SentLogMessage("-> подтвердить ", "");

            if (_newPassTextInput.text != null || _newPassTextInput.text != "")
            {
                _selectedEmployee.SetPassword(_newPassTextInput.text);
                _saveLoadService.Database.SetCurrentEmployeer(_selectedEmployee);
                _saveLoadService.SaveDatabase();
                _okPanel.gameObject.SetActive(true);
                _nokPanel.gameObject.SetActive(false);
                SentLogMessage("Пароль изменен", "");
                ClosePassPanel();
            }
            else
            {
                _okPanel.gameObject.SetActive(false);
                _nokPanel.gameObject.SetActive(true);
                _warningPanel.ShowWindow(WindowNames.EmptyPassword.ToString());
                ResetInputPass();
            }
        }

        private void ApplyTrolleyAction()
        {
            Employee responsibleEmployee = _saveLoadService.Employee;

            SentLogMessage(
                responsibleEmployee.Login + " Сбросил с  сотрудника " + _selectedEmployee.Login + " рохлю " + "",
                "Сброс рохли");

            SentDataMessage(new SentData("Сброс рохли ", _selectedEmployee.Login, "",
                "", "", DateTime.Now.ToString(), _selectedEmployee.Trolley.Number));

            _saveLoadService.SetTrolley(_selectedEmployee.GetTrolley());
            _saveLoadService.SetCurrentEmployee(_selectedEmployee);
            _saveLoadService.SaveDatabase();
            _textTrolley.text = "";

            if (responsibleEmployee.Login != _selectedEmployee.Login)
            {
                _saveLoadService.SetCurrentEmployee(responsibleEmployee);
            }

            Reset();
            Work();
        }

        private void ApplyEquipmentAction()
        {
            Employee responsibleEmployee = _saveLoadService.Employee;

            SentLogMessage(
                responsibleEmployee.Login + " Сбросил с  сотрудника " + _selectedEmployee.Login + " оборудование " +
                _selectedEmployee.Equipment.SerialNumber[^4..], "Сброс оборудования");

            SentDataMessage(new SentData("Сброс оборудования ", _selectedEmployee.Login, _selectedEmployee.Password,
                _selectedEmployee.Box.Key,
                _selectedEmployee.Box.Equipment.SerialNumber[^4..], DateTime.Now.ToString(), ""));

            _saveLoadService.SetBox(_selectedEmployee.GetBox());
            _saveLoadService.SetCurrentEmployee(_selectedEmployee);
            _saveLoadService.SaveDatabase();
            _textlogin.text = "";
            _textEquipment.text = "";

            if (responsibleEmployee.Login != _selectedEmployee.Login)
            {
                _saveLoadService.SetCurrentEmployee(responsibleEmployee);
            }

            Reset();
            Work();
        }

        private void ClosePassPanel()
        {
            _newPassTextInput.text = "";
            _applyButton.interactable = false;
            _applyTrolleyButton.interactable = false;
            _newPassTextInput.interactable = false;
            _passPanel.SetActive(false);
            _resetInputPassTextButton.interactable = false;
            _resetEquipmentButton.interactable = false;
            _resetEmployeePassButton.interactable = false;
        }

        private void AddListeners()
        {
            _resetEquipmentButton.onClick.AddListener(ResetEquipment);
            _resetEmployeePassButton.onClick.AddListener(ResetEmployeePass);
            _resetInputPassTextButton.onClick.AddListener(ResetInput);
            _resetTrolleyButton.onClick.AddListener(ResetTrolley);
            _applyButton.onClick.AddListener(ApplyAction);
            _applyTrolleyButton.onClick.AddListener(ApplyTrolleyAction);
            _applyEquimpmentButton.onClick.AddListener(ApplyEquipmentAction);
            _backButton.onClick.AddListener(OnCLickBackButton);
            _newPassTextInput.onValueChanged.AddListener(delegate { OnValueChangedPassInput();});
        }

        private void OnValueChangedPassInput()
        {
            int length = _newPassTextInput.text.Length;
            _hideText.text = new string(_simbol, length);
        }
        
        private void RemuveListeners()
        {
            _resetEquipmentButton.onClick.RemoveListener(ResetEquipment);
            _resetEmployeePassButton.onClick.RemoveListener(ResetEmployeePass);
            _resetInputPassTextButton.onClick.RemoveListener(ResetInput);
            _resetTrolleyButton.onClick.AddListener(ResetTrolley);
            _applyButton.onClick.RemoveListener(ApplyAction);
            _applyTrolleyButton.onClick.RemoveListener(ApplyTrolleyAction);
            _applyEquimpmentButton.onClick.RemoveListener(ApplyEquipmentAction);
            _backButton.onClick.RemoveListener(OnCLickBackButton);
            _newPassTextInput.onValueChanged.RemoveListener(delegate { OnValueChangedPassInput();});
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

        public void SwithState(bool state)
        {
            _panel.SetActive(state);
        }

        public void OnCLickBackButton()
        {
            SentLogMessage("<- Назад ", "");

            OnBackButtonCLick.Invoke();
            SwithState(false);
            OnBackButtonCLick.Invoke();
        }
    }
}