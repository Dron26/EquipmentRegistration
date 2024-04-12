using System.Collections;
using System.Collections.Generic;
using CodeBase.Infrastracture.Datas;
using CodeBase.Infrastracture.EquipmentGroup;
using CodeBase.Infrastracture.TrolleyGroup;
using UnityEngine;
using UnityEngine.Networking;

public class SheetProcessor : MonoBehaviour
{
    private const int _login = 0;
    private const int _password = 1;
    private const int _permission = 2;
    private const int _serialNumber = 3;
    private const int _key = 4;
    private const int _trolleNumber = 5;
    private const char _cellSeporator = ',';

    public WebData ProcessData(string cvsRawData)
    {
        char lineEnding = GetPlatformSpecificLineEnd();
        string[] rows = cvsRawData.Split(lineEnding);
        int dataStartRawIndex = 1;
        WebData data = new WebData();
        data.Employees = new ();
        data.Boxes = new ();
        data.Trolleys = new ();
        Employee employee;
        Equipment equipment;
        Trolley trolley;
        Box box;
        for (int i = dataStartRawIndex; i < rows.Length; i++)
        {
            string[] cells = rows[i].Split(_cellSeporator);

            if (cells[_login]!="" && cells[_password]!="" && cells[_permission]!="")
            {
                employee = new Employee(cells[_login], cells[_password], cells[_permission]);
                data.Employees.Add(employee);
            }

            if (cells[_serialNumber]!=""&& cells[_key]!="")
            {
                 equipment = new Equipment(cells[_serialNumber]);
                  box=new Box(cells[_key], equipment);
                  data.Boxes.Add(box);
            }
            
            if (cells[_trolleNumber] != ""&& cells[_trolleNumber] != "\r")
            {
                
                string x= cells[_trolleNumber];
                trolley = new Trolley(x);
                 
                 data.Trolleys.Add(trolley);
            }
        }
        
        return data;
    }

    private char GetPlatformSpecificLineEnd()
    {
        char lineEnding = '\n';
#if UNITY_IOS
        lineEnding = '\r';
#endif
        return lineEnding;
    }
}