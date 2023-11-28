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
        List<int> SearchID(Column ColumnParams, Column In, string Params);
    }
}
