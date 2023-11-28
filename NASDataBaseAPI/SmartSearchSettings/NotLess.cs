using NASDataBaseAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class NotLess : ISearch
    {
        public List<int> SearchID(Column ColumnParams, Column In, string Params)
        {
            List<int> data = new List<int>();

            switch (In.DataType.Name)
            {
                case "Text":
                    int L = Convert.ToInt32(Params);
                    ItemData[] datas = In.GetDatas();

                    foreach (ItemData item in datas)
                    {
                        if (item.Data.Length >= L)
                        {
                            data.Add(item.IDInTable);
                        }
                    }
                    break;
                case "Int":
                    L = Convert.ToInt32(Params);
                    datas = In.GetDatas();
                    foreach (ItemData item in datas)
                    {
                        if (Convert.ToInt32(item.Data) >= L)
                        {
                            data.Add(item.IDInTable);
                        }
                    }
                    break;
                case "Float":
                    L = Convert.ToInt32(Params);
                    datas = In.GetDatas();
                    foreach (ItemData item in datas)
                    {
                        if (Convert.ToDecimal(item.Data) >= L)
                        {
                            data.Add(item.IDInTable);
                        }
                    }
                    break;
                case "Time":
                    var time1 = DateTime.Parse(Params);
                    datas = In.GetDatas();
                    foreach (ItemData item in datas)
                    {
                        if (DateTime.Parse(item.Data) >= time1)
                        {
                            data.Add(item.IDInTable);
                        }
                    }
                    break;
                    break;
            }
            return data;
        }
    }
}
