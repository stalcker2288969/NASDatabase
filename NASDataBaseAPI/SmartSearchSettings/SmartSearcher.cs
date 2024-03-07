using NASDataBaseAPI.Data;
using NASDataBaseAPI.SmartSearchSettings;
using System.Collections.Generic;

namespace NASDataBaseAPI.Server.Data
{
    public class SmartSearcher
    {
        private Column _column;
        private Column _inColumn;
        private SearchType _searchTypes;
        private string _params;

        public SmartSearcher(Column ColumnParams,Column In, SearchType searchTypes, string Params)
        {
            this._column = ColumnParams;
            this._inColumn = In;
            this._searchTypes = searchTypes;
            this._params = Params;
        }

        public List<int> Search()
        {
            List<int> IDs = new List<int>();

            if (_inColumn.DataType.TryConvert(_params) || _searchTypes == SearchType.ByRange)   
            {
                switch (_searchTypes)
                {
                    case SearchType.More:
                        IDs = new MoreSettings().SearchID(_column, _inColumn, _params);
                        break;
                    case SearchType.Less:
                        IDs = new LessSettings().SearchID(_column, _inColumn, _params);
                        break;
                    case SearchType.NotMore:
                        IDs = new NotMore().SearchID(_column, _inColumn, _params);
                            break;
                    case SearchType.NotLess:
                        IDs = new NotLess().SearchID(_column, _inColumn, _params);
                        break;
                    case SearchType.Equally:
                        IDs = new Equally().SearchID(_column, _inColumn, _params);
                        break;
                    case SearchType.NotEqually:
                        IDs = new NotEqually().SearchID(_column, _inColumn, _params);
                        break;
                    case SearchType.MoreOrEqually:
                        IDs = new MoreOrEqually().SearchID(_column, _inColumn, _params);
                        break;
                    case SearchType.LessOrEqually:
                        IDs = new LessOrEqually().SearchID(_column, _inColumn, _params);
                        break;
                    case SearchType.StartWith:
                        IDs = new StartWith().SearchID(_column, _inColumn, _params);
                        break;
                    case SearchType.StopWith:
                        IDs = new StopWith().SearchID(_column, _inColumn, _params);
                        break;
                    case SearchType.ByRange:
                        IDs = new ByRange().SearchID(_column, _inColumn, _params);
                        break;
                    case SearchType.NotInRange:
                        IDs = new NotInRange().SearchID(_column, _inColumn, _params);
                        break;
                    case SearchType.Multiple:
                        IDs = new Multiple().SearchID(_column, _inColumn, _params);
                        break;
                    case SearchType.NotMultiple:
                        IDs = new NotMultiple().SearchID(_column, _inColumn, _params);
                        break;     
                }
            }
            return IDs;
        }
    }
}
