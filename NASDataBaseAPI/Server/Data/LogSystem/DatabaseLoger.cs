using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using NASDatabase.Server.Data.Modules;
using System;
using System.Collections.Generic;

namespace NASDataBaseAPI.Server.Data.LogSystem
{
    public class DatabaseLoger : Loger
    {
        public Database Database { get; private set; }
        public DateTime TimeStartLog { get; private set; }
        public readonly AFileWorker FileSystem = new FileWorker();

        private string _pathToFile;
        private Connector<Database, Database> _connector;

        public DatabaseLoger(Database db, AFileWorker fileWorker, string prefix)
        {
            Database = db;
            this.Prefix = prefix;
            this.FileSystem = fileWorker;
            InitConnection();
        }

        public DatabaseLoger(Database db, string prefix) : this(db, new FileWorker(), prefix)
        {

        }

        private void InitConnection()
        {
            _connector = new Connector<Database, Database>(Database, null);
            _connector.AddHandler(new OnAddDataLog(this));
            _connector.AddHandler(new OnCloneColumn(this));
            _connector.AddHandler(new OnRemoveData(this));
            _connector.AddHandler(new OnLoadedNewSector(this));
            _connector.AddHandler(new OnClearAllColumn(this));
            _connector.AddHandler(new OnClearAllBase(this));
            _connector.AddHandler(new OnRenameColumn(this));
        }

        /// <summary>
        /// !Если у вас включена система логгирования в настройках, то вам обязательно надо вызвать этот метод у базы данных! 
        /// </summary>
        public override void StartLog()
        {
            TimeStartLog = DateTime.Now;
            FileSystem.CreateDirectory(Database.Settings.Path + "\\Logs");
            _pathToFile = Database.Settings.Path + $"\\Logs\\Log{TimeStartLog.Day}_{TimeStartLog.Hour}_{TimeStartLog.Minute}.txt";
            FileSystem.WriteAllText($"Log started at {TimeStartLog}", _pathToFile);
        }

        public override void Log(string message)
        {
            List<string> list = new List<string>();
            list.AddRange(FileSystem.ReadAllLines(_pathToFile));
            list.Add($"{Prefix}| {message} | {DateTime.Now}");
            FileSystem.WriteLines(list.ToArray(), _pathToFile);
        }

        public override void StopLog()
        {
            TimeStartLog = new DateTime(0);
            _pathToFile = string.Empty;
        }
    }
}
