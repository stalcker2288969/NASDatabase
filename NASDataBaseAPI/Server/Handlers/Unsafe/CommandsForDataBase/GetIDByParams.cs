using NASDatabase.Client;
using NASDatabase.Client.Utilities;
using NASDatabase.Interfaces;
using System;

namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase
{
    internal class GetIDByParams : CommandHandler
    {
        private Func<string, string, int, int> _handler;
        private IDataConverter _dataConverter;
        private string _columnName;
        private string _param;
        private int _sector;

        public GetIDByParams(Func<string, string, int, int> handler, IDataConverter dataConverter)
        {
            _handler = handler;
            _dataConverter = dataConverter;
        }

        public override void SetData(string data)
        {
            var d = data.Split(BaseCommands.SEPARATION.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            _columnName = d[0];
            _param = d[1];
            _sector = int.Parse(d[2]);
        }

        public override string Use()
        {
            return _handler?.Invoke(_columnName, _param, _sector).ToString();
        }
    }
}
