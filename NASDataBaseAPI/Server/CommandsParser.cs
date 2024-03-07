using NASDataBaseAPI.Interfaces;
using System.Collections.Generic;


namespace NASDataBaseAPI.Server
{
    /// <summary>
    /// Фабрика для работы с командами 
    /// </summary>
    public class CommandsParser
    {
        private Dictionary<string, ServerCommand> Commands;

        public CommandsParser()
        {
            Commands = new Dictionary<string, ServerCommand>();
        }
        
        public void AddCommand(string command, ServerCommand commandHandler)
        {
            Commands.Add(command, commandHandler);
        }

        public void RemoveCommand(string command)
        {
            Commands.Remove(command);
        }

        public ServerCommand this[string key]
        {
            get { return Commands[key]; }
        }
    }
}
