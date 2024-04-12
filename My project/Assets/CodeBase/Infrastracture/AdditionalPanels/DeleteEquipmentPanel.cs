using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Infrastracture;
using CodeBase.Infrastracture.Datas;
using CodeBase.Infrastracture.EquipmentGroup;
using CodeBase.Infrastracture.TrolleyGroup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeleteEquipmentPanel : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _equipmentPanel;
    [SerializeField] private GameObject _trolleyPanel;
    [SerializeField] private GameObject _equipmentList;
    [SerializeField] private GameObject _trolleyList;
    [SerializeField] private GameObject _viweport;
    [SerializeField] private GameObject _mainItem;
    [SerializeField] private GameObject _mainTrolleyItem;
    
    [SerializeField] private Button _equipmentButton;
    [SerializeField] private Button _trolleyButton;
    [SerializeField] private Button _resetEquipmentInputTextButton;
    [SerializeField] private Button _resetTrolleyInputTextButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _okButton;
    [SerializeField] private Button _nokButton;
    [SerializeField] private Button _boxDeleteButton;
    [SerializeField] private Button _trolleyDeleteButton;
    [SerializeField] private ScrollRect _scroll;
    [SerializeField] private TMP_Text _boxText;
    [SerializeField] private TMP_Text _equipmentText;
    [SerializeField] private TMP_Text _trolleyText;
    [SerializeField] private GameObject _trolleyTextPanel;
    [SerializeField] private GameObject _equipmentTextPanel;

    public Action OnBackButtonCLick;
    private Employee _registeredEmployee;
    private SaveLoadService _saveLoadService;

    private List<Box> _boxes = new();
    private List<Box> _busyBoxes = new();
    private List<Trolley> _trolleys = new();

    private GameObject _tempEquipmentGroup;
    private GameObject _tempTrolleyGroup;
    private List<GameObject> _equipmentGroups = new();
    private List<GameObject> _trolleyGroups = new();
    private WarningPanel _warningPanel;

    private bool _isReseted;
    private Box _selectedForDeleteBox;
    private Trolley _selectedForDeleteTrolley;
    Dictionary<int, Equipment> _freeBoxes = new Dictionary<int, Equipment>();

    public delegate void ActionWithTextNumber(string textNumber);

    private List<int> _freeTrolleys = new();
    private List<Trolley> _busyTrolleys = new();

    public void Init(SaveLoadService saveLoadService, WarningPanel warningPanel)
    {
        _saveLoadService = saveLoadService;
        _warningPanel = warningPanel;
        _tempEquipmentGroup = Instantiate(_equipmentList, _viweport.transform);
        _tempTrolleyGroup = Instantiate(_trolleyList, _viweport.transform);
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
        _trolleyTextPanel.SetActive(false);
        _equipmentTextPanel.SetActive(false);

        _equipmentButton.interactable = true;
        _trolleyButton.interactable = true;
    }
    public void Reset()
    {
        _isReseted = true;
        _freeBoxes.Clear();
        _freeTrolleys.Clear();
        _busyBoxes.Clear();
        _busyTrolleys.Clear();
        _boxText.text = "";
        _equipmentText.text = "";
        _trolleyText.text = "";

        foreach (var gameObject in _equipmentGroups)
        {
            Button button = gameObject.GetComponent<Button>();
            button.onClick.RemoveListener(() => SelectBox(""));
            Destroy(gameObject);
        }
        foreach (var gameObject in _trolleyGroups)
        {
            Button button = gameObject.GetComponent<Button>();
            button.onClick.RemoveListener(() => SelectTrolley(""));
            Destroy(gameObject);
        }
        _equipmentGroups.Clear();
        _trolleyGroups.Clear();
        _isReseted = false;
    }
    private void FillEquipmentList()
    {
        _tempEquipmentGroup.SetActive(true);

        _boxes = _saveLoadService.GetBoxes();
        SortButtons();

        foreach (var info in _freeBoxes)
        {
            GameObject newBox = Instantiate(_mainItem, _tempEquipmentGroup.transform);
            TMP_Text textKey = newBox.GetComponent<ItemMain>().GetBox().GetComponentInChildren<TMP_Text>();
            TMP_Text textEquipment =
                newBox.GetComponent<ItemMain>().GetEquipment().GetComponentInChildren<TMP_Text>();
            Button button = newBox.GetComponent<Button>();
            button.onClick.AddListener(() => SelectBox(textKey.text));
            textKey.text = info.Key.ToString();
            textEquipment.text = info.Value.SerialNumber[^4..];
            _equipmentGroups.Add(newBox);
        }

        _tempEquipmentGroup.SetActive(false);
    }

    private void SortButtons()
    {
        foreach (Box box in _boxes)
        {
            if (!box.Busy)
            {
                if (_freeBoxes.ContainsKey(Convert.ToInt32(box.Key)))
                {
                    _warningPanel.ShowWindow(WindowNames.OnHaveDuplicate.ToString());
                }
                else
                {
                    _freeBoxes.Add(Convert.ToInt32(box.Key), box.Equipment);
                }
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
        
        _tempTrolleyGroup.SetActive(true);
        _trolleys = _saveLoadService.GetTrolleys();
        SortTrolleys();

        foreach (int number in _freeTrolleys)
        {
            GameObject newTrolley = Instantiate(_mainTrolleyItem, _tempTrolleyGroup.transform);
            Button button = newTrolley.GetComponent<Button>();
            TMP_Text textNumber = newTrolley.GetComponentInChildren<TMP_Text>();
            button.onClick.AddListener(() => SelectTrolley(textNumber.text));
            textNumber.text = number.ToString();
            _trolleyGroups.Add(newTrolley);
        }

        _tempTrolleyGroup.SetActive(false);
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

    private void SelectBox(string textKey)
    {
        ChangeStateAction(SelectBox);
        _selectedForDeleteBox=_boxes.FirstOrDefault(x => x.Key == textKey);
        SentLogMessage("Выбран ящик " + _selectedForDeleteBox.Key, "");
        SetBoxInfo();
    }

    private void SelectTrolley(string textNumber)
    {
        ChangeStateAction(SelectTrolley);

        _selectedForDeleteTrolley = _trolleys.FirstOrDefault(x=>x.Number==textNumber);
        SentLogMessage("Выбрана рохля " + _selectedForDeleteTrolley.Number, "");
        SetTrolleyInfo();
    }

    private void SetBoxInfo()
    {
        _boxText.text = _selectedForDeleteBox.Key;
        _equipmentText.text = _selectedForDeleteBox.Equipment.SerialNumber[^4..];
    }
    private void SetTrolleyInfo()
    {
        _trolleyText.text = _selectedForDeleteTrolley.Number;
    }

    private void DeleteBox()
    {
        _okButton.gameObject.SetActive(true);
        _nokButton.gameObject.SetActive(false);
        SentLogMessage("Удачная попытка удаления " + _selectedForDeleteBox.Key, "Удаление");
        _registeredEmployee = _saveLoadService.Employee;
        string action = "Выполнил удаление ящика" + " " + _selectedForDeleteBox.Key;
        string Login = _registeredEmployee.Login;
        string Pass = _registeredEmployee.Password;
        string Key = "*";
        string ShortNumber = "*";
        string Time = DateTime.Now.ToString();

        SentData sentData = new SentData(action, Login, Pass, Key, ShortNumber, Time, "");

        SentDataMessage(sentData);

        _saveLoadService.RemoveBox(_selectedForDeleteBox);

        Reset();
        Work();
    }

    private void DeleteTrolley()
    {
        _okButton.gameObject.SetActive(true);
        _nokButton.gameObject.SetActive(false);
        SentLogMessage("Удачная попытка удаления " + _selectedForDeleteTrolley.Number, "Удаление");
        _registeredEmployee = _saveLoadService.Employee;
        string action = "Выполнил удаление тележки" + " " + _selectedForDeleteTrolley.Number;
        string Login = _registeredEmployee.Login;
        string Pass = _registeredEmployee.Password;
        string Key = "*";
        string ShortNumber = "*";
        string Time = DateTime.Now.ToString();

        SentData sentData = new SentData(action, Login, Pass, Key, ShortNumber, Time, "");

        SentDataMessage(sentData);

        _saveLoadService.RemoveTrolley(_selectedForDeleteTrolley);

        Reset();
        Work();
    }
    
    private void ChangeStateAction(ActionWithTextNumber action)
    {
        if (action ==SelectTrolley )
        {
            _trolleyPanel.SetActive(true);
            _equipmentPanel.SetActive(false);
            _trolleyDeleteButton.interactable = true;
        }
        else if (action == SelectBox)
        {
            _trolleyPanel.SetActive(false);
            _equipmentPanel.SetActive(true);
            _boxDeleteButton.interactable = true;
        }
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
        _backButton.onClick.AddListener(OnCLickBackButton);
        _resetEquipmentInputTextButton.onClick.AddListener(ResetEquipmentInput);
        _resetTrolleyInputTextButton.onClick.AddListener(ResetTrolleyInput);
        _okButton.onClick.AddListener(OnApplyAddedEmployee);
        _nokButton.onClick.AddListener(OnApplyAddedEmployee);
        _boxDeleteButton.onClick.AddListener(DeleteBox);
        _trolleyDeleteButton.onClick.AddListener(DeleteTrolley);
        _equipmentButton.onClick.AddListener(SelectEquipmentGroup);
        _trolleyButton.onClick.AddListener(SelectTrolleyGroup);
    }

    private void SelectTrolleyGroup()
    {
        SentLogMessage("-> Удалить рохлю ", "");
        _tempTrolleyGroup.SetActive(true);
        _tempEquipmentGroup.SetActive(false);
        _scroll.content = _tempTrolleyGroup.GetComponent<RectTransform>();
        _scroll.verticalScrollbar.value = 1;    }

    private void SelectEquipmentGroup()
    {
        SentLogMessage("-> Удалить оборудование ", "");
        _tempEquipmentGroup.SetActive(true);
        _tempTrolleyGroup.SetActive(false);
        _scroll.content = _tempEquipmentGroup.GetComponent<RectTransform>();
        _scroll.verticalScrollbar.value = 1;
    }

    private void RemuveListeners()
    {
        _backButton.onClick.RemoveListener(OnCLickBackButton);
        _resetEquipmentInputTextButton.onClick.RemoveListener(ResetEquipmentInput);
        _resetTrolleyInputTextButton.onClick.RemoveListener(ResetTrolleyInput);
        _okButton.onClick.RemoveListener(OnApplyAddedEmployee);
        _nokButton.onClick.RemoveListener(OnApplyAddedEmployee);
        _boxDeleteButton.onClick.RemoveListener(DeleteBox);
        _trolleyDeleteButton.onClick.RemoveListener(DeleteTrolley);
        _trolleyButton.onClick.RemoveListener(SelectTrolleyGroup);
        _equipmentButton.onClick.RemoveListener(SelectEquipmentGroup);
    }
    
    public void OnCLickBackButton()
    {
        SentLogMessage("<- Назад ", "");
        SwithState(false);
        OnBackButtonCLick.Invoke();
    }
}