namespace NASDatabase.Server.Data
{
    public enum DatabaseEventType
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
