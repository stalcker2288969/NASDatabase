using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data;
using NASDataBaseAPI.Server.Data.DataBaseSettings;
using System.Text.Json;


namespace NASDataBaseAPI.Server.Handlers.Unsafe.CommandsForDataBase.AdditionalSet
{
    public class LoadDataBaseSettings : CommandHandler
    {
        private Table _db;

        public LoadDataBaseSettings(Table dataBase) { _db = dataBase; }

        public override string Use()
        {
            return JsonSerializer.Serialize<DatabaseSettings>(_db.Settings);
        }
    }
}
