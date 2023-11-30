﻿using NASDataBaseAPI.Data;
using NASDataBaseAPI.Data.DataTypesInColumn;
using NASDataBaseAPI.Server.Data;
using NASDataBaseAPI.Server.Data.Interfases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NASDataBaseAPI.Data
{
    /// <summary>
    /// Класс распределяющий данные по столбцу
    /// </summary>
    public class Column : IColumn
    {
        #region Ивенты
        public event Action<int> _DeleteData;
        public event Action<ItemData> _AddData;
        public event Action<DataType> _ChangType;
        public event Action<ItemData[]> _SetDatas;
        #endregion

        public DataType DataType { get; private set; }
        public string Name { get; set; }

        public uint OffSet { get;private set; }

        private HashTable<ItemData> boxes = new HashTable<ItemData>();
       
        #region конструкторы
        public Column(string Name, HashTable<ItemData> boxes, DataType dataType, uint offSet) 
        {
            this.Name = Name;
            this.boxes = boxes;
            this.DataType = dataType;
            this.OffSet = offSet;
        }
        public Column(string Name, DataType dataType, uint offSet)
        {
            this.Name = Name;
            this.DataType = dataType;
            this.OffSet = offSet;
        }
        public Column(string Name, uint offSet)
        {
            this.Name = Name;
            this.DataType = DataTypesInColumns.Text;
            OffSet = offSet;
        }
        public Column(string Name)
        {
            this.Name= Name;
            this.DataType = DataTypesInColumns.Text;
            OffSet = 0;
        }
        #endregion

        #region Суттеры
        /// <summary>
        /// Очищает данные и записывает новые 
        /// </summary>
        /// <param name="datas"></param>
        public bool SetDatas(ItemData[] datas)
        {
            bool result = false;
            try
            {
                ClearBoxes();
                boxes = new HashTable<ItemData>();
                foreach (var d in datas)
                {
                    boxes.AddElement(d);
                }
                _SetDatas?.Invoke(datas);
                result = true;
                
            }
            catch
            {

            }
            
            return result;
        }

        /// <summary>
        /// Заменяет данные на позиции айди, true - удалось false - нет 
        /// </summary>
        /// <param name="newData"></param>
        /// <param name="ID"></param>
        public bool SetDataByID(ItemData newData)
        {
            bool result = false;
            try
            {
                if (DataType.TryConvert(newData.Data) || newData.Data == " ")
                {
                    ItemData OldData = boxes.GetValues()[newData.IDInTable - (int)OffSet];
                    boxes.GetValues()[newData.IDInTable] = newData;//Замена данных
                    
                    boxes.RemoveNotData(OldData);//Хеширование без повторного добавления в таблицу
                    boxes.AddNotData(newData);

                    result = true;
                }
                else
                {
                    throw new Exception("Нельзя преобразовать введенные данные");
                }
            }
            catch
            {

            }
            return result;
        }
        #endregion

        #region Поиск
        /// <summary>
        /// Возвращает позицию в табличке первого совпавшего, если нет, то -1
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int FindID(string data)
        {
            if (!DataType.TryConvert(data))
            {
                return -1;
            }
            try
            {
                ulong key = (ulong)boxes.StringHashCode20(data);
                int x = (int)(key % boxes.CountBuckets);

                ItemData iD = boxes.GetFirstElementByKey((int)x);
                if(iD != null)
                {
                    int id = boxes.GetFirstElementByKey((int)x).IDInTable;
                    return id;
                }
                else
                {
                    return -1;
                }
                
            }
            catch 
            {
                return -1; 
            }
            
        }

        /// <summary>
        /// Возвращает айдишники всех ячеек в табличке в которых есть эти данные 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int[] FindIDs(string data)
        {
            List<int> id = new List<int>();
            if (DataType.TryConvert(data))
            {
                ulong key = (ulong)boxes.StringHashCode20(data.ToString());
                int x = (int)(key % boxes.CountBuckets);

                ItemData[] datas = boxes.GetElementsByKey(x);
                foreach (var d in datas)
                {
                    if (d?.Data == data)
                    {
                        id.Add(d.IDInTable);
                    }
                }
            }                  
           
            return id.ToArray();
        }
        
        /// <summary>
        /// Возвращает данные по id, если данных нет то возвращает: " ".
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string FindDataByID(int id)
        {
            try
            {
                return boxes.GetValues()[id - (int)OffSet]?.Data ?? " ";
            }
            catch
            {
                return " ";
            }
        }
        #endregion

        #region Добавление/Удаление 
        /// <summary>
        /// Добавляет данные в конец 
        /// </summary>
        /// <param name="data"></param>
        public bool Push(string data, uint CountBoxes)
        {
            bool result = false;
            if (DataType.TryConvert(data))
            {
                boxes.AddElement(new ItemData((int)CountBoxes, data));
                _AddData?.Invoke(new ItemData((int)CountBoxes, data));
                result = true;
            }
            return result;
        }

        public bool Push(string data, int ID)
        {
            bool result = false;
            if (DataType.TryConvert(data))
            {
                boxes.AddElement(new ItemData(ID, data));
                _AddData?.Invoke(new ItemData(ID, data));
                result = true;
            }          
            return result;
        }


        /// <summary>
        /// Удаляет первый совпавший элемент
        /// </summary>
        /// <param name="name"></param>
        public bool Pop(string data) 
        {
            int MixElement = -1;
            ItemData[] itemDatas = boxes.GetElementsByKey(boxes.StringHashCode20(data));
            
            bool result = false;
            if (DataType.TryConvert(data))
            {
                result = itemDatas != null;

                foreach (var d in itemDatas)
                {
                    if (d.IDInTable < MixElement)
                    {
                        MixElement = d.IDInTable;
                    }
                }
                boxes.RemoveElement(new ItemData(MixElement, data));
                _DeleteData?.Invoke(MixElement);        
            }
            return result;
        }
        /// <summary>
        /// Удаляет нужные данные по айди, если данные совпали и показывает удалось это сделать или нет 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public bool TryPopByIDAndData(ItemData itemData)
        {
            bool result = false;
            if (boxes.GetValues()[itemData.IDInTable - (int)OffSet].Data == itemData?.Data)
            {               
                boxes.RemoveElement(itemData);
                _DeleteData?.Invoke(itemData.IDInTable);
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Удаляет данные по айди 
        /// </summary>
        /// <param name="id"></param>
        public void PopByID(int id) 
        {
            boxes.RemoveElement(boxes.GetValues()[id - (int)OffSet]);
            _DeleteData?.Invoke(id);
        }

        /// <summary>
        /// Удаляет все данные из столбца/Не реализовано 
        /// </summary>
        public void ClearBoxes()
        {       
            boxes.Clear();
            boxes = new HashTable<ItemData>();
        }
        #endregion

        #region Вспомогательные методы
        /// <summary>
        /// Возвращает длину столбца 
        /// </summary>
        /// <returns></returns>
        public int GetCounts()
        {
            return boxes.GetValues().Count;
        }

        public ItemData[] GetDatas()
        {
            return boxes.GetValues().ToArray();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var d in boxes.GetValues())
            {
                sb.Append(d.IDInTable.ToString() +"/*\\|" + d.Data);
            }
            return sb.ToString();
        }

        /// <summary>
        /// !!Очищает себя и меняет тип! 
        /// </summary>
        /// <param name="type"></param>
        public void ChangType(DataType type)
        {
            ClearBoxes();
            DataType = type;
            _ChangType?.Invoke(type);
        }
        #endregion

        #region Операторы
        public string this[int id]
        {
            get
            {
                return boxes.GetValues()[id- (int)OffSet].Data;
            }
            set
            {
                this.boxes.GetValues()[id - (int)OffSet] = new ItemData(id,value);
            }
        }


        public static bool operator ==(Column left, Column right)
        {
            return left.DataType == right.DataType && left.Name == right.Name;
        }

        public static bool operator !=(Column left, Column right)
        {
            return left.DataType != right.DataType && left.Name != right.Name;
        }
        #endregion
    }

}
