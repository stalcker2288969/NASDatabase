﻿using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using NASDatabase.Server.Data.Modules.Handlers;

namespace NASDataBaseAPI.Server.Data.LogSystem
{
    internal class OnCloneColumn : Handler<Database, Database>
    {
        readonly Loger _loger;

        public OnCloneColumn(Loger loger) : base(DatabaseEventType.CloneColumn)
        {
            _loger = loger;
        }

        public override void Work(object v1, object v2)
        {
            string left = (string)v1;
            string right = (string)v2;

            _loger.Log($"A {left} was copied to {right}"); 
        }
    }
}
