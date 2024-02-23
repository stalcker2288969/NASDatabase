using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.DataBaseSettings.Handlers
{
    /// <summary>
    /// Стандрантный класс для обработки событий в базах данных, работает в Connector-е
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Handler<T> where T : DataBase
    {
        public readonly DataBaseEventType Type;
        protected T DB1, DB2;

        public Handler(DataBaseEventType Type)
        {
            this.Type = Type;
        }

        public void Init(T DB1, T DB2)
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
        public abstract void OnDestory();
    }
}
