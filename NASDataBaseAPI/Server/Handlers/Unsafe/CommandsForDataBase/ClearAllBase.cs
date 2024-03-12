using NASDatabase.Client;
using NASDatabase.Interfaces;
using System;

namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase
{
    public class ClearAllBase : CommandHandler
    {
        private Action _handler;

        public ClearAllBase(Action Handler) 
        {
            _handler = Handler;
        }

        public override string Use()
        {
            _handler?.Invoke();
            return BaseCommands.DONE;
        }
    }
}
