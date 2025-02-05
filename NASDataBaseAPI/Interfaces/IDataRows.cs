namespace NASDataBaseAPI.Interfaces
{
    /// <summary>
    /// Предоставляет возможность объекту стать строкой в базе 
    /// </summary>
    public interface IDataRows
    {
        int ID { get; }
        void Init(int ID, params string[] datas);
        string[] GetData();
    }
}
