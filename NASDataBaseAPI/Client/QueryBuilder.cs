using System;
using System.Collections.Generic;
using System.Linq;
using NASDataBaseAPI.Interfaces;       // For AColumn
using NASDataBaseAPI.SmartSearchSettings; // For SearchParameters, SmartSearcher
using NASDataBaseAPI.Data;            // For SearchType

namespace NASDataBaseAPI.Client
{
    public class QueryBuilder
    {
        private readonly AColumn _columnDefinition;
        private readonly AColumn _columnToSearchInData;
        private SearchParameters _searchParameters = null;

        public QueryBuilder(AColumn columnDefinition, AColumn columnToSearchInData)
        {
            _columnDefinition = columnDefinition ?? throw new ArgumentNullException(nameof(columnDefinition));
            _columnToSearchInData = columnToSearchInData ?? throw new ArgumentNullException(nameof(columnToSearchInData));
        }

        private void SetSearchParameters(SearchParameters parameters)
        {
            if (_searchParameters != null)
            {
                throw new InvalidOperationException("Search condition has already been specified for this QueryBuilder instance. Create a new QueryBuilder for additional searches.");
            }
            _searchParameters = parameters;
        }

        public QueryBuilder IsEqualTo(string value)
        {
            SetSearchParameters(new SearchParameters(value, SearchType.Equally));
            return this;
        }

        public QueryBuilder IsNotEqualTo(string value)
        {
            SetSearchParameters(new SearchParameters(value, SearchType.NotEqually));
            return this;
        }

        public QueryBuilder IsGreaterThan(string value)
        {
            SetSearchParameters(new SearchParameters(value, SearchType.More));
            return this;
        }

        public QueryBuilder IsGreaterThanOrEqualTo(string value)
        {
            SetSearchParameters(new SearchParameters(value, SearchType.MoreOrEqually));
            return this;
        }

        public QueryBuilder IsLessThan(string value)
        {
            SetSearchParameters(new SearchParameters(value, SearchType.Less));
            return this;
        }

        public QueryBuilder IsLessThanOrEqualTo(string value)
        {
            SetSearchParameters(new SearchParameters(value, SearchType.LessOrEqually));
            return this;
        }

        public QueryBuilder StartsWith(string value)
        {
            SetSearchParameters(new SearchParameters(value, SearchType.StartWith));
            return this;
        }

        public QueryBuilder EndsWith(string value)
        {
            SetSearchParameters(new SearchParameters(value, SearchType.StopWith));
            return this;
        }

        public QueryBuilder Contains(string value)
        {
            // Mapping "Contains" for text to SearchType.ByRange with a single value.
            // SearchParameters constructor handles making QueryValues a list with this single value.
            // ByRange's Text case logic checks if item.Data.ToString() is in QueryValues.
            SetSearchParameters(new SearchParameters(value, SearchType.ByRange));
            return this;
        }

        public QueryBuilder IsIn(IEnumerable<string> values)
        {
            if (values == null || !values.Any())
            {
                throw new ArgumentException("Input collection cannot be null or empty for IsIn.", nameof(values));
            }
            SetSearchParameters(new SearchParameters(string.Join(",", values), SearchType.ByRange));
            return this;
        }

        public QueryBuilder IsIn(string commaSeparatedValues)
        {
            if (string.IsNullOrWhiteSpace(commaSeparatedValues))
            {
                throw new ArgumentException("Input string cannot be null or whitespace for IsIn.", nameof(commaSeparatedValues));
            }
            SetSearchParameters(new SearchParameters(commaSeparatedValues, SearchType.ByRange));
            return this;
        }

        public QueryBuilder IsNotIn(IEnumerable<string> values)
        {
            if (values == null || !values.Any())
            {
                throw new ArgumentException("Input collection cannot be null or empty for IsNotIn.", nameof(values));
            }
            SetSearchParameters(new SearchParameters(string.Join(",", values), SearchType.NotInRange));
            return this;
        }

        public QueryBuilder IsNotIn(string commaSeparatedValues)
        {
            if (string.IsNullOrWhiteSpace(commaSeparatedValues))
            {
                throw new ArgumentException("Input string cannot be null or whitespace for IsNotIn.", nameof(commaSeparatedValues));
            }
            SetSearchParameters(new SearchParameters(commaSeparatedValues, SearchType.NotInRange));
            return this;
        }

        public QueryBuilder IsMultipleOf(string value)
        {
            SetSearchParameters(new SearchParameters(value, SearchType.Multiple));
            return this;
        }

        public QueryBuilder IsNotMultipleOf(string value)
        {
            SetSearchParameters(new SearchParameters(value, SearchType.NotMultiple));
            return this;
        }
        
        public List<int> Search()
        {
            if (_searchParameters == null)
            {
                throw new InvalidOperationException("Search condition not specified. Call one of the query methods (e.g., IsEqualTo, IsGreaterThan) before calling Search.");
            }

            SmartSearcher smartSearcher = new SmartSearcher(_columnDefinition, _columnToSearchInData, _searchParameters.SearchType, _searchParameters);
            return smartSearcher.Search();
        }
    }
}
