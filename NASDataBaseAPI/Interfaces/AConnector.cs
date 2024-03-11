using NASDataBaseAPI.Server.Data;
using NASDataBaseAPI.Server.Data.Modules.Handlers;
using NASDataBaseAPI.Server.Handlers.Unsafe.CommandsForDataBase;
using System;

namespace NASDataBaseAPI.Interfaces
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
