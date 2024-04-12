using System;
using CodeBase.Infrastracture.EquipmentGroup;
using Unity.VisualScripting;

namespace CodeBase.Infrastracture.Datas
{
    [Serializable]
    public class Box
    {
        [Serialize] public bool Busy { get; set; }

        [Serialize] public string Key { get; set; }

        [Serialize] public Equipment Equipment { get; set; }

        public Box(string key, Equipment equipment)
        {
            Key = key;
            SetEquipment(equipment);
            Busy = false;
        }

        public void SetBusy(bool state)
        {
            Busy = state;
        }

        public void SetEquipment(Equipment equipment)
        {
            Busy = false;
            Equipment = new(equipment.SerialNumber);
        }
    }
}