using NASDatabase.Interfaces;


namespace NASDatabase.Client.Utilities
{
    public interface IParserIColumnFromString
    {
        T GetColumn<T>(string text) where T : AColumn, new();
        string ParsColumn<T>(T column) where T : AColumn;
    }
}
