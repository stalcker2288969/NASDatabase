using NASDatabase.Interfaces;

namespace NASDatabase.Client.Utilities
{
    public interface IParserIDataRowFromString
    {
        T GetDataLine<T>(string text) where T : IDataRow, new();
        T[] GetDataLines<T>(string text) where T : IDataRow, new();
        string ParsDataLine<T>(T line) where T : IDataRow;
        string ParsDataLines<T>(T[] lines) where T : IDataRow;
    }
}
