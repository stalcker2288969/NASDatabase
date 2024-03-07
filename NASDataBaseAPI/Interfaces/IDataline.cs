namespace NASDataBaseAPI.Interfaces
{
    public interface IDataLine
    {
        int ID { get; }
        void Init(int ID, params string[] datas);
        string[] GetData();
    }
}
