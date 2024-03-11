using NASDataBaseAPI.Client.Utilities;
using System;

namespace NASDataBaseAPI.Interfaces
{
    public interface ICommandWorker : ISenderCommands, IRecipientMessage, IDisposable
    {
        string IP { get; }
        string Port { get; }
    }
}
