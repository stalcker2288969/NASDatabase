using NASDataBaseAPI.Server.Data.Interfases;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.DataBaseSettings
{
    /// <summary>
    /// Класс предоставляющий систему логирования  
    /// </summary>
    internal class DataBaseLoger : ILoger
    {
        public DataBaseSettings Settings { get; private set; }
        public DateTime TimeStartLog { get; private set; }
        
        private string _pathToFile;
        private string _prefix;

        public DataBaseLoger(DataBaseSettings settings, string Prefix)
        {
            this.Settings = settings;
            this._prefix = Prefix;
        }

        /// <summary>
        /// !Если у вас включена система логгирования в настройках, то вам обязательно надо вызвать этот метод у базы данных! 
        /// </summary>
        public void StartLog()
        {
            if(Settings.Logs == true)
            {
                TimeStartLog = DateTime.Now;
                Directory.CreateDirectory(Settings.Path + "\\Logs");
                _pathToFile = Settings.Path + $"\\Logs\\Log{TimeStartLog.Day}_{TimeStartLog.Hour}_{TimeStartLog.Minute}.txt";
                File.WriteAllText(_pathToFile, $"Log started at {TimeStartLog}");
            }
        }

        public void Log(string message)
        {
            if(Settings.Logs == true)
            {
                File.AppendAllLines(_pathToFile, new string[] {$"{_prefix}| Log at {DateTime.Now} | "+message});
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

        public void SetPrefix(string prefix)
        {
            _prefix = prefix;
        }
    }
}
