using NASDataBaseAPI.Server.Data.Interfases;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NASDataBaseAPI.Server.Data
{
    public class HashTable<T> : IDisposable, IHashTable<T>
    {
        public uint CountBuckets { get; private set; } = 10;
        public uint NumberElements { get; private set; }
        public uint BucketRatio = 2;

        private List<List<T>> _hashTable;
        private List<T> _datas = new List<T>();

        public HashTable(T[] values)
        {
            _hashTable = new List<List<T>>();
            for (int i = 0; i < 10; i++)
            {
                _hashTable.Add(new List<T>());
            }

            foreach (var item in values)
            {
                AddElement(item);
            }
        }

        public HashTable()
        {
            _hashTable = new List<List<T>>();
            for (int i = 0; i < 10; i++)
            {
                _hashTable.Add(new List<T>());
            }
        }

        /// <summary>
        /// Передает первый элемент по ключу
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public T GetFirstElementByKey(int code)
        {
            try
            {
                return _hashTable[code][0];
            }
            catch
            {
                return default(T);
            }
        }
        /// <summary>
        /// Проверяет есть ли такой элемент по такому ключу 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool HasElementByKeyAndData(int code, T data)
        {
            foreach (var v in _hashTable[code])
            {
                if (v != null)
                {
                    if (v.Equals(data))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Берет элементы по ключу 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public T[] GetElementsByKey(int code)
        {
            return _hashTable[code].ToArray();
        }
        /// <summary>
        /// Проверяет есть ли такой элемент 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool HasElement(T value)
        {
            ulong key = (ulong)StringHashCode20(value.ToString());
            int x = (int)(key % CountBuckets);
            foreach (var item in _hashTable[x])
            {
                if (item.Equals(value)) return true;
            }
            return false;
        }

        /// <summary>
        /// Проверяет есть ли такой элемент и возвращает его id
        /// </summary>
        /// <param name="value"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool HasElement(T value, ref int code)
        {
            ulong key = (ulong)StringHashCode20(value.ToString());
            int x = (int)(key % CountBuckets);
            int g = 0;
            foreach (var item in _hashTable[x])
            {
                if (item.Equals(value))
                {
                    code = g;
                    return true;
                }
                g++;
            }
            code = 0;
            return false;
        }
        /// <summary>
        /// Добавляет элемент 
        /// </summary>
        /// <param name="value"></param>
        public void AddElement(T value)
        {
            ulong key = (ulong)StringHashCode20(value.ToString());
            int x = (int)(key % CountBuckets);
            _hashTable[x].Add(value);
            NumberElements += 1;
            _datas.Add(value);
            if (CountBuckets < NumberElements)
            {
                OffsetElements();
            }
        }

        /// <summary>
        /// Добавляет данные без логирования 
        /// </summary>
        /// <param name="value"></param>
        public void AddNotData(T value)
        {
            ulong key = (ulong)StringHashCode20(value.ToString());
            int x = (int)(key % CountBuckets);
            _hashTable[x].Add(value);
        }
        /// <summary>
        /// Удаляет данные без логирования
        /// </summary>
        /// <param name="value"></param>
        public void RemoveNotData(T value)
        {
            ulong key = (ulong)StringHashCode20(value.ToString());
            int x = (int)(key % CountBuckets);
            _hashTable[x].Remove(value);
        }

        private void IteratingElements()
        {
            for (int i = 0; i < (uint)_hashTable.Count; i++)
            {
                lock (_hashTable[i])
                {
                    _hashTable[i] = new List<T>();
                }
            }
        }

        private void IteratingElements2()
        {
            for (int i = (int)_hashTable.Count; i < CountBuckets; i++)
            {
                _hashTable.Add(new List<T>());
            }
        }

        /// <summary>
        /// Смещение элементов при достижении лимита 
        /// </summary>
        private void OffsetElements()
        {
            CountBuckets *= BucketRatio;
            T[] values = GetValues().ToArray();

            IteratingElements();
            IteratingElements2();

            NumberElements = 0;
            _datas = new List<T>();
            foreach (var value in values)
            {
                AddElement(value);
            }
        }

        /// <summary>
        /// !Важно что удаляет все присутствующие данные в бакките! Пытается заменить данные по ключу 
        /// </summary>
        /// <param name="newData"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool TryReplacementByKey(T newData, int Key)
        {
            if (_hashTable[Key].Count == 1)
            {
                _hashTable[Key].Clear();
                _hashTable[Key].Add(newData);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Пытается заменить данные по ключу и старым данным
        /// </summary>
        /// <param name="newData"></param>
        /// <param name="OldData"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool TryReplacementByKeyAndOldData(T newData, T OldData, int Key)
        {
            for (int i = 0; i < _hashTable[Key].Count; i++)
            {
                if (_hashTable[Key][i].Equals(OldData))
                {
                    _hashTable[Key].RemoveAt(i);
                    _hashTable[Key].Add(newData);
                }
            }
            return false;
        }

        /// <summary>
        /// Удаляет данные 
        /// </summary>
        /// <param name="value"></param>
        public void RemoveElement(T value)
        {
            if(value != null)
            {
                ulong key = (ulong)StringHashCode20(value.ToString());
                int x = (int)(key % CountBuckets);
                _hashTable[x].Remove(value);
                _datas.Remove(value);
                NumberElements -= 1;
            }
            else
            {
                throw new ArgumentException("Объект не может равняться null! ");
            }
        }

        public List<T> GetValues()
        {
            return _datas;
        }

        /// <summary>
        /// Очищает всю HashTable и задает 10 пустых баккитов 
        /// </summary>
        public void Clear()
        {           
            _datas.Clear();
            _datas = null;
            _datas = new List<T> { };

            _hashTable.Clear();
            _hashTable = null;
            _hashTable = new List<List<T>> { };

            for (int i = 0; i < 10; i++)
            {
                _hashTable.Add(new List<T>());
            }
            CountBuckets = 10;
            NumberElements = 0;
        }

        /// <summary>
        /// Функция хеширования 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int StringHashCode20(string value)
        {
            int num = 352654597;
            int num2 = num;

            for (int i = 0; i < value.Length; i += 4)
            {
                int ptr0 = value[i] << 16;
                if (i + 1 < value.Length)
                    ptr0 |= value[i + 1];

                num = (num << 5) + num + (num >> 27) ^ ptr0;

                if (i + 2 < value.Length)
                {
                    int ptr1 = value[i + 2] << 16;
                    if (i + 3 < value.Length)
                        ptr1 |= value[i + 3];
                    num2 = (num2 << 5) + num2 + (num2 >> 27) ^ ptr1;
                }
            }

            return num + num2 * 1566083941;
        }

        public void Dispose()
        {
           
        }
    }
}
