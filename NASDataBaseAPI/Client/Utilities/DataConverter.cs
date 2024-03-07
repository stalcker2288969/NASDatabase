using NASDataBaseAPI.Data.DataTypesInColumn;
using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NASDataBaseAPI.Client.Utilities
{
    /// <summary>
    /// Класс отвечающий за базавую конвертацию данных в строку и из нее
    /// </summary>
    public class DataConverter : IDataConverter
    {
        public const string SeparationBetweenDateLine = "\\|/";
        public const string DataSeparationInIDataLine = "^|^";
        public const string DataSeparationInItemData = "%|%";

        public byte[] GetBytes(string v)
        {
            return Encoding.ASCII.GetBytes(v);
        }
     
        public int[] GetInts(string v)
        {
            List<int> ints = new List<int>();   
            int point = 0; 
          
            string value = "";
            foreach ( var i in v )
            {              
                if (i != ',')
                {
                    value += i;
                }
                else
                {
                    ints.Add(int.Parse(value));
                    point++;
                    value = "";
                }
            }
            return ints.ToArray();
        }

        public ItemData GetItemData(string text)
        {            
            string[] strings = text.Split(DataSeparationInItemData.ToCharArray());
            ItemData result;

            try
            {
                result = new ItemData(int.Parse(strings[0]), strings[1]);
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("Неправильно задана строка, не удается прочесть данные!");
            }
            
            
            return result;
        }

        public string ParsItemData(ItemData itemData)
        {
            var datas = itemData.Data;
            StringBuilder sb = new StringBuilder();
            sb.Append(datas[0] + DataSeparationInItemData + datas[1]);

            return sb.ToString();
        }

        public string GetString(byte[] data, int buffer)
        {
            return Encoding.ASCII.GetString(data, 0, buffer);
        }      

        public string ParsInts(int[] v)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var i in v )
            {
                sb.Append(i);
                sb.Append(',');
            }

            return sb.ToString();
        }

        /// <summary>
        /// Получают IColumn без самиз данных 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public T GetColumn<T>(string text) where T : IColumn, new()
        {
            var d = text.Split(DataSeparationInItemData.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            var t = new T();
            t.Init(d[0], DataTypesInColumns.GetTypeOfDataByClassName(d[1]), uint.Parse(d[2]));

            return t;
        }

        /// <summary>
        /// Получают строку из параметров столбца
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string ParsColumn<T>(T column) where T : IColumn
        {
            var sb = new StringBuilder();

            sb.Append(column.Name);
            sb.Append(DataSeparationInItemData);
            sb.Append(column.DataType.GetType().Name);
            sb.Append(DataSeparationInItemData);
            sb.Append(column.OffSet);

            return sb.ToString();
        }

        public T GetDataLine<T>(string text) where T : IDataLine, new()
        {
            string[] strings = text.Split(DataSeparationInIDataLine.ToCharArray());
            IDataLine dataLine;
            
            dataLine = Activator.CreateInstance<T>();


            List<string> Data = new List<string>();

            for (int i = 1; i < strings.Length; i++)
            {
                try
                {
                    if (strings[i] != "")
                        Data.Add(strings[i]);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new ArgumentException("Неудалось конвертировать введенную строку");
                }
            }
            //ID and Data
            dataLine.Init(int.Parse(strings[0]), Data.ToArray());

            return (T)dataLine;
        }

        public string ParsDataLine<T>(T line) where T : IDataLine
        {
            string[] data = line.GetData();
            StringBuilder sb = new StringBuilder();

            sb.Append(line.ID);
            sb.Append(DataSeparationInIDataLine);

            foreach (var i in data)
            {
                sb.Append(i.ToString());
                sb.Append(DataSeparationInIDataLine);
            }
            return sb.ToString();
        }

        public T[] GetDataLines<T>(string text) where T : IDataLine, new()
        {
            List<T> data = new List<T>();
            string[] strings = text.Split(SeparationBetweenDateLine.ToCharArray());
            foreach (var i in strings)
            {
                data.Add(GetDataLine<T>(i));
            }
            return data.ToArray();
        }

        public string ParsDataLines<T>(T[] lines) where T : IDataLine
        {
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                sb.Append(ParsDataLine<T>(line));
                sb.Append(SeparationBetweenDateLine);
            }
            return sb.ToString();
        }
    }
}
