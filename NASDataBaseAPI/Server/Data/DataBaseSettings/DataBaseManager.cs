using NASDataBaseAPI.Interfaces;
using System;
using System.Text.Json;
using NASDataBaseAPI.Server.Data.Safety;
using NASDataBaseAPI.Data.DataTypesInColumn;
using NASDataBaseAPI.Server.Data.DataBaseSettings.Loaders;
using NASDataBaseAPI.Server.Data.Modules;

namespace NASDataBaseAPI.Server.Data.DataBaseSettings
{
    /// <summary>
    /// Отвечает за упровление стандратной базой данных (базовый функционал)  
    /// </summary>
    public class DatabaseManager
    {
        public static DataBaseLoader DBLoader { get; private set; } = new DataBaseLoader();
        public static DBNoSaveLoader DBNoSaveLoader { get; private set; } = new DBNoSaveLoader();

        public static IEncoder Encoder { get; private set; } = new SimpleEncryptor();
        public static IFileWorker FileSystem { get; private set; } = new BaseFileWorker();

        public ILoader[] _databaseSavers { get; private set; }
        private IFileWorker _fileSystem;
        private IEncoder _encoder;

        public DatabaseManager()
        {
            _fileSystem = FileSystem;
            _encoder = Encoder;
            Init();
        }

        public DatabaseManager(IFileWorker fileWorker) 
        {            
            _fileSystem = fileWorker;
            _encoder = Encoder;
            Init();
        }

        public DatabaseManager(IFileWorker fileWorker, IEncoder encoder)
        {           
            _fileSystem = fileWorker;
            _encoder = encoder;
            Init();
        }

        private void Init()
        {
            _databaseSavers = new ILoader[2];
            _databaseSavers[0] = new DBNoSaveLoader(_encoder,_fileSystem);
            _databaseSavers[1] = new DataBaseLoader(_encoder, _fileSystem);
        }

        /// <summary>
        /// Меняет ключь и перезаписывает базу
        /// </summary>
        /// <param name="NewKey"></param>
        /// <param name="Path"></param>
        public virtual void ChangeKey(string OldKey, string NewKey, string Path)
        {
            DatabaseSettings dataBaseSettings = JsonSerializer.Deserialize<DatabaseSettings>(Encoder.Decode(_fileSystem.ReadAllText(Path + "\\Settings\\Settings.txt"), OldKey));
            
            for(int i = 0; i < dataBaseSettings.CountClusters; i++)
            {
               _fileSystem.WriteAllText(Encoder.Encode(Encoder.Decode(_fileSystem.ReadAllText(dataBaseSettings.Path + $"\\Cluster{i + 1}.txt"), dataBaseSettings.Key), NewKey), dataBaseSettings.Path + $"\\Cluster{i + 1}.txt");
            }
            dataBaseSettings.Key = NewKey;
            var content = JsonSerializer.Serialize<DatabaseSettings>(dataBaseSettings);
            _fileSystem.WriteAllText(Encoder.Encode(content, NewKey), dataBaseSettings.Path + "\\Settings\\Settings.txt");
        }
        /// <summary>
        /// Удаляет всю базу по указанному пути
        /// </summary>
        /// <param name="Path"></param>
        public virtual void DeliteDatabase(string Path)
        {
            try
            {
                _fileSystem.DeleteDirectory(Path);
            }
            catch { return; }
        }
        
        public virtual T CreateDatabase<T>(DatabaseSettings dataBaseSettings) where T : Table
        {
            dataBaseSettings.CountClusters = dataBaseSettings.CountClusters == 0 ? 1 : dataBaseSettings.CountClusters;

            _fileSystem.CreateDirectory(dataBaseSettings.Path + dataBaseSettings.Name);
            _fileSystem.CreateDirectory(dataBaseSettings.Path + dataBaseSettings.Name + "\\Settings");
            dataBaseSettings.Path = dataBaseSettings.Path + dataBaseSettings.Name;
            string Content = JsonSerializer.Serialize<DatabaseSettings>(dataBaseSettings);
            _fileSystem.WriteAllText(Encoder.Encode(Content, dataBaseSettings.Key), dataBaseSettings.Path + "\\Settings\\Settings.txt");

            string Types = "";
            for (int i = 0; i < dataBaseSettings.ColumnsCount; i++)
            {
                Types += $"Text|{i}\n";
            }
            _fileSystem.WriteAllText(Types, dataBaseSettings.Path + "\\Settings\\TablesType.txt");
            
            T dataBase = (T)Activator.CreateInstance(typeof(T), (int)dataBaseSettings.ColumnsCount, dataBaseSettings, 1);
            dataBase.InitManager(this);

            dataBase.DataBaseLogger = new DatabaseLoger(dataBaseSettings, "Loger");

            if (dataBaseSettings.SaveMod)
            {
                dataBase.EnableSafeMode();
            }
            else
            {
                dataBase.DisableSafeMode();
            }

            _fileSystem.WriteAllText(Encoder.Encode("", dataBaseSettings.Key), dataBaseSettings.Path + $"\\Cluster1.txt");
            return (T)dataBase;
        }

        /// <summary>
        /// Создает базу данных и задает ей начальные настройки
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Key"></param>
        /// <param name="dataBaseSettings"></param>
        public virtual Table CreateDatabase(DatabaseSettings dataBaseSettings)
        {
            return CreateDatabase<Table>(dataBaseSettings);
        }

        /// <summary>
        /// Сохраняет состояние базы данных
        /// </summary>
        /// <param name="dataBase"></param>
        public virtual void SaveStatesDataBase(Table dataBase)
        {
            string Content = JsonSerializer.Serialize<DatabaseSettings>(dataBase.Settings);
            _fileSystem.WriteAllText(Encoder.Encode(Content,dataBase.Settings.Key), dataBase.Settings.Path + "\\Settings\\FreeIDs.txt");
            uint[] FreeIDs = dataBase.FreeIDs.ToArray();
            string[] lines = new string[FreeIDs.Length];

            for(int i = 0; i < FreeIDs.Length; i++)
            {
                lines[i] = FreeIDs[i].ToString();   
            }
            _fileSystem.WriteLines(lines, dataBase.Settings.Path + "\\Settings\\FreeIDs.txt");
            
            //Сохранение имен и типов столбцов
            string ColumnsFileLines = "";

            for (int u = 0; u < dataBase.Columns.Count; u++)
            {
                ColumnsFileLines += dataBase.Columns[u].TypeOfData.Name + "|" + dataBase.Columns[u].Name + "\n";
            }

            _fileSystem.WriteAllText(ColumnsFileLines, dataBase.Settings.Path + "\\Settings\\TablesType.txt");
        }

        public virtual void SaveStatesDataBase(DatabaseSettings settings, string Key)
        {
            string Content = JsonSerializer.Serialize<DatabaseSettings>(settings);
            _fileSystem.WriteAllText(Encoder.Encode(Content, Key), settings.Path + "\\Settings\\Settings.txt");            
        }    


        public virtual T LoadDB<T>(string Path, string Key, int LoadCluster = -1) where T : Table
        {
            string settingsText = _fileSystem.ReadAllText(Path + "\\Settings\\Settings.txt");

            DatabaseSettings dataBaseSettings = JsonSerializer.Deserialize<DatabaseSettings>(Encoder.Decode(settingsText, Key));
            dataBaseSettings.Key = Key;

            T dataBase = (T)Activator.CreateInstance(typeof(T),(int)dataBaseSettings.ColumnsCount, dataBaseSettings, LoadCluster != -1 ? LoadCluster : 1);
            dataBase.InitManager(this);

            try
            {
                //Загрузка свободных ID-ков
                
                string[] lines = Encoder.Decode(_fileSystem.ReadAllText(dataBaseSettings.Path + "\\Settings\\FreeIDs.txt"),Key)
                    .Split('\n');
                uint[] FreeIDs = new uint[lines.Length];

                for (int i = 0; i < FreeIDs.Length; i++)
                {
                    FreeIDs[i] = Convert.ToUInt32(lines[i]);
                }

                dataBase.FreeIDs.AddRange(FreeIDs);
            }
            catch { _fileSystem.WriteAllText(Encoder.Encode("", Key), dataBaseSettings.Path + "\\Settings\\FreeIDs.txt"); }

            if (LoadCluster != -1)
            {
                dataBase.Columns.Clear();
                dataBase.Columns.AddRange(DBLoader.LoadCluster(dataBaseSettings.Path, (uint)LoadCluster, dataBaseSettings.Key));
            }
            else
            {
                string[] SettingsTables = _fileSystem.ReadAllLines(Path + "\\Settings\\TablesType.txt");

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
                    dataBase.Columns.Add(new Column(Names[i], DataTypesInColumns.GetType(Types[i]), 0));
                }
            }

            dataBase.DataBaseSaver = _databaseSavers[Convert.ToInt32(dataBaseSettings.SaveMod)] as IDataBaseSaver<Interfaces.AColumn>;
            dataBase.DataBaseLoader = _databaseSavers[Convert.ToInt32(dataBaseSettings.SaveMod)] as IDataBaseLoader<Interfaces.AColumn>;
            dataBase.DataBaseReplayser = _databaseSavers[Convert.ToInt32(dataBaseSettings.SaveMod)] as IDataBaseReplayser;
            dataBase.DataBaseLogger = new DatabaseLoger(dataBaseSettings, "Loger");

            return dataBase;
        }
        /// <summary>
        /// Если LoadCluster = -1 => по умолчанию ничто в оперативную память не грузится
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="LoadCluster"></param>
        /// <returns></returns>
        public virtual Table LoadDB(string Path, string Key, int LoadCluster = -1)
        {
            return LoadDB<Table>(Path, Key, LoadCluster);
        }

    }
}
