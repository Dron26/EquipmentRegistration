using System;
using System.Collections.Generic;
using CodeBase.Infrastracture.AdditionalPanels;
using CodeBase.Infrastracture.Datas;
using CodeBase.Infrastracture.EquipmentGroup;
using CodeBase.Infrastracture.TrolleyGroup;
using CodeBase.Infrastracture.UserManagerPanel;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Infrastracture
{
    public class Programm : MonoBehaviour
    {
        [SerializeField] private EmployeeRegistrationMenu _employeeRegistrationMenu;
        [SerializeField] private EquipmentRegistrationMenu _equipmentRegistrationMenu;
        [SerializeField] private TrolleyRegistrationMenu _trolleyRegistrationMenu;
        [SerializeField] private MainSwithMenu _mainSwithMenu;
        [SerializeField] private EquipmentSwithMenu _equipmentSwithMenu;
        [SerializeField] private TrolleySwithMenu _trolleySwithMenu;
        [SerializeField] private EquipmentReturnMenu _equipmentReturnMenu;
        [SerializeField] private TrolleyReturnMenu _trolleyReturnMenu;
        [SerializeField] private AdminPanel _adminPanel;
        [SerializeField] private WarningPanel _warningPanel;
        [SerializeField] private Button _additionalButton;
        [SerializeField] private Button _checkerButton;
        [SerializeField] private AdditionalMenu _additionalMenu;
        [SerializeField] private ChekerEuipment _chekerEuipment;
        [SerializeField] private Canvas _canvas;
        
        public Action OnButtonClick;
        public Action<bool> OnEnterAdmin;
        public Action OnExitAdmin;
        private SaveLoadService _saveLoadService;

        private bool _isLogged = false;
        private bool _isLoggedManager = false;
        private bool _isLoggedAdmin = false;
        private bool _isLoggedEmployee = false;
        private bool _isSelectedEquipment = false;
        private bool _isSelectedTrolley = false;
        private bool _isGetEquipmentSelect = false;
        private bool _isRegistrationTrolleyEnd = false;
        private bool _isGetTrolleySelect = false;


        public void Init(SaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
            _additionalButton.gameObject.SetActive(false);
            _warningPanel.Init(_saveLoadService);
            _employeeRegistrationMenu.Init(_saveLoadService, _warningPanel);
            _mainSwithMenu.Init(_saveLoadService, _warningPanel);
            _equipmentSwithMenu.Init(_saveLoadService, _warningPanel);
            _equipmentRegistrationMenu.Init(_saveLoadService);
            _equipmentReturnMenu.Init(_saveLoadService, _warningPanel);
            _trolleyRegistrationMenu.Init(_saveLoadService,_warningPanel);
            _trolleySwithMenu.Init(_saveLoadService, _warningPanel);
            _trolleyReturnMenu.Init(_saveLoadService, _warningPanel);
            _additionalMenu.Init(_saveLoadService, this, _warningPanel);
            _chekerEuipment.Init(_saveLoadService,_warningPanel);
            
            DontDestroyOnLoad(this);
            AddListeners();
            SentLogMessage("Программа инизиализированна");
        }

        public void Work()
        {
            _warningPanel.Work();
            _employeeRegistrationMenu.Work();
            _mainSwithMenu.Work();
            _additionalMenu.Work();
            
            _equipmentSwithMenu.Work();
            _equipmentRegistrationMenu.Work();
            _equipmentReturnMenu.Work();
            
            _trolleySwithMenu.Work();
            _trolleyReturnMenu.Work();
            _trolleyRegistrationMenu.Work();
        
            _employeeRegistrationMenu.SwithPanelState(true);
            _equipmentRegistrationMenu.SwitchValidatorState(false);
            _mainSwithMenu.SwithState(false);
            _trolleySwithMenu.SwithState(false);
            _equipmentSwithMenu.SwithState(false);
            
            _checkerButton.gameObject.SetActive(true);
            _canvas.sortingOrder = 0;
            SentLogMessage("Программа Запущена");
        }

        private void Reset()
        {
            SentLogMessage("Программа Сброшена");
            _isLoggedManager = false;
            _isLoggedAdmin = false;
            _isLoggedEmployee = false;
            _isGetEquipmentSelect = false;
            _adminPanel.Reset();
            _employeeRegistrationMenu.Reset();
            _equipmentRegistrationMenu.Reset();
            _equipmentReturnMenu.Reset();
            _additionalButton.gameObject.SetActive(false);
            Work();
        }

        private void OnLoggedAdmin()
        {
            _isLoggedAdmin = true;
            _employeeRegistrationMenu.SwithPanelState(false);   
            _additionalButton.gameObject.SetActive(true);
        }

        private void OnLoggedEmployee()
        {
            _employeeRegistrationMenu.SwithPanelState(false);
        }

        private void OnLoggedManager()
        {
            _isLoggedAdmin = false;
            _employeeRegistrationMenu.SwithPanelState(false);
            _additionalButton.gameObject.SetActive(true);
        }

        private void CheckPermission()
        {
            _isLoggedEmployee = true;
            SentLogMessage(" -> Выбор действий");
            _mainSwithMenu.SwithState(true);
            _checkerButton.gameObject.SetActive(false);
            _canvas.sortingOrder = 1;

            if (_saveLoadService.Employee.Permission == "0")
            {
                OnLoggedEmployee();
            }
            else if (_saveLoadService.Employee.Permission == "1")
            {
                OnLoggedManager();
            }
            else if (_saveLoadService.Employee.Permission == "2")
            {
                OnLoggedAdmin();
            }
        }

        private void OnApplyRegistration()
        {
            _equipmentRegistrationMenu.SwitchValidatorState(false);
            _employeeRegistrationMenu.SwithPanelState(true);
            Reset();
        }

        private void OnSelectEquipment()
        {
            _mainSwithMenu.SwithState(false);
            _isSelectedEquipment = true;
            _isSelectedTrolley = false;
            _additionalButton.gameObject.SetActive(false);
            _equipmentSwithMenu.SwithState(true);
        }
        
        private void OnGetEquipment()
        {
            _isGetEquipmentSelect = true;
            _equipmentSwithMenu.SwithState(false);
            _equipmentRegistrationMenu.SwitchValidatorState(true);
        }

        private void OnReturnEquipment()
        {
            _equipmentSwithMenu.SwithState(false);
            _equipmentReturnMenu.SwitchPanelState(true);
        }
        
        private void OnSelectTrolley()
        {
            _mainSwithMenu.SwithState(false);
            _isSelectedTrolley = true;
            _isSelectedEquipment = false;
            _additionalButton.gameObject.SetActive(false);
            _trolleySwithMenu.SwithState(true);
        }

        private void OnGetTrolley()
        {
            _isGetEquipmentSelect = true;
            _trolleySwithMenu.SwithState(false);
            _trolleyRegistrationMenu.SwitchValidatorState(true);
        }
        
        private void OnReturnTrolley()
        {
            _trolleySwithMenu.SwithState(false);
            _trolleyReturnMenu.SwitchPanelState(true);
        }
        
        private void OnApplyTrolleyRegistration()
        {
            _trolleyRegistrationMenu.SwitchValidatorState(false);
            Reset();
        }
        private void OnLoaded(List<Employee> employees, List<Box> boxes, List<Trolley> trollyes)
        {
            _saveLoadService.SetDatabase(employees, boxes, trollyes);
        }

        private void OnClickBackButton(IWindow window)
        {
            if (window == _equipmentSwithMenu)
            {
                _equipmentSwithMenu.SwithState(false);
                CheckPermission();
            }
            else if (window == _equipmentRegistrationMenu)
            {
                _equipmentRegistrationMenu.Reset();
                _equipmentRegistrationMenu.SwitchValidatorState(false);
                _equipmentSwithMenu.SwithState(true);
            }
            else if (window == _mainSwithMenu)
            {
                _mainSwithMenu.Reset();
                _mainSwithMenu.SwithState(false);
                Reset();
            }
            else if (window == _equipmentReturnMenu)
            {
                _equipmentReturnMenu.Reset();
                _equipmentReturnMenu.SwitchPanelState(false);
                Reset();
            }
            else if (window == _trolleyReturnMenu)
            {
                _trolleyReturnMenu.Reset();
                _trolleyReturnMenu.SwitchPanelState(false);
                Reset();
            }
            else if (window == _trolleySwithMenu)
            {
                _trolleySwithMenu.SwithState(false);
                CheckPermission();
            }
            else if (window == _trolleyRegistrationMenu)
            {
                _trolleyRegistrationMenu.Reset();
                _trolleyRegistrationMenu.SwitchValidatorState(false);
                _trolleySwithMenu.SwithState(true);
            }
            else if (window == _additionalMenu)
            {
                _additionalMenu.ExitAdmin();
                CheckPermission();
            }
           
        }

        private void AddListeners()
        {
            _adminPanel.Loaded += OnLoaded;
            _adminPanel.OnBackButtonCLick += (() => OnClickBackButton(_adminPanel));
            _employeeRegistrationMenu.OnLogged += CheckPermission;
            _equipmentRegistrationMenu.OnApplyRegistration += OnApplyRegistration;
            _mainSwithMenu.OnSelectEquipment += OnSelectEquipment;
            _mainSwithMenu.OnSelectTrolley += OnSelectTrolley;
            _trolleySwithMenu.OnGetTrolley += OnGetTrolley;
            _trolleySwithMenu.OnReturnTrolley += OnReturnTrolley;
            _equipmentSwithMenu.OnGetEquipment += OnGetEquipment;
            _equipmentSwithMenu.OnReturnEquipment += OnReturnEquipment;
            _additionalButton.onClick.AddListener(OnAdditionalButtonClick);
            _trolleyRegistrationMenu.OnApplyRegistration+= OnApplyTrolleyRegistration;
            _trolleyRegistrationMenu.OnBackButtonCLick += () => OnClickBackButton(_trolleyRegistrationMenu);
            _mainSwithMenu.OnBackButtonCLick += (() => OnClickBackButton(_mainSwithMenu));
            _equipmentSwithMenu.OnBackButtonCLick += (() => OnClickBackButton(_equipmentSwithMenu));
            _trolleySwithMenu.OnBackButtonCLick += (() => OnClickBackButton(_trolleySwithMenu));
            _equipmentRegistrationMenu.OnBackButtonCLick += (() => OnClickBackButton(_equipmentRegistrationMenu));
            _equipmentReturnMenu.OnBackButtonCLick += (() => OnClickBackButton(_equipmentReturnMenu));
            _trolleyReturnMenu.OnBackButtonCLick += (() => OnClickBackButton(_trolleyReturnMenu));
            _additionalMenu.OnBackButtonCLick += (() => OnClickBackButton(_additionalMenu));
        }

        private void RemuveListeners()
        {
            _adminPanel.Loaded -= OnLoaded;
            _employeeRegistrationMenu.OnLogged -= CheckPermission;
            _equipmentRegistrationMenu.OnApplyRegistration -= OnApplyRegistration;
            _mainSwithMenu.OnSelectEquipment -= OnSelectEquipment;
            _mainSwithMenu.OnSelectTrolley -= OnSelectTrolley;
            _trolleySwithMenu.OnGetTrolley -= OnGetTrolley;
            _trolleySwithMenu.OnReturnTrolley -= OnReturnTrolley;
            _equipmentSwithMenu.OnGetEquipment -= OnGetEquipment;
            _equipmentSwithMenu.OnReturnEquipment -= OnReturnEquipment;
            _additionalButton.onClick.RemoveListener(OnAdditionalButtonClick);
            _trolleyRegistrationMenu.OnApplyRegistration-= OnApplyTrolleyRegistration;
            _trolleyRegistrationMenu.OnBackButtonCLick -= () => OnClickBackButton(_trolleyRegistrationMenu);
            _mainSwithMenu.OnBackButtonCLick -= (() => OnClickBackButton(_mainSwithMenu));
            _trolleySwithMenu.OnBackButtonCLick -= (() => OnClickBackButton(_trolleySwithMenu));
            _equipmentSwithMenu.OnBackButtonCLick -= (() => OnClickBackButton(_equipmentSwithMenu));
            _equipmentRegistrationMenu.OnBackButtonCLick -= (() => OnClickBackButton(_equipmentRegistrationMenu));
            _equipmentReturnMenu.OnBackButtonCLick -= (() => OnClickBackButton(_equipmentReturnMenu));
            _adminPanel.OnBackButtonCLick -= (() => OnClickBackButton(_adminPanel));
            _additionalMenu.OnBackButtonCLick -= (() => OnClickBackButton(_additionalMenu));
            _trolleyReturnMenu.OnBackButtonCLick-= (() => OnClickBackButton(_trolleyReturnMenu));
        }

        private void OnAdditionalButtonClick()
        {
            SentLogMessage("-> Меню Дополнительных возможностей");
            OnEnterAdmin?.Invoke(_isLoggedAdmin);
            _additionalButton.gameObject.SetActive(false);
            _mainSwithMenu.SwithState(false);
        }

        private void SentLogMessage(string message)
        {
            _saveLoadService.SentLogInfo(message, "");
        }

        private void OnDisable()
        {
            RemuveListeners();
        }
    }
}