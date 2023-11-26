using NASDataBaseAPI.Data;
using NASDataBaseAPI.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.SmartSort
{
    internal class Ascending : ISorter
    {
        public ItemData[] SortItemsData(ItemData[] data, string Params = " ")
        {
            Array.Sort(data);
            return data;
        }

        public Tables[] SortTables(Tables[] tables,string TablesName, string Params = " ")
        {
            List<int> indexes = new List<int>();

            for(int i =0; i< tables.Length;i++)
            {
                if (tables[i].Name == TablesName)
                {
                    switch(tables[i].DataType.Name)
                    {
                        case "Int":
                            
                            break;
                        case "Float":

                            break;
                        case "Text":

                            break;
                        case "Boolean":

                            break;
                    }
                }
            }

            return tables;
        }
    }
}
