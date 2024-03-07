using NASDataBaseAPI.Client;
using NASDataBaseAPI.Data.DataTypesInColumn;
using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data;
using System;


namespace NASDataBaseAPI.Server.Handlers.Unsafe.CommandsForDataBase
{
    public class AddColumn : ServerCommand
    {
        private Action<IColumn> _handler;
        private IColumn _column;

        public AddColumn(Action<IColumn> Handler) 
        {
            _handler = Handler;
        }

        public override void SetData(string data)
        {
            var d = data.Split(BaseCommands.SEPARATION.ToCharArray());

            _column = new Column(d[0], GetTypeByName(d[1]), 0);
        }

        private DataType GetTypeByName(string name)
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
