namespace NASDatabase.Interfaces
{
    public interface IEncoder
    {
        string Encode(string data,string key);
        string Decode(string data, string key);
    }
}
