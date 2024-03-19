using NASDatabase.Interfaces;

namespace NASDatabase.Server.Data.DatabaseSettings.Loaders
{
    public class DBNoSaveLoader : DatabaseLoader
    {

        public DBNoSaveLoader() { }

        public DBNoSaveLoader(IEncoder encoder,FileWorker fileWorker) : base(encoder,fileWorker)
        {

        }

        public override void AddElement(DatabaseSettings dataBaseSettings, uint ClusterNumber, ItemData[] itemDatas)
        {

        }

        public override Interfaces.AColumn[] LoadCluster(string path, uint ClusterNumber, string DecodeKey)
        {
            return base.LoadCluster(path, ClusterNumber, DecodeKey);
        }

        public override Interfaces.AColumn[] LoadCluster(DatabaseSettings dataBaseSettings, uint ClusterNumber)
        {
            return base.LoadCluster(dataBaseSettings.Path, ClusterNumber, dataBaseSettings.Key);
        }

        public override void ReplayesElement(DatabaseSettings dataBaseSettings, uint ClusterNumber, ItemData[] itemDatas)
        {
            
        }

        public override void SaveAllCluster(DatabaseSettings dataBaseSettings, uint ClusterNumber, Interfaces.AColumn[] tables)
        {
            base.SaveAllCluster(dataBaseSettings, ClusterNumber, tables);
        }
    }
}
