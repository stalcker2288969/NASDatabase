using NASDataBaseAPI.Client;
using NASDataBaseAPI.Interfaces;
using System;

namespace NASDataBaseAPI.Server.Handlers.Unsafe.CommandsForDataBase.AdditionalSet
{
    public class MSGFromClient : ServerCommand
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
