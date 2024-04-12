using System.Collections.Generic;
using System.Linq;
using CodeBase.Infrastracture;
using CodeBase.Infrastracture.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ChekerEuipment:MonoBehaviour
{
    [SerializeField] private GameObject _employeeList;
    [SerializeField] private GameObject _equipmentList;
    [SerializeField] private GameObject _itemLogin;
    [SerializeField] private GameObject _itemKey;
    [SerializeField] private GameObject _employeeViewport;
    [SerializeField] private GameObject _equipmentViewport;
    [SerializeField] private Button _backButton;
    [SerializeField] private GameObject _panel;
    [SerializeField] private  ScrollRect _employeeScroll;
    [SerializeField] private ScrollRect _equipmentScroll;

    private SaveLoadService _saveLoadService;
    private List<Employee> _employees = new();
    private Employee _selectedEmployee;
    private GameObject _tempPanelEmployee;
    private WarningPanel _warningPanel;
    private Dictionary<string,string> _employeeGroup = new();
    private GameObject _tempPanelEquipment;
    private List<Box> _boxes = new();
    private Dictionary<string, string> _busyBox = new ();

    
    public void Init(SaveLoadService saveLoadService, WarningPanel warningPanel)
    {
        _saveLoadService = saveLoadService;
        _warningPanel = warningPanel;
        AddListeners();
    }

    public void Work()
    {
        SwithState(true);
        SortUserInfo();
        FillUserInfo();
    }

    private void FillUserInfo()
    {
        _tempPanelEmployee = Instantiate(_employeeList, _employeeViewport.transform);
        _tempPanelEmployee.SetActive(true);
        _employeeScroll.content= _employeeViewport.transform.parent.transform.GetComponent<RectTransform>();
        _employeeScroll.verticalScrollbar.value = 1;  
        
        _tempPanelEquipment = Instantiate(_equipmentList, _equipmentViewport.transform);
        _tempPanelEquipment.SetActive(true);
         _equipmentScroll.content = _equipmentViewport.transform.parent.transform.GetComponent<RectTransform>();
         _equipmentScroll.verticalScrollbar.value = 1;

         foreach (KeyValuePair<string, string> employee in _employeeGroup)
         {
             GameObject infoEmployee = Instantiate(_itemLogin, _tempPanelEmployee.transform);
             TMP_Text employeeLogin =
                 infoEmployee.GetComponent<ItemChecker>().GetLogin().GetComponentInChildren<TMP_Text>();
             employeeLogin.text = employee.Key;
             TMP_Text employeeKey =
                 infoEmployee.GetComponent<ItemChecker>().GetKey().GetComponentInChildren<TMP_Text>();
             employeeKey.text = employee.Value;
         }

         foreach (KeyValuePair<string, string> box in _busyBox)
         {
             GameObject newEmployee = Instantiate(_itemKey, _tempPanelEquipment.transform);
             TMP_Text textKey = newEmployee.GetComponent<ItemChecker>().GetKey().GetComponentInChildren<TMP_Text>();
             textKey.text = box.Value;
             TMP_Text textLogin = newEmployee.GetComponent<ItemChecker>().GetLogin().GetComponentInChildren<TMP_Text>();
             textLogin.text = box.Key;
         }
    }
    
    private void SortUserInfo()
    {
        _employees = _saveLoadService.GetEmployees();
        _boxes = _saveLoadService.GetBoxes();
        
        Dictionary<string,string> tempGroup = new();
        
        foreach (Employee employee in _employees)
        {
            if (employee.HaveBox)
            {
                tempGroup.Add(employee.Login, employee.Box.Key);
            }
        }

        _employeeGroup = new(tempGroup.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value));
        _busyBox=new(tempGroup.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value));
    }
    
    
    
    private void AddListeners()
    {
        _backButton.onClick.AddListener(Reset);
    }
    
    public void Reset()
    {
        
        foreach (var employee in _tempPanelEmployee.GetComponentsInChildren<Transform>())
        {
            Destroy(employee.gameObject);
        }
        
        foreach (var employee in _tempPanelEquipment.GetComponentsInChildren<Transform>())
        {
            Destroy(employee.gameObject);
        }
        
        SwithState(false);
    }
    
    public void SwithState(bool state)
    {
        _panel.SetActive(state);
    }
}