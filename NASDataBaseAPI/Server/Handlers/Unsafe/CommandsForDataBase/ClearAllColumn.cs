using NASDatabase.Client;
using NASDatabase.Interfaces;
using System;

namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase
{
    internal class ClearAllColumn : CommandHandler
    {
        private Action<string, int> _handler;
        private string _columnName;
        private string _inSector;

        public ClearAllColumn(Action<string, int> handler) 
        { 
            _handler = handler;
        }

        public override void SetData(string data)
        {
            var d = data.Split(BaseCommands.SEPARATION.ToCharArray());
            _columnName = d[0];
            _inSector = d[1];
        }

        public override string Use()
        {
            _handler?.Invoke(_columnName, int.Parse(_inSector));
            return BaseCommands.DONE;
        }
    }
}
