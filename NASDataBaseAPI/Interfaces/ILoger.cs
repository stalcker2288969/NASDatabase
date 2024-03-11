
namespace NASDataBaseAPI.Interfaces
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
