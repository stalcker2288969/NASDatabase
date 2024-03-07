using NASDataBaseAPI.Data;
using NASDataBaseAPI.Data.DataTypesInColumn;
using NASDataBaseAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data
{
    /// <summary>
    /// База данных основанная на распараллеливание задач, есть минус - жрет много оперативной памяти и небезопасна  
    /// </summary>
    public class FasterDataBase : DataBase
    {
        public const int StandardNumberExecutingThreads = 4;
        
        /// <summary>
        /// Параметр отвечающий за кол-во патоков при распараллеливание задач
        /// </summary>
        public int MaxDegreeOfParallelism = StandardNumberExecutingThreads;

        public FasterDataBase(int countColumn, DataBaseSettings.DataBaseSettings settings, int loadedSector = 0, int MaxDegreeOfParallelism = StandardNumberExecutingThreads)
            : base(countColumn, settings, loadedSector)
        {
            this.MaxDegreeOfParallelism = MaxDegreeOfParallelism;
        }

        public FasterDataBase(int countColumn, DataBaseSettings.DataBaseSettings settings, int loadedSector = 0) : this(countColumn, settings, loadedSector, StandardNumberExecutingThreads) { }

        public FasterDataBase(List<IColumn> Column, DataBaseSettings.DataBaseSettings settings, int loadedSector = 0, int MaxDegreeOfParallelism = StandardNumberExecutingThreads)
            : base(Column, settings, loadedSector)
        {
            this.MaxDegreeOfParallelism = MaxDegreeOfParallelism;
        }

        #region Глобальное взаимодействое

        public override void ChengTypeInColumn(string Column, DataType DataType)
        {
            lock (Columns)
            {
                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {
                    var db = _LoadDataBase((int)i);
                    foreach (var j in Columns)
                    {
                        j.ChangType(DataType);
                    }
                    DataBaseSaver.SaveAllCluster(db.Settings, LoadedSector, db.Columns.ToArray());
                });
            }
        }

        public override int[] GetAllIDsByParams(string nameColumn, string data)
        {
            lock (Columns)
            {
                List<int> IDs = new List<int>();

                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {
                    var DataBase = _LoadDataBase((int)i);

                    lock (IDs)
                    {
                        IDs.AddRange(DataBase[nameColumn].FindIDs(data) ?? new int[0]);
                    }

                    DataBaseLoger.Log($"Get all IDs by {nameColumn} and {data} in {i} cluester");
                });

                return IDs.ToArray();
            }
        }

        public override void RemoveColumn(string ColumnName)
        {
            lock (Columns)
            {
                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {
                    var DataBase = _LoadDataBase((int)i);

                    DataBase[ColumnName].ClearBoxes();

                    DataBaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                });
                Settings.ColumnsCount -= 1;
                _RemoveColumn?.Invoke(ColumnName);     
            }
        }

        public override void AddColumn(string Name)
        {
            lock (Columns)
            {
                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {
                    var DB = _LoadDataBase((int)i);
                    Column table = new Column(Name, DB.Columns[0].OffSet);//Новый столбец

                    ItemData[] itemDatas = new ItemData[DB.Columns[0].GetCounts()];

                    for (int g = 0; g < itemDatas.Length; g++)//связывает ивенд дестроя и столбец
                    {
                        itemDatas[g] = new ItemData(g, " ");
                    }

                    table.SetDatas(itemDatas); //записываем пустые ячейки в новый столбец
                    DB.Columns.Add(table);

                    DataBaseSaver.SaveAllCluster(Settings, (uint)i, DB.Columns.ToArray());
                });
                Settings.ColumnsCount += 1;       
                _AddColumn?.Invoke(Name);
            }
        }

        public override void AddColumn(string Name, DataType dataType)
        {
            lock (Columns)
            {
                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {
                    var DB = _LoadDataBase((int)i);

                    Column table = new Column(Name, dataType, DB.Columns[0].OffSet);//Новый столбец

                    ItemData[] itemDatas = new ItemData[DB.Columns[0].GetCounts()];

                    for (int g = 0; g < itemDatas.Length; g++)//связывает ивенд дестроя и столбец
                    {
                        itemDatas[g] = new ItemData(g, " ");
                    }

                    table.SetDatas(itemDatas); //записываем пустые ячейки данные в новый столбец

                    DB.Columns.Add(table);
                    DataBaseSaver.SaveAllCluster(Settings, (uint)i, DB.Columns.ToArray());
                });
                Settings.ColumnsCount += 1;               
                _AddColumn?.Invoke(Name);
            }
        }

        public override void CloneTo(IColumn left, IColumn right)
        {
            lock (Columns)
            {
                var leftName = left.Name;
                var rightName = right.Name;

                if (this[leftName].DataType != this[rightName].DataType)
                {
                    right.ChangType(right.DataType);
                    DataBaseSaver.SaveAllCluster(Settings, LoadedSector, Columns.ToArray());
                }

                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {
                    var DB = _LoadDataBase((int)i);
                    ItemData[] itemDatas = DB[leftName].GetDatas();
                    DB[rightName].SetDatas(itemDatas);
                    DataBaseSaver.SaveAllCluster(Settings, (uint)i, DB.Columns.ToArray());
                });
    
                _CloneColumn?.Invoke(leftName, rightName);
            }
        }

        public override void CloneTo(string left, string right)
        {
            lock (Columns)
            {
                var rightColumn = this[right];
                var leftColumn = this[left];

                if (leftColumn.DataType != rightColumn.DataType)
                {
                    rightColumn.ChangType(leftColumn.DataType);
                    DataBaseSaver.SaveAllCluster(Settings, LoadedSector, Columns.ToArray());
                }

                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {
                    var DB = _LoadDataBase((int)i);
                    ItemData[] itemDatas = DB[left].GetDatas();
                    DB[right].SetDatas(itemDatas);
                    DataBaseSaver.SaveAllCluster(Settings, (uint)i, DB.Columns.ToArray());
                });

                _CloneColumn?.Invoke(left, right);
            }
        }

        public override void ClearAllColumn(IColumn column, int InSector)
        {
            lock (Columns)
            {
                if (InSector == -1)
                {
                    Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                    {
                        MaxDegreeOfParallelism = MaxDegreeOfParallelism
                    },
                    i =>
                    {
                        var _column = this[column.Name];
                        if (_column.DataType == column.DataType)
                        {
                            var DB = _LoadDataBase((int)i);
                            _column = DB[column.Name];
                            _column.ClearBoxes();
                            DataBaseSaver.SaveAllCluster(Settings, (uint)i, DB.Columns.ToArray());
                        }
                    });
                }
                else
                {
                    var _column = this[column.Name];
                    if (_column.DataType == column.DataType)
                    {
                        var DB = _LoadDataBase(InSector);
                        _column = DB[column.Name];
                        _column.ClearBoxes();
                        DataBaseSaver.SaveAllCluster(Settings, (uint)InSector, DB.Columns.ToArray());     
                    }
                }
                _ClearAllColumn?.Invoke(column.Name, InSector);
            }
        }

        public override void ClearAllColumn(string ColumnName, int InSector)
        {
            ClearAllColumn(this[ColumnName], InSector);
        }

        public override void ClearAllBase()
        {
            lock (Columns)
            {
                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {
                    _LoadDataBase((int)i);
                    foreach (IColumn t in Columns)
                    {
                        t.ClearBoxes();
                    }
                    DataBaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                });
                _ClearAllBase?.Invoke();
            }
        }


        #endregion

        #region Добавляение/замена данных

        /// <summary>
        /// Выполняет замену данных по параметрам в ассинхронном порядке
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="Params"></param>
        /// <param name="New"></param>
        /// <param name="SectorID"></param>
        public override void ChangeEverythingTo(string ColumnName, string Params, string New, int SectorID = -1)
        {
            lock (Columns)
            {
                if (SectorID == -1)
                {
                    Parallel.For(1, Settings.CountClusters, i =>
                    {
                        _LoadAndChengeDataInCluster((int)i, ColumnName, Params, New);
                    });
                }
                else
                {
                    _LoadAndChengeDataInCluster((int)SectorID, ColumnName, Params, New);
                }                
            }
        }

        /// <summary>
        /// Приватынй метод для ChangeEverythingTo
        /// </summary>
        /// <param name="sector"></param>
        /// <param name="ColumnName"></param>
        /// <param name="Params"></param>
        /// <param name="New"></param>
        private void _LoadAndChengeDataInCluster(int sector, string ColumnName, string Params, string New)
        {
            var _Columns = (IEnumerable<Column>)DataBaseLoader.LoadCluster(Settings.Path, (uint)sector, Settings.Key);

            foreach (Column t in _Columns)
            {
                if (t.Name == ColumnName)
                {
                    var ids = t.FindIDs(Params);
                    foreach(var id in ids)
                    {
                        t.SetDataByID(new ItemData(id, Params));
                    }
                }
            }
            DataBaseSaver.SaveAllCluster(Settings, (uint)sector, Columns.ToArray());
        }

        #endregion

        #region Сортировка
        public override BaseLine[] GetAllDataInBaseByColumnName(string ColumnName, string data)
        {
            lock (Columns)
            {
                List<BaseLine> Boxes = new List<BaseLine>();

                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {
                    var DB = _LoadDataBase((int)i);

                    int[] ids = DB[ColumnName].FindIDs(data);

                    for (int j = 0; j < ids.Length; j++)
                    {
                        var box = new BaseLine();

                        string[] strings = new string[Columns.Count];

                        for (int k = 0; k < Columns.Count; k++)
                        {
                            strings[k] = Columns[k].FindDataByID((int)ids[j]);
                        }

                        box.Init(ids[j], strings);
                        lock (Boxes)
                            Boxes.Add(box);
                    }
                });

                return Boxes.ToArray();
            }
        }

        public override List<List<ItemData>> SmartSearch(IColumn[] columns, SearchType[] searchTypes, string[] Params, int InSectro = -1)
        {
            lock (Columns)
            {
                List<List<ItemData>> Boxes = new List<List<ItemData>>();
                List<List<int>> Search = new List<List<int>>();
                List<int> resultIDs = new List<int>();

                if (columns.Length != searchTypes.Length && columns.Length != Params.Length)
                    throw new ArgumentException("Параметры не совпадают по кол-ву!");

                if (InSectro == -1)
                {
                    Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                    {
                        MaxDegreeOfParallelism = MaxDegreeOfParallelism
                    },
                    i =>
                    {
                        Boxes.Add(SmartSearch(columns, searchTypes, Params, (int)i)[0]);
                    });
                }
                else
                {
                    var DB = _LoadDataBase(InSectro);

                    for (int j = 0; j < Params.Length; j++)
                    {
                        var _colomn = DB[columns[j].Name];

                        if (_colomn.DataType == columns[j].DataType)
                        {
                            List<int> IDs = new List<int>();
                            IDs = new SmartSearcher((Column)columns[j], (Column)_colomn, searchTypes[j], Params[j]).Search();
                            Search.Add(IDs);
                        }
                    }

                    for (int i = 0; i < Search.Count; i++)
                    {
                        if (Search.Count > i + 1)
                        {
                            int[] _result = Search[i].Intersect<int>(Search[i + 1]).ToArray<int>();
                            Search[i + 1].Clear();
                            Search[i + 1].AddRange(_result);
                        }
                    }

                    resultIDs = Search[Search.Count - 1];

                    for (int i = 0; i < resultIDs.Count; i++)
                    {
                        List<ItemData> data = new List<ItemData>();
                        foreach (var t in DB.Columns)
                        {
                            data.Add(new ItemData(resultIDs[i], t.FindDataByID(resultIDs[i])));
                        }
                        Boxes.Add(data);
                    }
                }
                return Boxes;
            }
        }
 
        #endregion

        private DataBase _LoadDataBase(int i)
        {
            var DataBase = new DataBase((int)Settings.ColumnsCount, Settings, 0);

            if (LoadedSector != i)
            {
                DataBase.Columns.AddRange((IEnumerable<Column>)DataBaseLoader.LoadCluster(Settings.Path, (uint)i + 1, Settings.Key));
            }
            else
            {
                DataBase = this;
            }
            return DataBase;
        }

    }
}

