using NASDataBaseAPI.Server.Data.Safety;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.DataBaseSettings
{
    /// <summary>
    /// Отвечает за настроку базы данных, а именно фармат хронения данных и прочии вещи под копотом
    /// </summary>
    public struct DataBaseSettings
    {
        #region Settings
        public string Name { get; internal set; }
        public string Path { get; internal set; }
        public string Key;
        public uint CountBucketsInSector { get; internal set; }
        public uint CountBuckets { get; internal set; }
        public uint ColumnsCount { get; internal set; }
        public uint CountClusters { get; internal set; }
        public bool Logs { get; internal set; }
        /// <summary>
        /// Тип мода сохранения данных(безопасные, небезопасный -  программист сам решает когда отработать сохранению) !Изменять тип только в классе DataBase через спец метод! 
        /// </summary>
        public bool SaveMod { get; set; } 
        #endregion

        [JsonConstructor]
        public DataBaseSettings(string name, string path, uint countBucketsInSector, uint countBuckets, uint ColumnsCount, uint countClusters, bool Logs, bool SaveMod)
        {          
            this.Name = name;
            this.Path = path;
            this.CountBucketsInSector = countBucketsInSector;
            this.CountBuckets = countBuckets;
            this.ColumnsCount = ColumnsCount;
            this.CountClusters = countClusters;
            this.Logs = Logs;
            this.SaveMod = SaveMod;
            this.Key = "";
        }

        public DataBaseSettings(string name, string path,string key, uint countBucketsInSector, uint countBuckets, uint ColumnsCount, uint countClusters, bool Logs, bool SaveMod)
        {
            this.Name = name;
            this.Path = path;
            this.CountBucketsInSector = countBucketsInSector;
            this.CountBuckets = countBuckets;
            this.ColumnsCount = ColumnsCount;
            this.CountClusters = countClusters;
            this.Logs = Logs;
            this.SaveMod = SaveMod;
            this.Key = key;
        }

        public DataBaseSettings(string name, string path, string key, uint ColumnsCount, uint CountBucketsInSector = 1000000, bool Logs = false, bool SaveMod = true) 
        {
            this.Name = name;
            this.Path = path;
            this.Key = key;
            this.ColumnsCount = ColumnsCount;
            this.CountBucketsInSector = CountBucketsInSector;
            this.CountBuckets = 0;
            this.CountClusters = 0;
            this.Logs = Logs;
            this.SaveMod = SaveMod;
        }

        public DataBaseSettings(string name, string path, uint ColumnsCount = 4, uint CountBucketsInSector = 1000000, bool Logs = false, bool SaveMod = true)
        {
            this.Name = name;
            this.Path = path;
            this.Key = SimpleEncryptor.GenerateRandomKey(128);
            this.ColumnsCount = ColumnsCount;
            this.CountBucketsInSector = CountBucketsInSector;
            this.CountBuckets = 0;
            this.CountClusters = 0;
            this.Logs = Logs;
            this.SaveMod = SaveMod;
        }

        public DataBaseSettings(DataBaseSettings settings, bool SaveMod)
        {
            this.Name = settings.Name;
            this.Path = settings.Path;
            this.Key = settings.Key;
            this.CountBucketsInSector = settings.CountBucketsInSector;
            this.CountBuckets = settings.CountBuckets;
            this.ColumnsCount = settings.ColumnsCount;
            this.CountClusters = settings.CountClusters;
            this.Logs = settings.Logs;
            this.SaveMod = SaveMod;
        }
    }
}
