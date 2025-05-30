using NASDataBaseAPI.Interfaces;
using System.Collections.Generic;

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class NotEqually : ISearch
    {
        // Method signature updated to use SearchParameters
        public List<int> SearchID(AColumn columnParams, AColumn inColumn, SearchParameters searchParameters)
        {
            List<int> data = new List<int>();
            var type = columnParams.TypeOfData; // Using columnParams to get the type information
            string query = searchParameters.Query; // Extract the query string

            foreach (var p in inColumn.GetDatas())
            {
                // Using the query from searchParameters for comparison
                if (type.NotEqual(query, p.Data))
                    data.Add(p.ID);
            }

            return data;
        }
    }
}
