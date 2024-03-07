using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NASDataBaseAPI.Client;
using NASDataBaseAPI.Server.Data;
using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Handlers.Unsafe.CommandsForDataBase.AdditionalSet;
using NASDataBaseAPI.Server.Handlers.Unsafe.CommandsForDataBase;

namespace NASDataBaseAPI.Server
{
    /// <summary>
    /// Основной класс для работы с базой данных по сети (новый функционал не вводит, просто улучшает tcp систему)
    /// </summary>
    public class DBServer<T> : DataBaseServer where T : BaseServerCommandPusher, new()
    {
        public const string ErrorOccurredWhenConnectingClientExceptionText = "Во время подключения клиента произошла ошибка: ";

        public TcpListener Server { get; private set; }

        public DBServer(ServerSettings serverSettings, DataBase dataBase) : base(serverSettings, dataBase,  new CommandsParser())
        { 
        }

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

        private void Handler(ICommandWorker worker)
        {
            string res = "";
            var sb = new StringBuilder(); sb.Append("");
            
            while(true)
            {
                res = worker.Listen();

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

                    worker.Push(resText);
                }
                catch(Exception ex)
                {
                    worker.Push(BaseCommands.ERROR + BaseCommands.SEPARATION + ex.Message);

                    break;
                }

                sb.Clear();
            }
        }

        public override void DisconnectClient(ICommandWorker client)
        {
            throw new NotImplementedException();
        }

        public override void Shutdown()
        {
            if (_isServerRunning)
            {
                foreach(var c in Clients)
                {
                    c.Push(BaseCommands.Disconnect);
                }

                _isServerRunning = false;
                OnServerStop();
                Server.Stop();
            }
        }

        private void InitCommands(CommandsParser commandsParser, DataBase db)
        {
            commandsParser.AddCommand(BaseCommands.AddData, new AddData(db, DataConverter));
            commandsParser.AddCommand(BaseCommands.RemoveDataByID, new RemoveDataByID(db.RemoveDataByID));
            commandsParser.AddCommand(BaseCommands.Login, new Login(ServerSettings, ClientHandler));
            commandsParser.AddCommand(BaseCommands.MSG, new MSGFromClient());
            commandsParser.AddCommand(BaseCommands.SetDataInColumn, new SetDataInColumn(DataConverter, db.SetDataInColumn));
            commandsParser.AddCommand(BaseCommands.GetAllIDsByParams, new GetAllIDsByParams(db.GetAllIDsByParams, DataConverter));
            commandsParser.AddCommand(BaseCommands.AddColumn, new AddColumn(db.AddColumn));
            commandsParser.AddCommand(BaseCommands.RemoveColumn, new RemoveColumn(db.RemoveColumn));
            commandsParser.AddCommand(BaseCommands.CloneColumn, new CloneTo(db.CloneTo));
            commandsParser.AddCommand(BaseCommands.ClearColumn, new ClearAllColumn(db.ClearAllColumn));
            commandsParser.AddCommand(BaseCommands.RenameColumn, new RenameColumn(db.RenameColumn));
            commandsParser.AddCommand(BaseCommands.LoadDataBaseState, new LoadDataBaseSettings(db));
            commandsParser.AddCommand(BaseCommands.LoadDataBaseColumnsState, new LoadDataBaseColumnsState(db, DataConverter));
            commandsParser.AddCommand(BaseCommands.ClearAllBase, new ClearAllBase(db.ClearAllBase));
            commandsParser.AddCommand(BaseCommands.ChangeEverythingTo, new ChangeEverythingTo(db.ChangeEverythingTo));
            commandsParser.AddCommand(BaseCommands.SetData, new SetDataServerCommand(db.SetData<BaseLine>, DataConverter));
            commandsParser.AddCommand(BaseCommands.ChengTypeInColumn, new ChengTypeInColumn(db.ChengTypeInColumn));
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
