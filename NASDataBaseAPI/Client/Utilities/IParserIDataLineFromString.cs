using NASDataBaseAPI.Interfaces;

namespace NASDataBaseAPI.Client.Utilities
{
    public interface IParserIDataLineFromString
    {
        T GetDataLine<T>(string text) where T : IDataLine, new();
        T[] GetDataLines<T>(string text) where T : IDataLine, new();
        string ParsDataLine<T>(T line) where T : IDataLine;
        string ParsDataLines<T>(T[] lines) where T : IDataLine;
    }
}
