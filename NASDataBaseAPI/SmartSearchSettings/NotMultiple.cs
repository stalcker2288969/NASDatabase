using NASDataBaseAPI.Server.Data;
using System.Collections.Generic;

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class NotMultiple : ISearch
    {
        public List<int> SearchID(Column ColumnParams, Column In, string Params)
        {
            List<int> data = new List<int>();

            switch (In.DataType.Name)
            {
                case "Text":
                    ItemData[] datas = In.GetDatas();
                    int Number = int.Parse(Params);

                    foreach (ItemData item in datas)
                    {
                        if (item.Data.Length % Number != 0)
                        {
                            data.Add(item.ID);
                        }
                    }
                    break;
                case "Int":
                    datas = In.GetDatas();
                    Number = int.Parse(Params);
                    foreach (ItemData item in datas)
                    {
                        if (item.Data.Length % Number != 0)
                        {
                            data.Add(item.ID);
                        }
                    }
                    break;
                case "Float":
                    datas = In.GetDatas();
                    Number = int.Parse(Params);
                    foreach (ItemData item in datas)
                    {
                        if (item.Data.Length % Number != 0)
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
