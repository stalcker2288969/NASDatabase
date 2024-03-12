using NASDatabase.Client.Utilities;
using NASDatabase.Interfaces;
using NASDatabase.Server.Data;


namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase
{
    public class AddData : CommandHandler
    {
        private Database _db;
        private string[] _data;
        private IDataConverter _converter;

        public AddData(Database dataBase, IDataConverter dataConverter) 
        {
            _db = dataBase;
            _converter = dataConverter;
        }

        public override void SetData(string data)
        {
            var d = _converter.GetDataLine<Row>(data);

            this._data = d.GetData();
        }

        public override string Use()
        {
            _db.AddData(_data);

            var x = _db.GetIDByParams(_db[0].Name, _data[0]).ToString();

            return x;
        }
    }
}
