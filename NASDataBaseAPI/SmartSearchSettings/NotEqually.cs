using NASDataBaseAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class NotEqually : ISearch
    {
        public List<int> SearchID(Table TableParams, Table In, string Params)
        {
            List<int> data = new List<int>();

            switch (In.DataType.Name)
            {
                case "Text":
                    int L = 0;
                    ItemData[] datas = In.GetDatas();

                    foreach (ItemData item in datas)
                    {
                        if (item.Data != Params)
                        {
                            data.Add(item.IDInTable);
                        }
                    }
                    break;
                case "Boolean":                   
                    datas = In.GetDatas();
                    foreach (ItemData item in datas)
                    {
                        if (item.Data != Params)
                        {
                            data.Add(item.IDInTable);
                        }
                    }
                    break;
                case "Int":                    
                    datas = In.GetDatas();
                    foreach (ItemData item in datas)
                    {
                        if (item.Data != Params)
                        {
                            data.Add(item.IDInTable);
                        }
                    }
                    break;
                case "Float":                  
                    datas = In.GetDatas();
                    foreach (ItemData item in datas)
                    {
                        if (item.Data != Params)
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
                        if (DateTime.Parse(item.Data) != time1)
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
