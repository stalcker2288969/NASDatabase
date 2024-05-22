using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using NASDatabase.Server.Data.Modules.Handlers;


namespace NASDataBaseAPI.Server.Data.LogSystem
{
    internal class OnClearAllBase : Handler<Database, Database>
    {
        readonly Loger _loger;

        public OnClearAllBase(Loger loger) : base(DatabaseEventType.ClearAllBase)
        {
            _loger = loger;
        }

        public override void Work(object v1, object v2) => _loger.Log("The database has been cleared!");
    }
}
