using NASDatabase.Interfaces;
using NASDatabase.Server.Data;
using System.Collections.Generic;

namespace NASDatabase.SmartSearchSettings
{
    internal class Equally : ISearch
    {
        public List<int> SearchID(AColumn ColumnParams, AColumn In, string Params)
        {
            List<int> data = new List<int>();
            var type = ColumnParams.TypeOfData;

            foreach (var p in In.GetDatas())
            {
                if (type.Equal(Params, p.Data))
                    data.Add(p.ID);
            }

            return data;
        }
    }
}
