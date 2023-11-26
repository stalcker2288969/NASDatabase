using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.Interfases
{
    public interface ILoger
    {
        void StartLog();
        void Log(string message);
        void SetPrefix(string prefix);
        void StopLog();
    }
}
