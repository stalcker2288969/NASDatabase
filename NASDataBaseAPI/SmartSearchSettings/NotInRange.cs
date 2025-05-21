using NASDataBaseAPI.Interfaces; // Keep: AColumn, ISearch, ItemData
using System.Collections.Generic; // Keep: List
using System.Linq; // Keep: For Enumerable.Contains or other LINQ methods on QueryValues

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class NotInRange : ISearch
    {
        public List<int> SearchID(AColumn aColumnParams, AColumn In, SearchParameters searchParameters)
        {
            List<int> data = new List<int>();
            // Use aColumnParams for definitive type information for the data being searched.
            // In.TypeOfData might be relevant if comparing data from two different columns with potentially different types,
            // but for filtering a single column's data, aColumnParams.TypeOfData is the authority.
            var columnDataType = aColumnParams.TypeOfData; 

            if (searchParameters.QueryValues == null || !searchParameters.QueryValues.Any())
            {
                // If there are no query values, then everything is "not in range" of an empty set.
                // However, typical behavior might be to return nothing or everything.
                // For now, returning nothing if QueryValues is empty, aligning with ByRange.
                // This behavior might need further clarification based on desired product logic.
                return data; 
            }

            // TODO: Future refactoring could involve moving type-specific logic
            // into the TypeOfData hierarchy, e.g., columnDataType.DoesNotMatchAny(p.Data, searchParameters.QueryValues)
            switch (columnDataType.Name)
            {
                case "Text":
                    foreach (ItemData item in In.GetDatas())
                    {
                        // Check if the item's data (as string) is NOT contained in the provided query values.
                        // This assumes item.Data can be meaningfully compared as a string.
                        if (item.Data != null && !searchParameters.QueryValues.Contains(item.Data.ToString()))
                        {
                            data.Add(item.ID);
                        }
                        // If item.Data is null, it's considered "not in range" of any non-null QueryValues.
                        // If QueryValues can contain null/empty strings, this logic might need adjustment.
                        else if (item.Data == null) 
                        {
                            data.Add(item.ID);
                        }
                    }
                    break;
                default: // For types other than Text (e.g., numeric types)
                    foreach (var p in In.GetDatas()) // p is likely ItemData
                    {
                        bool foundMatchInQueryValues = false;
                        foreach (var valFromQuery in searchParameters.QueryValues)
                        {
                            // Use the column's type for comparison logic
                            if (columnDataType.Equal(valFromQuery, p.Data))
                            {
                                foundMatchInQueryValues = true;
                                break; // Found a match for this item in QueryValues
                            }
                        }
                        // If no match was found in any of the QueryValues, then it's "NotInRange".
                        if (!foundMatchInQueryValues)
                        {
                            data.Add(p.ID);
                        }
                    }
                    break;
            }

            return data;
        }
    }
}
