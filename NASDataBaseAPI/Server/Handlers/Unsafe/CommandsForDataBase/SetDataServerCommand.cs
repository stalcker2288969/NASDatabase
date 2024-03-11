using NASDataBaseAPI.Client;
using NASDataBaseAPI.Client.Utilities;
using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Handlers.Unsafe.CommandsForDataBase
{
    public class SetDataServerCommand : CommandHandler
    {
        private Rows _line;
        private Action<Rows> _handler;
        private IDataConverter _dataConverter;

        public SetDataServerCommand(Action<Rows> handler, IDataConverter dataConverter)
        {
            _handler = handler;
            _dataConverter = dataConverter;
        }

        public override void SetData(string data)
        {
             _line = _dataConverter.GetDataLine<Rows>(data);
        }

        public override string Use()
        {
            _handler?.Invoke(_line);
            return BaseCommands.DONE;
        }
    }
}
