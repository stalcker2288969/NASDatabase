using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using NASDatabase.Server.Data.Modules.Handlers;

namespace NASDataBaseAPI.Server.Data.LogSystem
{
    internal class OnLoadedNewSector : Handler<Database, Database>
    {
        readonly Loger _loger;

        public OnLoadedNewSector(Loger loger) : base(DatabaseEventType.LoadedNewSector)
        {
            _loger = loger;
        }

        public override void Work(object v1, object v2)
        {
            int sector = (int)v1;
            _loger.Log($"Loaded new sector [{sector}]!");
        }
    }
}
