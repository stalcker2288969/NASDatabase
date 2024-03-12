
namespace NASDatabase.Server
{
    public struct ServerSettings
    {
        public string IP;
        public int Port;
        public string Key;

        public ServerSettings(string ip, int port, string key)
        {
            IP = ip;
            Port = port;
            Key = key;
        }

        public ServerSettings(string ip, int port)
        {
            IP = ip;
            Port = port;
            Key = "";
        }

        public override string ToString()
        {
            return $"IP: {IP}, Port: {Port}, Key: {Key}";
        }
    }
}
