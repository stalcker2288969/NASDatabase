using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using NASDatabase.Server.Data.Modules.Handlers;

namespace NASDataBaseAPI.Server.Data.LogSystem
{
    internal class RemoveColumn : Handler<Database, Database>
    {
        readonly Loger _loger;

        public RemoveColumn(Loger loger) : base(DatabaseEventType.RemoveColumn)
        {
            _loger = loger;
        }

        public override void Work(object v1, object v2)
        {
            string columnName = (string)v1;

            _loger.Log($"The column [{columnName}] has been deleted!");
        }
    }
}
