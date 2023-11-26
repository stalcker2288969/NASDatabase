using NASDataBaseAPI.Data;
using NASDataBaseAPI.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.SmartSort
{
    internal interface ISorter
    {
        ItemData[] SortItemsData(ItemData[] data, string Params = " ");
        Tables[] SortTables(Tables[] tables,string TableName ,string Params = " ");
    }
}
