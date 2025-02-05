using NASDataBaseAPI.Interfaces;

namespace NASDataBaseAPI.Client.Utilities
{
    public interface IParserIDataLineFromString
    {
        T GetDataLine<T>(string text) where T : IDataRows, new();
        T[] GetDataLines<T>(string text) where T : IDataRows, new();
        string ParsDataLine<T>(T line) where T : IDataRows;
        string ParsDataLines<T>(T[] lines) where T : IDataRows;
    }
}
