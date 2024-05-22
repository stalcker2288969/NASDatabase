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
        /// <summary>
        /// Стандартное количество выполняющих потоков
        /// </summary>
        public const int StandardNumberExecutingThreads = 4;

        /// <summary>
        /// Параметр отвечающий за кол-во потоков при распараллеливание задач 
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

        public override void ChangTypeInColumn(string Column, TypeOfData newTypeOfColumn)
        {
            lock (Columns)
            {
                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {
                    using (var db = _LoadDatabase((int)i))
                    {
                        foreach (var column in Columns)
                        {
                            column.ChangType(newTypeOfColumn);
                        }
                        DatabaseSaver.SaveAllCluster(db.Settings, LoadedSector, db.Columns.ToArray());
                    }                    
                });
            }
        }

        public override int[] GetAllIDsByParams(string columnName, string data)
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
                    using (var db = _LoadDatabase((int)i))

                    lock (IDs)
                    {
                        IDs.AddRange(db[columnName].FindIDs(data) ?? new int[0]);
                    }
                    
                });

                return IDs.ToArray();
            }
        }

        public override void RemoveColumn(string columnName)
        {
            lock (Columns)
            {
                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {

                    using (var Database = _LoadDatabase((int)i))
                    {
                        Database[columnName].ClearBoxes();

                        DatabaseSaver.SaveAllCluster(Settings, (uint)i,Database.Columns.ToArray());
                    }
                });
                Settings.ColumnsCount -= 1;
                _RemoveColumn?.Invoke(columnName);
            }
        }

        public override void AddColumn(string name)
        {
            lock (Columns)
            {
                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {
                    using (var DB = _LoadDatabase((int)i))
                    {
                        Column column = new Column(name, DB.Columns[0].Offset);//Новый столбец

                        ItemData[] itemDatas = new ItemData[DB.Columns[0].GetCounts()];

                        for (int g = 0; g < itemDatas.Length; g++)//связывает ивенд дестроя и столбец
                        {
                            itemDatas[g] = new ItemData(g, " ");
                        }

                        column.SetDatas(itemDatas); //записываем пустые ячейки в новый столбец
                        DB.Columns.Add(column);

                        DatabaseSaver.SaveAllCluster(Settings, (uint)i, DB.Columns.ToArray());
                    }                  
                });
                Settings.ColumnsCount += 1;
                _AddColumn?.Invoke(name);
            }
        }

        public override void AddColumn(string name, TypeOfData typeOfData)
        {
            lock (Columns)
            {
                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {
                    using (var DB = _LoadDatabase((int)i))
                    {
                        Column table = new Column(name, typeOfData, DB.Columns[0].Offset);//Новый столбец

                        ItemData[] itemDatas = new ItemData[DB.Columns[0].GetCounts()];

                        for (int g = 0; g < itemDatas.Length; g++)//связывает ивенд дестроя и столбец
                        {
                            itemDatas[g] = new ItemData(g, " ");
                        }

                        table.SetDatas(itemDatas); //записываем пустые ячейки данные в новый столбец

                        DB.Columns.Add(table);
                        DatabaseSaver.SaveAllCluster(Settings, (uint)i, DB.Columns.ToArray());
                    }       
                });
                Settings.ColumnsCount += 1;
                _AddColumn?.Invoke(name);
            }
        }

        public override void CloneTo(AColumn left, AColumn right)
        {
            lock (Columns)
            {
                var leftName = left.Name;
                var rightName = right.Name;

                if (this[leftName].TypeOfData != this[rightName].TypeOfData)
                {
                    right.ChangType(right.TypeOfData);
                    DatabaseSaver.SaveAllCluster(Settings, LoadedSector, Columns.ToArray());
                }

                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {
                    using (var DB = _LoadDatabase((int)i))
                    {
                        ItemData[] itemDatas = DB[leftName].GetDatas();
                        DB[rightName].SetDatas(itemDatas);
                        DatabaseSaver.SaveAllCluster(Settings, (uint)i, DB.Columns.ToArray());
                    }    
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
                    DatabaseSaver.SaveAllCluster(Settings, LoadedSector, Columns.ToArray());
                }

                Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = MaxDegreeOfParallelism
                },
                i =>
                {
                    using(var DB = _LoadDatabase((int)i))
                    {
                        ItemData[] itemDatas = DB[left].GetDatas();
                        DB[right].SetDatas(itemDatas);
                        DatabaseSaver.SaveAllCluster(Settings, (uint)i, DB.Columns.ToArray());
                    }             
                });

                _CloneColumn?.Invoke(left, right);
            }
        }

        public override void ClearAllColumn(AColumn column, int inSector)
        {
            lock (Columns)
            {
                if (inSector == -1)
                {
                    var _column = this[column.Name];
                    Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                    {
                        MaxDegreeOfParallelism = MaxDegreeOfParallelism
                    },
                    i =>
                    {                       
                        if (_column.TypeOfData == column.TypeOfData)
                        {
                            var DB = _LoadDatabase((int)i);
                            _column = DB[column.Name];
                            _column.ClearBoxes();
                            DatabaseSaver.SaveAllCluster(Settings, (uint)i, DB.Columns.ToArray());
                        }
                    });
                }
                else
                {
                    var _column = this[column.Name];
                    if (_column.TypeOfData == column.TypeOfData)
                    {
                        var DB = _LoadDatabase(inSector);
                        _column = DB[column.Name];
                        _column.ClearBoxes();
                        DatabaseSaver.SaveAllCluster(Settings, (uint)inSector, DB.Columns.ToArray());
                    }
                }
                _ClearAllColumn?.Invoke(column.Name, inSector);
            }
        }

        public override void ClearAllColumn(string columnName, int inSector)
        {
            ClearAllColumn(this[columnName], inSector);
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
                    DatabaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                });
                _ClearAllBase?.Invoke();
            }
        }


        #endregion

        #region Добавляение/замена данных

        /// <summary>
        /// Выполняет замену данных по параметрам в ассинхронном порядке
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="param"></param>
        /// <param name="newData"></param>
        /// <param name="sectorID"></param>
        public override void ChangeEverythingTo(string columnName, string param, string newData, int sectorID = -1)
        {
            lock (Columns)
            {
                if (sectorID == -1)
                {
                    Parallel.For(1, Settings.CountClusters, i =>
                    {
                        _LoadAndChengeDataInCluster((int)i, columnName, param, newData);
                    });
                }
                else
                {
                    _LoadAndChengeDataInCluster((int)sectorID, columnName, param, newData);
                }
            }
        }

        /// <summary>
        /// Приватынй метод для ChangeEverythingTo
        /// </summary>
        /// <param name="sector"></param>
        /// <param name="columnName"></param>
        /// <param name="param"></param>
        /// <param name="newData"></param>
        private void _LoadAndChengeDataInCluster(int sector, string columnName, string param, string newData)
        {
            var _Columns = (IEnumerable<AColumn>)DatabaseLoader.LoadCluster(Settings.Path, (uint)sector, Settings.Key);

            foreach (AColumn t in _Columns)
            {
                if (t.Name == columnName)
                {
                    var ids = t.FindIDs(param);
                    foreach (var id in ids)
                    {
                        t.SetDataByID(new ItemData(id, newData));
                    }
                }
            }
            DatabaseSaver.SaveAllCluster(Settings, (uint)sector, Columns.ToArray());
        }

        #endregion

        #region Сортировка
        public override Row[] GetAllDataInBaseByColumnName(string columnName, string data)
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

                    int[] ids = DB[columnName].FindIDs(data);

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

        public override TRow[] SmartSearch<TRow>(AColumn[] columns, SearchType[] typesOfSearch, string[] param, int inSectro = -1)
        {
            lock (Columns)
            {
                List<TRow> Boxes = new List<TRow>();
                List<List<int>> Search = new List<List<int>>();
                List<int> resultIDs = new List<int>();

                if (columns.Length != typesOfSearch.Length && columns.Length != param.Length)
                    throw new ArgumentException("Параметры не совпадают по кол-ву!");

                if (inSectro == -1)
                {
                    Parallel.For(1, Settings.CountClusters, new ParallelOptions()
                    {
                        MaxDegreeOfParallelism = MaxDegreeOfParallelism
                    },
                    i =>
                    {
                        Boxes.AddRange(SmartSearch<TRow>(columns, typesOfSearch, param, (int)i));
                    });
                }
                else
                {
                    var DB = _LoadDatabase(inSectro);

                    for (int j = 0; j < param.Length; j++)
                    {
                        var _colomn = DB[columns[j].Name];

                        if (_colomn.TypeOfData == columns[j].TypeOfData)
                        {
                            List<int> IDs = new List<int>();
                            IDs = new SmartSearcher((AColumn)columns[j], (AColumn)_colomn, typesOfSearch[j], param[j]).Search();
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

                        var dl = new TRow();
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
                Database.Columns.AddRange(DatabaseLoader.LoadCluster(Settings.Path, (uint)i + 1, Settings.Key));
            }
            else
            {
                Database = this;
            }
            return Database;
        }

    }
}

