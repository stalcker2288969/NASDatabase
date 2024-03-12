using NASDatabase.Client;
using NASDatabase.Data.DataTypesInColumn;
using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using System;


namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase
{
    public class AddColumn : CommandHandler
    {
        private Action<Column> _handler;
        private Column _column;

        public AddColumn(Action<Column> Handler) 
        {
            _handler = Handler;
        }

        public override void SetData(string data)
        {
            var d = data.Split(BaseCommands.SEPARATION.ToCharArray());

            _column = new Column(d[0], GetTypeByName(d[1]), 0);
        }

        private TypeOfData GetTypeByName(string name)
        {
           return DataTypesInColumns.GetTypeOfDataByClassName(name);
        }

        public override string Use()
        {
            _handler?.Invoke(_column);

            return BaseCommands.DONE;
        }
    }
}
