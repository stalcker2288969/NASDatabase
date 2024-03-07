using NASDataBaseAPI.Server.Data.Modules.Handlers;

namespace NASDataBaseAPI.Server.Data.Modules
{
    /// <summary>
    /// Устанавливает связь между двуня БД 
    /// </summary>
    public class BinaryConnector<T1,T2> : Connector<T1,T2> where T1 : DataBase where T2 : DataBase
    {
        public bool DefaultConnectionIsLeft;
        private Connector<T1,T2> LeftConnector;
        private Connector<T1, T2> RightConnector;

        public BinaryConnector(T1 DB1, T2 DB2,bool DefaultConnectionIsLeft = true) : base(DB1, DB2)
        {
            LeftConnector = new Connector<T1, T2>(DB1, DB2);
            RightConnector = new Connector<T1, T2>(DB1, DB2);
            this.DefaultConnectionIsLeft = DefaultConnectionIsLeft;
        }

        public override void AddConectionByHandler(Handler<T1, T2> Handler)
        {
            if (DefaultConnectionIsLeft)
            {
                LeftConnector.AddConectionByHandler(Handler);
            }
            else
            {
                RightConnector.AddConectionByHandler(Handler);
            }
        }

        //Добавляет обработчки к левой базе данных
        public void AddLeftConectionByHandler(Handler<T1, T2> Handler)
        {
            LeftConnector.AddConectionByHandler(Handler);
        }

        //Добавляет обработчик к правой базе данных
        public void AddRightConectionByHandler(Handler<T1, T2> Handler)
        {
            RightConnector.AddConectionByHandler(Handler);
        }

        //Добавляет обработчик в оба направления сразу 
        public void AddHandlerInBothDirections(Handler<T1, T2> Handler)
        {
            RightConnector.AddConectionByHandler(Handler);
            LeftConnector.AddConectionByHandler(Handler);
        }
    }
}
