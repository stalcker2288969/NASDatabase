using NASDatabase.Client;
using NASDatabase.Client.Utilities;
using NASDatabase.Interfaces;
using System;

namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase
{
    internal class GetAllIDsByParams : CommandHandler
    {
        private Func<string, string, int[]> _handler;
        private string _columnName;
        private string _param;
        private IDataConverter _converter;
        
        public GetAllIDsByParams(Func<string, string, int[]> handler, IDataConverter converter)
        { _handler = handler;
            _converter = converter;
        }

        public override void SetData(string data)
        {
            var d = data.Split(BaseCommands.SEPARATION.ToCharArray());
            _columnName = d[0];
            _param = d[1];
        }

        public override string Use()
        {
            var res = _handler?.Invoke(_columnName, _param);
            return _converter.ParsInts(res);
        }
    }
}
