using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data.Modules;

namespace NASDataBaseAPI.Server.Data.DataBaseSettings.Loaders
{
    public class DBNoSaveLoader : DataBaseLoader
    {

        public DBNoSaveLoader() { }

        public DBNoSaveLoader(IEncoder encoder,IFileWorker fileWorker) : base(encoder,fileWorker)
        {

        }

        public override void AddElement(DataBaseSettings dataBaseSettings, uint ClusterNumber, ItemData[] itemDatas)
        {

        }

        public override IColumn[] LoadCluster(string path, uint ClusterNumber, string DecodeKey)
        {
            return base.LoadCluster(path, ClusterNumber, DecodeKey);
        }

        public override IColumn[] LoadCluster(DataBaseSettings dataBaseSettings, uint ClusterNumber)
        {
            return base.LoadCluster(dataBaseSettings.Path, ClusterNumber, dataBaseSettings.Key);
        }

        public override void ReplayesElement(DataBaseSettings dataBaseSettings, uint ClusterNumber, ItemData[] itemDatas)
        {
            
        }

        public override void SaveAllCluster(DataBaseSettings dataBaseSettings, uint ClusterNumber, IColumn[] tables)
        {
            base.SaveAllCluster(dataBaseSettings, ClusterNumber, tables);
        }
    }
}
