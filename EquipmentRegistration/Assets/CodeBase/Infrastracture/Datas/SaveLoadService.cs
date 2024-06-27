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
        public Employee Employee ;
        public Trolley SelectedTrolley=>_selectedTrolley;
        public bool IsDatabaseLoaded => _isDatabaseLoaded;
        public SwithStatus SwithStatus=>_swithStatus;
        public bool IsLogged => _isLogged;

        public Action OnSelectEquipment;
        public event Action ChangeLogStatus;
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
        private bool _isLogged;
        private bool _isBuyed;
        
        private Trolley _selectedTrolley;

        public void Init()
        {
            _isLogged = true;
            _filePathEmployees = Path.Combine(Application.persistentDataPath, Const.EmployeesInfoJs);
            _filePathBoxes = Path.Combine(Application.persistentDataPath,  Const.BoxesInfoJs);
            _filePathTrollies= Path.Combine(Application.persistentDataPath,  Const.TrolliesInfoJs);
            _filePathSettings= Path.Combine(Application.persistentDataPath,  Const.SettingsInfoJs);
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
            
            Trolley trolley= new Trolley("1");
            List<Trolley> trollies = new List<Trolley>();
            trollies.Add(trolley);

            employee = new Employee("Admin", "Admin", "2");
            employee.SetPermision("2");
            employees = new List<Employee>();
            employees.Add(employee);
            _database = new Data(employees, boxes,trollies);
            //_database.SetCurrentEmployeer(_database.GetEmployees()[0]);
            SentLogIntroInfo("___________________________\n___________________________");
            SentLogIntroInfo("База данных успешно создана.");
        }

        private void GetSettings()
        {
            if (File.Exists(_filePathSettings))
            {
               string  json = File.ReadAllText(_filePathSettings);
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
                            
                            if (employee.Box.Printer != null && employee.Box.Printer.SerialNumber != "")
                            {
                                newEmployee.Box.SetPrinter(employee.Box.Printer);
                            }
                        }

                        if (employee.Trolley != null && employee.Trolley.Number != "")
                        {
                            newEmployee.SetTrolley(employee.Trolley);
                        }

                        if (employee.HavePrinter)
                        {
                            newEmployee.SetPrinter(employee.Printer);
                        }

                        employees.Add(newEmployee);
                    }

                    json = File.ReadAllText(_filePathBoxes);
                    boxes = JsonConvert.DeserializeObject<List<Box>>(json);
                    json = File.ReadAllText(_filePathTrollies);
                    trollies = JsonConvert.DeserializeObject<List<Trolley>>(json);

                    json = File.ReadAllText(_filePathSettings);
                    _swithStatus = JsonConvert.DeserializeObject<SwithStatus>(json);
                    
                    _database = new Data(employees, boxes,trollies);
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

        public void SetDatabase(List<Employee> employees, List<Box> boxes,List<Trolley> trollyes)
        {
            _database = new Data(employees, boxes,trollyes);
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
        
        public List<Printer> GetPinters()
        {
            return _database.GetPrinters();
        }
        
        public void SetPrinter(Printer printer)
        {
            _database.SetPrinter(printer);
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
        
        public void RemovePrinter(Printer printer)
        {
            _database.RemovePrinter(printer);
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
            _selectedTrolley=trolley;
        }
        public void SetLoggedStatus(bool isActive)
        {
            _isLogged = isActive;
            ChangeLogStatus?.Invoke();
        }

        public void SetSertificate()
        {
            
        }
    }
}