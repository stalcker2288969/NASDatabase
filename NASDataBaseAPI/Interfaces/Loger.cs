
namespace NASDatabase.Interfaces
{
    public abstract class Loger
    {
        public string Prefix { get; set; }
        /// <summary>
        /// Запускет логирование
        /// </summary>
        public abstract void StartLog();
        /// <summary>
        /// Выводит сообщение во время логирования
        /// </summary>
        /// <param name="message"></param>
        public abstract void Log(string message);
        /// <summary>
        /// Остоновка логирования
        /// </summary>
        public abstract void StopLog();
    }
}
