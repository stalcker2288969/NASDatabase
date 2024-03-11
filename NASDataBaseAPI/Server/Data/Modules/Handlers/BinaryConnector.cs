using NASDataBaseAPI.Server.Data.Modules.Handlers;

namespace NASDataBaseAPI.Server.Data.Modules
{
    /// <summary>
    /// Устанавливает связь между двуня БД 
    /// </summary>
    public class BinaryConnector<T1,T2> : Connector<T1,T2> where T1 : Database where T2 : Database
    {
        public bool DefaultConnectionIsLeft;
        private Connector<T1,T2> _leftConnector;
        private Connector<T1, T2> _rightConnector;

        public BinaryConnector(T1 DB1, T2 DB2,bool DefaultConnectionIsLeft = true) : base(DB1, DB2)
        {
            _leftConnector = new Connector<T1, T2>(DB1, DB2);
            _rightConnector = new Connector<T1, T2>(DB1, DB2);
            this.DefaultConnectionIsLeft = DefaultConnectionIsLeft;
        }

        public override void AddHandler(Handler<T1, T2> Handler)
        {
            if (DefaultConnectionIsLeft)
            {
                _leftConnector.AddHandler(Handler);
            }
            else
            {
                _rightConnector.AddHandler(Handler);
            }
        }

        //Добавляет обработчки к левой базе данных
        public void AddLeftConectionByHandler(Handler<T1, T2> Handler)
        {
            _leftConnector.AddHandler(Handler);
        }

        //Добавляет обработчик к правой базе данных
        public void AddRightConectionByHandler(Handler<T1, T2> Handler)
        {
            _rightConnector.AddHandler(Handler);
        }

        //Добавляет обработчик в оба направления сразу 
        public void AddHandlerInBothDirections(Handler<T1, T2> Handler)
        {
            _rightConnector.AddHandler(Handler);
            _leftConnector.AddHandler(Handler);
        }
    }
}
