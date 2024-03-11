using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Net;
using NASDataBaseAPI.Client;

namespace NASDataBaseAPI.Server
{
    public class BaseServerCommandPusher : ServerCommandsPusher
    {
        #region Ошибки
        public const string NotActiveExeption = "ServerCommandWorker не был актевирован!";
        public const string ErrorOccurredWhileReadingDataFromClient = "При чтении данных с клиента произошла ошибка!";
        public const string ErrorSendingMessage = "Произошла ошибка при отправке сообщения клиенту!";
        #endregion

        public override string IP { get => ((IPEndPoint)_client.Client.RemoteEndPoint).Address.ToString(); }
        public override string Port { get => ((IPEndPoint)_client.Client.RemoteEndPoint).Port.ToString(); }
        public bool Connected;

        private TcpClient _client;      

        private NetworkStream _stream;     
        private StreamReader _reader;
        private StreamWriter _writer;
        private BufferedStream _bufferedStream;

        private bool _IsActivated;
        
        public virtual void Init(TcpClient client)
        {
            _client = client;
            _stream = _client.GetStream();
            _bufferedStream = new BufferedStream(_stream);
            _reader = new StreamReader(_bufferedStream, Encoding.UTF8);
            _writer = new StreamWriter(_bufferedStream, Encoding.UTF8);
            _IsActivated = true;
        }

        public override string Listen()
        {
            if (!_IsActivated)
                throw new Exception(NotActiveExeption);
            try
            {
                string receivedMessage = _reader.ReadLine();
                return receivedMessage;
            }
            catch
            {
                throw new Exception(ErrorOccurredWhileReadingDataFromClient);
            }
        }

        public override void Push(string message)
        {
            if (!_IsActivated)
                throw new Exception(NotActiveExeption);
            try
            {
                _writer.WriteLine(message);
                _writer.Flush();
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorSendingMessage, ex);
            }
        }

        public override void CloseConnection()
        {
            Dispose();
        }

        public override void Dispose()
        {
            Push(BaseCommands.Disconnect);
            _client.Close();
        }
    }
}

