namespace NASDatabase.Interfaces
{
    /// <summary>
    /// Предоставляет возможность объекту стать строкой в базе 
    /// </summary>
    public interface IDataRow
    {
        int ID { get; }
        void Init(int ID, params string[] datas);
        string[] GetData();
    }
}
