using NASDataBaseAPI.Server.Data;
using System;
using System.Collections.Generic;


namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class StopWith : ISearch
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
                            data.Add(item.ID);
                        }
                    }
                    break;
                case "Int":
                    datas = In.GetDatas();
                    foreach (ItemData item in datas)
                    {
                        if (item.Data.StartsWith(Params))
                        {
                            data.Add(item.ID);
                        }
                    }
                    break;
                case "Float":
                    datas = In.GetDatas();
                    foreach (ItemData item in datas)
                    {
                        if (item.Data.StartsWith(Params))
                        {
                            data.Add(item.ID);
                        }
                    }
                    break;
            }

            return data;
        }
    }
}
