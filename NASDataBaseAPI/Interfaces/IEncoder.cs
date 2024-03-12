namespace NASDatabase.Interfaces
{
    public interface IEncoder
    {
        string Encode(string Data,string Key);
        string Decode(string Data, string Key);
    }
}
