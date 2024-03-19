using NASDatabase.Client;
using NASDatabase.Interfaces;
using System;

namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase
{
    internal class ChangeEverythingTo : CommandHandler
    {
        private Action<string, string, string, int> _handler;
        private string[] _data;
        public ChangeEverythingTo(Action<string, string, string, int> handler) { _handler = handler; }

        public override void SetData(string data)
        {
            _data = data.Split(BaseCommands.SEPARATION.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        public override string Use()
        {
            _handler?.Invoke(_data[0], _data[1], _data[2], int.Parse(_data[3]));

            return BaseCommands.DONE;
        }
    }
}
