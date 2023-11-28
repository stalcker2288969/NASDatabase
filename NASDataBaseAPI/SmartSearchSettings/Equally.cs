using NASDataBaseAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class Equally : ISearch
    {
        public List<int> SearchID(Column ColumnParams, Column In, string Params)
        {
            List<int> data = new List<int>();

            switch (In.DataType.Name)
            {
                case "Text":
                    int L = 0;
                    ItemData[] datas = In.GetDatas();

                    int[] ids = In.FindeIDs(Params);
                    data.AddRange(ids);                    
                    break;
                case "Boolean":      
                    
                    ids = In.FindeIDs(Params);
                    data.AddRange(ids);
                    break;
                case "Int":
                    ids = In.FindeIDs(Params);
                    data.AddRange(ids);
                    break;
                case "Float":
                    ids = In.FindeIDs(Params);
                    data.AddRange(ids);
                    break;
                case "Time":
                    ids = In.FindeIDs(Params);
                    data.AddRange(ids);
                    break;
            }
            return data;
        }
    }
}
