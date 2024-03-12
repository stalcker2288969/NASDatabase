using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NASDatabase.Client;
using NASDatabase.Server.Data;
using NASDatabase.Interfaces;
using NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase.AdditionalSet;
using NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase;
using NASDatabase.Client.Utilities;

namespace NASDatabase.Server
{
    /// <summary>
    /// Основной класс для работы с базой данных по сети (новый функционал не вводит, просто улучшает tcp систему)
    /// </summary>
    public class DBServer<T> : DatabaseServer where T : BaseServerCommandPusher, new()
    {
        public const string ErrorOccurredWhenConnectingClientExceptionText = "Во время подключения клиента произошла ошибка: ";

        public TcpListener Server { get; private set; }

        public DBServer(ServerSettings serverSettings, Database dataBase) : base(serverSettings, dataBase,  new CommandsFactory(), new DataConverter())
        { 
        }

        public DBServer(ServerSettings ServerSettings, Database DataBase, CommandsFactory CommandsParser, IDataConverter DataConverter) : base(ServerSettings, DataBase, CommandsParser, DataConverter) { }

        public string ServerIP { get; private set; }
        public int Port { get; private set; }

        private bool _isServerRunning = false;

        public override void InitServer()
        {
            if (!_isServerRunning)
            {
                ServerIP = ServerSettings.IP;
                Port = ServerSettings.Port;
            
                Server = new TcpListener(IPAddress.Parse(ServerIP), Port);  
                Server.Start();

                _isServerRunning = true;
                OnServerInit();

                InitCommands(Commands, DataBase);
                WaitForClients();
            }
        }

        private async void WaitForClients()
        {
            try
            {
                while(_isServerRunning)
                {
                    TcpClient client = await Server.AcceptTcpClientAsync();

                    var commandWorker = new T();
                    commandWorker.Init(client);

                    Clients.Add(commandWorker);
                    
                    OnClientConnect(Clients[Clients.Count - 1]);

                    Task.Run(() => Handler(Clients[Clients.Count - 1]));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorOccurredWhenConnectingClientExceptionText + ex.Message);
            }
        }

        private void Handler(ICommandWorker Worker)
        {
            string res = "";
            var sb = new StringBuilder();
            
            while(true)
            {
                res = Worker.Listen();

                string[] datas = res.Split(BaseCommands.SEPARATION.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                
                if(datas.Length > 3)
                {
                    sb.Append(datas[3]);
                    for (int i = 4; i < datas.Length; i++)
                    {
                        sb.Append(BaseCommands.SEPARATION);
                        sb.Append(datas[i]);
                    }            
                }
                
                var c = Commands[datas[2]];
                              
                try
                {
                    c.SetData(sb.ToString());
                    
                    var resText = c.Use();

                    Worker.Push(resText);
                }
                catch(Exception ex)
                {
                    Worker.Push(BaseCommands.ERROR + BaseCommands.SEPARATION + ex.Message);

                    break;
                }

                sb.Clear();
            }
        }

        public override void DisconnectClient(ServerCommandsPusher client)
        {
            client.CloseConnection();
            Clients.Remove(client);
        }

        public override void Shutdown()
        {
            if (_isServerRunning)
            {
                foreach(var client in Clients)
                {
                    client.Push(BaseCommands.Disconnect);
                }

                _isServerRunning = false;
                OnServerStop();
                Server.Stop();
            }
        }

        private void InitCommands(CommandsFactory CommandsParser, Database DB)
        {
            CommandsParser.AddCommand(BaseCommands.AddData, new AddData(DB, DataConverter));
            CommandsParser.AddCommand(BaseCommands.RemoveDataByID, new RemoveDataByID(DB.RemoveDataByID));
            CommandsParser.AddCommand(BaseCommands.Login, new Login(ServerSettings, ClientHandler));
            CommandsParser.AddCommand(BaseCommands.MSG, new MSGFromClient());
            CommandsParser.AddCommand(BaseCommands.SetDataInColumn, new SetDataInColumn(DataConverter, DB.SetDataInColumn));
            CommandsParser.AddCommand(BaseCommands.GetAllIDsByParams, new GetAllIDsByParams(DB.GetAllIDsByParams, DataConverter));
            CommandsParser.AddCommand(BaseCommands.AddColumn, new AddColumn(DB.AddColumn));
            CommandsParser.AddCommand(BaseCommands.RemoveColumn, new RemoveColumn(DB.RemoveColumn));
            CommandsParser.AddCommand(BaseCommands.CloneColumn, new CloneTo(DB.CloneTo));
            CommandsParser.AddCommand(BaseCommands.ClearColumn, new ClearAllColumn(DB.ClearAllColumn));
            CommandsParser.AddCommand(BaseCommands.RenameColumn, new RenameColumn(DB.RenameColumn));
            CommandsParser.AddCommand(BaseCommands.LoadDataBaseState, new LoadDataBaseSettings(DB));
            CommandsParser.AddCommand(BaseCommands.LoadDataBaseColumnsState, new LoadDataBaseColumnsState(DB, DataConverter));
            CommandsParser.AddCommand(BaseCommands.ClearAllBase, new ClearAllBase(DB.ClearAllBase));
            CommandsParser.AddCommand(BaseCommands.ChangeEverythingTo, new ChangeEverythingTo(DB.ChangeEverythingTo));
            CommandsParser.AddCommand(BaseCommands.SetData, new SetDataServerCommand(DB.SetData<Row>, DataConverter));
            CommandsParser.AddCommand(BaseCommands.ChengTypeInColumn, new ChengTypeInColumn(DB.ChengTypeInColumn));
            CommandsParser.AddCommand(BaseCommands.PrintBase, new PrintBase(DB));
            CommandsParser.AddCommand(BaseCommands.GetAllDataInBaseByColumnName, new GetAllDataInBaseByColumnName(DB.GetAllDataInBaseByColumnName, DataConverter));
            CommandsParser.AddCommand(BaseCommands.GetIDByParams, new GetIDByParams(DB.GetIDByParams, DataConverter));
            CommandsParser.AddCommand(BaseCommands.GetDataByID, new GetDataByID(DB.GetDataByID, DataConverter));
        }

        /// <summary>
        /// Обрабатывает подключение клиента
        /// </summary>
        /// <param name="canConnect"></param>
        private void ClientHandler(bool canConnect)
        {
            if (!canConnect)
            {
                Clients[Clients.Count - 1].CloseConnection();
                Clients.Remove(Clients[Clients.Count - 1]);
            }
        }
    }
}
