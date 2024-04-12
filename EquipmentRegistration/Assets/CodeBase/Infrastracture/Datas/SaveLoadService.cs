using System;
using System.Collections.Generic;
using System.IO;
using CodeBase.Infrastracture.EquipmentGroup;
using CodeBase.Infrastracture.TrolleyGroup;
using Newtonsoft.Json;
using UnityEngine;

namespace CodeBase.Infrastracture.Datas
{
    public class SaveLoadService : MonoBehaviour
    {
        [SerializeField] private Logger _logger;
        public Data Database => _database;
        public Employee Employee;
        public Trolley SelectedTrolley => _selectedTrolley;
        public bool IsDatabaseLoaded => _isDatabaseLoaded;
        public SwithStatus SwithStatus => _swithStatus;
        public Action OnSelectEquipment;
        public Action OnSelectTrolley;

        private Data _database;
        private SwithStatus _swithStatus;
        private string _filePathEmployees;
        private string _filePathBoxes;
        private string _filePathTrollies;
        private string _filePathSettings;
        private bool _isSelectedEquipment;
        private bool _isSelectetTrolley;
        private bool _isDatabaseLoaded;
        private Trolley _selectedTrolley;

        public void Init()
        {
            _filePathEmployees = Path.Combine(Application.persistentDataPath, Const.EmployeesInfoJs);
            _filePathBoxes = Path.Combine(Application.persistentDataPath, Const.BoxesInfoJs);
            _filePathTrollies = Path.Combine(Application.persistentDataPath, Const.TrolliesInfoJs);
            _filePathSettings = Path.Combine(Application.persistentDataPath, Const.SettingsInfoJs);
            GetSettings();
            _logger.Init(this);
            DontDestroyOnLoad(this);
            //_logger.CheckTime();

            CreateBase();
            _database = LoadDatabase();
        }

        public void CreateBase()
        {
            List<Box> boxes;
            List<Employee> employees;
            Employee employee;

            Box box = new Box("1", new Equipment("000001"));
            boxes = new List<Box>();
            boxes.Add(box);

            Trolley trolley = new Trolley("1");
            List<Trolley> trollies = new List<Trolley>();
            trollies.Add(trolley);

            employee = new Employee("Admin", "Admin", "2");
            employee.SetPermision("2");
            employees = new List<Employee>();
            employees.Add(employee);
            _database = new Data(employees, boxes, trollies);
            //_database.SetCurrentEmployeer(_database.GetEmployees()[0]);
            SentLogIntroInfo("___________________________\n___________________________");
            SentLogIntroInfo("База данных успешно создана.");
        }

        private void GetSettings()
        {
            if (File.Exists(_filePathSettings))
            {
                string json = File.ReadAllText(_filePathSettings);
                _swithStatus = JsonConvert.DeserializeObject<SwithStatus>(json);
            }
            else
            {
                _isSelectedEquipment = true;
                _isSelectetTrolley = false;
                _swithStatus = new SwithStatus(_isSelectedEquipment, _isSelectetTrolley);
            }
        }

        public void SaveDatabase()
        {
            try
            {
                // saveLoadFile.SaveToJson(_databaseFile, _database);
                string json = JsonConvert.SerializeObject(_database.GetBoxes());
                File.WriteAllText(_filePathBoxes, json);
                json = JsonConvert.SerializeObject(_database.GetEmployees());
                File.WriteAllText(_filePathEmployees, json);
                json = JsonConvert.SerializeObject(_database.GetTrolleys());
                File.WriteAllText(_filePathTrollies, json);
                json = JsonConvert.SerializeObject(_swithStatus);
                File.WriteAllText(_filePathSettings, json);
                SentLogIntroInfo("База данных успешно сохранена.");
            }
            catch (Exception ex)
            {
                SentLogIntroInfo("Ошибка сохранения базы данных: " + ex.Message);
            }
        }

        public Data LoadDatabase()
        {
            List<Box> boxes;
            List<Employee> employees;
            List<Trolley> trollies;

            if (!File.Exists(_filePathEmployees))
            {
                SaveDatabase();
            }
            else
            {
                try
                {
                    string json = File.ReadAllText(_filePathEmployees);

                    employees = new List<Employee>();

                    foreach (var employee in JsonConvert.DeserializeObject<List<Employee>>(json))
                    {
                        Employee newEmployee = new Employee(employee.Login, employee.Password, employee.Permission);

                        if (employee.Box != null && employee.Box.Key != "")
                        {
                            newEmployee.SetBox(employee.Box);
                            newEmployee.SetEquipmentData(employee.DateTakenEquipment);
                        }

                        if (employee.Trolley != null && employee.Trolley.Number != "")
                        {
                            newEmployee.SetTrolley(employee.Trolley);
                        }

                        employees.Add(newEmployee);
                    }

                    json = File.ReadAllText(_filePathBoxes);
                    boxes = JsonConvert.DeserializeObject<List<Box>>(json);
                    json = File.ReadAllText(_filePathTrollies);
                    trollies = JsonConvert.DeserializeObject<List<Trolley>>(json);

                    json = File.ReadAllText(_filePathSettings);
                    _swithStatus = JsonConvert.DeserializeObject<SwithStatus>(json);

                    _database = new Data(employees, boxes, trollies);
                    //_database.SetCurrentEmployeer(_database.GetEmployees()[0]);
                    _isDatabaseLoaded = true;

                    SentLogIntroInfo("База данных успешно загружена.");
                }
                catch (Exception ex)
                {
                    SentLogIntroInfo("Ошибка загрузки базы данных: " + ex.Message);
                }
            }

            return _database;
        }

        public void SetDatabase(List<Employee> employees, List<Box> boxes, List<Trolley> trollyes)
        {
            _database = new Data(employees, boxes, trollyes);
            SetCurrentEmployee(Employee);
            SaveDatabase();
        }

        public void SetCurrentEmployee(Employee employee)
        {
            Employee = employee;
            _database.SetCurrentEmployeer(employee);
        }

        public List<Employee> GetEmployees()
        {
            return _database.GetEmployees();
        }

        public Employee GetCurrentEmployee()
        {
            return _database.Employee;
        }

        public List<Box> GetBoxes()
        {
            return _database.GetBoxes();
        }

        public void SetBox(Box box)
        {
            _database.SetBox(box);
            SaveDatabase();
        }

        public List<Trolley> GetTrolleys()
        {
            return _database.GetTrolleys();
        }

        public void SetTrolley(Trolley trolley)
        {
            _database.SetTrolleys(trolley);
        }

        public void SentDataInfo(SentData sentData)
        {
            _logger.SendData(sentData);
        }

        private void SentLogIntroInfo(string action)
        {
            SentLogInfo(action, "");
        }

        public void SentLogInfo(string action, string comment)
        {
            SentData sentData = new SentData(action, "", "", "", "", "", "");
            _logger.SendLog(sentData);
        }

        public void SentDataTrolleyMessage(SentData sentData)
        {
            _logger.SendTrolleyData(sentData);
        }

        public void AddNewEmployee(Employee employee)
        {
            _database.AddNewEmployee(employee);
            SaveDatabase();
        }

        public void RemoveEmployee(Employee employee)
        {
            _database.RemoveEmployee(employee);
            SaveDatabase();
        }

        public void RemoveBox(Box box)
        {
            _database.RemoveBox(box);
            SaveDatabase();
        }

        public void RemoveTrolley(Trolley trolley)
        {
            _database.RemoveTrolley(trolley);
            SaveDatabase();
        }

        public SwithStatus GetSwithStatus()
        {
            return _swithStatus;
        }

        public void SetSwithEquipmentState(bool isSelected)
        {
            _swithStatus.IsEquipmentSelected = isSelected;
            OnSelectEquipment.Invoke();
            SaveDatabase();
        }

        public void SetSwithTrolleyState(bool isSelected)
        {
            _swithStatus.IsTrolleySelected = isSelected;
            OnSelectTrolley.Invoke();
            SaveDatabase();
        }

        public void SetSelectedTrolley(Trolley trolley)
        {
            _selectedTrolley = trolley;
        }
    }
}

[Serializable]
public class SwithStatus
{
    public bool IsEquipmentSelected;
    public bool IsTrolleySelected;

    public SwithStatus(bool state, bool state2)
    {
        IsEquipmentSelected = state;
        IsTrolleySelected = state2;
    }
}