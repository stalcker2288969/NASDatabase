using NASDatabase.Client.Utilities;
using System;

namespace NASDatabase.Interfaces
{
    public interface ICommandWorker : ISenderCommands, IRecipientMessage, IDisposable
    {
        string IP { get; }
        string Port { get; }
    }
}
