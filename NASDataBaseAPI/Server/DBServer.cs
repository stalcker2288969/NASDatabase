using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server
{
    public class DBServer
    {
        public TcpListener server { get; private set; }
        public List<TcpClient> tcpClients = new List<TcpClient>();
        public event Action<TcpClient> _ClientConnect;
        public event Action _OnStartServer;
        public event Action _OnStopServer; 
        public string ServerIP { get; private set; }
        public int Port { get; private set; }

        public void StartServer()
        {
            server = new TcpListener(IPAddress.Parse(ServerIP), Port);
            server.Start();
            _OnStartServer?.Invoke();
            while (true)
            {
                tcpClients.Add(server.AcceptTcpClient());
                Thread clientThread = new Thread(ClientConnect);
                clientThread.Start(tcpClients[tcpClients.Count - 1]);
            }
        }

        public void StopServer()
        {
            server.Stop();
            _OnStopServer?.Invoke();
        }

        private void ClientConnect(object tcpClient)
        {
            TcpClient TcpClient = (TcpClient)tcpClient;
            _ClientConnect?.Invoke(TcpClient);
        }
    }
}
