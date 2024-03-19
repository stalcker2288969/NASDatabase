using NASDatabase.Client;
using NASDatabase.Interfaces;
using System;


namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase
{
    internal class RemoveColumn : CommandHandler
    {
        private Action<string> _handler;
        private string _columnName;

        public RemoveColumn(Action<string> handler) 
        { 
            _handler = handler;
        }

        public override void SetData(string data)
        {
            _columnName = data;
        }

        public override string Use()
        {
            _handler?.Invoke(_columnName);

            return BaseCommands.DONE;
        }
    }
}
