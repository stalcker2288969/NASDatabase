using nas = NASDataBaseAPI.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data.DataBaseSettings;
using System.Data;

namespace NASAPITests.DataBase.Base
{
    public class DataBase_TestAddData_4Column_100InSector
    {
        public const int ColumnCount = 4;
        
        [Fact]
        public void TestAddData_1()
        {
            var DB = new DataBaseManager().CreateDataBase(new nas.DataBaseSettings.DataBaseSettings("DataBase_TestAddData_4Column_100InSector", "D:\\", 4, 100));

            DB.DataBaseSaver = new DataBaseLoader();

            DB.AddData("TestData1", "TestData2", "TestData3", "TestData4");

            Assert.Equal(1u, 
                new DataBaseManager()
                .LoadDB(DB.Settings.Path, DB.Settings.Key)
                .Settings.CountBuckets);
        }

        [Fact]
        public void TestAddData_100()
        {
            var DB = new DataBaseManager().CreateDataBase(new nas.DataBaseSettings.DataBaseSettings("DataBase_TestAddData_4Column_100InSector", "D:\\", 4, 100));

            for (int i = 0; i < 100; i++)
            {
                DB.AddData("TestData1", "TestData2", "TestData3", "TestData4");
            }

            Assert.Equal(100u,
                 new DataBaseManager()
                .LoadDB(DB.Settings.Path, DB.Settings.Key)
                .Settings.CountBuckets);
        }
    }
}
