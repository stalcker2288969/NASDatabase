using NASDataBaseAPI.Interfaces; // For AColumn, ISearch, ItemData
using System.Collections.Generic; // For List<int>

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class StopWith : ISearch
    {
        public List<int> SearchID(AColumn columnParams, AColumn inColumn, SearchParameters searchParameters)
        {
            List<int> data = new List<int>();
            string query = searchParameters.Query; 

            foreach (var p in inColumn.GetDatas()) 
            {
                if (p.Data != null && p.Data.ToString().EndsWith(query))
                    data.Add(p.ID);
            }

            return data;
        }
    }
}
