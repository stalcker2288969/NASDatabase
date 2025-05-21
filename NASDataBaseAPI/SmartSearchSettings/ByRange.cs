using NASDataBaseAPI.Interfaces; // Keep: AColumn, ISearch, ItemData (if returned by GetDatas)
using System.Collections.Generic; // Keep: List
using System.Linq; // Keep: For Enumerable.Contains on QueryValues if used directly

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class ByRange : ISearch
    {
        public List<int> SearchID(AColumn aColumnParams, AColumn In, SearchParameters searchParameters)
        {
            List<int> data = new List<int>();
            var columnDataType = aColumnParams.TypeOfData; // Use aColumnParams for definitive type information

            if (searchParameters.QueryValues == null || !searchParameters.QueryValues.Any())
            {
                return data; // No values to search for
            }

            // TODO: Future refactoring could involve moving type-specific logic
            // into the TypeOfData hierarchy, e.g., columnDataType.MatchesAny(p.Data, searchParameters.QueryValues)
            switch (columnDataType.Name)
            {
                case "Text":
                    foreach (ItemData item in In.GetDatas())
                    {
                        // Check if the item's data (as string) is equal to any of the provided query values.
                        // This assumes item.Data can be meaningfully compared as a string.
                        if (item.Data != null && searchParameters.QueryValues.Contains(item.Data.ToString()))
                        {
                            data.Add(item.ID);
                        }
                    }
                    break;
                default: // For types other than Text (e.g., numeric types)
                    foreach (var p in In.GetDatas()) // p is likely ItemData
                    {
                        foreach (var valFromQuery in searchParameters.QueryValues)
                        {
                            // Use the column's type for comparison logic
                            if (columnDataType.Equal(valFromQuery, p.Data))
                            {
                                data.Add(p.ID);
                                break; // Found a match for this item from one of the QueryValues, move to the next item
                            }
                        }
                    }
                    break;
            }

            return data;
        }
    }
}
