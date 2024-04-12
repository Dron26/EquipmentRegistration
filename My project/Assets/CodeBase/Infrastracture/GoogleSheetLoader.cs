using System;
using System.Collections.Generic;
using CodeBase.Infrastracture.Datas;
using CodeBase.Infrastracture.TrolleyGroup;
using UnityEngine;

[RequireComponent(typeof(CVSLoader), typeof(SheetProcessor))]
public class GoogleSheetLoader : MonoBehaviour
{
    public event Action<WebData> OnProcessData;
    
    [SerializeField] private WebData _data;
    
    private CVSLoader _cvsLoader;
    private SheetProcessor _sheetProcessor;

    public void StartDownload()
    {
        _cvsLoader = GetComponent<CVSLoader>();
        _sheetProcessor = GetComponent<SheetProcessor>();
        DownloadTable();
    }

    private void DownloadTable()
    {
        _cvsLoader.DownloadTable(OnRawCVSLoaded);
    }

    private void OnRawCVSLoaded(string rawCVSText)
    {
        _data = _sheetProcessor.ProcessData(rawCVSText);
        OnProcessData?.Invoke(_data);
    }
}

[Serializable]
public class WebData
{
    public List<Employee> Employees;
    public List<Box> Boxes;
    public List<Trolley> Trolleys;
    
    public override string ToString()
    {
        string result = "";
        Employees.ForEach(o =>
        {
            result += o.ToString();
        });
        return result;
    }
}