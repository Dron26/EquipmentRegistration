using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Infrastracture.Datas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Infrastracture.EquipmentGroup
{
    public class EquipmentValidator : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private GameObject _buttonsPanel;
        [SerializeField] private GameObject _viewport;
        [SerializeField] private ScrollRect _skroll;
        [SerializeField] private TMP_Text _equipmentNumber;
        [SerializeField] private TMP_Text _boxNumber;
        [SerializeField] private GameObject _freeBox;
        [SerializeField] private Button _buttonApply;
        
        public Action OnTakeKey;
        public Action OnTakeTsd;
        public Action OnBackButtonCLick;
        private int _selectedKey;
        private GameObject _tempPanel;
        private List<Box> _boxes;
        private SaveLoadService _saveLoadService;
        private List<Box> _busyBoxes = new();
        private Employee _employee;
        private Dictionary<int,Equipment> _freeBoxes = new Dictionary<int,Equipment>();
        private string _applyText = ": подтвердил выбор ";
        public bool _canTakeKey = false;
        public bool _canTakeTsd = false;
        private bool _isButtonFilling;

        public void Init(SaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
            AddListeners();
        }

        public void Work()
        {
            _boxes = _saveLoadService.GetBoxes();
            _buttonApply.interactable = false;
            FillButtons();
        }

        public void Reset()
        {
            _boxes = new List<Box>();
            _equipmentNumber.text = "";
            _boxNumber.text = "";
            _freeBoxes = new();
            _busyBoxes = new();
            _canTakeKey = false;
            _canTakeTsd = false;
            _isButtonFilling = false;
            Destroy(_tempPanel);
        }

        private void FillButtons()
        {
            _freeBox.gameObject.SetActive(true);
            _tempPanel = Instantiate(_buttonsPanel, _viewport.transform);
            _tempPanel.SetActive(true);

            SortButtons();
            
            foreach (var info in _freeBoxes)
            {
                GameObject button = Instantiate(_freeBox, _tempPanel.transform);

                TMP_Text text = button.GetComponentInChildren<TMP_Text>();
                text.text = info.Key.ToString();
                button.GetComponent<Button>().onClick.AddListener(() => ShowInfo(text.text));
            }
            
            _skroll.content=_tempPanel.GetComponent<RectTransform>();
            _isButtonFilling = true;
        }

        private void SortButtons()
        {
            foreach (Box box in _boxes)
            {
                if (!box.Busy)
                {
                    _freeBoxes.Add(Convert.ToInt32(box.Key),box.Equipment);
                }
                else
                {
                    _busyBoxes.Add(box);
                }
            }

            _freeBoxes = _freeBoxes.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }
        
        private void ShowInfo(string text)
        {
            _employee = _saveLoadService.Employee;
            _selectedKey = Convert.ToInt32(text);
            
            if (_freeBoxes.ContainsKey(_selectedKey))
            {
                _boxNumber.text= text;
                _equipmentNumber.text = _freeBoxes[_selectedKey].SerialNumber[^4..];
                _buttonApply.interactable = true;
                SentLogMessage(_employee.Login + " выбрал ячейку : " + _boxNumber.text + " сканер : " + _equipmentNumber.text, "");
            }
        }

        private void OnButtonClick()
        {
            _employee = _saveLoadService.Employee;
            
            string convertNumber=_selectedKey.ToString();
            
            foreach (Box box in _boxes)
            {
                if (box.Key==convertNumber)
                {
                    int index = _boxes.IndexOf(box);
                    Box temp = new Box(_boxes[index].Key, _boxes[index].Equipment);
                    temp.SetBusy(true);
                    _saveLoadService.SetBox(temp);
                    _employee.SetEquipmentData(DateTime.Now.Day);
                    _employee.SetBox(temp);
                    SentLogMessage(_employee.Login + _applyText + " ячейки : " + _employee.Box.Key + " сканера : " +
                                   _employee.Equipment.SerialNumber[^4..], " Выдача оборудования ");
            
                    SentDataMessage(new SentData(" Выдача оборудования ", _employee.Login, _employee.Password,
                        _employee.Box.Key,
                        _employee.Box.Equipment.SerialNumber[^4..], DateTime.Now.ToString(), ""));

                    _saveLoadService.SetCurrentEmployee(_employee);
                    _saveLoadService.SaveDatabase();
                    _buttonApply.interactable = false;

                    OnTakeKey?.Invoke();
                    break;
                }
            }
        }
        
        public void SwithState(bool state)
        {
            _panel.SetActive(state);
        }

        private void AddListeners()
        {
            _buttonApply.onClick.AddListener(OnButtonClick);
        }

        private void RemuveListeners()
        {
            _buttonApply.onClick.RemoveListener(OnButtonClick);
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
    }
}