using NASDataBaseAPI.Data;
using NASDataBaseAPI.Data.DataTypesInColumn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.Interfases.Column
{
    public interface IColumn
    {
        string Name { get; }
        DataType DataType { get; }
        uint OffSet { get; }

        bool SetDatas(ItemData[] datas);
        bool SetDataByID(ItemData newData);

        int FindID(string data);
        int[] FindIDs(string data);
        string FindDataByID(int id);

        bool Push(string data, uint CountBoxes);
        bool Pop(string data);
        bool TryPopByIDAndData(ItemData itemData);
        void PopByID(int id);

        void ChangType(DataType type);
        void ClearBoxes();
        int GetCounts();
        ItemData[] GetDatas();
    }
}
