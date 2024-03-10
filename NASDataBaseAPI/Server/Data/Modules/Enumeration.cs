using NASDataBaseAPI.Interfaces;
using System;


namespace NASDataBaseAPI.Server.Data.Modules
{
    /// <summary>
    /// Инструмент для переборки и обработки элементов базы 
    /// </summary>
    public class Enumeration<T> : IEnumerationHandler, IDisposable where T : DataBase
    {
        private T _dataBase;

        public Enumeration(T dataBase)
        {
            this._dataBase = dataBase;
        }

        public void ForLine(Action<BaseLine> Handler)
        {
            for(int i = 0; i < _dataBase.Settings.CountBuckets; i++) 
            {
                Handler?.Invoke(_dataBase.GetDataLineByID<BaseLine>(i));
            }
        }

        public void ForLine<T1>(Action<T1> Handler) where T1 : IDataLine
        {
            for (int i = 0; i < _dataBase.Settings.CountBuckets; i++)
            {
                Handler?.Invoke(_dataBase.GetDataLineByID<T1>(i));
            }
        }

        public void ForBoxes(string InColumn, Action<ItemData> Handler)
        {
            ForBoxes(_dataBase[InColumn], Handler);
        }

        public void ForBoxes(Interfaces.AColumn inAColumn, Action<ItemData> Handler)
        {
            for (int i = 0; i < _dataBase.Settings.CountBuckets; i++)
            {
                Handler?.Invoke(_dataBase.GetDataByParams(inAColumn.Name, i));
            }
        }

        public void ForBoxes<T1>(T1 InColumn, Action<ItemData> Handler) where T1 : Interfaces.AColumn
        {
            for (int i = 0; i < _dataBase.Settings.CountBuckets; i++)
            {
                Handler?.Invoke(_dataBase.GetDataByParams(InColumn.Name, i));
            }
        }

        public void Dispose()
        {
            
        }
    }
}
