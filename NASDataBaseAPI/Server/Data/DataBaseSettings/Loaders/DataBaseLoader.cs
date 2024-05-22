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
        public Interfaces.AFileWorker FileSystem { get; private set; }

        public DatabaseLoader() 
        { 
            _Encoder = new SimpleEncryptor();
            FileSystem = new Modules.FileWorker();
        }

        public DatabaseLoader(IEncoder encoder)
        {
            this._Encoder = encoder;
            FileSystem = new Modules.FileWorker();
        }

        public DatabaseLoader(IEncoder encoder, Interfaces.AFileWorker fileWorker)
        {
            this._Encoder = encoder;
            FileSystem = fileWorker;
        }

        /// <summary>
        /// Загружает кластер, нужно понимать что если база данных хранится в одном файле он будет декодировать всю базу, а вернет только нужный кластер (Очень долгий процесс) 
        /// </summary>
        /// <param name="databaseSettings"></param>
        /// <param name="clusterNumber"></param>
        /// <returns></returns>
        public virtual AColumn[] LoadCluster(DatabaseSettings databaseSettings, uint clusterNumber)
        {
            return LoadCluster(databaseSettings.Path, clusterNumber, databaseSettings.Key);
        }

        /// <summary>
        /// Загружает кластер, нужно понимать что если база данных хранится в одном файле он будет декодировать всю базу, а вернет только нужный кластер (Очень долгий процесс) 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="clusterNumber"></param>
        /// <returns></returns>
        public virtual AColumn[] LoadCluster(string path, uint clusterNumber, string decodeKey)
        {   
            if (clusterNumber == 0)
                clusterNumber = 1;

            List<AColumn> tables = new List<AColumn>();

            DatabaseSettings dataBaseSettings = JsonSerializer.Deserialize<DatabaseSettings>(_Encoder.Decode(FileSystem.ReadAllText(path + "\\Settings\\Settings.txt"), decodeKey));
            dataBaseSettings.Key = decodeKey;

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
                Lines = _Encoder.Decode(FileSystem.ReadAllText(path + $"\\Cluster{clusterNumber}.txt")
                    .TrimEnd('\n', '\r'), dataBaseSettings.Key)
                    .Split(new char[] { '\n' },StringSplitOptions.RemoveEmptyEntries);
            }
            catch
            {
                try
                {
                    _Encoder.Decode(FileSystem.ReadAllText(path + $"\\Cluster{clusterNumber}.txt").TrimEnd('\n','\r'), dataBaseSettings.Key).Split('\n');
                }
                catch
                {
                    try
                    {
                        FileSystem.ReadAllText(path + $"\\Cluster{clusterNumber}.txt");
                    }
                    catch
                    {
                        if (clusterNumber != 0)
                            FileSystem.WriteAllText(" ", path + $"\\Cluster{clusterNumber}.txt");
                        Lines = new string[0];
                    }
                }              
            }         
            
            string ID = "0";            

            if (SettingsTables.Length != dataBaseSettings.ColumnsCount) throw new Exception(NumberColumnsAndTheirTypesDoNotMatchInNumber);

            for (int i = 0; i < dataBaseSettings.ColumnsCount; i++)
            {
                tables.Add(new Column(Names[i], DataTypesInColumns.GetBaseTypeOfData(Types[i]), dataBaseSettings.CountBucketsInSector * (clusterNumber - 1)));
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

        public virtual void AddElement(DatabaseSettings databaseSettings, uint clusterNumber, ItemData[] itemDatas)
        {
            if (clusterNumber == 0)
                clusterNumber = 1;
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(itemDatas[0].ID.ToString());
            foreach(var i in itemDatas)
            {
                stringBuilder.Append($"|/*\\|{i.Data}");
            }

            List<string> str = new List<string>();

            var l = _Encoder.Decode(FileSystem.ReadAllText(databaseSettings.Path + $"\\Cluster{clusterNumber}.txt"), databaseSettings.Key);

            str.AddRange(l.Split('\n'));

            str.Add(stringBuilder.ToString());
            
            FileSystem.WriteAllText(_Encoder.Encode(string.Join("\n",str.ToArray()),databaseSettings.Key), databaseSettings.Path + $"\\Cluster{clusterNumber}.txt");
            string Content = JsonSerializer.Serialize<DatabaseSettings>(databaseSettings);
            FileSystem.WriteAllText(_Encoder.Encode(Content, databaseSettings.Key), databaseSettings.Path + "\\Settings\\Settings.txt");
        }

        public virtual void ReplayesElement(DatabaseSettings DatabaseSettings, uint clusterNumber, ItemData[] itemDatas)
        {
            if (clusterNumber == 0)
                clusterNumber = 1;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(itemDatas[0].ID.ToString());
            foreach (var i in itemDatas)
            {
                stringBuilder.Append($"|/*\\|{i.Data}");
            }

            string[] lines = _Encoder.Decode(FileSystem.ReadAllText(DatabaseSettings.Path + $"\\Cluster{clusterNumber}.txt"),DatabaseSettings.Key).Split('\n');
            lines[itemDatas[0].ID] = stringBuilder.ToString();
            stringBuilder.Clear();

            string result = _Encoder.Encode(string.Join("\n", lines), DatabaseSettings.Key);
            FileSystem.WriteAllText(result, DatabaseSettings.Path + $"\\Cluster{clusterNumber}.txt");
        }

        public virtual void SaveAllCluster(DatabaseSettings DatabaseSettings, uint clusterNumber, AColumn[] column)
        {
            if (clusterNumber == 0)
                clusterNumber = 1;
            uint i = DatabaseSettings.CountBucketsInSector * (clusterNumber-1);
            StringBuilder stringBuilder = new StringBuilder();
            
            string lines = "";
            
            for(int u =0; u < column.Length; u++)
            {
                lines += column[u].TypeOfData.Name + "|" + column[u].Name +"\n"; 
            }

            FileSystem.WriteAllText(lines, DatabaseSettings.Path + "\\Settings\\TablesType.txt");
            //Запоминаем типы столбцов

            string Content = JsonSerializer.Serialize<DatabaseSettings>(DatabaseSettings);
            FileSystem.WriteAllText(_Encoder.Encode(Content,DatabaseSettings.Key), DatabaseSettings.Path + "\\Settings\\Settings.txt");

            int x = column[0].GetCounts();
            for (uint g = 0; g < x; g++)
            {
                stringBuilder.Append((g+i).ToString());
                foreach(var t in column)
                {
                    stringBuilder.Append($"|/*\\|{t.GetDatas()[g]}");
                }
                stringBuilder.Append("\n");
            }
            FileSystem.WriteAllText(_Encoder.Encode(stringBuilder.ToString(),DatabaseSettings.Key), DatabaseSettings.Path + $"\\Cluster{clusterNumber}.txt");
        }    
    }
}
