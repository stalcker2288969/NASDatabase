using System;
using System.Collections.Generic;
using NASDataBaseAPI.Data; // Assuming SearchType enum is in this namespace

namespace NASDataBaseAPI.SmartSearchSettings
{
    public static class SearchFactory
    {
        // Production strategies
        private static readonly Dictionary<SearchType, Func<ISearch>> _searchStrategies = new Dictionary<SearchType, Func<ISearch>>
        {
            { SearchType.More, () => new MoreSettings() },
            { SearchType.Less, () => new LessSettings() },
            { SearchType.NotMore, () => new NotMore() },
            { SearchType.NotLess, () => new NotLess() },
            { SearchType.Equally, () => new Equally() },
            { SearchType.NotEqually, () => new NotEqually() },
            { SearchType.MoreOrEqually, () => new MoreOrEqually() },
            { SearchType.LessOrEqually, () => new LessOrEqually() },
            { SearchType.StartWith, () => new StartWith() }, 
            { SearchType.StopWith, () => new StopWith() },
            { SearchType.ByRange, () => new ByRange() },
            { SearchType.NotInRange, () => new NotInRange() },
            { SearchType.Multiple, () => new Multiple() },
            { SearchType.NotMultiple, () => new NotMultiple() },
        };

        // Testing-specific strategies
        private static Dictionary<SearchType, Func<ISearch>> _testingSearchStrategies = null;

        public static void RegisterSearchStrategyForTesting(SearchType searchType, Func<ISearch> factoryMethod)
        {
            if (_testingSearchStrategies == null)
            {
                _testingSearchStrategies = new Dictionary<SearchType, Func<ISearch>>();
            }
            _testingSearchStrategies[searchType] = factoryMethod; // Allows overriding for testing
        }

        public static void ClearTestingStrategies()
        {
            _testingSearchStrategies?.Clear();
            // _testingSearchStrategies = null; // Optionally set to null
        }
        
        // Method to check if currently in testing mode (i.e. if any testing strategies are registered)
        // This is optional, but can be useful for debugging or conditional logic.
        public static bool IsInTestingMode()
        {
            return _testingSearchStrategies != null && _testingSearchStrategies.Count > 0;
        }

        public static ISearch GetSearchStrategy(SearchType searchType)
        {
            // Prefer testing strategies if they exist and have a match
            if (_testingSearchStrategies != null && _testingSearchStrategies.TryGetValue(searchType, out var testingConstructor))
            {
                return testingConstructor();
            }
            
            // Fallback to production strategies
            if (_searchStrategies.TryGetValue(searchType, out var constructor))
            {
                return constructor();
            }
            
            throw new ArgumentException($"Unsupported search type: {searchType}");
        }

        // Original RegisterSearchStrategy remains for non-testing dynamic registration if ever needed
        public static void RegisterSearchStrategy(SearchType searchType, Func<ISearch> constructor)
        {
            if (!_searchStrategies.ContainsKey(searchType))
            {
                _searchStrategies.Add(searchType, constructor);
            }
            else
            {
                throw new InvalidOperationException($"Search type {searchType} is already registered in production strategies.");
            }
        }
    }
}
