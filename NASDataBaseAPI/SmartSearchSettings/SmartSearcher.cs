using NASDataBaseAPI.Data;
using NASDataBaseAPI.SmartSearchSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Data
{
    internal class SmartSearcher
    {
        private Column table;
        private Column Intable;
        private SearchType searchTypes;
        private string Params;

        public SmartSearcher(Column ColumnParams,Column In, SearchType searchTypes, string Params)
        {
            this.table = ColumnParams;
            this.Intable = In;
            this.searchTypes = searchTypes;
            this.Params = Params;
        }

        public List<int> Search()
        {
            List<int> IDs = new List<int>();

            if (Intable.DataType.TryConvert(Params) || searchTypes == SearchType.ByRange)   
            {
                switch (searchTypes)
                {
                    case SearchType.More:
                        IDs = new MoreSettings().SearchID(table, Intable, Params);
                        break;
                    case SearchType.Less:
                        IDs = new LessSettings().SearchID(table, Intable, Params);
                        break;
                    case SearchType.NotMore:
                        IDs = new NotMore().SearchID(table, Intable, Params);
                            break;
                    case SearchType.NotLess:
                        IDs = new NotLess().SearchID(table, Intable, Params);
                        break;
                    case SearchType.Equally:
                        IDs = new Equally().SearchID(table, Intable, Params);
                        break;
                    case SearchType.NotEqually:
                        IDs = new NotEqually().SearchID(table, Intable, Params);
                        break;
                    case SearchType.MoreOrEqually:
                        IDs = new MoreOrEqually().SearchID(table, Intable, Params);
                        break;
                    case SearchType.LessOrEqually:
                        IDs = new LessOrEqually().SearchID(table, Intable, Params);
                        break;
                    case SearchType.StartWith:
                        IDs = new StartWith().SearchID(table, Intable, Params);
                        break;
                    case SearchType.StopWith:
                        IDs = new StopWith().SearchID(table, Intable, Params);
                        break;
                    case SearchType.ByRange:
                        IDs = new ByRange().SearchID(table, Intable, Params);
                        break;
                    case SearchType.NotInRange:
                        IDs = new NotInRange().SearchID(table, Intable, Params);
                        break;
                    case SearchType.Multiple:
                        IDs = new Multiple().SearchID(table, Intable, Params);
                        break;
                    case SearchType.NotMultiple:
                        IDs = new NotMultiple().SearchID(table, Intable, Params);
                        break;     
                }
            }
            return IDs;
        }
    }
}
