using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data.Modules;
using System;
using System.Collections.Generic;


namespace NASDataBaseAPI.Server.Data.DataBaseSettings
{
    /// <summary>
    /// Класс предоставляющий систему логирования  
    /// </summary>
    internal class DatabaseLoger : ILoger
    {
        public string Prefix { get; set; }
        public DatabaseSettings Settings { get; private set; }
        public DateTime TimeStartLog { get; private set; }
        public IFileWorker FileSystem { get; private set; } = new BaseFileWorker();

        private string _pathToFile;


        public DatabaseLoger(DatabaseSettings settings, string Prefix)
        {
            this.Settings = settings;
            this.Prefix = Prefix;
        }

        public DatabaseLoger(DatabaseSettings settings, IFileWorker fileWorker, string Prefix)
        {
            this.Settings = settings;
            this.Prefix = Prefix;
            this.FileSystem = fileWorker;
        }

        /// <summary>
        /// !Если у вас включена система логгирования в настройках, то вам обязательно надо вызвать этот метод у базы данных! 
        /// </summary>
        public void StartLog()
        {
            if(Settings.Logs == true)
            {
                TimeStartLog = DateTime.Now;
                FileSystem.CreateDirectory(Settings.Path + "\\Logs");
                _pathToFile = Settings.Path + $"\\Logs\\Log{TimeStartLog.Day}_{TimeStartLog.Hour}_{TimeStartLog.Minute}.txt";
                FileSystem.WriteAllText($"Log started at {TimeStartLog}", _pathToFile);
            }
        }

        public void Log(string message)
        {
            if(Settings.Logs == true)
            {
                List<string> list = new List<string>();
                list.AddRange(FileSystem.ReadAllLines(_pathToFile));
                list.Add($"{Prefix}| Log at {DateTime.Now} | " + message);
                FileSystem.WriteLines(list.ToArray(), _pathToFile);
            }
        }

        public void StopLog()
        {
            if(Settings.Logs == true)
            {
                TimeStartLog = new DateTime(0);
                _pathToFile = "";
            }
        }
    }
}
