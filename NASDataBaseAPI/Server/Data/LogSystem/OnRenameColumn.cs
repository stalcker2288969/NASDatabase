using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using NASDatabase.Server.Data.Modules.Handlers;

namespace NASDataBaseAPI.Server.Data.LogSystem
{
    internal class OnRenameColumn : Handler<Database, Database>
    {
        readonly Loger _loger;

        public OnRenameColumn(Loger loger) : base(DatabaseEventType.RenamedColumn)
        {
            this._loger = loger;
        }

        public override void Work(object v1, object v2)
        {
            string oldName = (string)v1;
            string newName = (string)v2;
            _loger.Log($"Column [{oldName}] was renamed to [{newName}]!");
        }
    }
}
