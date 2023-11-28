using NASDataBaseAPI.Server.Data.Interfases;
using NASDataBaseAPI.Data;
using System;
using System.IO;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Data.SqlTypes;
using NASDataBaseAPI.Server.Data.Safety;

namespace NASDataBaseAPI.Server.Data.DataBaseSettings
{
    /// <summary>
    /// Отвечает за упровление базой данных (базовый функционал)  
    /// </summary>
    public static class DataBaseManager
    {
        public static IDataBaseSaver loader = new DataBaseLoader();
        
        /// <summary>
        /// Меняет ключь и перезаписывает базу
        /// </summary>
        /// <param name="NewKey"></param>
        /// <param name="Path"></param>
        public static void ChengKey(string NewKey, string Path)
        {
            DataBaseSettings dataBaseSettings = JsonSerializer.Deserialize<DataBaseSettings>(File.ReadAllText(Path + "\\Settings\\Settings.txt"));
            
            for(int i = 0; i < dataBaseSettings.CountClusters; i++)
            {
               File.WriteAllText(dataBaseSettings.Path + $"\\Cluster{i + 1}.txt", SimpleEncryptor.Encrypt(SimpleEncryptor.Decrypt(File.ReadAllText(dataBaseSettings.Path + $"\\Cluster{i + 1}.txt"), dataBaseSettings.Key),
                    NewKey));
            }
            dataBaseSettings.Key = NewKey;
            File.WriteAllText(dataBaseSettings.Path + "\\Settings\\Settings.txt", JsonSerializer.Serialize<DataBaseSettings>(dataBaseSettings));
        }
        /// <summary>
        /// Удоляет всю базу по указанному пути
        /// </summary>
        /// <param name="Path"></param>
        public static void DeliteDataBase(string Path)
        {
            try
            {
                string[] lines = File.ReadAllLines(Path + "\\Settings\\FreeIDs.txt");
            }
            catch { return; }
            
            Directory.Delete(Path, true);
        }
        /// <summary>
        /// Создает базу данных и задает ей начальные настройки
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Key"></param>
        /// <param name="dataBaseSettings"></param>
        public static DataBase CreateDataBase(string Path, DataBaseSettings dataBaseSettings)
        {
            Directory.CreateDirectory(Path + dataBaseSettings.Name);
            Directory.CreateDirectory(Path + dataBaseSettings.Name + "\\Settings");
            dataBaseSettings.Path = Path + dataBaseSettings.Name;
            string Content = JsonSerializer.Serialize<DataBaseSettings>(dataBaseSettings);
            File.WriteAllText(Path + dataBaseSettings.Name + "\\Settings\\Settings.txt", Content);
            
            string Types = "";
            for(int i = 0;i< dataBaseSettings.ColumnsCount; i++)
            {
                Types += $"Text|{i}\n";
            }
            File.WriteAllText(Path + dataBaseSettings.Name + "\\Settings\\TablesType.txt", Types);
           
            DataBase dataBase = new DataBase((int)dataBaseSettings.ColumnsCount, dataBaseSettings);
            dataBase.settings = dataBaseSettings;

            dataBase.DataBaseLoger = new DataBaseLoger(dataBaseSettings, "Loger");

            dataBase.DataBaseSaver = loader;//задаем загрусщик по умолчанию
            File.WriteAllText(Path + dataBaseSettings.Name + $"\\Cluster1.txt"," ");
            return dataBase;
        }
        /// <summary>
        /// Сохраняет состояние базы данных
        /// </summary>
        /// <param name="dataBase"></param>
        public static void SaveStatesDataBase(DataBase dataBase)
        {
            string Content = JsonSerializer.Serialize<DataBaseSettings>(dataBase.settings);
            File.WriteAllText(dataBase.settings.Path + "\\Settings\\Settings.txt", Content);
            uint[] FreeIDs = dataBase.freeIDs.ToArray();
            string[] lines = new string[FreeIDs.Length];

            for(int i = 0; i < FreeIDs.Length; i++)
            {
                lines[i] = FreeIDs[i].ToString();   
            }
            File.WriteAllLines(dataBase.settings.Path + "\\Settings\\FreeIDs.txt", lines);
        }

        public static void SaveStatesDataBase(DataBaseSettings settings)
        {
            string Content = JsonSerializer.Serialize<DataBaseSettings>(settings);
            File.WriteAllText(settings.Path + "\\Settings\\Settings.txt", Content);            
        }

        /// <summary>
        /// Если LoadCluster = -1 => по умолчанию ничто в оперативную память не грузится
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="LoadCluster"></param>
        /// <returns></returns>
        public static DataBase LoadDB(string Path, int LoadCluster = -1)
        {
            DataBaseSettings dataBaseSettings = JsonSerializer.Deserialize<DataBaseSettings>(File.ReadAllText(Path+"\\Settings\\Settings.txt"));           
            DataBase dataBase = new DataBase((int)dataBaseSettings.ColumnsCount, dataBaseSettings);
            
            try
            {
                //Загрузка свободных ID-ков
                string[] lines = File.ReadAllLines(dataBaseSettings.Path + "\\Settings\\FreeIDs.txt");
                uint[] FreeIDs = new uint[lines.Length];

                for (int i = 0; i < FreeIDs.Length; i++)
                {
                    FreeIDs[i] = Convert.ToUInt32(lines[i]);
                }

                dataBase.freeIDs.AddRange(FreeIDs);
            }
            catch { File.WriteAllText(dataBaseSettings.Path + "\\Settings\\FreeIDs.txt", ""); }

            if (LoadCluster != -1)
            {
                dataBase.Columns.Clear();
                dataBase.Columns.AddRange(loader.LoadCluster(dataBaseSettings.Path, (uint)LoadCluster, dataBaseSettings.Key));
            }
            else
            {
                string[] SettingsTables = File.ReadAllLines(Path + "\\Settings\\TablesType.txt");

                string[] Types = new string[SettingsTables.Length];
                string[] Names = new string[SettingsTables.Length];

                for (int i = 0; i < SettingsTables.Length; i++)
                {
                    string[] data = SettingsTables[i].Split('|');
                    Types[i] = data[0];
                    Names[i] = data[1];
                }
                dataBase.Columns.Clear();
                for (int i = 0; i < dataBaseSettings.ColumnsCount; i++)
                {
                    dataBase.Columns.Add(new Column(Names[i], DataBaseLoader.GetType(Types[i]), 0));
                }
            }
            dataBase.DataBaseSaver = loader;
            dataBase.DataBaseLoger = new DataBaseLoger(dataBaseSettings, "Loger");
            return dataBase;
        }

    }
}
