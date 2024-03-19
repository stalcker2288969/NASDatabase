using NASDatabase.Server.Data;
using NASDatabase.Server.Data.DatabaseSettings;

namespace NASDatabase.Interfaces
{
    /// <summary>
    /// Является объектом сохраняющим БД  
    /// </summary>
    public interface IDataBaseSaver<T> where T : AColumn
    {       
        void AddElement(DatabaseSettings databaseSettings, uint clusterNumber, ItemData[] itemDatas);        
        void SaveAllCluster(DatabaseSettings databaseSettings, uint clusterNumber, T[] columns);
    }

    /// <summary>
    /// Является объектом загружающий БД  
    /// </summary>
    public interface IDataBaseLoader<T> where T : AColumn
    {
        T[] LoadCluster(string path, uint clusterNumber, string decodeKey);
        T[] LoadCluster(DatabaseSettings databaseSettings, uint clusterNumber);
    }

    /// <summary>
    /// Основной обьект по замене данных в БД 
    /// </summary>
    public interface IDataBaseReplayser
    {
        void ReplayesElement(DatabaseSettings databaseSettings, uint clusterNumber, ItemData[] itemDatas);
    }
}
