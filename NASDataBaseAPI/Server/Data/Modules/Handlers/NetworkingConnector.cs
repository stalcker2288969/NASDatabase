using System.Collections.Generic;

namespace NASDataBaseAPI.Server.Data.Modules.Handlers
{
    public class NetworkingConnector : Connector<Table, Client.Client>
    {
        private List<Connector<Table, Client.Client>> _connectors = new List<Connector<Table, Client.Client>>();

        private NetworkingConnector(Table dataBase, Client.Client client) : base(dataBase, client)
        {

        }

        public NetworkingConnector(Table dataBase, params Client.Client[] clients) : base(dataBase, null)
        {
            for (int i = 0; i < clients.Length; i++)
            {
                _connectors.Add(new Connector<Table, Client.Client>(dataBase, clients[i]));
            }
        }

        public override void AddHandler(Handler<Table, Client.Client> Handler)
        {
            for (int i = 0; i < _connectors.Count; i = 0)
            {
                _connectors[i].AddHandler(Handler);
            }
        }

        public override void DestroyHandler(Handler<Table, Client.Client> Handler)
        {
            for (int i = 0; i < _connectors.Count; i = 0)
            {
                _connectors[i].DestroyHandler(Handler);
            }
        }
    }
}
