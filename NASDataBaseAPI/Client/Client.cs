using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Client
{
    public class Client
    {
        public string Name { get; private set; }
        public string Password { get; private set; }

        private NetworkStream stream;
        private TcpClient client;

        public Client(string Name, string Password)
        {
            this.Name = Name;
            this.Password = Password;
        }        

        public bool ConnectTo(string IP, int Port)
        {
            bool result = false;

            client = new TcpClient(IP, Port);
            stream = client.GetStream();

            byte[] data = Encoding.ASCII.GetBytes($"Com_Login:{Name}:{Password}");
            stream.Write(data, 0, data.Length);

            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            
            if(response == "Disconnect")
            {
                result = false;
            }
            else if(response == "Connect")
            {
                result = true;
            }
            else
            {
                return true;
            }

            return result;
        }
        
        /// <summary>
        /// Отправляет команду на сервер
        /// </summary>
        /// <param name="Command"></param>
        public void PushCommand(string Command)
        {
            byte[] data = Encoding.ASCII.GetBytes(Command);
            stream.Write(data, 0, data.Length);           
        }

        /// <summary>
        /// Получает информацию с сервера
        /// </summary>
        /// <returns></returns>
        public string PopMSG()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            int x = int.Parse(response);
            byte[] buffer2 = new byte[x];
            bytesRead = stream.Read(buffer2, 0, buffer2.Length);
            response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            return response;
        }

        public NetworkStream GetNetwork()
        {
            return stream ?? client?.GetStream();
        }

    }
}
