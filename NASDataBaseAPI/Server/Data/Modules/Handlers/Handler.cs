﻿namespace NASDatabase.Server.Data.Modules.Handlers
{
    /// <summary>
    /// Стандрантный класс для обработки событий в базах данных, работает в Connector-е
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Handler<T1, T2> where T1 : Database where T2 : Database
    {
        public readonly DatabaseEventType Type;
        protected T1 DB1;
        protected T2 DB2;

        public Handler(DatabaseEventType type)
        {
            this.Type = type;
        }

        public void Init(T1 DB1, T2 DB2)
        {
            this.DB1 = DB1;
            this.DB2 = DB2;
        }

        /// <summary>
        /// Метод будет выполняться для обработки события указанного в DataBaseEventType Type
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        public abstract void Work(object v1, object v2);
        
        /// <summary>
        /// Метод будет выполняться при разрыве связи баз данных
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        public virtual void OnDestory() { }
    }
}
