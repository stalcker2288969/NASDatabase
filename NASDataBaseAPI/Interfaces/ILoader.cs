
namespace NASDatabase.Interfaces
{
    public interface ILoader : IDataBaseSaver<AColumn>, IDataBaseLoader<AColumn>, IDataBaseReplayser
    {
        FileWorker FileSystem { get; }
    }
}
