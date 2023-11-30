using NASDataBaseAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.Interfases
{
    public interface IColumn
    {
        bool SetDatas(ItemData[] datas);
        bool SetDataByID(ItemData newData);
        int FindeID(string data);
        int[] FindeIDs(string data);
        string FindeDataByID(int id);
        bool Push(string data, uint CountBoxes);//Uper
        bool Pop(string data);
        bool TryPopByIDAndData(ItemData itemData);
        void PopByID(int id);
        void ClearBoxes();
        int GetCounts();
        ItemData[] GetDatas();
    }
}
