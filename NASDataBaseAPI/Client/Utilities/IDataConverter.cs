using NASDatabase.Server.Data;
using NASDatabase.Interfaces;

namespace NASDatabase.Client.Utilities
{
    /// <summary>
    /// Конвертация стандартных данных и кодировка данных в байты и обратно
    /// </summary>
    public interface IDataConverter : IParserIColumnFromString, IParserIDataLineFromString, IParserItemDataFromString 
    {
        /// <summary>
        /// Получение строки из байт 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        string GetString(byte[] data, int buffer);
        /// <summary>
        /// Получение байт из строки
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        byte[] GetBytes(string v);
        /// <summary>
        /// Получение интов из строки
        /// </summary>
        /// <param name="data"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        int[] GetInts(string v);
        /// <summary>
        /// Получение строки из списка чисел
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        string ParsInts(int[] v);      
    }
}