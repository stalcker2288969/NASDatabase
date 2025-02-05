﻿using NASDataBaseAPI.Interfaces;
using System;


namespace NASDataBaseAPI.Server.Data.Modules
{
    /// <summary>
    /// Инструмент для переборки и обработки элементов базы 
    /// </summary>
    public static class Enumeration<T> where T : Table
    {
        public static void ForLine(T _dataBase, Action<Rows> Handler)
        {
            for(int i = 0; i < _dataBase.Settings.CountBuckets; i++) 
            {
                Handler?.Invoke(_dataBase.GetDataLineByID<Rows>(i));
            }
        }

        public static void ForLine<T1>(T _dataBase, Action<T1> Handler) where T1 : IDataRows
        {
            for (int i = 0; i < _dataBase.Settings.CountBuckets; i++)
            {
                Handler?.Invoke(_dataBase.GetDataLineByID<T1>(i));
            }
        }

        public static void ForBoxes(T _dataBase, string InColumn, Action<ItemData> Handler)
        {
            ForBoxes(_dataBase, _dataBase[InColumn], Handler);
        }

        public static void ForBoxes(T _dataBase, AColumn inAColumn, Action<ItemData> Handler)
        {
            for (int i = 0; i < _dataBase.Settings.CountBuckets; i++)
            {
                Handler?.Invoke(_dataBase.GetDataByParams(inAColumn.Name, i));
            }
        }

        public static void ForBoxes<T1>(T _dataBase, T1 InColumn, Action<ItemData> Handler) where T1 : Interfaces.AColumn
        {
            for (int i = 0; i < _dataBase.Settings.CountBuckets; i++)
            {
                Handler?.Invoke(_dataBase.GetDataByParams(InColumn.Name, i));
            }
        }
    }
}
