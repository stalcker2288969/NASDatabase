using NASDataBaseAPI.Client;
using NASDataBaseAPI.Data.DataTypesInColumn;
using NASDataBaseAPI.Interfaces;
using System;

namespace NASDataBaseAPI.Server.Handlers.Unsafe.CommandsForDataBase
{
    public class ChengTypeInColumn : ServerCommand
    {
        private Action<string, DataType> _handler;
        private DataType _dataType;
        private string _columnName;

        public ChengTypeInColumn(Action<string, DataType> handler)
        {
            _handler = handler;
        }

        public override void SetData(string data)
        {
            var d = data.Split(BaseCommands.SEPARATION.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            _dataType = DataTypesInColumns.GetTypeOfDataByClassName(d[1]);
            _columnName = d[0];
        }

        public override string Use()
        {
            _handler.Invoke(_columnName, _dataType);
            return BaseCommands.DONE;
        }
    }
}
