using NASDataBaseAPI.Server.Data.Interfases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data
{
    public class BaseLine : IDataLine
    {
        private string[] Datas;

        public BaseLine(int ID, string[] datas)
        {
            Datas = datas;
            this.ID = ID;
        }

        public int ID { get; private set; }

        public string[] GetData()
        {
            return Datas;
        }
    }
}
