using NASDataBaseAPI.Data; // Assuming SearchType enum is in this namespace
using NASDataBaseAPI.SmartSearchSettings;
using System.Collections.Generic;
using NASDataBaseAPI.Interfaces;
using System; // Required for ArgumentException if ValidateParameters throws it and it's not caught here

namespace NASDataBaseAPI.Server.Data // The namespace was NASDataBaseAPI.Server.Data, keeping it as is.
{
    public class SmartSearcher
    {
        private AColumn _column; 
        private AColumn _inColumn; // This field seems to be unused if the old check is removed. Review if it can be removed.
        private SearchType _searchType; 
        private SearchParameters _searchParameters; 

        public SmartSearcher(AColumn columnParams, AColumn inColumn, SearchType searchType, SearchParameters searchParameters)
        {
            _column = columnParams;
            _inColumn = inColumn; // Keep for now, but note its usage in the old check.
            _searchType = searchType;
            _searchParameters = searchParameters;

            // Ensure the passed-in searchParameters is not null and its SearchType matches the one passed to SmartSearcher.
            // This is an important consistency check.
            if (_searchParameters == null)
            {
                throw new ArgumentNullException(nameof(searchParameters), "SearchParameters cannot be null.");
            }
            if (_searchParameters.SearchType != _searchType)
            {
                // Or, alternatively, SmartSearcher could trust searchParameters.SearchType exclusively and not need its own _searchType field.
                // For now, maintaining both and ensuring consistency.
                throw new ArgumentException("SearchType mismatch between SmartSearcher's searchType parameter and searchParameters.SearchType property.");
            }
        }

        public List<int> Search()
        {
            // Call ValidateParameters as the first step.
            // _column is the column where the search is performed.
            _searchParameters.ValidateParameters(_column);

            // The old check:
            // if (_inColumn.TypeOfData.CanConvert(_searchParameters.Query) || _searchType == SearchType.ByRange)
            // is now handled by _searchParameters.ValidateParameters(_column);

            // If ValidateParameters passes, proceed to get the strategy and search.
            // Note: _searchType is used here, which should be consistent with _searchParameters.SearchType
            ISearch searchStrategy = SearchFactory.GetSearchStrategy(_searchType); 
            return searchStrategy.SearchID(_column, _inColumn, _searchParameters);
        }
    }
}
