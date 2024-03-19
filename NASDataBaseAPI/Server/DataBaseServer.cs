using NASDatabase.Server.Data;
using System;
using System.Collections.Generic;
using NASDatabase.Client.Utilities;

namespace NASDatabase.Server
{
    /// <summary>
    /// Базовый класс для создания сервера у БД
    /// </summary>
    public abstract class DatabaseServer 
    {
        #region События
        public event Action<ServerCommandsPusher> _ClientConnect;
        public event Action _OnStartServer;
        public event Action _OnStopServer;
        #endregion

        public readonly IDataConverter DataConverter;
        public List<ServerCommandsPusher> Clients { get; protected set; } = new List<ServerCommandsPusher>();
        public CommandsFactory Commands { get; protected set; }

        protected Database Database { get; private set; }
        public ServerSettings ServerSettings { get; protected set; }
        
        #region Конструкторы 
        public DatabaseServer(ServerSettings serverSettings, Database db, CommandsFactory commandsParser)
        {
            Database = db;
            ServerSettings = serverSettings;

            #region Создание обработчиков на команды с сервера           
            Commands = commandsParser;
            
            if(Commands == null)
            {
                throw new ArgumentNullException("CommandsParser должен быть инициализирован!");
            }
            #endregion
        }

        public DatabaseServer(ServerSettings serverSettings, Database dataBase, CommandsFactory commandsParser, IDataConverter dataConverter) : this(serverSettings, dataBase, commandsParser)
        {
            DataConverter = dataConverter;
        }
        #endregion
  
        /// <summary>
        /// Запуск сервера 
        /// </summary>
        public abstract void InitServer();

        public abstract void DisconnectClient(ServerCommandsPusher client);

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
