using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace NASDataBaseAPI.Client.Utilities
{
    public class ClientCommandsWorker : ICommandWorker
    {
        public string Name { get; private set; } = "";
        public string Password { get; private set; } = "";

        public string IP { get; private set; }
        public string Port { get; private set; }

        private NetworkStream _stream;
        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;
        private BufferedStream _buffered;

        public ClientCommandsWorker(string IP, string Port, string name, string password)
        {
            Name = name;
            Password = password;
            this.IP = IP;
            this.Port = Port;

            _client = new TcpClient(IP, int.Parse(Port));
            _stream = _client.GetStream();
            _buffered = new BufferedStream(_stream);
            _reader = new StreamReader(_buffered, Encoding.UTF8);
            _writer = new StreamWriter(_buffered, Encoding.UTF8);
        }     

        public virtual string Listen()
        {
            string receivedMessage = _reader.ReadLine();
            return receivedMessage;
        }

        public virtual void Push(string message)
        {
            string resMessage = Name + BaseCommands.SEPARATION + (Password ?? " ") + BaseCommands.SEPARATION + message;
            _writer.WriteLine(resMessage);
            _writer.Flush();
        }

        public void Dispose()
        {
            Push(BaseCommands.Disconnect + BaseCommands.SEPARATION + IP + BaseCommands.SEPARATION + Port);
            Listen();

            try
            {
                _client.Close();
                _reader.Close();
                _writer.Close();
                _buffered.Close();
            }
            finally { }
        }
    }
}