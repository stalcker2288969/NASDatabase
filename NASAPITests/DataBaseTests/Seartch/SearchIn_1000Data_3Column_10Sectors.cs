using NASDataBaseAPI.Server.Data;
using NASDataBaseAPI.Server.Data.DataBaseSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NASAPITests.DataBaseTests.Seartch
{
    public class SearchIn_1000Data_3Column_10Sectors
    {
        DataBaseManager manager = new DataBaseManager();

        string[] Names = { "Tom", "Bob", "Tim", "Kek", "Artemy" };
        Random rand = new Random();

        public DataBase Init()
        {
            var DB = manager.CreateDataBase(new DataBaseSettings("Init_SearchIn_1000Data_3Column_10Sectors", "D:\\",3, 100));

            DB.AddData("UwU", "-1", "uWu");

            for (int i = 0; i < 999; i++)
            {
                DB.AddData(Names[rand.Next(Names.Length)], i.ToString(), Names[rand.Next(Names.Length)]);
            }

            return DB;
        }

        [Fact]
        public void GetIDByData()
        {
            var db = Init();
            Assert.True(db.GetIDByParams("0","UwU") == 0, "ID now: " + db.GetIDByParams("0", "UwU"));
        }

        [Fact]
        public void GetDataByID()
        {
            var db = Init();
            Assert.True(db.GetDataByID(db.GetIDByParams("0", "UwU"))[1] == "-1");
        }
    }
}
