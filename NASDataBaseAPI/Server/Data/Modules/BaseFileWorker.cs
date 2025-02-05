using NASDataBaseAPI.Interfaces;
using System.IO;


namespace NASDataBaseAPI.Server.Data.Modules
{
    /// <summary>
    /// Работает со стандартной файловой системой  
    /// </summary>
    public class BaseFileWorker : IFileWorker
    {
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void DeleteDirectory(string path)
        {
            Directory.Delete(path, true);
        }

        public string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public void RemoveFile(string path)
        {
            File.Delete(path);
        }

        public void WriteAllText(string text, string path)
        {
            File.WriteAllText(path, text);
        }

        public void WriteLines(string[] lines, string path)
        {
            File.WriteAllLines(path, lines);
        }
    }
}
