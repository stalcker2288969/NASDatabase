using NASDataBaseAPI.Client;
using NASDataBaseAPI.Client.Utilities;
using NASDataBaseAPI.Interfaces;
using System;

namespace NASDataBaseAPI.Server.Handlers.Unsafe.CommandsForDataBase
{
    public class GetAllIDsByParams : CommandHandler
    {
        private Func<string, string, int[]> _handler;
        private string _columnName;
        private string _param;
        private IDataConverter _converter;
        
        public GetAllIDsByParams(Func<string, string, int[]> Handler, IDataConverter converter)
        { _handler = Handler;
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
