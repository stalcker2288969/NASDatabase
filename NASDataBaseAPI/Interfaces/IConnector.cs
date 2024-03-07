using NASDataBaseAPI.Server.Data;
using NASDataBaseAPI.Server.Data.Modules.Handlers;

namespace NASDataBaseAPI.Interfaces
{
    public interface IConnector<T1, T2> where T1 : DataBase where T2 : DataBase
    {
        T1 DB1 { get; }
        T2 DB2 { get; }

        void AddConectionByHandler(Handler<T1,T2> Handler);
        void DestroyConectionByHandler(Handler<T1, T2> Handler);       
    }
}
