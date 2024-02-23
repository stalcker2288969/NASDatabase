using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.Interfases
{
    public interface ILoger
    {
        string Prefix { get; set; }
        /// <summary>
        /// Запускет логирование
        /// </summary>
        void StartLog();
        /// <summary>
        /// Выводит сообщение во время логирования
        /// </summary>
        /// <param name="message"></param>
        void Log(string message);

        void StopLog();
    }
}
