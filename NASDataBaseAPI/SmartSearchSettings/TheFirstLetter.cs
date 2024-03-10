using NASDataBaseAPI.Interfaces;
using System.Collections.Generic;


namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class StartWith : ISearch
    {
        public List<int> SearchID(AColumn ColumnParams, AColumn In, string Params)
        {
            List<int> data = new List<int>();

            foreach (var p in In.GetDatas())
            {
                if(p.Data.StartsWith(Params))
                    data.Add(p.ID);
            }

            return data;
        }
    }
}
