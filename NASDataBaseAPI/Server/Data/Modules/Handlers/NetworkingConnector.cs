using System.Collections.Generic;

namespace NASDataBaseAPI.Server.Data.Modules.Handlers
{
    public class NetworkingConnector : Connector<Database, Client.Client>
    {
        private List<Connector<Database, Client.Client>> _connectors = new List<Connector<Database, Client.Client>>();

        private NetworkingConnector(Database dataBase, Client.Client client) : base(dataBase, client)
        {

        }

        public NetworkingConnector(Database dataBase, params Client.Client[] clients) : base(dataBase, null)
        {
            for (int i = 0; i < clients.Length; i++)
            {
                _connectors.Add(new Connector<Database, Client.Client>(dataBase, clients[i]));
            }
        }

        public override void AddHandler(Handler<Database, Client.Client> Handler)
        {
            for (int i = 0; i < _connectors.Count; i = 0)
            {
                _connectors[i].AddHandler(Handler);
            }
        }

        public override void DestroyHandler(Handler<Database, Client.Client> Handler)
        {
            for (int i = 0; i < _connectors.Count; i = 0)
            {
                _connectors[i].DestroyHandler(Handler);
            }
        }
    }
}
