using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using NASDatabase.Server.Data.Modules.Handlers;

namespace NASDataBaseAPI.Server.Data.LogSystem
{
    internal class OnClearAllColumn : Handler<Database, Database>
    {
        readonly Loger _loger;

        public OnClearAllColumn(Loger loger) : base(DatabaseEventType.ClearAllColumn)
        {
            _loger = loger;
        }

        public override void Work(object v1, object v2)
        {
            string column = (string)v1;
            _loger.Log($"Column [{column}] was cleared!");
        }
    }
}
