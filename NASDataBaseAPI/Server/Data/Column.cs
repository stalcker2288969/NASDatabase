using NASDataBaseAPI.Data.DataTypesInColumn;
using NASDataBaseAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NASDataBaseAPI.Server.Data
{
    /// <summary>
    /// Класс распределяющий данные по столбцу
    /// </summary>
    public class Column : AColumn, IDisposable
    {
        #region Ивенты
        public event Action<int> _DeleteData;
        public event Action<ItemData> _AddData;
        public event Action<TypeOfData> _ChangType;
        public event Action<ItemData[]> _SetDatas;
        #endregion

        private HashTable<ItemData> boxes = new HashTable<ItemData>();

        private bool _initialized = false;
        
        #region конструкторы
        public Column(string Name, HashTable<ItemData> boxes, TypeOfData dataType, uint offSet)
        {
            this.Name = Name;
            this.boxes = boxes;
            this.TypeOfData = dataType;
            this.Offset = offSet;
        }
        public Column(string Name, TypeOfData dataType, uint offSet)
        {
            this.Name = Name;
            this.TypeOfData = dataType;
            this.Offset = offSet;
        }
        public Column(string Name, uint offSet)
        {
            this.Name = Name;
            this.TypeOfData = DataTypesInColumns.Text;
            Offset = offSet;
        }
        public Column(string Name)
        {
            this.Name = Name;
            this.TypeOfData = DataTypesInColumns.Text;
            Offset = 0;
        }

        public Column()
        {
            this.Name = "";
            this.TypeOfData = DataTypesInColumns.Text;
            Offset = 0;
        }

        #endregion

        #region Сеттеры
        /// <summary>
        /// Очищает данные и записывает новые 
        /// </summary>
        /// <param name="datas"></param>
        public override bool SetDatas(ItemData[] datas)
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
                ClearBoxes();
            }

            return result;
        }

        /// <summary>
        /// Заменяет данные на позиции айди, true - удалось false - нет 
        /// </summary>
        /// <param name="newData"></param>
        /// <param name="ID"></param>
        public override bool SetDataByID(ItemData newData)
        {
            bool result = false;
            try
            {
                if (TypeOfData.CanConvert(newData.Data) || newData.Data == " ")
                {
                    ItemData OldData = boxes.GetValues()[newData.ID - (int)Offset];
                    boxes.GetValues()[newData.ID - (int)Offset] = newData;//Замена данных

                    boxes.RemoveNotData(OldData);//Хеширование без повторного добавления в таблицу
                    boxes.AddNotData(newData);

                    result = true;
                }
                else
                {
                    throw new ArgumentException("Нельзя преобразовать введенные данные");
                }
            }
            catch(IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException($"Попытка взять элемент по ID: {newData.ID}, но данные физически с учетом offset {Offset} не загруженны!");
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
        public override int FindID(string data)
        {
            if (!TypeOfData.CanConvert(data))
            {
                return -1;
            }
            try
            {
                ulong key = (ulong)boxes.StringHashCode20(data);
                int x = (int)(key % boxes.CountBuckets);

                ItemData _id = boxes.GetFirstElementByKey((int)x);
                if (_id != null)
                {
                    return _id.ID;
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
        public override int[] FindIDs(string data)
        {
            List<int> id = new List<int>();
            if (TypeOfData.CanConvert(data))
            {
                ulong key = (ulong)boxes.StringHashCode20(data.ToString());
                int x = (int)(key % boxes.CountBuckets);

                ItemData[] datas = boxes.GetElementsByKey(x);
                foreach (var d in datas)
                {
                    if (d.Data == data)
                    {
                        id.Add(d.ID);
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
        public override string FindDataByID(int id)
        {
            try
            {
                return boxes.GetValues()[id - (int)Offset].Data ?? " ";
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException($"Попытка взять элемент по ID: {id}, но данные физически с учетом offset {Offset} не загруженны!");
            }
        }
        #endregion

        #region Добавление/Удаление 
        /// <summary>
        /// Добавляет данные в конец 
        /// </summary>
        /// <param name="data"></param>
        public override bool Push(string data, uint CountBoxes)
        {
            bool result = false;
            if (TypeOfData.CanConvert(data))
            {
                boxes.AddElement(new ItemData((int)CountBoxes, data));
                _AddData?.Invoke(new ItemData((int)CountBoxes, data));
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Добавляет данные в связке с ID
        /// </summary>
        public override bool Push(string data, int ID)
        {
            bool result = false;
            if (TypeOfData.CanConvert(data) && FindID(data) != ID && FindDataByID(ID) == " ")
            {
                boxes.AddElement(new ItemData(ID, data));
                _AddData?.Invoke(new ItemData(ID, data));
                result = true;
            }
            return result;
        }

        public bool Push(ItemData data)
        {
            bool result = false;
            if (TypeOfData.CanConvert(data.Data) && FindID(data.Data) != data.ID && FindDataByID(data.ID) == " ")
            {
                boxes.AddElement(data);
                _AddData?.Invoke(data);
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Удаляет первый совпавший элемент
        /// </summary>
        /// <param name="name"></param>
        public override bool Pop(string data)
        {
            int ID = -1;
            ItemData[] itemDatas = boxes.GetElementsByKey(boxes.StringHashCode20(data));

            bool result = false;
            if (TypeOfData.CanConvert(data))
            {
                result = itemDatas != null;

                foreach (var d in itemDatas)
                {
                    if (d.ID < ID)
                    {
                        ID = d.ID;
                    }
                }
                boxes.RemoveElement(new ItemData(ID, data));
                _DeleteData?.Invoke(ID);
            }
            return result;
        }
        /// <summary>
        /// Удаляет нужные данные по айди, если данные совпали и показывает удалось это сделать или нет 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public override bool TryPopByIDAndData(ItemData itemData)
        {
            bool result = false;
            if (boxes.GetValues()[itemData.ID - (int)Offset].Data == itemData.Data)
            {
                boxes.RemoveElement(itemData);
                _DeleteData?.Invoke(itemData.ID);
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Удаляет данные по айди 
        /// </summary>
        /// <param name="id"></param>
        public override void PopByID(int id)
        {
            boxes.RemoveElement(boxes.GetValues()[id - (int)Offset]);
            _DeleteData?.Invoke(id);
        }

        /// <summary>
        /// Удаляет все данные из столбца
        /// </summary>
        public override void ClearBoxes()
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
        public override int GetCounts()
        {
            return boxes.GetValues().Count;
        }

        public override ItemData[] GetDatas()
        {
            return boxes.GetValues().ToArray();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var d in boxes.GetValues())
            {
                sb.Append(d.ID.ToString() + "/*\\|" + d.Data);
            }
            return sb.ToString();
        }

        /// <summary>
        /// !!Очищает себя и меняет тип! 
        /// </summary>
        /// <param name="type"></param>
        public override void ChangType(TypeOfData type)
        {
            ClearBoxes();
            TypeOfData = type;
            _ChangType?.Invoke(type);
        }

        public TypeOfData GetDataType()
        {
            return TypeOfData;
        }

        public string GetDataTypeName()
        {
            return Name;
        }

        public void Dispose()
        {
            boxes.Dispose();
        }
        #endregion

        #region Операторы
        public string this[int id]
        {
            get
            {
                return boxes.GetValues()[id - (int)Offset].Data;
            }
            set
            {
                this.boxes.GetValues()[id - (int)Offset] = new ItemData(id, value);
            }
        }

        public static bool operator ==(Column left, Column right)
        {
            return left.TypeOfData == right.TypeOfData;
        }

        public static bool operator !=(Column left, Column right)
        {
            return left.TypeOfData != right.TypeOfData;
        }
        #endregion

        public override void Init(string Name, TypeOfData dataType, uint offSet)
        {
            if (_initialized)
                throw new Exception("Столбец уже проинициализирован!");
            this.Name = Name;
            this.TypeOfData = dataType;
            this.Offset = offSet;
            _initialized = true;
        }
    }
}
