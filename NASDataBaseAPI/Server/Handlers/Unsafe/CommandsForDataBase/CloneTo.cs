using NASDatabase.Client;
using NASDatabase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase
{
    internal class CloneTo : CommandHandler
    {
        private Action<string, string> _handler;

        private string _left;
        private string _right;

        public CloneTo(Action<string, string> handler)
        {
            _handler = handler;
        }

        public override void SetData(string data)
        {
            var d = data.Split(BaseCommands.SEPARATION.ToCharArray());

            _left = d[0];
            _right = d[1];
        }

        public override string Use()
        {
            _handler?.Invoke(_left, _right);
            return BaseCommands.DONE;
        }
    }
}
