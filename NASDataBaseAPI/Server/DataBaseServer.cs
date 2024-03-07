using NASDataBaseAPI.Server.Data;
using System;
using System.Collections.Generic;
using NASDataBaseAPI.Client.Utilities;
using NASDataBaseAPI.Interfaces;

namespace NASDataBaseAPI.Server
{
    public abstract class DataBaseServer 
    {
        #region События
        public event Action<ServerCommandsPusher> _ClientConnect;
        public event Action _OnStartServer;
        public event Action _OnStopServer;
        #endregion

        public IDataConverter DataConverter = new DataConverter();
        public List<ServerCommandsPusher> Clients { get; protected set; } = new List<ServerCommandsPusher>();
        public CommandsParser Commands { get; protected set; }

        protected DataBase DataBase { get; private set; }
        public ServerSettings ServerSettings { get; protected set; }
        
        #region Конструкторы 
        public DataBaseServer(ServerSettings serverSettings, DataBase db, CommandsParser commandsParser)
        {
            DataBase = db;
            ServerSettings = serverSettings;

            #region Создание обработчиков на команды с сервера           
            Commands = commandsParser;
            
            if(Commands == null)
            {
                throw new ArgumentNullException("CommandsParser должен быть инициализирован!");
            }
            #endregion
        }

        public DataBaseServer(ServerSettings serverSettings, DataBase dataBase, CommandsParser commandsParser, IDataConverter dataConverter) : this(serverSettings, dataBase, commandsParser)
        {
            DataConverter = dataConverter;
        }
        #endregion
  
        /// <summary>
        /// Запуск сервера 
        /// </summary>
        public abstract void InitServer();

        public abstract void DisconnectClient(ICommandWorker Client);

        /// <summary>
        /// Выключение сервера
        /// </summary>
        public abstract void Shutdown();

        #region Вызов событий для наследников 
        protected void OnServerInit()
        {
            _OnStartServer?.Invoke();
        }

        protected void OnServerStop()
        {
            _OnStopServer?.Invoke();
        }

        protected void OnClientConnect(ServerCommandsPusher client)
        {
            _ClientConnect?.Invoke(client);
        }
        #endregion
    }
}
