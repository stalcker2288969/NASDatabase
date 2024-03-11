using NASDataBaseAPI.Client;
using NASDataBaseAPI.Interfaces;
using System;


namespace NASDataBaseAPI.Server.Handlers.Unsafe.CommandsForDataBase
{
    public class Login : CommandHandler
    {
        private ServerSettings ServerSettings;
        private bool _canConnect = false;
        private Action<bool> Handler;

        public Login(ServerSettings ServerSettings, Action<bool> Handler)
        {
            this.ServerSettings = ServerSettings;
            this.Handler = Handler;
        }

        public override void SetData(string data)
        {
            string[] d = data.Split(BaseCommands.SEPARATION.ToCharArray());

            if (d[1] == ServerSettings.Key || (d[1] != "" && ServerSettings.Key == ""))
            {
                _canConnect = true;
            }
        }

        public override string Use()
        {
            Handler?.Invoke(_canConnect);
            if(_canConnect)
                return BaseCommands.Connect;
            else
                return BaseCommands.Disconnect;
        }
    }
}
