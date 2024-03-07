using NASAPITests.DataBaseTests.Tools;
using NASDataBaseAPI.Server.Data.DataBaseSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASAPITests.DataBaseTests.Loaded
{
    public class LoadDataBase
    {
        DataBaseManager DBM = new DataBaseManager();
        
        [Fact]
        public void Load_CountBuckets()
        {
            int DataCount = 1000;

            var DB = DBM.LoadDB(GenerateBase_3_1000Data.Path, GenerateBase_3_1000Data.LoadKey(), 1);

            Assert.True(DataCount == DB.Settings.CountBuckets, $"DataCount = {DataCount}|DB.settings.CountBuckets = {DB.Settings.CountBuckets}");
        }

        [Fact]
        public void Load_HaveData()
        {
            int ColumnCount = 3;

            var DB = DBM.LoadDB(GenerateBase_3_1000Data.Path, GenerateBase_3_1000Data.LoadKey(), 1); 

            Assert.True(DB.Columns.Count == ColumnCount && DB.Columns[0].GetDatas().Length == 100, $"LoadedData = {DB.Columns[0].GetDatas().Length}");
        }

        [Fact]
        public void Load_CountColumns()
        {
            int ColumnCount = 3;

            var DB = DBM.LoadDB(GenerateBase_3_1000Data.Path, GenerateBase_3_1000Data.LoadKey(), 1);

            Assert.True(ColumnCount == DB.Settings.ColumnsCount, $"ColumnCount = {ColumnCount}|DB.settings.ColumnsCount = {DB.Settings.ColumnsCount}");
        }

        [Fact]
        public void Load_CountClusters()
        {
            int DataCount = 1000;
            int InClusters = 100;

            var DB = DBM.LoadDB(GenerateBase_3_1000Data.Path, GenerateBase_3_1000Data.LoadKey(), 1);

            Assert.True(DataCount/InClusters == DB.Settings.CountClusters, $"ColumnClusters = {DataCount / InClusters}|DB.settings.CountClusters = {DB.Settings.CountClusters}");
        }


        [Fact]
        public void Load_LoadedSavers()
        {
            var DB = DBM.LoadDB(GenerateBase_3_1000Data.Path, GenerateBase_3_1000Data.LoadKey(), 1);

            Assert.NotNull(DB.DataBaseSaver);
        }

        [Fact]
        public void Load_LoadedLoader()
        {
            var DB = DBM.LoadDB(GenerateBase_3_1000Data.Path, GenerateBase_3_1000Data.LoadKey(), 1);

            Assert.NotNull(DB.DataBaseLoader);
        }

        [Fact]
        public void Load_LoadedRemover()
        {
            var DB = DBM.LoadDB(GenerateBase_3_1000Data.Path, GenerateBase_3_1000Data.LoadKey(), 1);

            Assert.NotNull(DB.DataBaseReplayser);
        }


        [Fact]
        public void Load_LoadedLoger()
        {
            var DB = DBM.LoadDB(GenerateBase_3_1000Data.Path, GenerateBase_3_1000Data.LoadKey(), 1);

            Assert.NotNull(DB.DataBaseLoger);
        }
    }
}
