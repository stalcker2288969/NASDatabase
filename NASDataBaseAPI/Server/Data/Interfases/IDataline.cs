using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.Interfases
{
    public interface IDataLine
    {
        int ID { get; }
        string[] GetData();
    }
}
