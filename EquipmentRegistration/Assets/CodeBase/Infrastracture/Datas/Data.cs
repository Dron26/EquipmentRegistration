using System;
using System.Collections.Generic;
using CodeBase.Infrastracture.EquipmentGroup;
using CodeBase.Infrastracture.TrolleyGroup;
using Unity.VisualScripting;

namespace CodeBase.Infrastracture.Datas
{
    [Serializable]
    public class Data
    {
        [Serialize] public List<Employee> _employees = new();
        [Serialize] public List<Equipment> _equipments = new();
        [Serialize] public List<Box> _boxes = new();
        [Serialize] public List<int> _keys = new();
        [Serialize] public List<Trolley> _trollyes = new();
        [Serialize] public Employee Employee;
        [Serialize] public Employee _employee;

        private int _currentIndex;

        public Data(List<Employee> employees, List<Box> boxes, List<Trolley> trollyes)
        {
            foreach (var employee in employees)
            {
                Employee newEmployee = new Employee(employee.Login, employee.Password, employee.Permission);

                if (employee.HaveBox)
                {
                    newEmployee.SetBox(employee.Box);
                    newEmployee.SetEquipmentData(employee.DateTakenEquipment);
                }

                if (employee.HaveTrolley)
                {
                    newEmployee.SetTrolley(employee.Trolley);
                    newEmployee.Trolley.SetBusy(true);
                }

                _employees.Add(newEmployee);
            }

            foreach (var box in boxes)
            {
                Box newBox = new Box(box.Key, box.Equipment);
                newBox.SetBusy(box.Busy);
                _boxes.Add(newBox);
                _keys.Add(Convert.ToInt32(box.Key));
            }

            foreach (var trolly in trollyes)
            {
                Trolley newTrolley = new Trolley(trolly.Number);
                newTrolley.SetBusy(newTrolley.Busy);
                _trollyes.Add(newTrolley);
            }
        }

        public void SetCurrentEmployeer(Employee employee)
        {
            foreach (var thisEmployee in _employees)
            {
                if (thisEmployee.Login == employee.Login)
                {
                    _currentIndex = _employees.IndexOf(thisEmployee);
                    _employees[_currentIndex] = new(employee.Login, employee.Password, employee.Permission);
                    ;

                    if (employee.HaveBox)
                    {
                        Box box = new Box(employee.Box.Key, employee.Box.Equipment);
                        _employees[_currentIndex].SetBox(box);
                        _employees[_currentIndex].SetEquipmentData(employee.DateTakenEquipment);
                    }

                    if (employee.HaveTrolley)
                    {
                        Trolley trolley = new Trolley(employee.Trolley.Number);
                        _employees[_currentIndex].SetTrolley(trolley);
                    }

                    Employee = _employees[_currentIndex];

                    break;
                }
            }
        }

        public void SetBox(Box box)
        {
            int key = Convert.ToInt32(box.Key);

            if (_keys.Contains(key))
            {
                int index = _boxes.IndexOf(_boxes.Find(x => x.Key == box.Key));
                _boxes[index].SetBusy(box.Busy);
            }
            else
            {
                _boxes.Add(box);
                _keys.Add(Convert.ToInt32(box.Key));
            }
        }

        public List<Box> GetBoxes()
        {
            return new List<Box>(_boxes);
        }

        public List<Trolley> GetTrolleys()
        {
            return new List<Trolley>(_trollyes);
        }

        public void SetTrolleys(Trolley trolley)
        {
            bool changed = false;

            foreach (Trolley thisTrolley in _trollyes)
            {
                if (thisTrolley.Number == trolley.Number)
                {
                    thisTrolley.Busy = trolley.Busy;
                    changed = true;
                    break;
                }
            }

            if (!changed)
            {
                _trollyes.Add(trolley);
            }
        }

        public List<Employee> GetEmployees()
        {
            return new List<Employee>(_employees);
        }

        public void AddNewEmployee(Employee employee)
        {
            foreach (var thisEmployee in _employees)
            {
                if (thisEmployee.Login == employee.Login)
                {
                    int index = _employees.IndexOf(thisEmployee);

                    if (index < _currentIndex)
                    {
                        _currentIndex--;
                    }

                    break;
                }
            }

            _employees.Add(employee);
        }

        public void RemoveEmployee(Employee employee)
        {
            _employees.Remove(employee);
        }


        public void RemoveBox(Box box)
        {
            _boxes.Remove(box);
            _keys.Remove(Convert.ToInt32(box.Key));
        }

        public void RemoveTrolley(Trolley trolley)
        {
            _trollyes.Remove(trolley);
        }
    }
}