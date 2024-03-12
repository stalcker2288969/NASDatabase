using NASDatabase.Interfaces;
using System;
using System.Text.Json;
using NASDatabase.Server.Data.Safety;
using NASDatabase.Data.DataTypesInColumn;
using NASDatabase.Server.Data.DatabaseSettings.Loaders;
using NASDatabase.Server.Data.Modules;

namespace NASDatabase.Server.Data.DatabaseSettings
{
    /// <summary>
    /// Отвечает за упровление стандратной базой данных (базовый функционал)  
    /// </summary>
    public class DatabaseManager
    {
        public static DatabaseLoader DBLoader { get; private set; } = new DatabaseLoader();
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

        public DatabaseManager(IFileWorker FileWorker) 
        {            
            _fileSystem = FileWorker;
            _encoder = Encoder;
            Init();
        }

        public DatabaseManager(IFileWorker FileWorker, IEncoder Encoder)
        {           
            _fileSystem = FileWorker;
            _encoder = Encoder;
            Init();
        }

        private void Init()
        {
            _databaseSavers = new ILoader[2];
            _databaseSavers[0] = new DBNoSaveLoader(_encoder,_fileSystem);
            _databaseSavers[1] = new DatabaseLoader(_encoder, _fileSystem);
        }

        /// <summary>
        /// Меняет ключ и перезаписывает базу
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
                _fileSystem.DeleteDictinory(Path);
            }
            catch { return; }
        }
        
        public virtual T CreateDatabase<T>(DatabaseSettings DatabaseSettings) where T : Database
        {
            DatabaseSettings.CountClusters = DatabaseSettings.CountClusters == 0 ? 1 : DatabaseSettings.CountClusters;

            _fileSystem.CreateDirectory(DatabaseSettings.Path + DatabaseSettings.Name);
            _fileSystem.CreateDirectory(DatabaseSettings.Path + DatabaseSettings.Name + "\\Settings");
            DatabaseSettings.Path = DatabaseSettings.Path + DatabaseSettings.Name;
            string Content = JsonSerializer.Serialize<DatabaseSettings>(DatabaseSettings);
            _fileSystem.WriteAllText(Encoder.Encode(Content, DatabaseSettings.Key), DatabaseSettings.Path + "\\Settings\\Settings.txt");

            string Types = "";
            for (int i = 0; i < DatabaseSettings.ColumnsCount; i++)
            {
                Types += $"Text|{i}\n";
            }
            _fileSystem.WriteAllText(Types, DatabaseSettings.Path + "\\Settings\\TablesType.txt");
            
            T dataBase = (T)Activator.CreateInstance(typeof(T), (int)DatabaseSettings.ColumnsCount, DatabaseSettings, 1);
            dataBase.InitManager(this);

            dataBase.DataBaseLoger = new DatabaseLoger(DatabaseSettings, "Loger");

            if (DatabaseSettings.SaveMod)
            {
                dataBase.EnableSafeMode();
            }
            else
            {
                dataBase.DisableSafeMode();
            }

            _fileSystem.WriteAllText(Encoder.Encode("", DatabaseSettings.Key), DatabaseSettings.Path + $"\\Cluster1.txt");
            return (T)dataBase;
        }

        /// <summary>
        /// Создает базу данных и задает ей начальные настройки
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Key"></param>
        /// <param name="DatabaseSettings"></param>
        public virtual Database CreateDataBase(DatabaseSettings DatabaseSettings)
        {
            return CreateDatabase<Database>(DatabaseSettings);
        }

        /// <summary>
        /// Сохраняет состояние базы данных
        /// </summary>
        /// <param name="Database"></param>
        public virtual void SaveStatesDatabase(Database Database)
        {
            string Content = JsonSerializer.Serialize<DatabaseSettings>(Database.Settings);
            _fileSystem.WriteAllText(Encoder.Encode(Content,Database.Settings.Key), Database.Settings.Path + "\\Settings\\FreeIDs.txt");
            uint[] FreeIDs = Database.FreeIDs.ToArray();
            string[] lines = new string[FreeIDs.Length];

            for(int i = 0; i < FreeIDs.Length; i++)
            {
                lines[i] = FreeIDs[i].ToString();   
            }
            _fileSystem.WriteLines(lines, Database.Settings.Path + "\\Settings\\FreeIDs.txt");
            
            //Сохранение имен и типов столбцов
            string ColumnsFileLines = "";

            for (int u = 0; u < Database.Columns.Count; u++)
            {
                ColumnsFileLines += Database.Columns[u].TypeOfData.Name + "|" + Database.Columns[u].Name + "\n";
            }

            _fileSystem.WriteAllText(ColumnsFileLines, Database.Settings.Path + "\\Settings\\TablesType.txt");
        }

        public virtual void SaveStatesDatabase(DatabaseSettings Settings, string Key)
        {
            string Content = JsonSerializer.Serialize<DatabaseSettings>(Settings);
            _fileSystem.WriteAllText(Encoder.Encode(Content, Key), Settings.Path + "\\Settings\\Settings.txt");            
        }    


        public virtual T LoadDB<T>(string Path, string Key, int LoadCluster = -1) where T : Database
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
            dataBase.DataBaseLoger = new DatabaseLoger(dataBaseSettings, "Loger");

            return dataBase;
        }
        /// <summary>
        /// Если LoadCluster = -1 => по умолчанию ничто в оперативную память не грузится
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="LoadCluster"></param>
        /// <returns></returns>
        public virtual Database LoadDB(string Path, string Key, int LoadCluster = -1)
        {
            return LoadDB<Database>(Path, Key, LoadCluster);
        }

    }
}
