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
            Rows baseLine = new Rows();
            baseLine.Init(_id, _handler?.Invoke(_id));

            return _dataConverter.ParsDataLine(baseLine);
        }
    }
}
