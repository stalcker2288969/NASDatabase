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
    internal class DataBaseLoger : ILoger
    {
        public DataBaseSettings settings { get; private set; }
        public DateTime TimeStartLog { get; private set; }
        private string PathToFile;
        private string Prefix;

        public DataBaseLoger(DataBaseSettings settings, string Prefix)
        {
            this.settings = settings;
            this.Prefix = Prefix;
        }

        public void StartLog()
        {
            if(settings.Logs == true)
            {
                TimeStartLog = DateTime.Now;
                Directory.CreateDirectory(settings.Path + "\\Logs");
                PathToFile = settings.Path + $"\\Logs\\Log.txt";
                File.WriteAllText(PathToFile, $"Log started at {TimeStartLog}");
            }
        }
        public void Log(string message)
        {
            if(settings.Logs == true)
            {
                File.AppendAllLines(PathToFile, new string[] {$"{Prefix}| Log at {DateTime.Now} | "+message});
            }
        }

        public void StopLog()
        {
            if(settings.Logs == true)
            {
                TimeStartLog = new DateTime(0);
                PathToFile = "";
            }
        }

        public void SetPrefix(string prefix)
        {
            Prefix = prefix;
        }
    }
}
