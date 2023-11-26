using NASDataBaseAPI.Data;
using NASDataBaseAPI.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal interface ISearch
    {
        List<int> SearchID(Tables TableParams, Tables In, string Params);
    }
}
