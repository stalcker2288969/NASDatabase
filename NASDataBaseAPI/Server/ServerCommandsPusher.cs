using NASDatabase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDatabase.Server
{
    public abstract class ServerCommandsPusher : ICommandWorker
    {
        public const string ExceptionInFeild = "IP И Port нельзя достать у ServerCommandsPusher";

        public abstract string IP { get; }

        public abstract string Port { get; }

        public abstract string Listen();

        public abstract void Push(string message);

        public abstract void CloseConnection();

        public abstract void Dispose();
    }
}
