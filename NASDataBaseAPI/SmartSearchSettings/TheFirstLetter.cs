using NASDataBaseAPI.Data;
using NASDataBaseAPI.SmartSearchSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class StartWith : ISearch
    {
        public List<int> SearchID(Column ColumnParams, Column In, string Params)
        {
            List<int> data = new List<int>();

            switch (In.DataType.Name)
            {
                case "Text":                    
                    ItemData[] datas = In.GetDatas();

                    foreach (ItemData item in datas)
                    {                        
                        if (item.Data.StartsWith(Params))
                        {
                            data.Add(item.IDInTable);
                        }
                    }
                    break;
                case "Int":                    
                    datas = In.GetDatas();
                    foreach (ItemData item in datas)
                    {
                        if (item.Data.StartsWith(Params))
                        {
                            data.Add(item.IDInTable);
                        }
                    }
                    break;
                case "Float":                    
                    datas = In.GetDatas();
                    foreach (ItemData item in datas)
                    {
                        if (item.Data.StartsWith(Params))
                        {
                            data.Add(item.IDInTable);
                        }
                    }
                    break;
            }

            return data;
        }
    }
}
