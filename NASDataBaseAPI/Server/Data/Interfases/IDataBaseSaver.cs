using NASDataBaseAPI.Data;
using NASDataBaseAPI.Server.Data.DataBaseSettings;
using NASDataBaseAPI.Server.Data.Interfases.Column;

namespace NASDataBaseAPI.Server.Data.Interfases
{
    /// <summary>
    /// Является объектом сохраняющим БД  
    /// </summary>
    public interface IDataBaseSaver<T> where T : IColumn
    {       
        void AddElement(DataBaseSettings.DataBaseSettings dataBaseSettings, uint ClusterNumber, ItemData[] itemDatas);        
        void SaveAllCluster(DataBaseSettings.DataBaseSettings dataBaseSettings, uint ClusterNumber, T[] tables);
    }

    /// <summary>
    /// Является объектом загружающий БД  
    /// </summary>
    public interface IDataBaseLoader<T> where T : IColumn
    {
        T[] LoadCluster(string path, uint ClusterNumber, string DecodeKey);
        T[] LoadCluster(DataBaseSettings.DataBaseSettings dataBaseSettings, uint ClusterNumber);
    }

    /// <summary>
    /// Основной обьект по замене данных в БД 
    /// </summary>
    public interface IDataBaseReplayser
    {
        void ReplayesElement(DataBaseSettings.DataBaseSettings dataBaseSettings, uint ClusterNumber, ItemData[] itemDatas);
    }
}
