namespace NASDataBaseAPI.Interfaces
{
    /// <summary>
    /// Предоставляет возможность объекту стать строкой в базе 
    /// </summary>
    public interface IDatRows
    {
        int ID { get; }
        void Init(int ID, params string[] datas);
        string[] GetData();
    }
}
