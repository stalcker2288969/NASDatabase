using NASDataBaseAPI.Client;
using NASDataBaseAPI.Interfaces;
using System;

namespace NASDataBaseAPI.Server.Handlers.Unsafe.CommandsForDataBase
{
    public class RemoveDataByID : ServerCommand
    {
        private string _data = "";
        private Action<int> _handler;

        public RemoveDataByID(Action<int> Handler)
        {
            this._handler = Handler;
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