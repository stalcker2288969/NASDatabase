using NASDatabase.Client;
using NASDatabase.Client.Utilities;
using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using System;

namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase
{
    internal class GetDataByID : CommandHandler
    {
        private Func<int, string[]> _handler;
        private IDataConverter _dataConverter;
        private int _id;

        public GetDataByID(Func<int, string[]> handler, IDataConverter dataConverter) 
        {
            _handler = handler;
            _dataConverter = dataConverter;
        }

        public override void SetData(string data)
        {     
            _id = int.Parse(data);
        }

        public override string Use()
        {
            Row baseLine = new Row();
            baseLine.Init(_id, _handler?.Invoke(_id));

            return _dataConverter.ParsDataLine(baseLine);
        }
    }
}
