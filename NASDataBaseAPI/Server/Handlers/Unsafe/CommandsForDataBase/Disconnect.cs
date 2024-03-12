using NASDatabase.Client;
using NASDatabase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase
{
    public class Disconnect : CommandHandler
    {
        List<ServerCommandsPusher> _pushers;
        private ServerCommandsPusher _serverCommandsPusher;

        public Disconnect(List<ServerCommandsPusher> pushers)
        {
            _pushers = pushers;
        }

        public override void SetData(string data)
        {
            var d = data.Split(BaseCommands.SEPARATION.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            lock(_pushers)
            {
                foreach (var pusher in _pushers)
                {
                    if(pusher.IP == d[0]  && pusher.Port == d[1])
                    {
                        _serverCommandsPusher = pusher;
                        break;
                    }
                }
            }
        }

        public override string Use()
        {
            throw new NotImplementedException();
        }
    }
}
