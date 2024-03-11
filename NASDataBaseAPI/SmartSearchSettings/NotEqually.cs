using NASDataBaseAPI.Interfaces;
using System.Collections.Generic;

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class NotEqually : ISearch
    {
        public List<int> SearchID(AColumn ColumnParams, AColumn In, string Params)
        {
            List<int> data = new List<int>();
            var type = ColumnParams.TypeOfData;

            foreach (var p in In.GetDatas())
            {
                if (type.NotEqual(Params, p.Data))
                    data.Add(p.ID);
            }

            return data;
        }
    }
}
