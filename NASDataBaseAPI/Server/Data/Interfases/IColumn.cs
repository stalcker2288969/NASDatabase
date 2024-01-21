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
        int FindID(string data);
        int[] FindIDs(string data);
        string FindDataByID(int id);
        bool Push(string data, uint CountBoxes);
        bool Pop(string data);
        bool TryPopByIDAndData(ItemData itemData);
        void PopByID(int id);
        void ClearBoxes();
        int GetCounts();
        ItemData[] GetDatas();
    }
}
