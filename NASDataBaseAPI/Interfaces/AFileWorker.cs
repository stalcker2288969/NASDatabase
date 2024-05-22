namespace NASDatabase.Interfaces
{
    /// <summary>
    /// Интерфейс файловой системы для работы БД
    /// </summary>
    public abstract class AFileWorker
    {
        /// <summary>
        /// Записть текста в файл
        /// </summary>
        /// <param name="text"></param>
        /// <param name="path"></param>
        public abstract void WriteAllText(string text, string path);
        /// <summary>
        /// Построчная запись текста в файл
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="path"></param>
        public abstract void WriteLines(string[] lines, string path);
        /// <summary>
        /// Чтение текста с файла
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract string ReadAllText(string path);
        /// <summary>
        /// Чтение строк файла
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract string[] ReadAllLines(string path);
        /// <summary>
        /// Удаление файла 
        /// </summary>
        /// <param name="path"></param>
        public abstract void RemoveFile(string path);
        /// <summary>
        /// Создание директории 
        /// </summary>
        /// <param name="path"></param>
        public abstract void CreateDirectory(string path);
        /// <summary>
        /// Удаление директории  
        /// </summary>
        /// <param name="path"></param>
        public abstract void DeleteDictinory(string path);
    }
}
