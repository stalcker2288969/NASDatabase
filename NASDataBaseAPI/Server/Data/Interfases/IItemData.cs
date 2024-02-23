using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.Interfases
{
    public interface IItemData
    {
        int ID { get; }
        string Data { get; }
    }
}
