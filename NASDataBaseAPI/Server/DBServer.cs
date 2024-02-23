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
    /// <summary>
    /// Основной класс для работы с базой данных по сети (новый функционал не вводит, просто улучшает tcp систему)
    /// </summary>
    public class DBServer
    {
        public TcpListener Server { get; private set; }
        public List<TcpClient> TcpClients = new List<TcpClient>();
        public event Action<TcpClient> _ClientConnect;
        public event Action _OnStartServer;
        public event Action _OnStopServer; 
        public string ServerIP { get; private set; }
        public int Port { get; private set; }


        public DBServer(string serverIP, int port)
        {
            this.ServerIP = serverIP;
            this.Port = port;
        }

        /// <summary>
        /// Необходим для старта сервера 
        /// </summary>
        public void StartServer()
        {
            Server = new TcpListener(IPAddress.Parse(ServerIP), Port);
            Server.Start();
            _OnStartServer?.Invoke();
            while (true)
            {
                TcpClients.Add(Server.AcceptTcpClient());
                Thread clientThread = new Thread(ClientConnect);
                clientThread.Start(TcpClients[TcpClients.Count - 1]);
            }
        }

        /// <summary>
        /// Остановка сервера 
        /// </summary>
        public void StopServer()
        {
            Server.Stop();
            _OnStopServer?.Invoke();
        }

        private void ClientConnect(object tcpClient)
        {
            TcpClient TcpClient = (TcpClient)tcpClient;
            _ClientConnect?.Invoke(TcpClient);
        }
    }
}
