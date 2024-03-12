using NASDatabase.Data;
using NASDatabase.Data.DataTypesInColumn;
using NASDatabase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NASDatabase.Server.Data
{
    /// <summary>
    /// База данных основанная на распараллеливание задач, есть минус - жрет много оперативной памяти и небезопасна  
    /// </summary>
    public class FasterDatabase : Database
    {
        public const int StandardNumberExecutingThreads = 4;
        
        /// <summary>
        /// Параметр отвечающий за кол-во патоков при распараллеливание задач
        /// </summary>
        public int MaxDegreeOfParallelism = StandardNumberExecutingThreads;

        public FasterDatabase(int countColumn, DatabaseSettings.DatabaseSettings Settings, int loadedSector = 0, int MaxDegreeOfParallelism = StandardNumberExecutingThreads)
            : base(countColumn, Settings, loadedSector)
        {
            this.MaxDegreeOfParallelism = MaxDegreeOfParallelism;
        }

        public FasterDatabase(int countColumn, DatabaseSettings.DatabaseSettings Settings, int loadedSector = 0) : this(countColumn, Settings, loadedSector, StandardNumberExecutingThreads) { }

        public FasterDatabase(List<AColumn> Column, DatabaseSettings.DatabaseSettings Settings, int loadedSector = 0, int MaxDegreeOfParallelism = StandardNumberExecutingThreads)
            : base(Column, Settings, loadedSector)
        {
            this.MaxDegreeOfParallelism = MaxDegreeOfParallelism;
        }

        #region Глобальное взаимодействое

        public override void ChangTypeInColumn(string Column, TypeOfData TypeOfColumn)
        {
            lock (Columns)
            {
                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {
                    var db = _LoadDatabase((int)i);
                    foreach (var column in Columns)
                    {
                        column.ChangType(TypeOfColumn);
                    }
                    DataBaseSaver.SaveAllCluster(db.Settings, LoadedSector, db.Columns.ToArray());
                });
            }
        }

        public override int[] GetAllIDsByParams(string ColumnName, string Data)
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
                    var DataBase = _LoadDatabase((int)i);

                    lock (IDs)
                    {
                        IDs.AddRange(DataBase[ColumnName].FindIDs(Data) ?? new int[0]);
                    }
                   
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
                    var Database = _LoadDatabase((int)i);

                    Database[ColumnName].ClearBoxes();

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
                    var DB = _LoadDatabase((int)i);
                    Column column = new Column(Name, DB.Columns[0].Offset);//Новый столбец

                    ItemData[] itemDatas = new ItemData[DB.Columns[0].GetCounts()];

                    for (int g = 0; g < itemDatas.Length; g++)//связывает ивенд дестроя и столбец
                    {
                        itemDatas[g] = new ItemData(g, " ");
                    }

                    column.SetDatas(itemDatas); //записываем пустые ячейки в новый столбец
                    DB.Columns.Add(column);

                    DataBaseSaver.SaveAllCluster(Settings, (uint)i, DB.Columns.ToArray());
                });
                Settings.ColumnsCount += 1;       
                _AddColumn?.Invoke(Name);
            }
        }

        public override void AddColumn(string Name, TypeOfData dataType)
        {
            lock (Columns)
            {
                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {
                    var DB = _LoadDatabase((int)i);

                    Column table = new Column(Name, dataType, DB.Columns[0].Offset);//Новый столбец

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

        public override void CloneTo(Interfaces.AColumn left, Interfaces.AColumn right)
        {
            lock (Columns)
            {
                var leftName = left.Name;
                var rightName = right.Name;

                if (this[leftName].TypeOfData != this[rightName].TypeOfData)
                {
                    right.ChangType(right.TypeOfData);
                    DataBaseSaver.SaveAllCluster(Settings, LoadedSector, Columns.ToArray());
                }

                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {
                    var DB = _LoadDatabase((int)i);
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

                if (leftColumn.TypeOfData != rightColumn.TypeOfData)
                {
                    rightColumn.ChangType(leftColumn.TypeOfData);
                    DataBaseSaver.SaveAllCluster(Settings, LoadedSector, Columns.ToArray());
                }

                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {
                    var DB = _LoadDatabase((int)i);
                    ItemData[] itemDatas = DB[left].GetDatas();
                    DB[right].SetDatas(itemDatas);
                    DataBaseSaver.SaveAllCluster(Settings, (uint)i, DB.Columns.ToArray());
                });

                _CloneColumn?.Invoke(left, right);
            }
        }

        public override void ClearAllColumn(AColumn Column, int InSector)
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
                        var _column = this[Column.Name];
                        if (_column.TypeOfData == Column.TypeOfData)
                        {
                            var DB = _LoadDatabase((int)i);
                            _column = DB[Column.Name];
                            _column.ClearBoxes();
                            DataBaseSaver.SaveAllCluster(Settings, (uint)i, DB.Columns.ToArray());
                        }
                    });
                }
                else
                {
                    var _column = this[Column.Name];
                    if (_column.TypeOfData == Column.TypeOfData)
                    {
                        var DB = _LoadDatabase(InSector);
                        _column = DB[Column.Name];
                        _column.ClearBoxes();
                        DataBaseSaver.SaveAllCluster(Settings, (uint)InSector, DB.Columns.ToArray());     
                    }
                }
                _ClearAllColumn?.Invoke(Column.Name, InSector);
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
                    _LoadDatabase((int)i);
                    foreach (Interfaces.AColumn t in Columns)
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
            var _Columns = (IEnumerable<Interfaces.AColumn>)DataBaseLoader.LoadCluster(Settings.Path, (uint)sector, Settings.Key);

            foreach (Interfaces.AColumn t in _Columns)
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
        public override Row[] GetAllDataInBaseByColumnName(string ColumnName, string data)
        {
            lock (Columns)
            {
                List<Row> Boxes = new List<Row>();

                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {
                    var DB = _LoadDatabase((int)i);

                    int[] ids = DB[ColumnName].FindIDs(data);

                    for (int j = 0; j < ids.Length; j++)
                    {
                        var box = new Row();

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

        public override T[] SmartSearch<T>(AColumn[] Columns, SearchType[] SearchType, string[] Params, int InSectro = -1)
        {
            lock (Columns)
            {
                List<T> Boxes = new List<T>();
                List<List<int>> Search = new List<List<int>>();
                List<int> resultIDs = new List<int>();

                if (Columns.Length != SearchType.Length && Columns.Length != Params.Length)
                    throw new ArgumentException("Параметры не совпадают по кол-ву!");

                if (InSectro == -1)
                {
                    Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                    {
                        MaxDegreeOfParallelism = MaxDegreeOfParallelism
                    },
                    i =>
                    {
                        Boxes.AddRange(SmartSearch<T>(Columns, SearchType, Params, (int)i));
                    });
                }
                else
                {
                    var DB = _LoadDatabase(InSectro);

                    for (int j = 0; j < Params.Length; j++)
                    {
                        var _colomn = DB[Columns[j].Name];

                        if (_colomn.TypeOfData == Columns[j].TypeOfData)
                        {
                            List<int> IDs = new List<int>();
                            IDs = new SmartSearcher((AColumn)Columns[j], (AColumn)_colomn, SearchType[j], Params[j]).Search();
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
                        List<string> data = new List<string>();
                        foreach (var t in this.Columns)
                        {
                            data.Add(t.FindDataByID(resultIDs[i]));
                        }

                        var dl = new T();
                        dl.Init(resultIDs[i], data.ToArray());

                        Boxes.Add(dl);
                    }
                }
                return Boxes.ToArray();
            }
        }
 
        #endregion

        private Database _LoadDatabase(int i)
        {
            var Database = new Database((int)Settings.ColumnsCount, Settings, 0);

            if (LoadedSector != i)
            {
                Database.Columns.AddRange((IEnumerable<AColumn>)DataBaseLoader.LoadCluster(Settings.Path, (uint)i + 1, Settings.Key));
            }
            else
            {
                Database = this;
            }
            return Database;
        }

    }
}

