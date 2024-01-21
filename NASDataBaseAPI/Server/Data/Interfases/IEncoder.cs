using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.Interfases
{
    public interface IEncoder
    {
        string Encode(string Data,string Key);
        string Decode(string Data, string Key);
    }
}
