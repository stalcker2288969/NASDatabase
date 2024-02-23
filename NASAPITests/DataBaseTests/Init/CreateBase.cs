using NASDataBaseAPI.Server.Data.DataBaseSettings;
using NASDataBaseAPI.Server.Data.Safety;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NASAPITests.DataBaseTests.Init
{

    public class CreateBase
    {
        [Fact]
        public void CreateBase_1Column_BaseManager()
        {
            DataBaseManager DBM = new DataBaseManager();

            var DB = DBM.CreateDataBase(new DataBaseSettings("BMTest1", "D:\\", SimpleEncryptor.GenerateRandomKey(128)
            , 1));

            Assert.NotNull(DB);
            Assert.NotNull(DB.DataBaseLoader);
            Assert.NotNull(DB.DataBaseLoger);
            Assert.NotNull(DB.DataBaseReplayser);
            Assert.NotNull(DB.DataBaseSaver);
            Assert.True(DB.Columns.Count == 1);
            Assert.True(File.Exists("D:\\BMTest1\\Settings\\Settings.txt"));
        }

        [Fact]
        public void CreateBase_2Column_BaseManager()
        {
            int x = 2;
            DataBaseManager DBM = new DataBaseManager();

            var DB = DBM.CreateDataBase(new DataBaseSettings("BMTest1", "D:\\", SimpleEncryptor.GenerateRandomKey(128)
            , (uint)x));

            Assert.NotNull(DB);
            Assert.NotNull(DB.DataBaseLoader);
            Assert.NotNull(DB.DataBaseLoger);
            Assert.NotNull(DB.DataBaseReplayser);
            Assert.NotNull(DB.DataBaseSaver);
            Assert.True(DB.Columns.Count == x);
            Assert.True(File.Exists("D:\\BMTest1\\Settings\\Settings.txt"));
        }


        [Fact]
        public void CreateBase_10Column_BaseManager()
        {
            int x = 10;
            DataBaseManager DBM = new DataBaseManager();

            var DB = DBM.CreateDataBase(new DataBaseSettings("BMTest1", "D:\\", SimpleEncryptor.GenerateRandomKey(128)
            , (uint)x));

            Assert.NotNull(DB);
            Assert.NotNull(DB.DataBaseLoader);
            Assert.NotNull(DB.DataBaseLoger);
            Assert.NotNull(DB.DataBaseReplayser);
            Assert.NotNull(DB.DataBaseSaver);
            Assert.True(DB.Columns.Count == x);
            Assert.True(File.Exists("D:\\BMTest1\\Settings\\Settings.txt"));
        }

        [Fact]
        public void CreateBase_20Column_BaseManager()
        {
            int x = 20;
            DataBaseManager DBM = new DataBaseManager();

            var DB = DBM.CreateDataBase(new DataBaseSettings("BMTest1", "D:\\", SimpleEncryptor.GenerateRandomKey(128)
            , (uint)x));

            Assert.NotNull(DB);
            Assert.NotNull(DB.DataBaseLoader);
            Assert.NotNull(DB.DataBaseLoger);
            Assert.NotNull(DB.DataBaseReplayser);
            Assert.NotNull(DB.DataBaseSaver);
            Assert.True(DB.Columns.Count == x);
            Assert.True(File.Exists("D:\\BMTest1\\Settings\\Settings.txt"));
        }
    }
}
