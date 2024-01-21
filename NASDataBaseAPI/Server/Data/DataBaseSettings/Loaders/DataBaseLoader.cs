using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using NASDataBaseAPI.Server.Data.Safety;
using NASDataBaseAPI.Data.DataTypesInColumn;
using NASDataBaseAPI.Data;
using NASDataBaseAPI.Server.Data.Interfases;

namespace NASDataBaseAPI.Server.Data.DataBaseSettings
{
    /// <summary>
    /// Подгружает частями базу данных
    /// </summary>
    public class DataBaseLoader : IDataBaseSaver
    {
        /// <summary>
        /// Шифрование данных
        /// </summary>
        private IEncoder _Encoder;

        public DataBaseLoader() 
        { 
            _Encoder = new SimpleEncryptor();
        }

        public DataBaseLoader(IEncoder Encoder)
        {
            this._Encoder = Encoder;
        }
        /// <summary>
        /// Загружает кластер, нужно понимать что если база данных хранится в одном файле он будет декодировать всю базу, а вернет только нужный кластер (Очень долгий процесс) 
        /// Ошибки:
        /// Exception("При декодирование была выявлена ошибка! ")
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ClusterNumber"></param>
        /// <returns></returns>
        public Column[] LoadCluster(string path, uint ClusterNumber, string DecodeKey)
        {
            if (ClusterNumber == 0)
                ClusterNumber = 1;

            List<Column> tables = new List<Column>();

            DataBaseSettings dataBaseSettings = JsonSerializer.Deserialize<DataBaseSettings>(File.ReadAllText(path + "\\Settings\\Settings.txt"));
            

            string[] SettingsTables = File.ReadAllLines(path + "\\Settings\\TablesType.txt");
            
            string[] Types = new string[SettingsTables.Length];
            string[] Names = new string[SettingsTables.Length];

            for(int i = 0; i < SettingsTables.Length; i++)
            {
                string[] data = SettingsTables[i].Split('|');
                Types[i] = data[0];
                Names[i] = data[1];
            }

            string[] Lines = new string[0];

            try
            {
                Lines = _Encoder.Encode(File.ReadAllText(path + $"\\Cluster{ClusterNumber}.txt", Encoding.UTF8).TrimEnd('\n', '\r'), dataBaseSettings.Key).Split('\n');
            }
            catch
            {
                try
                {
                    _Encoder.Encode(File.ReadAllText(path + $"\\Cluster{ClusterNumber}.txt", Encoding.UTF8).TrimEnd('\n','\r'), dataBaseSettings.Key).Split('\n');
                }
                catch(Exception ex)
                {
                    throw new Exception($"При декодирование была выявлена ошибка! {ex.Message}");
                }              
                finally
                {
                    try
                    {
                        File.ReadAllText(path + $"\\Cluster{ClusterNumber}.txt");
                    }
                    catch
                    {
                        if (ClusterNumber != 0)
                            File.WriteAllText(path + $"\\Cluster{ClusterNumber}.txt", " ");
                    }
                }
            }         
            
            string ID = "0";            

            if (SettingsTables.Length != dataBaseSettings.ColumnsCount) throw new Exception("Кол-во столбцов и их типы не совпадают по количеству!");

            for (int i = 0; i < dataBaseSettings.ColumnsCount; i++)
            {
                tables.Add(new Column(Names[i], DataTypesInColumns.GetType(Types[i]), dataBaseSettings.CountBucketsInSector * (ClusterNumber-1)));
            }

            foreach (var l in Lines)
            {
                string[] boxes = l.Split(new string[] { "|/*\\|" }, StringSplitOptions.RemoveEmptyEntries);

                if (boxes.Length > tables.Count)
                {
                    string[] newBoxes = new string[tables.Count+1];
                    int counter = 0;
                    for (int i = 0; i < boxes.Length; i++)
                    {
                        if (boxes[i] != "")
                        {
                            newBoxes[counter] = boxes[i];
                            counter++;
                        }
                    }
                    boxes = newBoxes;    
                }
                ID = boxes[0];
                try
                {
                    for (int i = 1; i < boxes.Length; i++)
                    {
                        tables[i - 1].Push(boxes[i], Convert.ToInt32(ID));
                    }
                }
                catch
                {
                    throw new Exception($"Ошиба при попытки чтения ID - {ID}");
                }
            }

            return tables.ToArray();
        }

        public void AddElement(DataBaseSettings dataBaseSettings, uint ClusterNumber, ItemData[] itemDatas)
        {
            if (ClusterNumber == 0)
                ClusterNumber = 1;
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(itemDatas[0].IDInTable.ToString());
            foreach(var i in itemDatas)
            {
                stringBuilder.Append($"|/*\\|{i.Data}");
            }

            List<string> str = new List<string>();

            var l = File.ReadAllLines(dataBaseSettings.Path + $"\\Cluster{ClusterNumber}.txt");

            str = l.ToList();

            str.Add(stringBuilder.ToString());//Тут ошибка хотя itemDatas.Linght = 4 
            
            File.WriteAllText(dataBaseSettings.Path + $"\\Cluster{ClusterNumber}.txt", _Encoder.Encode(string.Join("\n",str.ToArray()),dataBaseSettings.Key));
            string Content = JsonSerializer.Serialize<DataBaseSettings>(dataBaseSettings);
            File.WriteAllText(dataBaseSettings.Path + "\\Settings\\Settings.txt", Content);
        }

        public void ReplayesElement(DataBaseSettings dataBaseSettings, uint ClusterNumber, ItemData[] itemDatas)
        {
            if (ClusterNumber == 0)
                ClusterNumber = 1;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(itemDatas[0].IDInTable.ToString());
            foreach (var i in itemDatas)
            {
                stringBuilder.Append($"|/*\\|{i.Data}");
            }

            string[] lines = _Encoder.Encode(File.ReadAllText(dataBaseSettings.Path + $"\\Cluster{ClusterNumber}.txt"),dataBaseSettings.Key).Split('\n');
            lines[itemDatas[0].IDInTable] = stringBuilder.ToString();
            stringBuilder.Clear();

            string result = _Encoder.Encode(string.Join("\n", lines), dataBaseSettings.Key);
            File.WriteAllText(dataBaseSettings.Path + $"\\Cluster{ClusterNumber}.txt", result);
        }

        public void SaveAllCluster(DataBaseSettings dataBaseSettings, uint ClusterNumber, Column[] tables)
        {
            if (ClusterNumber == 0)
                ClusterNumber = 1;
            uint i = dataBaseSettings.CountBucketsInSector * (ClusterNumber-1);
            StringBuilder stringBuilder = new StringBuilder();
            
            string lines = "";
            
            for(int u =0; u < tables.Length; u++)
            {
                lines += tables[u].DataType.Name + "|" + tables[u].Name +"\n";
                File.WriteAllText(dataBaseSettings.Path + "\\Settings\\TablesType.txt", lines);
            }            //Запоминаем типы столбцов
           
            string Content = JsonSerializer.Serialize<DataBaseSettings>(dataBaseSettings);
            File.WriteAllText(dataBaseSettings.Path + "\\Settings\\Settings.txt", Content);

            int x = tables[0].GetCounts();
            for (uint g = 0; g < x; g++)
            {
                stringBuilder.Append((g+i).ToString());
                foreach(var t in tables)
                {
                    stringBuilder.Append($"|/*\\|{t.GetDatas()[g]}");
                }
                stringBuilder.Append("\n");
            }
            File.WriteAllText(dataBaseSettings.Path + $"\\Cluster{ClusterNumber}.txt", _Encoder.Encode(stringBuilder.ToString(),dataBaseSettings.Key));
        }
    }
}
