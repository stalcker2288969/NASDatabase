using NASDataBaseAPI.Data;
using NASDataBaseAPI.Server.Data.DataBaseSettings;

namespace NASDataBaseAPI.Server.Data.Interfases
{
    /// <summary>
    /// Является объектом сохраняющим БД  
    /// </summary>
    public interface IDataBaseSaver
    {
        Column[] LoadCluster(string path, uint ClusterNumber, string DecodeKey);
        void AddElement(DataBaseSettings.DataBaseSettings dataBaseSettings, uint ClusterNumber, ItemData[] itemDatas);
        void ReplayesElement(DataBaseSettings.DataBaseSettings dataBaseSettings, uint ClusterNumber, ItemData[] itemDatas);
        void SaveAllCluster(DataBaseSettings.DataBaseSettings dataBaseSettings, uint ClusterNumber, Column[] tables);
    }
}
