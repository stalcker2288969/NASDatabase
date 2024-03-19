using NASDatabase.Client;
using NASDatabase.Client.Utilities;
using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using System;

namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase
{
    internal class GetAllDataInBaseByColumnName : CommandHandler
    {
        private Func<string, string, Row[]> _handler;
        IDataConverter _dataConverter;
        private string _columnName;
        private string _param;

        public GetAllDataInBaseByColumnName(Func<string, string, Row[]> handler, IDataConverter dataConverter) 
        { _handler = handler; _dataConverter = dataConverter; }

        public override void SetData(string data)
        {
            var d = data.Split(BaseCommands.SEPARATION.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            _columnName = d[0];
            _param = d[1];
        }

        public override string Use()
        {
            var res = _handler?.Invoke(_columnName, _param);
            return _dataConverter.ParsDataLines(res);
        }
    }
}
