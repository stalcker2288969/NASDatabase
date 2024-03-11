using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;


namespace NASDataBaseAPI.SmartSearchSettings
{
    internal class LessSettings : ISearch
    {
        public List<int> SearchID(AColumn ColumnParams, AColumn In, string Params)
        {
            List<int> data = new List<int>();
            var type = ColumnParams.TypeOfData;

            foreach(var p in In.GetDatas())
            {
                if (type.Less(Params, p.Data))
                    data.Add(p.ID);
            }

            return data;
        }
    }
}
