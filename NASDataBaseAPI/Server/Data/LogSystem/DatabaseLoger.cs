using NASDatabase.Interfaces;
using NASDatabase.Server.Data.DatabaseSettings;
using System;
using System.Collections.Generic;

namespace NASDataBaseAPI.Server.Data.LogSystem
{
    public class DatabaseLoger : Loger
    {
        public DatabaseSettings Settings { get; private set; }
        public DateTime TimeStartLog { get; private set; }
        public readonly NASDatabase.Interfaces.FileWorker FileSystem = new NASDatabase.Server.Data.Modules.FileWorker();

        private string _pathToFile;

        public DatabaseLoger(DatabaseSettings settings, NASDatabase.Interfaces.FileWorker fileWorker, string prefix)
        {
            this.Settings = settings;
            this.Prefix = prefix;
            this.FileSystem = fileWorker;
        }

        public DatabaseLoger(DatabaseSettings settings, string prefix)
        {
            this.Settings = settings;
            this.Prefix = prefix;
        }

        /// <summary>
        /// !Если у вас включена система логгирования в настройках, то вам обязательно надо вызвать этот метод у базы данных! 
        /// </summary>
        public override void StartLog()
        {
            TimeStartLog = DateTime.Now;
            FileSystem.CreateDirectory(Settings.Path + "\\Logs");
            _pathToFile = Settings.Path + $"\\Logs\\Log{TimeStartLog.Day}_{TimeStartLog.Hour}_{TimeStartLog.Minute}.txt";
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
