using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data;
using System.Collections.Generic;

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class NotMultiple : ISearch
    {
        public List<int> SearchID(AColumn ColumnParams, AColumn In, string Params)
        {
            List<int> data = new List<int>();
            var type = ColumnParams.TypeOfData;

            foreach (var p in In.GetDatas())
            {
                if (!type.Multiple(p.Data, Params))
                    data.Add(p.ID);
            }

            return data;
        }
    }
}
