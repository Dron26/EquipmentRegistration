using System;
using CodeBase.Infrastracture.Datas;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.AdditionalPanels
{
    public class AdditionalMenu : MonoBehaviour, IWindow
    {
        [SerializeField] private Button _adminButton;
        [SerializeField] private Button _managerButton;
        [SerializeField] private Button _historyButton;
        [SerializeField] private Button _addedButton;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private Button _equipmentButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private GameObject _buttonPanel;

        public Action OnBackButtonCLick;
        [SerializeField] private AdminPanel _adminPanel;
        [SerializeField] private ResetPanel _resetPanel;
        [SerializeField] private HistoryPanel _historyPanel;
        [SerializeField] private DeletePanel _deletePanel;
        [SerializeField] private SwichEquipmentAction _swichEquipmentAction;
        [SerializeField] private EmployeeAddPanel _employeeAddPanel;
        

        private SaveLoadService _saveLoadService;
        private Programm _programm;

        private bool _isAdminEnter = false;
        private bool _isManagerEnter = false;


        public void Init(SaveLoadService saveLoadService, Programm programm, WarningPanel warningPanel)
        {
            _saveLoadService = saveLoadService;
            _programm = programm;
            _employeeAddPanel.Init(_saveLoadService, warningPanel); 
            _resetPanel.Init(_saveLoadService, warningPanel);
            _swichEquipmentAction.Init(_saveLoadService, warningPanel);
            _deletePanel.Init(_saveLoadService, warningPanel);
            _historyPanel.Init(_saveLoadService);
            _adminPanel.Init(_saveLoadService,warningPanel);
            
            _adminPanel.SwithState(false);
            _resetPanel.SwithState(false);
            _swichEquipmentAction.SwithState(false);
            _deletePanel.SwithState(false);
            _employeeAddPanel.SwithState(false);
            _historyPanel.SwithAdminState(false);

            AddListeners();
        }

        public void Work()
        {
            _adminButton.interactable = true;
            _managerButton.interactable = true;
            _historyButton.interactable = true;
            _addedButton.interactable = true;
            _deleteButton.interactable = true;
            
        }

        private void Reset()
        {
            _resetPanel.Reset();
            _swichEquipmentAction.Reset();
            _deletePanel.Reset();
            _employeeAddPanel.Reset();
            _adminPanel.Reset();
            _historyPanel.Reset();
        }

        private void OnExitChildrenPanel()
        {
            Reset();
            OnEnter();
        }

        private void EnterAdmin(bool isAdmin)
        {
            if (isAdmin)
            {
                _isAdminEnter = true;
                _isManagerEnter = false;
            }
            else
            {
                _isAdminEnter = false;
                _isManagerEnter = true;
            }

            OnEnter();
        }

        public void ExitAdmin()
        {
            _managerButton.gameObject.SetActive(false);
            _addedButton.gameObject.SetActive(false);
            _deleteButton.gameObject.SetActive(false);
            _equipmentButton.gameObject.SetActive(false);
            _historyButton.gameObject.SetActive(false);
            _adminButton.gameObject.SetActive(false);

            _resetPanel.SwithState(false);
            _swichEquipmentAction.SwithState(false);
            _deletePanel.SwithState(false);
            _employeeAddPanel.SwithState(false);
            _historyPanel.SwithAdminState(false);
            _adminPanel.SwithState(false);
        }

        private void OnEnter()
        {
            _buttonPanel.SetActive(true);
            _backButton.interactable = true;
            _managerButton.gameObject.SetActive(true);
            _historyButton.gameObject.SetActive(true);
            _addedButton.gameObject.SetActive(true);
            
            if (!_isManagerEnter)
            {
                _adminButton.gameObject.SetActive(true);
                _deleteButton.gameObject.SetActive(true);
                _equipmentButton.gameObject.SetActive(true);
            }
        }

        private void OpenEmployeeAddPanel()
        {
            SentLogMessage("->Панель Записи");
            SwitchButtonState(false);
            _employeeAddPanel.SwithState(true);
            _employeeAddPanel.Work();
        }

        private void OpenDeletePanel()
        {
            SentLogMessage("->Панель Удаление");
            SwitchButtonState(false);
            _deletePanel.SwithState(true);
            _deletePanel.Work();
        }
        
        private void OpenManagerPanel()
        {
            SentLogMessage("->Панель Сброса");
            SwitchButtonState(false);
            _resetPanel.SwithState(true);
            _resetPanel.Work();
        }

        private void OpenAdminPanel()
        {
            SentLogMessage("->Панель Админа");
            SwitchButtonState(false);
            _adminPanel.SwithState(true);
        }

        private void OpenHistoryPanel()
        {
            SentLogMessage("->Панель Истории");
            SwitchButtonState(false);
            if (!_isManagerEnter)
            {
                _historyPanel.SwithAdminState(true);
            }
            else
            {
                _historyPanel.SwithManagerState(true);
            }
        }
        private void OpenEquipmentPanel()
        {
            SentLogMessage("->Панель Оборудование");
            SwitchButtonState(false);
            _swichEquipmentAction.SwithState(true);
            _swichEquipmentAction.Work();
        }


        private void SwitchButtonState(bool state)
        {
            _buttonPanel.SetActive(state);
        }

        private void SentLogMessage(string message)
        {
            _saveLoadService.SentLogInfo(message, "");
        }

        private void AddListeners()
        {
            _managerButton.onClick.AddListener(OpenManagerPanel);
            _adminButton.onClick.AddListener(OpenAdminPanel);
            _historyButton.onClick.AddListener(OpenHistoryPanel);
            _addedButton.onClick.AddListener(OpenEmployeeAddPanel);
            _deleteButton.onClick.AddListener(OpenDeletePanel);
            _equipmentButton.onClick.AddListener(OpenEquipmentPanel);
            _backButton.onClick.AddListener(OnCLickBackButton);
            _adminPanel.OnBackButtonCLick += OnExitChildrenPanel;
            _resetPanel.OnBackButtonCLick += OnExitChildrenPanel;
            _historyPanel.OnBackButtonCLick += OnExitChildrenPanel;
            _swichEquipmentAction.OnBackButtonCLick += OnExitChildrenPanel;
            _deletePanel.OnBackButtonCLick += OnExitChildrenPanel; ;
            _employeeAddPanel.OnBackButtonCLick += OnExitChildrenPanel;
            _programm.OnExitAdmin += ExitAdmin;
            _programm.OnEnterAdmin += EnterAdmin;
        }

        private void RemuveListeners()
        {
            _employeeAddPanel.OnBackButtonCLick -= OnExitChildrenPanel;
            _managerButton.onClick.RemoveListener(OpenManagerPanel);
            _adminButton.onClick.RemoveListener(OpenAdminPanel);
            _historyButton.onClick.RemoveListener(OpenHistoryPanel);
            _addedButton.onClick.RemoveListener(OpenEmployeeAddPanel);
            _deleteButton.onClick.RemoveListener(OpenDeletePanel);
            _equipmentButton.onClick.RemoveListener(OpenEquipmentPanel);
            _backButton.onClick.RemoveListener(OnCLickBackButton);
            _adminPanel.OnBackButtonCLick -= OnExitChildrenPanel;
            _resetPanel.OnBackButtonCLick -= OnExitChildrenPanel;
            _swichEquipmentAction.OnBackButtonCLick -= OnExitChildrenPanel;
            _historyPanel.OnBackButtonCLick -= OnExitChildrenPanel;
            _deletePanel.OnBackButtonCLick -= OnExitChildrenPanel;
            _programm.OnExitAdmin -= ExitAdmin;
            _programm.OnEnterAdmin -= EnterAdmin;
        }


        private void OnDisable()
        {
            RemuveListeners();
        }

        public void OnCLickBackButton()
        {
            SentLogMessage("<-Назад");
            _buttonPanel.SetActive(false);
            OnBackButtonCLick.Invoke();
        }
    }
}