using NASDatabase.Client;
using NASDatabase.Interfaces;
using System;

namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase
{
    internal class ClearAllBase : CommandHandler
    {
        private Action _handler;

        public ClearAllBase(Action handler) 
        {
            _handler = handler;
        }

        public override string Use()
        {
            _handler?.Invoke();
            return BaseCommands.DONE;
        }
    }
}
