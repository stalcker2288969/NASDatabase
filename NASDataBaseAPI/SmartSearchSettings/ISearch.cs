using NASDataBaseAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal interface ISearch
    {
        List<int> SearchID(Table TableParams, Table In, string Params);
    }
}
