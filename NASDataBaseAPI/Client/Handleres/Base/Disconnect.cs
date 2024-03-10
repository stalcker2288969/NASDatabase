using NASDataBaseAPI.Interfaces;
using System;


namespace NASDataBaseAPI.Client.Handleres.Base
{
    public class Disconnect : CommandHandler
    {
        public override string Use()
        {
            throw new Exception("Клиент был отключен от сервера! ");
        }
    }
}
