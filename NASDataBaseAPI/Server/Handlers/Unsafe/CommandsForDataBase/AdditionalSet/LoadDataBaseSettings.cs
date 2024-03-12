using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using NASDatabase.Server.Data.DatabaseSettings;
using System.Text.Json;


namespace NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase.AdditionalSet
{
    public class LoadDataBaseSettings : CommandHandler
    {
        private Database _db;

        public LoadDataBaseSettings(Database dataBase) { _db = dataBase; }

        public override string Use()
        {
            return JsonSerializer.Serialize<DatabaseSettings>(_db.Settings);
        }
    }
}
