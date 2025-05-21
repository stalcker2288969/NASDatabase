using NASDataBaseAPI.Interfaces;
using System.Collections.Generic;

namespace NASDataBaseAPI.SmartSearchSettings
{
    // Class name is already StartWith, if it were TheFirstLetter, it would be changed here.
    internal class StartWith : ISearch 
    {
        // Method signature updated to use SearchParameters
        public List<int> SearchID(AColumn columnParams, AColumn inColumn, SearchParameters searchParameters)
        {
            List<int> data = new List<int>();
            string query = searchParameters.Query; // Extract the query string

            foreach (var p in inColumn.GetDatas()) // p is likely ItemData
            {
                // Assuming p.Data is a string or has a string representation that can be used with StartsWith
                if (p.Data != null && p.Data.ToString().StartsWith(query))
                    data.Add(p.ID);
            }

            return data;
        }
    }
}
