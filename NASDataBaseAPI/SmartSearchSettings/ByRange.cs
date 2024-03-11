using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data;
using System.Collections.Generic;

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class ByRange : ISearch
    {
        public List<int> SearchID(AColumn aColumnParams, AColumn In, string Params)
        {
            List<int> data = new List<int>();

            var d = Params.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

            switch (In.TypeOfData.Name)
            {
                case "Text":
                    foreach (ItemData item in In.GetDatas())
                    {
                        if (item.Data.Contains(Params))
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
                            if (In.TypeOfData.Equal(c, p.Data))
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
