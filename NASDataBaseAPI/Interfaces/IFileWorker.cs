namespace NASDataBaseAPI.Interfaces
{
    /// <summary>
    /// Интерфейс файловой системы для работы БД
    /// </summary>
    public interface IFileWorker
    {
        /// <summary>
        /// Записть текста в файл
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Path"></param>
        void WriteAllText(string Text, string Path);
        /// <summary>
        /// Построчная запись текста в файл
        /// </summary>
        /// <param name="Lines"></param>
        /// <param name="Path"></param>
        void WriteLines(string[] Lines, string Path);
        /// <summary>
        /// Чтение текста с файла
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        string ReadAllText(string Path);
        /// <summary>
        /// Чтение строк файла
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        string[] ReadAllLines(string Path);
        /// <summary>
        /// Удаление файла 
        /// </summary>
        /// <param name="Path"></param>
        void RemoveFile(string Path);
        /// <summary>
        /// Создание директории 
        /// </summary>
        /// <param name="Path"></param>
        void CreateDirectory(string Path);
        /// <summary>
        /// Удаление директории  
        /// </summary>
        /// <param name="Path"></param>
        void DeleteDictinory(string Path);
    }
}
