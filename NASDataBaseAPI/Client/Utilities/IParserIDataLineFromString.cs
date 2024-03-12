using NASDatabase.Interfaces;

namespace NASDatabase.Client.Utilities
{
    public interface IParserIDataLineFromString
    {
        T GetDataLine<T>(string text) where T : IDatRows, new();
        T[] GetDataLines<T>(string text) where T : IDatRows, new();
        string ParsDataLine<T>(T line) where T : IDatRows;
        string ParsDataLines<T>(T[] lines) where T : IDatRows;
    }
}
