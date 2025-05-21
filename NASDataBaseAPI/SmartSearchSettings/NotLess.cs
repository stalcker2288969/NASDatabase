using NASDataBaseAPI.Interfaces; // For AColumn, ISearch, ItemData
using System.Collections.Generic; // For List<int>

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class NotLess : ISearch // "Not less" means "greater than or equal to"
    {
        public List<int> SearchID(AColumn columnParams, AColumn inColumn, SearchParameters searchParameters)
        {
            List<int> data = new List<int>();
            var type = columnParams.TypeOfData; 
            string query = searchParameters.Query; 

            foreach (var p in inColumn.GetDatas())
            {
                // "Not less" ( val >= query ) is equivalent to ( val > query OR val == query )
                if (type.More(query, p.Data) || type.Equal(query, p.Data))
                    data.Add(p.ID);
            }

            return data;
        }
    }
}
