using NASDataBaseAPI.Server.Data.DataBaseSettings;
using NASDataBaseAPI.Server.Data.Safety;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASAPITests.DataBaseTests.Init.Column_1
{
    public class CreateDataBase_1_ColumnAnd_10_AddData__SafeMode
    {
        [Fact]
        public void Test_SettingsFile()
        {
            int ColumnCount = 1;
            int DataCount = 10;
            int InClusters = 100;

            Directory.Delete("D:\\BMTest1", true);

            string[] datas = { "Tom", "Bob", "Tad" };

            DataBaseManager DBM = new DataBaseManager();

            var DB = DBM.CreateDataBase(new DataBaseSettings("BMTest1", "D:\\", SimpleEncryptor.GenerateRandomKey(128)
             , (uint)ColumnCount, CountBucketsInSector: (uint)InClusters));

            Random rnd = new Random();

            for (int i = 0; i < DataCount; i++)
            {
                DB.AddData(new string[1] { datas[rnd.Next(3)] });
            }

            //Assert.True(DB.Columns.Count == ColumnCount);
            //Assert.True(DB.settings.CountClusters == ClustersCount);
            //Assert.True(DB.settings.CountBuckets == DataCount);
            //Assert.True(DB.settings.CountBucketsInSector == (uint)InClusters);
            Assert.True(File.Exists("D:\\BMTest1\\Settings\\Settings.txt"));
        }

        [Fact]
        public void Test_CountBucketsInSector()
        {
            int ColumnCount = 1;
            int DataCount = 10;
            int InClusters = 100;

            Directory.Delete("D:\\BMTest1", true);

            string[] datas = { "Tom", "Bob", "Tad" };

            DataBaseManager DBM = new DataBaseManager();

            var DB = DBM.CreateDataBase(new DataBaseSettings("BMTest1", "D:\\", SimpleEncryptor.GenerateRandomKey(128)
             , (uint)ColumnCount, CountBucketsInSector: (uint)InClusters));

            Random rnd = new Random();

            for (int i = 0; i < DataCount; i++)
            {
                DB.AddData(new string[1] { datas[rnd.Next(3)] });
            }

            //Assert.True(DB.Columns.Count == ColumnCount);
            //Assert.True(DB.settings.CountClusters == ClustersCount);
            //Assert.True(DB.settings.CountBuckets == DataCount);
            Assert.True(DB.Settings.CountBucketsInSector == (uint)InClusters, $"InClusters = {InClusters}|CountBucketsInSector = {DB.Settings.CountBucketsInSector}");
            //Assert.True(File.Exists("D:\\BMTest1\\Settings\\Settings.txt"));
        }

        [Fact]
        public void Test_CountBuckets()
        {
            int ColumnCount = 1;
            int DataCount = 10;
            int InClusters = 100;

            Directory.Delete("D:\\BMTest1", true);

            string[] datas = { "Tom", "Bob", "Tad" };

            DataBaseManager DBM = new DataBaseManager();

            var DB = DBM.CreateDataBase(new DataBaseSettings("BMTest1", "D:\\", SimpleEncryptor.GenerateRandomKey(128)
             , (uint)ColumnCount, CountBucketsInSector: (uint)InClusters));

            Random rnd = new Random();

            for (int i = 0; i < DataCount; i++)
            {
                DB.AddData(new string[1] { datas[rnd.Next(3)] });
            }

            //Assert.True(DB.Columns.Count == ColumnCount);
            //Assert.True(DB.settings.CountClusters == ClustersCount);
            Assert.True(DB.Settings.CountBuckets == DataCount);
            //Assert.True(DB.settings.CountBucketsInSector == (uint)InClusters);
            //Assert.True(File.Exists("D:\\BMTest1\\Settings\\Settings.txt"));
        }

        [Fact]
        public void Test_CountClusters()
        {
            int ColumnCount = 1;
            int DataCount = 10;
            int ClustersCount = 1;
            int InClusters = 100;

            Directory.Delete("D:\\BMTest1", true);

            string[] datas = { "Tom", "Bob", "Tad" };

            DataBaseManager DBM = new DataBaseManager();

            var DB = DBM.CreateDataBase(new DataBaseSettings("BMTest1", "D:\\", SimpleEncryptor.GenerateRandomKey(128)
             , (uint)ColumnCount, CountBucketsInSector: (uint)InClusters));

            Random rnd = new Random();

            for (int i = 0; i < DataCount; i++)
            {
                DB.AddData(new string[1] { datas[rnd.Next(3)] });
            }

            Assert.True(DB.Settings.CountClusters == ClustersCount, $"ClustersCount = {ClustersCount}|DB.settings.CountClusters = {DB.Settings.CountClusters}");
        }

        [Fact]
        public void Test_ColumnCount()
        {
            int ColumnCount = 1;
            int DataCount = 10;
            int InClusters = 100;

            Directory.Delete("D:\\BMTest1", true);

            string[] datas = { "Tom", "Bob", "Tad" };

            DataBaseManager DBM = new DataBaseManager();

            var DB = DBM.CreateDataBase(new DataBaseSettings("BMTest1", "D:\\", SimpleEncryptor.GenerateRandomKey(128)
             , (uint)ColumnCount, CountBucketsInSector: (uint)InClusters));

            Random rnd = new Random();

            for (int i = 0; i < DataCount; i++)
            {
                DB.AddData(new string[1] { datas[rnd.Next(3)] });
            }

            Assert.True(DB.Columns.Count == ColumnCount);
            //Assert.True(DB.settings.CountClusters == ClustersCount);
            //Assert.True(DB.settings.CountBuckets == DataCount);
            //Assert.True(DB.settings.CountBucketsInSector == (uint)InClusters);
            //Assert.True(File.Exists("D:\\BMTest1\\Settings\\Settings.txt"));
        }
    }
}
