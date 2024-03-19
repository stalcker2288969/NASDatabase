using NASDatabase.Client;
using NASDatabase.Interfaces;
using System;

namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase
{
    internal class RemoveDataByID : CommandHandler
    {
        private string _data = "";
        private Action<int> _handler;

        public RemoveDataByID(Action<int> handler)
        {
            this._handler = handler;
        }

        public override void SetData(string data)
        {
            this._data = data;
        }

        public override string Use()
        {
            _handler(int.Parse(_data));
            return BaseCommands.DONE;
        }
    }
}