using NASDatabase.Client;
using NASDatabase.Client.Utilities;
using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using System;

namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase
{
    internal class SetDataServerCommand : CommandHandler
    {
        private Row _line;
        private Action<Row> _handler;
        private IDataConverter _dataConverter;

        public SetDataServerCommand(Action<Row> handler, IDataConverter dataConverter)
        {
            _handler = handler;
            _dataConverter = dataConverter;
        }

        public override void SetData(string data)
        {
             _line = _dataConverter.GetDataLine<Row>(data);
        }

        public override string Use()
        {
            _handler?.Invoke(_line);
            return BaseCommands.DONE;
        }
    }
}
