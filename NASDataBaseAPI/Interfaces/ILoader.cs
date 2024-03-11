
namespace NASDataBaseAPI.Interfaces
{
    public interface ILoader : IDataBaseSaver<AColumn>, IDataBaseLoader<AColumn>, IDataBaseReplayser
    {
        IFileWorker FileSystem { get; }
    }
}
