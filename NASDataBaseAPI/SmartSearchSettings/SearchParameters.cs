using System;
using System.Collections.Generic;
using System.Linq; // Required for ToList() and Any()
using NASDataBaseAPI.Interfaces; // Required for AColumn
using NASDataBaseAPI.Data;      // Required for SearchType enum (assuming it's here)

namespace NASDataBaseAPI.SmartSearchSettings
{
    public class SearchParameters
    {
        public string Query { get; }
        public IReadOnlyList<string> QueryValues { get; private set; }
        public SearchType SearchType { get; }

        public SearchParameters(string query, SearchType searchType)
        {
            Query = query;
            SearchType = searchType;

            if (!string.IsNullOrEmpty(query))
            {
                if (query.Contains(","))
                {
                    QueryValues = query.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(s => s.Trim())
                                       .ToList();
                }
                else
                {
                    QueryValues = new List<string> { query.Trim() };
                }
            }
            else
            {
                QueryValues = new List<string>();
            }
        }

        public void ValidateParameters(AColumn columnToSearch)
        {
            if (columnToSearch == null)
            {
                throw new ArgumentNullException(nameof(columnToSearch), "Column to search cannot be null.");
            }

            var columnType = columnToSearch.TypeOfData;

            switch (SearchType)
            {
                // Types requiring a single, non-empty, convertible value
                case SearchType.More:
                case SearchType.Less:
                case SearchType.Equally:
                case SearchType.NotEqually: // NotEqually still needs a comparable value
                case SearchType.MoreOrEqually:
                case SearchType.LessOrEqually:
                case SearchType.StartWith:
                case SearchType.StopWith:
                case SearchType.Multiple:
                case SearchType.NotMultiple:
                    if (string.IsNullOrEmpty(Query))
                    {
                        throw new ArgumentException($"Query cannot be null or empty for search type '{SearchType}'.", nameof(Query));
                    }
                    if (!columnType.CanConvert(Query))
                    {
                        throw new ArgumentException($"Invalid parameter '{Query}' for search type '{SearchType}' on column '{columnToSearch.Name}' of type '{columnType.Name}'.");
                    }
                    break;

                // Types requiring one or more convertible values
                case SearchType.ByRange:
                case SearchType.NotInRange:
                    if (QueryValues == null || !QueryValues.Any())
                    {
                        throw new ArgumentException($"QueryValues cannot be null or empty for search type '{SearchType}'.", nameof(QueryValues));
                    }
                    foreach (var value in QueryValues)
                    {
                        if (string.IsNullOrEmpty(value)) // Individual values in a list also shouldn't be empty
                        {
                             throw new ArgumentException($"Individual values in a list cannot be null or empty for search type '{SearchType}' on column '{columnToSearch.Name}'.");
                        }
                        if (!columnType.CanConvert(value))
                        {
                            throw new ArgumentException($"Invalid value '{value}' in parameters for search type '{SearchType}' on column '{columnToSearch.Name}' of type '{columnType.Name}'.");
                        }
                    }
                    break;
                
                // Potentially other search types might have different validation logic or no validation needed
                // For example, a hypothetical SearchType.All would not need a query.
                default:
                    // Or throw new NotSupportedException($"Validation for SearchType '{SearchType}' is not implemented.");
                    break; 
            }
        }
    }
}
