using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using NASDatabase.Server.Data.Modules.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.LogSystem
{
    internal class OnRemoveData : Handler<Database, Database>
    {
        private Loger _loger;

        public OnRemoveData(Loger loger) : base(DatabaseEventType.RemoveData)
        {
            this._loger = loger;
        }

        public override void Work(object v1, object v2)
        {
            int ID = (int)v2;
            string[] datas = (string[])v1;
            _loger.Log($"The ID [{ID}] data [{string.Join(",", datas)}] was deleted!");
        }
    }
}
