using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using NASDatabase.Server.Data.Modules.Handlers;
using System.Text;

namespace NASDataBaseAPI.Server.Data.LogSystem
{
    internal class OnAddDataLog : Handler<Database, Database>
    {
        private readonly Loger _loger;

        public OnAddDataLog(Loger loger) : base(DatabaseEventType.AddData)//Указываем на что реагируем 
        {
            _loger = loger;
        }

        public override void Work(object v1, object v2)
        {
            var datas = v1 as string[];
            var ID = (int)v2;
            
            _loger.Log($"AddData to [{ID}] data [{string.Join(",", datas)}]");
        }
    }
}
