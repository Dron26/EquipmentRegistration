using System;
using CodeBase.Infrastracture.EquipmentGroup;
using CodeBase.Infrastracture.TrolleyGroup;

namespace CodeBase.Infrastracture.Datas
{
    [Serializable]
    public class Employee
    {
        public Box Box;
        public Equipment Equipment;
        public Trolley Trolley;
        public string Login;
        public string Password;
        public string Permission;
        public bool HaveEquipment;
        public bool HaveBox;
        public bool HaveTrolley;
        public int DateTakenEquipment;

        public Employee(string login, string password, string permission)
        {
            Login = login;
            Password = password;
            Permission = permission;
            HaveBox = false;
            HaveEquipment = false;
            Box = null;
        }

        public Box GetBox()
        {
            Box oldBox = new(Box.Key, Box.Equipment);
            Equipment=new Equipment("");
            Box = null;
            HaveBox = false;
            HaveEquipment= false;
            DateTakenEquipment = 0;
            return oldBox;
        }

        public void SetBox(Box box)
        {
            Box = new(box.Key, box.Equipment);
            HaveBox = true;
            Equipment = Box.Equipment;
            HaveEquipment = true;
        }
        
        public void SetPermision(string permission)
        {
            Permission = permission;
        }
        
        public void SetPassword(string password)
        {
            Password = password;
        }
        
        public void SetTrolley(Trolley trolley)
        {
            HaveTrolley= true;
            Trolley = new Trolley(trolley.Number);
        }
        
        public Trolley GetTrolley()
        {
            Trolley oldTrolley = Trolley;
            Trolley = null;
            HaveTrolley = false;
            return oldTrolley;
        }
        
        public void SetEquipmentData(int day)
        {
            DateTakenEquipment = day;
        }
    }
}