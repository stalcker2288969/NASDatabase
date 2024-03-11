using NASDataBaseAPI.Client.Utilities;
using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data;


namespace NASDataBaseAPI.Server.Handlers.Unsafe.CommandsForDataBase
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
            var d = _converter.GetDataLine<Rows>(data);

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
