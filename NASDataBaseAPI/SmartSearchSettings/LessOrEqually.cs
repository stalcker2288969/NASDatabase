using NASDataBaseAPI.Interfaces; // For AColumn, ISearch, ItemData
using System.Collections.Generic; // For List<int>

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class LessOrEqually : ISearch
    {
        public List<int> SearchID(AColumn columnParams, AColumn inColumn, SearchParameters searchParameters)
        {
            List<int> data = new List<int>();
            var type = columnParams.TypeOfData; 
            string query = searchParameters.Query; 

            foreach (var p in inColumn.GetDatas())
            {
                if (type.Equal(query, p.Data) || type.Less(query, p.Data))
                    data.Add(p.ID);
            }

            return data;
        }
    }
}
