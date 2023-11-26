using NASDataBaseAPI.Data;
using NASDataBaseAPI.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.SmartSort
{
    internal class Descending : ISorter
    {
        public ItemData[] SortItemsData(ItemData[] data, string Params = " ")
        {
            Array.Sort<ItemData>(data,(a,b) => b.Data.CompareTo(a.Data));
            return data;
        }

        public Tables[] SortTables(Tables[] tables, string TableName, string Params = " ")
        {
            throw new NotImplementedException();
        }
    }
}
