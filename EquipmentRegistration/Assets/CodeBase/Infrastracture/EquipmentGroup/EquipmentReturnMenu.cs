using System;
using System.Collections.Generic;
using CodeBase.Infrastracture.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.EquipmentGroup
{
    public class EquipmentReturnMenu : MonoBehaviour, IWindow
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _buttonApply;
        [SerializeField] private TMP_Text _employeeField;
        [SerializeField] private TMP_Text _keyField;
        [SerializeField] private TMP_Text _tsdField;
        [SerializeField] private TMP_InputField _inputReturnField;
        [SerializeField] private Button _resetInput;
        [SerializeField] private Image _CheckUp;
        [SerializeField] private Image _CheckDown;
        [SerializeField] private Button _backButton;
        [SerializeField] private TMP_Text _inputHideField;

        public Action OnBackButtonCLick;
        
        private char _simbol = '*';
        private SaveLoadService _saveLoadService;
        private Employee _employee;
        private WarningPanel _warningPanel;
        private bool _isReseted;
        private bool _isSerialNumberInputed;

        public void Init(SaveLoadService saveLoadService, WarningPanel warningPanel)
        {
            _saveLoadService = saveLoadService;
            _warningPanel = warningPanel;
            AddListeners();
        }

        public void Work()
        {
            _employee = _saveLoadService.Employee;
            _inputReturnField.Select();
            _inputReturnField.ActivateInputField();
            _inputReturnField.interactable = true;
            _buttonApply.interactable = false;
            _CheckDown.enabled = true;
            _CheckUp.enabled = false;
        }
        
        private void OnApplyButtonClick()
        {
            SentDataMessage(new SentData("Возврат оборудования ", _employee.Login, _employee.Password, _employee.Box.Key,
                _employee.Box.Equipment.SerialNumber[^4..], DateTime.Now.ToString(), ""));
            SentLogMessage("Возврат оборудования" +"//"+ _employee.Login +"//"+  _employee.Password +"//"+  _employee.Box.Key +"//"+ 
             _employee.Box.Equipment.SerialNumber[^4..] +"//"+  DateTime.Now.ToString(), " Возврат оборудования ");
            _saveLoadService.SetBox(_employee.GetBox());
            _saveLoadService.SetCurrentEmployee(_employee);
            _saveLoadService.SaveDatabase();
            OnCLickBackButton();
        }

        public void SetData()
        {
            _employee = _saveLoadService.GetCurrentEmployee();
            _employeeField.text = _employee.Login;
            _keyField.text = _employee.Box.Key;
            _tsdField.text = _employee.Equipment.SerialNumber[^4..];
        }

        public void Reset()
        {
            _isReseted = true;
            _employeeField.text = "";
            _keyField.text = "";
            _tsdField.text = "";
            _inputHideField.text = "";
            _inputReturnField.text = null;
            _inputReturnField.interactable = true;
            _inputReturnField.ActivateInputField();
            _inputReturnField.Select();
            _buttonApply.interactable = false;
            _CheckDown.enabled = true;
            _CheckUp.enabled = false;
            _resetInput.interactable = false;
            _isSerialNumberInputed = false;
            _isReseted = false;
        }

        private void ResetInput()
        {
            SentLogMessage(_employee.Login + "Выполнил сброс ввода QR сканера", "");
            Reset();
            SetData();
        }

        public void ValidateReturn()
        {
            if (_isReseted == false)
            {
                CheckInput();
            }
        }


        private void CheckInput()
        {
            string text = _inputReturnField.text;
            _inputHideField.text = new string(_simbol, _inputReturnField.text.Length);
            if (_inputReturnField.text != "")
            {
                _resetInput.interactable = true;
            }
            else
            {
                _resetInput.interactable = false;
            }

            if (text == _employee.Equipment.SerialNumber[^4..]|| text == _employee.Box.Equipment.SerialNumber[^5..] && _isSerialNumberInputed == false)
            {
                SentLogMessage(_employee.Login + ": отсканировал верный QR", "");

                _isSerialNumberInputed = true;
                _inputReturnField.interactable = false;
                _buttonApply.interactable = true;
                _CheckDown.enabled = false;
                _CheckUp.enabled = true;
            }

            if (_isSerialNumberInputed == false)
            {
                List<Employee> employees = _saveLoadService.GetEmployees();

                try
                {
                    foreach (var employee in employees)
                    {
                        if (employee != _employee && _inputReturnField.text == employee.Equipment.SerialNumber[^4..]|| _inputReturnField.text == employee.Equipment.SerialNumber[^5..])
                        {
                            _warningPanel.ShowWindow(WindowNames.CanNotReturnOtherEquipment.ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public void SwitchPanelState(bool state)
        {
            if (state)
            {
                SetData();
            }
            else
            {
                Reset();
            }

            _panel.SetActive(state);
        }

        private void AddListeners()
        {
            _buttonApply.onClick.AddListener(OnApplyButtonClick);
            _inputReturnField.onValueChanged.AddListener(delegate { ValidateReturn(); });
            _resetInput.onClick.AddListener(ResetInput);
            _backButton.onClick.AddListener(OnCLickBackButton);
        }

        private void RemuveListeners()
        {
            _inputReturnField.onValueChanged.RemoveListener(delegate { ValidateReturn(); });
            _buttonApply.onClick.RemoveListener(OnApplyButtonClick);
            _resetInput.onClick.RemoveListener(ResetInput);
            _backButton.onClick.RemoveListener(OnCLickBackButton);
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

        public void OnCLickBackButton()
        {
            OnBackButtonCLick.Invoke();
        }
    }
}