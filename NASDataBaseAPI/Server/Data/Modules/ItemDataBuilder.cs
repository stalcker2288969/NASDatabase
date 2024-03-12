using NASDatabase.Interfaces;
using System;

namespace NASDatabase.Server.Data.Modules
{
    public static class ItemDataBuilder
    {
        public static ItemData[] GetItemDatas(int ID, string[] Data)
        {
            ItemData[] itemDatas = new ItemData[Data.Length];

            for (int i = 0; i < Data.Length; i++)
            {
                itemDatas[i] = new ItemData(ID, Data[i]);
            }

            return itemDatas;
        }

        public static T GetDataLine<T>(int ID, string[] Data) where T : IDatRows, new() 
        {
            T DL;
 
            DL = Activator.CreateInstance<T>();         
            
            DL?.Init(ID, Data);
            return DL;
        }
    }
}
