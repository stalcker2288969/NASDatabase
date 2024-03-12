using NASDatabase.Client;
using NASDatabase.Interfaces;
using System;

namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase.AdditionalSet
{
    public class MSGFromClient : CommandHandler
    {
        private string _command;

        public override void SetData(string data)
        {
            _command = data;
        }

        public override string Use()
        {
            Console.WriteLine("%" + _command);
            return BaseCommands.DONE;
        }
    }
}
