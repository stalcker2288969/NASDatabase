namespace NASDataBaseAPI.Server.Data.Modules.Handlers
{
    /// <summary>
    /// Стандрантный класс для обработки событий в базах данных, работает в Connector-е
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Handler<T1, T2> where T1 : DataBase where T2 : DataBase
    {
        public readonly DataBaseEventType Type;
        protected T1 DB1;
        protected T2 DB2;

        public Handler(DataBaseEventType Type)
        {
            this.Type = Type;
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
