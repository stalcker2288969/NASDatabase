using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using NASDataBaseAPI.Server.Data.Safety;
using NASDataBaseAPI.Data.DataTypesInColumn;
using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data.Modules;

namespace NASDataBaseAPI.Server.Data.DataBaseSettings
{
    /// <summary>
    /// Подгружает частями базу данных
    /// </summary>
    public class DataBaseLoader : ILoader
    {
        public const string NumberColumnsAndTheirTypesDoNotMatchInNumber = "Кол-во столбцов и их типы не совпадают по количеству!";
        /// <summary>
        /// Шифрование данных
        /// </summary>
        public IEncoder _Encoder { get; private set; }
        public IFileWorker FileSystem { get; private set; }

        public DataBaseLoader() 
        { 
            _Encoder = new SimpleEncryptor();
            FileSystem = new BaseFileWorker();
        }

        public DataBaseLoader(IEncoder Encoder)
        {
            this._Encoder = Encoder;
            FileSystem = new BaseFileWorker();
        }

        public DataBaseLoader(IEncoder Encoder, IFileWorker fileWorker)
        {
            this._Encoder = Encoder;
            FileSystem = fileWorker;
        }

        /// <summary>
        /// Загружает кластер, нужно понимать что если база данных хранится в одном файле он будет декодировать всю базу, а вернет только нужный кластер (Очень долгий процесс) 
        /// </summary>
        /// <param name="dataBaseSettings"></param>
        /// <param name="ClusterNumber"></param>
        /// <returns></returns>
        public virtual IColumn[] LoadCluster(DataBaseSettings dataBaseSettings, uint ClusterNumber)
        {
            return LoadCluster(dataBaseSettings.Path, ClusterNumber, dataBaseSettings.Key);
        }

        /// <summary>
        /// Загружает кластер, нужно понимать что если база данных хранится в одном файле он будет декодировать всю базу, а вернет только нужный кластер (Очень долгий процесс) 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ClusterNumber"></param>
        /// <returns></returns>
        public virtual IColumn[] LoadCluster(string path, uint ClusterNumber, string DecodeKey)
        {   
            if (ClusterNumber == 0)
                ClusterNumber = 1;

            List<Column> tables = new List<Column>();

            DataBaseSettings dataBaseSettings = JsonSerializer.Deserialize<DataBaseSettings>(_Encoder.Decode(FileSystem.ReadAllText(path + "\\Settings\\Settings.txt"), DecodeKey));
            dataBaseSettings.Key = DecodeKey;

            string[] SettingsTables = FileSystem.ReadAllLines(path + "\\Settings\\TablesType.txt");
            
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
                Lines = _Encoder.Decode(FileSystem.ReadAllText(path + $"\\Cluster{ClusterNumber}.txt")
                    .TrimEnd('\n', '\r'), dataBaseSettings.Key)
                    .Split(new char[] { '\n' },StringSplitOptions.RemoveEmptyEntries);
            }
            catch
            {
                try
                {
                    _Encoder.Decode(FileSystem.ReadAllText(path + $"\\Cluster{ClusterNumber}.txt").TrimEnd('\n','\r'), dataBaseSettings.Key).Split('\n');
                }
                catch
                {
                    try
                    {
                        FileSystem.ReadAllText(path + $"\\Cluster{ClusterNumber}.txt");
                    }
                    catch
                    {
                        if (ClusterNumber != 0)
                            FileSystem.WriteAllText(" ", path + $"\\Cluster{ClusterNumber}.txt");
                        Lines = new string[0];
                    }
                }              
            }         
            
            string ID = "0";            

            if (SettingsTables.Length != dataBaseSettings.ColumnsCount) throw new Exception(NumberColumnsAndTheirTypesDoNotMatchInNumber);

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
                        tables[i - 1].Push(boxes[i], (UInt32)Convert.ToInt32(ID));
                    }
                }
                catch
                {
                    throw new Exception($"Ошиба при попытки чтения ID - {ID}");
                }
            }

            return tables.ToArray();
        }

        public virtual void AddElement(DataBaseSettings dataBaseSettings, uint ClusterNumber, ItemData[] itemDatas)
        {
            if (ClusterNumber == 0)
                ClusterNumber = 1;
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(itemDatas[0].ID.ToString());
            foreach(var i in itemDatas)
            {
                stringBuilder.Append($"|/*\\|{i.Data}");
            }

            List<string> str = new List<string>();

            var l = _Encoder.Decode(FileSystem.ReadAllText(dataBaseSettings.Path + $"\\Cluster{ClusterNumber}.txt"), dataBaseSettings.Key);

            str.AddRange(l.Split('\n'));

            str.Add(stringBuilder.ToString());
            
            FileSystem.WriteAllText(_Encoder.Encode(string.Join("\n",str.ToArray()),dataBaseSettings.Key), dataBaseSettings.Path + $"\\Cluster{ClusterNumber}.txt");
            string Content = JsonSerializer.Serialize<DataBaseSettings>(dataBaseSettings);
            FileSystem.WriteAllText(_Encoder.Encode(Content, dataBaseSettings.Key), dataBaseSettings.Path + "\\Settings\\Settings.txt");
        }

        public virtual void ReplayesElement(DataBaseSettings dataBaseSettings, uint ClusterNumber, ItemData[] itemDatas)
        {
            if (ClusterNumber == 0)
                ClusterNumber = 1;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(itemDatas[0].ID.ToString());
            foreach (var i in itemDatas)
            {
                stringBuilder.Append($"|/*\\|{i.Data}");
            }

            string[] lines = _Encoder.Decode(FileSystem.ReadAllText(dataBaseSettings.Path + $"\\Cluster{ClusterNumber}.txt"),dataBaseSettings.Key).Split('\n');
            lines[itemDatas[0].ID] = stringBuilder.ToString();
            stringBuilder.Clear();

            string result = _Encoder.Encode(string.Join("\n", lines), dataBaseSettings.Key);
            FileSystem.WriteAllText(result, dataBaseSettings.Path + $"\\Cluster{ClusterNumber}.txt");
        }

        public virtual void SaveAllCluster(DataBaseSettings dataBaseSettings, uint ClusterNumber, IColumn[] tables)
        {
            if (ClusterNumber == 0)
                ClusterNumber = 1;
            uint i = dataBaseSettings.CountBucketsInSector * (ClusterNumber-1);
            StringBuilder stringBuilder = new StringBuilder();
            
            string lines = "";
            
            for(int u =0; u < tables.Length; u++)
            {
                lines += tables[u].DataType.Name + "|" + tables[u].Name +"\n"; 
            }

            FileSystem.WriteAllText(lines, dataBaseSettings.Path + "\\Settings\\TablesType.txt");
            //Запоминаем типы столбцов

            string Content = JsonSerializer.Serialize<DataBaseSettings>(dataBaseSettings);
            FileSystem.WriteAllText(_Encoder.Encode(Content,dataBaseSettings.Key), dataBaseSettings.Path + "\\Settings\\Settings.txt");

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
            FileSystem.WriteAllText(_Encoder.Encode(stringBuilder.ToString(),dataBaseSettings.Key), dataBaseSettings.Path + $"\\Cluster{ClusterNumber}.txt");
        }    
    }
}
