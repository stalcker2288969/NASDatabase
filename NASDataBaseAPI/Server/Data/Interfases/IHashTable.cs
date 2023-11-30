using System.Collections.Generic;

namespace NASDataBaseAPI.Server.Data.Interfases
{
    public interface IHashTable<T>
    {
        T GetFirstElementByKey(int code);
        bool HasElementByKeyAndData(int code, T data);
        T[] GetElementsByKey(int code);
        bool HasElement(T value);
        bool HasElement(T value, ref int code);
        void AddElement(T value);             
        bool TryReplacementByKey(T newData, int Key);
        bool TryReplacementByKeyAndOldData(T newData, T OldData, int Key);
        void RemoveElement(T value);
        List<T> GetValues();
        void Clear();
    }
}
