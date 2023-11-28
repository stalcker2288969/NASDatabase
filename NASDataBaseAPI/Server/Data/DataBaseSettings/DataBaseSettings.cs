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
    public class DataBaseSettings
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Key { get; set; }
        public uint CountBucketsInSector { get; set; } = 1000000;
        public uint CountBuckets { get; set; }
        public uint ColumnsCount { get; set; } = 5;
        public uint CountClusters { get; set; } = 1;
        public bool Logs { get; set; } = true;

        public DataBaseSettings(string name, string path, string key, uint countBucketsInSector, uint countBuckets, uint tablesCount, uint countClusters)
        {
            Name = name;
            Path = path;
            Key = key;
            CountBucketsInSector = countBucketsInSector;
            CountBuckets = countBuckets;
            ColumnsCount = tablesCount;
            CountClusters = countClusters;     
        }
    }

    public enum TypeСlusterSeparation
    {
        None,
        Tables,
        Chunks,
    }

    public enum DataStorageType
    {
        OneFile,
        Сlusters,
    }

    public enum BucketRatio
    {
        None,
        OneToOne,
        TwoToOne,
        ManyToOne
    }
}
