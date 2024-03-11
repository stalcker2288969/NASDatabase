using NASDataBaseAPI.Client;
using NASDataBaseAPI.Client.Utilities;
using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Handlers.Unsafe.CommandsForDataBase
{
    public class GetAllDataInBaseByColumnName : CommandHandler
    {
        private Func<string, string, Rows[]> _handler;
        IDataConverter _dataConverter;
        private string _columnName;
        private string _param;

        public GetAllDataInBaseByColumnName(Func<string, string, Rows[]> handler, IDataConverter dataConverter) 
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
