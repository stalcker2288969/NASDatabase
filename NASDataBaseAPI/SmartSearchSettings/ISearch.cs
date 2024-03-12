using NASDatabase.Interfaces;
using System.Collections.Generic;


namespace NASDatabase.SmartSearchSettings
{
    internal interface ISearch
    {
        List<int> SearchID(AColumn aColumnParams, AColumn In, string Params);
    }
}
