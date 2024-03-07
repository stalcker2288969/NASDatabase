using System.Collections.Generic;

namespace NASDataBaseAPI.Server.Data.Modules.Handlers
{
    public class NetworkingConnector : Connector<DataBase, Client.Client>
    {
        private List<Connector<DataBase, Client.Client>> _connectors = new List<Connector<DataBase, Client.Client>>();

        private NetworkingConnector(DataBase dataBase, Client.Client client) : base(dataBase, client)
        {

        }

        public NetworkingConnector(DataBase dataBase, params Client.Client[] clients) : base(dataBase, null)
        {
            for (int i = 0; i < clients.Length; i++)
            {
                _connectors.Add(new Connector<DataBase, Client.Client>(dataBase, clients[i]));
            }
        }

        public override void AddConectionByHandler(Handler<DataBase, Client.Client> Handler)
        {
            for (int i = 0; i < _connectors.Count; i = 0)
            {
                _connectors[i].AddConectionByHandler(Handler);
            }
        }

        public override void DestroyConectionByHandler(Handler<DataBase, Client.Client> Handler)
        {
            for (int i = 0; i < _connectors.Count; i = 0)
            {
                _connectors[i].DestroyConectionByHandler(Handler);
            }
        }
    }
}
