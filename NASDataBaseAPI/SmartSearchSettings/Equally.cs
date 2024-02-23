using NASDataBaseAPI.Server.Data;
using System.Collections.Generic;

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

                    int[] ids = In.FindIDs(Params);
                    data.AddRange(ids);                    
                    break;
                case "Boolean":      
                    
                    ids = In.FindIDs(Params);
                    data.AddRange(ids);
                    break;
                case "Int":
                    ids = In.FindIDs(Params);
                    data.AddRange(ids);
                    break;
                case "Float":
                    ids = In.FindIDs(Params);
                    data.AddRange(ids);
                    break;
                case "Time":
                    ids = In.FindIDs(Params);
                    data.AddRange(ids);
                    break;
            }
            return data;
        }
    }
}
