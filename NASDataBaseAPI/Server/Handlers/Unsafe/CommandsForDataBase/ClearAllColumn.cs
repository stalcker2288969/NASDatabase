using NASDataBaseAPI.Client;
using NASDataBaseAPI.Interfaces;
using System;

namespace NASDataBaseAPI.Server.Handlers.Unsafe.CommandsForDataBase
{
    public class ClearAllColumn : CommandHandler
    {
        private Action<string, int> _handler;
        private string _columnName;
        private string _inSector;

        public ClearAllColumn(Action<string, int> Handler) 
        { 
            _handler = Handler;
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
