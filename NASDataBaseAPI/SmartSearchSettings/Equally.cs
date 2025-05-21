using NASDataBaseAPI.Interfaces; // For AColumn, ISearch, ItemData
using System.Collections.Generic; // For List<int>

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class Equally : ISearch
    {
        public List<int> SearchID(AColumn columnParams, AColumn inColumn, SearchParameters searchParameters)
        {
            List<int> data = new List<int>();
            var type = columnParams.TypeOfData; 

            // For "Equally", we use the original Query string as it represents a single value.
            // The SearchParameters constructor ensures QueryValues is populated for multi-value scenarios (like ByRange),
            // but Query still holds the original input.
            string query = searchParameters.Query;

            foreach (var p in inColumn.GetDatas())
            {
                if (type.Equal(query, p.Data)) 
                    data.Add(p.ID);
            }

            return data;
        }
    }
}
