using NASDatabase.Interfaces;
using System;


namespace NASDatabase.Client.Handleres.Base
{
    public class Disconnect : CommandHandler
    {
        public override string Use()
        {
            throw new Exception("Клиент был отключен от сервера! ");
        }
    }
}
