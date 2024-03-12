using NASDatabase.Server.Data;
using NASDatabase.Server.Data.DatabaseSettings;

namespace NASDatabase.Interfaces
{
    /// <summary>
    /// Является объектом сохраняющим БД  
    /// </summary>
    public interface IDataBaseSaver<T> where T : AColumn
    {       
        void AddElement(DatabaseSettings DataBaseSettings, uint ClusterNumber, ItemData[] ItemDatas);        
        void SaveAllCluster(DatabaseSettings DataBaseSettings, uint ClusterNumber, T[] Columns);
    }

    /// <summary>
    /// Является объектом загружающий БД  
    /// </summary>
    public interface IDataBaseLoader<T> where T : AColumn
    {
        T[] LoadCluster(string Path, uint ClusterNumber, string DecodeKey);
        T[] LoadCluster(DatabaseSettings DataBaseSettings, uint ClusterNumber);
    }

    /// <summary>
    /// Основной обьект по замене данных в БД 
    /// </summary>
    public interface IDataBaseReplayser
    {
        void ReplayesElement(DatabaseSettings DataBaseSettings, uint ClusterNumber, ItemData[] ItemDatas);
    }
}
