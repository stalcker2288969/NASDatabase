using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using NASDatabase.Server.Data.Modules.Handlers;


namespace NASDataBaseAPI.Server.Data.LogSystem
{
    internal class OnSetDataInColumn : Handler<Database, Database>
    {
        readonly Loger _loger;

        public OnSetDataInColumn(Loger loger) : base(DatabaseEventType.SetDataInColumn)
        {
            _loger = loger;
        }

        public override void Work(object v1, object v2)
        {
            string columnName = (string)v1;
            ItemData itemData = (ItemData)v2;
            _loger.Log($"The data [{itemData}] in the column [{columnName}] has been set!");
        }
    }
}
