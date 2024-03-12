using NASDatabase.Data.DataTypesInColumn;
using NASDatabase.Interfaces;
using NASDatabase.Server.Data.Modules;
using NASDatabase.Server.Data.Safety;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace NASDatabase.Server.Data.DatabaseSettings.Loaders
{
    /// <summary>
    /// Подгружает частями базу данных
    /// </summary>
    public class DatabaseLoader : ILoader
    {
        public const string NumberColumnsAndTheirTypesDoNotMatchInNumber = "Кол-во столбцов и их типы не совпадают по количеству!";
        /// <summary>
        /// Шифрование данных
        /// </summary>
        public IEncoder _Encoder { get; private set; }
        public IFileWorker FileSystem { get; private set; }

        public DatabaseLoader() 
        { 
            _Encoder = new SimpleEncryptor();
            FileSystem = new BaseFileWorker();
        }

        public DatabaseLoader(IEncoder Encoder)
        {
            this._Encoder = Encoder;
            FileSystem = new BaseFileWorker();
        }

        public DatabaseLoader(IEncoder Encoder, IFileWorker fileWorker)
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
        public virtual AColumn[] LoadCluster(DatabaseSettings dataBaseSettings, uint ClusterNumber)
        {
            return LoadCluster(dataBaseSettings.Path, ClusterNumber, dataBaseSettings.Key);
        }

        /// <summary>
        /// Загружает кластер, нужно понимать что если база данных хранится в одном файле он будет декодировать всю базу, а вернет только нужный кластер (Очень долгий процесс) 
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="ClusterNumber"></param>
        /// <returns></returns>
        public virtual AColumn[] LoadCluster(string Path, uint ClusterNumber, string DecodeKey)
        {   
            if (ClusterNumber == 0)
                ClusterNumber = 1;

            List<AColumn> tables = new List<AColumn>();

            DatabaseSettings dataBaseSettings = JsonSerializer.Deserialize<DatabaseSettings>(_Encoder.Decode(FileSystem.ReadAllText(Path + "\\Settings\\Settings.txt"), DecodeKey));
            dataBaseSettings.Key = DecodeKey;

            string[] SettingsTables = FileSystem.ReadAllLines(Path + "\\Settings\\TablesType.txt");
            
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
                Lines = _Encoder.Decode(FileSystem.ReadAllText(Path + $"\\Cluster{ClusterNumber}.txt")
                    .TrimEnd('\n', '\r'), dataBaseSettings.Key)
                    .Split(new char[] { '\n' },StringSplitOptions.RemoveEmptyEntries);
            }
            catch
            {
                try
                {
                    _Encoder.Decode(FileSystem.ReadAllText(Path + $"\\Cluster{ClusterNumber}.txt").TrimEnd('\n','\r'), dataBaseSettings.Key).Split('\n');
                }
                catch
                {
                    try
                    {
                        FileSystem.ReadAllText(Path + $"\\Cluster{ClusterNumber}.txt");
                    }
                    catch
                    {
                        if (ClusterNumber != 0)
                            FileSystem.WriteAllText(" ", Path + $"\\Cluster{ClusterNumber}.txt");
                        Lines = new string[0];
                    }
                }              
            }         
            
            string ID = "0";            

            if (SettingsTables.Length != dataBaseSettings.ColumnsCount) throw new Exception(NumberColumnsAndTheirTypesDoNotMatchInNumber);

            for (int i = 0; i < dataBaseSettings.ColumnsCount; i++)
            {
                tables.Add(new Column(Names[i], DataTypesInColumns.GetType(Types[i]), dataBaseSettings.CountBucketsInSector * (ClusterNumber - 1)));
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

        public virtual void AddElement(DatabaseSettings DataBaseSettings, uint ClusterNumber, ItemData[] itemDatas)
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

            var l = _Encoder.Decode(FileSystem.ReadAllText(DataBaseSettings.Path + $"\\Cluster{ClusterNumber}.txt"), DataBaseSettings.Key);

            str.AddRange(l.Split('\n'));

            str.Add(stringBuilder.ToString());
            
            FileSystem.WriteAllText(_Encoder.Encode(string.Join("\n",str.ToArray()),DataBaseSettings.Key), DataBaseSettings.Path + $"\\Cluster{ClusterNumber}.txt");
            string Content = JsonSerializer.Serialize<DatabaseSettings>(DataBaseSettings);
            FileSystem.WriteAllText(_Encoder.Encode(Content, DataBaseSettings.Key), DataBaseSettings.Path + "\\Settings\\Settings.txt");
        }

        public virtual void ReplayesElement(DatabaseSettings DatabaseSettings, uint ClusterNumber, ItemData[] itemDatas)
        {
            if (ClusterNumber == 0)
                ClusterNumber = 1;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(itemDatas[0].ID.ToString());
            foreach (var i in itemDatas)
            {
                stringBuilder.Append($"|/*\\|{i.Data}");
            }

            string[] lines = _Encoder.Decode(FileSystem.ReadAllText(DatabaseSettings.Path + $"\\Cluster{ClusterNumber}.txt"),DatabaseSettings.Key).Split('\n');
            lines[itemDatas[0].ID] = stringBuilder.ToString();
            stringBuilder.Clear();

            string result = _Encoder.Encode(string.Join("\n", lines), DatabaseSettings.Key);
            FileSystem.WriteAllText(result, DatabaseSettings.Path + $"\\Cluster{ClusterNumber}.txt");
        }

        public virtual void SaveAllCluster(DatabaseSettings DatabaseSettings, uint ClusterNumber, AColumn[] Column)
        {
            if (ClusterNumber == 0)
                ClusterNumber = 1;
            uint i = DatabaseSettings.CountBucketsInSector * (ClusterNumber-1);
            StringBuilder stringBuilder = new StringBuilder();
            
            string lines = "";
            
            for(int u =0; u < Column.Length; u++)
            {
                lines += Column[u].TypeOfData.Name + "|" + Column[u].Name +"\n"; 
            }

            FileSystem.WriteAllText(lines, DatabaseSettings.Path + "\\Settings\\TablesType.txt");
            //Запоминаем типы столбцов

            string Content = JsonSerializer.Serialize<DatabaseSettings>(DatabaseSettings);
            FileSystem.WriteAllText(_Encoder.Encode(Content,DatabaseSettings.Key), DatabaseSettings.Path + "\\Settings\\Settings.txt");

            int x = Column[0].GetCounts();
            for (uint g = 0; g < x; g++)
            {
                stringBuilder.Append((g+i).ToString());
                foreach(var t in Column)
                {
                    stringBuilder.Append($"|/*\\|{t.GetDatas()[g]}");
                }
                stringBuilder.Append("\n");
            }
            FileSystem.WriteAllText(_Encoder.Encode(stringBuilder.ToString(),DatabaseSettings.Key), DatabaseSettings.Path + $"\\Cluster{ClusterNumber}.txt");
        }    
    }
}
