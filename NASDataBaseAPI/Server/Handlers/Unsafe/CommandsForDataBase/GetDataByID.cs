using NASDatabase.Client;
using NASDatabase.Client.Utilities;
using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase
{
    public class GetDataByID : CommandHandler
    {
        private Func<int, string[]> _handler;
        private IDataConverter _dataConverter;
        private int _id;

        public GetDataByID(Func<int, string[]> Handler, IDataConverter DataConverter) 
        {
            _handler = Handler;
            _dataConverter = DataConverter;
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
