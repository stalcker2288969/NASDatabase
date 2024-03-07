using NASDataBaseAPI.Client;
using NASDataBaseAPI.Interfaces;
using System;

namespace NASDataBaseAPI.Server.Handlers.Unsafe.CommandsForDataBase
{
    public class ClearAllBase : ServerCommand
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
