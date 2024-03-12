using NASDatabase.Data;
using NASDatabase.SmartSearchSettings;
using System.Collections.Generic;
using NASDatabase.Interfaces;

namespace NASDatabase.Server.Data
{
    public class SmartSearcher
    {
        private AColumn _Column;
        private AColumn _inColumn;
        private SearchType _searchTypes;
        private string _params;

        public SmartSearcher(AColumn ColumnParams,AColumn In, SearchType SearchTypes, string Params)
        {
            this._Column = ColumnParams;
            this._inColumn = In;
            this._searchTypes = SearchTypes;
            this._params = Params;
        }

        public List<int> Search()
        {
            List<int> IDs = new List<int>();

            if (_inColumn.TypeOfData.CanConvert(_params) || _searchTypes == SearchType.ByRange)   
            {
                switch (_searchTypes)
                {
                    case SearchType.More:
                        IDs = new MoreSettings().SearchID(_Column, _inColumn, _params);
                        break;
                    case SearchType.Less:
                        IDs = new LessSettings().SearchID(_Column, _inColumn, _params);
                        break;
                    case SearchType.NotMore:
                        IDs = new NotMore().SearchID(_Column, _inColumn, _params);
                            break;
                    case SearchType.NotLess:
                        IDs = new NotLess().SearchID(_Column, _inColumn, _params);
                        break;
                    case SearchType.Equally:
                        IDs = new Equally().SearchID(_Column, _inColumn, _params);
                        break;
                    case SearchType.NotEqually:
                        IDs = new NotEqually().SearchID(_Column, _inColumn, _params);
                        break;
                    case SearchType.MoreOrEqually:
                        IDs = new MoreOrEqually().SearchID(_Column, _inColumn, _params);
                        break;
                    case SearchType.LessOrEqually:
                        IDs = new LessOrEqually().SearchID(_Column, _inColumn, _params);
                        break;
                    case SearchType.StartWith:
                        IDs = new StartWith().SearchID(_Column, _inColumn, _params);
                        break;
                    case SearchType.StopWith:
                        IDs = new StopWith().SearchID(_Column, _inColumn, _params);
                        break;
                    case SearchType.ByRange:
                        IDs = new ByRange().SearchID(_Column, _inColumn, _params);
                        break;
                    case SearchType.NotInRange:
                        IDs = new NotInRange().SearchID(_Column, _inColumn, _params);
                        break;
                    case SearchType.Multiple:
                        IDs = new Multiple().SearchID(_Column, _inColumn, _params);
                        break;
                    case SearchType.NotMultiple:
                        IDs = new NotMultiple().SearchID(_Column, _inColumn, _params);
                        break;     
                }
            }
            return IDs;
        }
    }
}
