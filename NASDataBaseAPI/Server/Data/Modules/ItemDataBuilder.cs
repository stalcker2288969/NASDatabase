using NASDataBaseAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.Modules
{
    public static class ItemDataBuilder
    {
        public static ItemData[] GetItemDatas(int ID, string[] data)
        {
            ItemData[] itemDatas = new ItemData[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                itemDatas[i] = new ItemData(ID, data[i]);
            }

            return itemDatas;
        }

        public static T GetDataLine<T>(int ID, string[] data) where T : IDataLine
        {
            T DL;

            try
            {
                DL = Activator.CreateInstance<T>();
            }
            catch(MethodAccessException)
            {
                throw new Exception("IDataLine должен иметь пустой конструктор!");
            }
            
            DL?.Init(ID, data);
            return DL;
        }
    }
}
