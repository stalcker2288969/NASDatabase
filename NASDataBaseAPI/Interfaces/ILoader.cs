
namespace NASDataBaseAPI.Interfaces
{
    public interface ILoader : IDataBaseSaver<IColumn>, IDataBaseLoader<IColumn>, IDataBaseReplayser
    {
        IFileWorker FileSystem { get; }
    }
}
