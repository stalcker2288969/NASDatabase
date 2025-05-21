using NASDataBaseAPI.Interfaces;
using System.Collections.Generic;

namespace NASDataBaseAPI.SmartSearchSettings
{
    internal interface ISearch
    {
        List<int> SearchID(AColumn aColumnParams, AColumn In, SearchParameters searchParameters);
    }
}
