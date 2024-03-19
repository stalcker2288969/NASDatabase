using System.IO;


namespace NASDatabase.Server.Data.Modules
{
    /// <summary>
    /// Работает со стандартной файловой системой  
    /// </summary>
    public class FileWorker : Interfaces.FileWorker
    {
        public override void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public override void DeleteDictinory(string path)
        {
            Directory.Delete(path, true);
        }

        public override string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }

        public override string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public override void RemoveFile(string path)
        {
            File.Delete(path);
        }

        public override void WriteAllText(string text, string path)
        {
            File.WriteAllText(path, text);
        }

        public override void WriteLines(string[] lines, string path)
        {
            File.WriteAllLines(path, lines);
        }
    }
}
