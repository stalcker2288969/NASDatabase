using NASDataBaseAPI.Data;
using NASDataBaseAPI.Server.Data.Interfases;
using System;

namespace NASDataBaseAPI.Server.Data.DataBaseSettings.Loaders
{
    public class DBNoSaveLoader : IDataBaseSaver
    {
        public void AddElement(DataBaseSettings dataBaseSettings, uint ClusterNumber, ItemData[] itemDatas)
        {

        }

        public Column[] LoadCluster(string path, uint ClusterNumber, string DecodeKey)
        {
            return DataBaseManager.DBLoader.LoadCluster(path, ClusterNumber, DecodeKey);
        }

        public void ReplayesElement(DataBaseSettings dataBaseSettings, uint ClusterNumber, ItemData[] itemDatas)
        {
            
        }

        public void SaveAllCluster(DataBaseSettings dataBaseSettings, uint ClusterNumber, Column[] tables)
        {
            DataBaseManager.DBLoader.SaveAllCluster(dataBaseSettings, ClusterNumber, tables);
        }
    }
}
