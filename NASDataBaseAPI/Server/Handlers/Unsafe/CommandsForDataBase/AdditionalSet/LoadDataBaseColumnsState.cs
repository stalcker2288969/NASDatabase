using NASDatabase.Client;
using NASDatabase.Client.Utilities;
using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using System.Text;


namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase.AdditionalSet
{
    public class LoadDataBaseColumnsState : CommandHandler
    {
        private Database _db;
        private IDataConverter _converter;

        public LoadDataBaseColumnsState(Database db, IDataConverter converter)
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
