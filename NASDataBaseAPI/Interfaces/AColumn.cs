using NASDataBaseAPI.Data.DataTypesInColumn;
using NASDataBaseAPI.Server.Data;

namespace NASDataBaseAPI.Interfaces
{
    public abstract class AColumn
    {
        public string Name { get; set; }
        public TypeOfData TypeOfData { get; protected set; }
        public uint Offset { get; protected set; }

        public abstract bool SetDatas(ItemData[] datas);
        public abstract bool SetDataByID(ItemData newData);

        public abstract int FindID(string data);
        public abstract int[] FindIDs(string data);
        public abstract string FindDataByID(int id);

        public abstract bool Push(string data, uint CountBoxes);
        public abstract bool Push(string data, int ID);
        public abstract bool Pop(string data);
        public abstract bool TryPopByIDAndData(ItemData itemData);
        public abstract void PopByID(int id);

        public abstract void ChangType(TypeOfData type);
        public abstract void ClearBoxes();
        public abstract int GetCounts();
        public abstract ItemData[] GetDatas();

        public abstract void Init(string Name, TypeOfData dataType, uint Offset);
    }
}
