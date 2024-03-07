using NASDataBaseAPI.Client;
using NASDataBaseAPI.Client.Utilities;
using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data;
using System.Text;


namespace NASDataBaseAPI.Server.Handlers.Unsafe.CommandsForDataBase.AdditionalSet
{
    public class LoadDataBaseColumnsState : ServerCommand
    {
        private DataBase _db;
        private IDataConverter _converter;

        public LoadDataBaseColumnsState(DataBase db, IDataConverter converter)
        {
            _db = db;
            _converter = converter;
        }

        public override string Use()
        {
            var sb = new StringBuilder();
            foreach (var column in _db.Columns)
            {
                sb.Append(_converter.ParsColumn(column));
                sb.Append(BaseCommands.SEPARATION);
            }
            return sb.ToString();
        }
    }
}
