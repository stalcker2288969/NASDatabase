using NASDataBaseAPI.Server.Data;
using NASDataBaseAPI.Server.Data.DataBaseSettings;

namespace NASDataBaseAPI.Interfaces
{
    /// <summary>
    /// Является объектом сохраняющим БД  
    /// </summary>
    public interface IDataBaseSaver<T> where T : IColumn
    {       
        void AddElement(DataBaseSettings DataBaseSettings, uint ClusterNumber, ItemData[] ItemDatas);        
        void SaveAllCluster(DataBaseSettings DataBaseSettings, uint ClusterNumber, T[] Columns);
    }

    /// <summary>
    /// Является объектом загружающий БД  
    /// </summary>
    public interface IDataBaseLoader<T> where T : IColumn
    {
        T[] LoadCluster(string Path, uint ClusterNumber, string DecodeKey);
        T[] LoadCluster(DataBaseSettings DataBaseSettings, uint ClusterNumber);
    }

    /// <summary>
    /// Основной обьект по замене данных в БД 
    /// </summary>
    public interface IDataBaseReplayser
    {
        void ReplayesElement(DataBaseSettings DataBaseSettings, uint ClusterNumber, ItemData[] ItemDatas);
    }
}
