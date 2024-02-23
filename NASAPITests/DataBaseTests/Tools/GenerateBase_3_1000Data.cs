using NASDataBaseAPI.Server.Data.DataBaseSettings;
using NASDataBaseAPI.Server.Data.Safety;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASAPITests.DataBaseTests.Tools
{
    public class GenerateBase_3_1000Data
    {
        public static string Path = "D:\\TestUTest";

        [Fact]
        public void Generate()
        {
            int ColumnCount = 3;
            int DataCount = 1000;
            int InClusters = 100;

            if(Directory.Exists(Path))
            {
                Directory.Delete(Path, true);
            }

            string[] datas = { "Tom", "Bob", "Tad" };

            DataBaseManager DBM = new DataBaseManager();

            var DB = DBM.CreateDataBase(new DataBaseSettings("TestUTest", "D:\\", SimpleEncryptor.GenerateRandomKey(128)
             , (uint)ColumnCount, CountBucketsInSector: (uint)InClusters));

            Random rnd = new Random();

            for (int i = 0; i < DataCount; i++)
            {
                DB.AddData(new string[3] { datas[rnd.Next(3)], datas[rnd.Next(3)], datas[rnd.Next(3)] });
            }
        }
    }
}
