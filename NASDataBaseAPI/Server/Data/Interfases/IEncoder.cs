using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.Interfases
{
    internal interface IEncoder
    {
        string Encode(string Data,string Key);
    }

    interface IDecoder
    {
        string Decode(string Data,string Key);
    }
}
