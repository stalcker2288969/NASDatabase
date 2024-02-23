using NASDataBaseAPI.Server.Data.DataBaseSettings.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.Interfases
{
    public interface IConnector<T> where T : DataBase
    {
        T DB1 { get; }
        T DB2 { get; }

        void AddConectionByHandler(Handler<T> Handler);
        void DestroyConectionByHandler(Handler<T> Handler);       
    }
}
