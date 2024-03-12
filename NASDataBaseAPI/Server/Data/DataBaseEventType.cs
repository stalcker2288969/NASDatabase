namespace NASDatabase.Server.Data
{
    public enum DataBaseEventType
    {
        AddData,
        RemoveData,
        RemoveDataByData,
        AddColumn,
        RemoveColumn,
        LoadedNewSector,
        CloneColumn,
        ClearAllColumn,
        ClearAllBase,
        RenamedColumn,
        SetDataInColumn
    }

}
