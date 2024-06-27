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
        public Printer Printer;
        public string Login;
        public string Password;
        public string Permission;
        public bool HaveEquipment;
        public bool HaveBox;
        public bool HaveTrolley;
        public bool HavePrinter;
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
            if (HavePrinter)
            {
                oldBox.SetPrinter(Printer);
            }
            
            Equipment=new Equipment("");
            Printer = null;
            Box = null;
            HaveBox = false;
            HaveEquipment= false;
            HavePrinter = false;
            DateTakenEquipment = 0;
            return oldBox;
        }

        public void SetBox(Box box)
        {
            Box = new(box.Key, box.Equipment);
            Box.SetBusy(true);
            HaveBox = true;
            Equipment = Box.Equipment;
            
            if (box.Printer!=null)
            {
                Printer = Box.Printer;
                HavePrinter = true;
            }
            
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
        
        public void SetPrinter(Printer printer)
        {
            HavePrinter = true;
            Printer = new Printer(printer.SerialNumber);
            Printer.SetBusy(true);
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