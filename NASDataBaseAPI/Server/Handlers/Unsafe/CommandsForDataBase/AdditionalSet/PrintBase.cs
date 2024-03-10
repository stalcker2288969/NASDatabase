using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data;


namespace NASDataBaseAPI.Server.Handlers.Unsafe.CommandsForDataBase.AdditionalSet
{
    public class PrintBase : CommandHandler
    {
        private DataBase _db;

        public PrintBase(DataBase db) { _db = db; }

        public override string Use()
        {
            return _db.PrintBase();
        }
    }
}
