using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using System.Collections.Generic;

namespace NASDatabase.SmartSearchSettings
{
    internal class NotInRange : ISearch
    {
        public List<int> SearchID(AColumn aColumnParams, AColumn In, string Params)
        {
            List<int> data = new List<int>();

            var d = Params.Split(new char[] {','}, System.StringSplitOptions.RemoveEmptyEntries);

            switch (In.TypeOfData.Name)
            {
                case "Text":
                    ItemData[] datas = In.GetDatas();

                    foreach (ItemData item in datas)
                    {
                        if (!item.Data.Contains(Params))
                        {
                            data.Add(item.ID);
                        }
                    }
                    break;
                default:
                    foreach (var p in In.GetDatas())
                    {
                        foreach (var c in d)
                        {
                            if (In.TypeOfData.NotEqual(c, p.Data))
                            {
                                data.Add(p.ID);
                                break;
                            }
                        }
                    }
                    break;
            }

            return data;
        }
    }
}

