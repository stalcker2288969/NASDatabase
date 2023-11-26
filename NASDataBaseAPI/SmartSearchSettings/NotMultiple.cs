using NASDataBaseAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class NotMultiple : ISearch
    {
        public List<int> SearchID(Table TableParams, Table In, string Params)
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
                            data.Add(item.IDInTable);
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
                            data.Add(item.IDInTable);
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
                            data.Add(item.IDInTable);
                        }
                    }
                    break;
            }

            return data;
        }
    }
}
