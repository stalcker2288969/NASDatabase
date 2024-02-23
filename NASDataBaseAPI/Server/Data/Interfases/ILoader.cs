using NASDataBaseAPI.Server.Data.Interfases.Column;
using NASDataBaseAPI.Server.Data.Interfases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NASDataBaseAPI.Data;
using NASDataBaseAPI.Server.Data.Modules;

namespace NASDataBaseAPI.Server.Data.Interfases
{
    public interface ILoader : IDataBaseSaver<IColumn>, IDataBaseLoader<IColumn>, IDataBaseReplayser
    {
        IFileWorker FileSystem { get; }
    }
}
