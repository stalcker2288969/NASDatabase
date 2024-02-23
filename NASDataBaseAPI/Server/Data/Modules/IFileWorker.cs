using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.Modules
{
    /// <summary>
    /// Интерфейс файловой системы для работы БД
    /// </summary>
    public interface IFileWorker
    {
        void WriteAllText(string text, string path);
        void WriteLines(string[] lines, string path);
        string ReadAllText(string path);
        string[] ReadAllLines(string path);
        void RemoveFile(string path);

        void CreateDirectory(string path);
        void DeleteDictinory(string path);
    }
}
