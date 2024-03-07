using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data;
using NASDataBaseAPI.Server.Data.DataBaseSettings;
using System.Text.Json;


namespace NASDataBaseAPI.Server.Handlers.Unsafe.CommandsForDataBase.AdditionalSet
{
    public class LoadDataBaseSettings : ServerCommand
    {
        private DataBase _db;

        public LoadDataBaseSettings(DataBase dataBase) { _db = dataBase; }

        public override string Use()
        {
            return JsonSerializer.Serialize<DataBaseSettings>(_db.Settings);
        }
    }
}
