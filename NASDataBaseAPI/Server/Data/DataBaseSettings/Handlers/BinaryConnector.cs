using NASDataBaseAPI.Server.Data.DataBaseSettings.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.DataBaseSettings
{
    /// <summary>
    /// Устанавливает связь между двуня БД 
    /// </summary>
    public class BinaryConnector : Connector
    {
        public bool DefaultConnectionIsLeft;
        private Connector LeftConnector;
        private Connector RightConnector;

        public BinaryConnector(DataBase DB1, DataBase DB2,bool DefaultConnectionIsLeft = true) : base(DB1, DB2)
        {
            LeftConnector = new Connector(DB1, DB2);
            RightConnector = new Connector(DB1, DB2);
            this.DefaultConnectionIsLeft = DefaultConnectionIsLeft;
        }

        public override void AddConectionByHandler(Handler<DataBase> Handler)
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
        public void AddLeftConectionByHandler(Handler<DataBase> Handler)
        {
            LeftConnector.AddConectionByHandler(Handler);
        }

        //Добавляет обработчик к правой базе данных
        public void AddRightConectionByHandler(Handler<DataBase> Handler)
        {
            RightConnector.AddConectionByHandler(Handler);
        }

        //Добавляет обработчик в оба направления сразу 
        public void AddHandlerInBothDirections(Handler<DataBase> Handler)
        {
            RightConnector.AddConectionByHandler(Handler);
            LeftConnector.AddConectionByHandler(Handler);
        }
    }
}
