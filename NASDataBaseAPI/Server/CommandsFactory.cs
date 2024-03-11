using NASDataBaseAPI.Interfaces;
using System.Collections.Generic;


namespace NASDataBaseAPI.Server
{
    /// <summary>
    /// Фабрика для работы с командами 
    /// </summary>
    public class CommandsFactory
    {
        private Dictionary<string, CommandHandler> Commands;

        public CommandsFactory()
        {
            Commands = new Dictionary<string, CommandHandler>();
        }
        
        public void AddCommand(string command, CommandHandler commandHandler)
        {
            Commands.Add(command, commandHandler);
        }

        public void RemoveCommand(string command)
        {
            Commands.Remove(command);
        }

        public CommandHandler this[string key]
        {
            get { return Commands[key]; }
        }
    }
}
