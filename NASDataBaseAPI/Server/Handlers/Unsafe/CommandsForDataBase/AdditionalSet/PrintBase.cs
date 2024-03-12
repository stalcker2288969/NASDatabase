using NASDatabase.Interfaces;
using NASDatabase.Server.Data;


namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase.AdditionalSet
{
    public class PrintBase : CommandHandler
    {
        private Database _db;

        public PrintBase(Database db) { _db = db; }

        public override string Use()
        {
            return _db.PrintBase();
        }
    }
}
