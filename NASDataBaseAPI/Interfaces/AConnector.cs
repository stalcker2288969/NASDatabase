using NASDatabase.Server.Data;
using NASDatabase.Server.Data.Modules.Handlers;
using NASDatabase.Server.Handlers.Unsafe.CommandsForDataBase;
using System;

namespace NASDatabase.Interfaces
{
    public abstract class AConnector<T1, T2> : IDisposable where T1 : Database where T2 : Database 
    {
        public T1 DB1 { get; protected set; }
        public T2 DB2 { get; protected set; }
        /// <summary>
        /// Добавляет связь через обработчик
        /// </summary>
        /// <param name="Handler"></param>
        public abstract void AddHandler(Handler<T1,T2> Handler);
        public abstract void DestroyHandler(Handler<T1, T2> Handler);

        public abstract void Dispose();
    }
}
