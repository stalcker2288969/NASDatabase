using NASDataBaseAPI.Data.DataTypesInColumn;
using NASDataBaseAPI.Server.Data;

namespace NASDataBaseAPI.Interfaces
{
    public interface IColumn
    {
        string Name { get; set; }
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

        void Init(string Name, DataType dataType, uint Offset);
    }
}
