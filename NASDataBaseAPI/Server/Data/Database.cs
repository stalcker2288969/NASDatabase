using NASDatabase.Data;
using NASDatabase.Data.DataTypesInColumn;
using NASDatabase.Interfaces;
using NASDatabase.Server.Data.DatabaseSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NASDatabase.Server.Data
{
    public class Database : IDisposable
    {
        #region Events
        /// <summary>
        /// [data]
        /// </summary>
        public Action<string[]> _RemoveDataByData;
        /// <summary>
        /// [data, id]
        /// </summary>
        public Action<string[], int> _RemoveData;
        /// <summary>
        /// [data, id] Добавить данные 
        /// </summary>
        public Action<string[], int> _AddData;
        /// <summary>
        /// [name] Добавляет столбец
        /// </summary>
        public Action<string> _AddColumn;
        /// <summary>
        /// [name] Удаление столбца 
        /// </summary>
        public Action<string> _RemoveColumn;
        /// <summary>
        /// [number] !Часто вызывается в сложных функциях и сложной логикой не стоит наделять! 
        /// </summary>
        public Action<int> _LoadedNewSector;
        /// <summary>
        /// [left, right] Произошло копирование одного столбца в другой
        /// </summary>
        public Action<string, string> _CloneColumn;
        /// <summary>
        /// [name] [sector]
        /// </summary>
        public Action<string, int> _ClearAllColumn;

        public Action _ClearAllBase;
        /// <summary>
        /// [oldName] [newName]
        /// </summary>
        public Action<string, string> _RenameColumn;
        /// <summary>
        /// [columnName] [itemData]
        /// </summary>
        public Action<string, ItemData> _SetDataInColumn;
        #endregion

        #region Exeption
        private const string ExeptionThereIsNotColumn = "Не был обнаружен данный столбец!";
        private const string ExeptionLengthReceivedDataDoesNotMatchNumberOfColumns = "Длина поступивших данных не совпадает c количество столбцов  ";
        private const string ExeptionTheParametersDoNotMatchInQuantity = "Параметры не совпадают по количеству!";
        #endregion

        public List<AColumn> Columns { get; protected set; }

        public List<uint> FreeIDs { get; protected set; } = new List<uint>();

        public DatabaseSettings.DatabaseSettings Settings;

        public IDataBaseSaver<AColumn> DatabaseSaver;
        public IDataBaseReplayser DatabaseReplayser;
        public IDataBaseLoader<AColumn> DatabaseLoader;
       
        public Loger DatabaseLoger
        {
            set 
            {
                DatabaseLoger = value;               
            }
        }

        public DatabaseServer Server;

        private DatabaseManager _myManager;

        public uint LoadedSector { get; private set; } = 1;

        private StringBuilder _ForPrint = new StringBuilder();

        #region Конструкторы

        public Database(int countColumn, DatabaseSettings.DatabaseSettings settings, int loadedSector = 1)
        {
            Columns = new List<AColumn>();
            this.Settings = settings;
            SetLoadedSector((int)loadedSector);
            for (int i = 0; i < countColumn; i++)
            {
                Columns.Add((AColumn)new Column(i.ToString()));
            }
        }

        public Database(List<AColumn> columns, DatabaseSettings.DatabaseSettings settings, int loadedSector = 1)
        {
            Columns = columns;
            this.Settings = settings;
            SetLoadedSector((int)loadedSector);
        }

        /// <summary>
        /// Изменяет тип работы сохранения данных на безопасный
        /// </summary>
        public virtual void EnableSafeMode()
        {
            lock (this)
            {
                if (Settings.SaveMod != true)
                {
                    Settings = new DatabaseSettings.DatabaseSettings(Settings, true);
                }

                DatabaseSaver = _myManager._databaseSavers[Convert.ToInt32(true)];
                DatabaseLoader = _myManager._databaseSavers[(int)Convert.ToInt32(true)];
                DatabaseReplayser = _myManager._databaseSavers[((int)Convert.ToInt32(true))];
            }
        }
        /// <summary>
        /// Изменяет тип мода сохранения данных на не безопасный
        /// </summary>
        public virtual void DisableSafeMode()
        {
            lock (this)
            {
                if (Settings.SaveMod != false)
                {
                    Settings = new DatabaseSettings.DatabaseSettings(Settings, false);
                }
                DatabaseSaver = _myManager._databaseSavers[Convert.ToInt32(false)];
                DatabaseLoader = _myManager._databaseSavers[(int)Convert.ToInt32(false)];
                DatabaseReplayser = _myManager._databaseSavers[((int)Convert.ToInt32(false))];
            }
        }

        /// <summary>
        /// Вычисляет сектор к которому нужно обратиться чтобы получить данные по ID 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public uint GetSectorByID(uint ID)
        {
            return (ID / Settings.CountBucketsInSector) + 1;
        }
        /// <summary>
        /// Сеттер для LoadedSector, оповещает о изменение свойства
        /// </summary>
        /// <param name="newSectorsID"></param>
        protected void SetLoadedSector(int newSectorsID)
        {
            LoadedSector = (uint)newSectorsID;
            Settings.CountClusters = LoadedSector - 1 == Settings.CountClusters ? LoadedSector : Settings.CountClusters;
            _LoadedNewSector?.Invoke(newSectorsID);
        }

        /// <summary>
        /// Загрузка класера/просто сокрщает код  
        /// </summary>
        /// <param name="newSector"></param>
        /// <returns></returns>
        private void _LoadDataBase(int newSector)
        {
            if (newSector == 0)
                newSector = 1;

            if (LoadedSector != newSector)
            {
                Columns.Clear();
                Columns.AddRange((IEnumerable<AColumn>)DatabaseLoader.LoadCluster(Settings.Path, (uint)newSector, Settings.Key));
                SetLoadedSector((int)newSector);
            }
        }
        #endregion

        #region Глобальное взаимодействое
        /// <summary>
        /// Получает айдишики все строк по параметрам
        /// </summary>
        /// <param name="nameColumn"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual int[] GetAllIDsByParams(string nameColumn, string data)
        {
            lock (Columns)
            {
                List<int> IDs = new List<int>();

                for (int i = 0; i < Settings.CountClusters; i++)
                {
                    _LoadDataBase(i);

                    IDs.AddRange(this[nameColumn].FindIDs(data) ?? new int[0]);
                }

                return IDs.ToArray();
            }
        }

        public virtual int[] GetAllIDsByParams(int numberColumn, string data)
        {
            return GetAllIDsByParams(Columns[numberColumn].Name, data);
        }
        /// <summary>
        /// Изменяет тип в указанном столбце 
        /// </summary>
        /// <param name="nameColumn"></param>
        /// <param name="typeOfData"></param>
        public virtual void ChangTypeInColumn(string nameColumn, TypeOfData typeOfData)
        {
            lock (Columns)
            {
                for (int i = 0; i < Settings.CountClusters; i++)
                {
                    _LoadDataBase(i);
                    foreach (var column in Columns)
                    {
                        column.ChangType(typeOfData);
                    }
                    DatabaseSaver.SaveAllCluster(Settings, LoadedSector, Columns.ToArray());
                }
            }
        }

        public void ChangTypeInColumn(AColumn column, TypeOfData typeOfData)
        {
            ChangTypeInColumn(column.Name, typeOfData);
        }

        public void ChangTypeInColumn(int column, TypeOfData typeOfData)
        {
            ChangTypeInColumn(Columns[column].Name, typeOfData);
        }
        /// <summary>
        /// Удаляет столбец
        /// </summary>
        /// <param name="columnName"></param>
        public virtual void RemoveColumn(string columnName)
        {
            lock (Columns)
            {
                for (int i = 0; i < Settings.CountClusters; i++)
                {
                    _LoadDataBase(i);
                    this[columnName].ClearBoxes();

                    DatabaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                }

                Settings.ColumnsCount -= 1;
                _RemoveColumn?.Invoke(columnName);
            }
        }

        public virtual void RemoveColumn(int numberOfColumn)
        {
            RemoveColumn(Columns[numberOfColumn].Name);
        }

        public virtual void RemoveColumn(AColumn column)
        {
            RemoveColumn(column.Name);
        }

        /// <summary>
        /// Добавляет новый столбец. Процедура очень не продуктивная
        /// </summary>
        /// <param name="name"></param>
        public virtual void AddColumn(string name)
        {
            lock (Columns)
            {
                Settings.ColumnsCount += 1;
                for (int i = 0; i < Settings.CountClusters; i++)
                {
                    _LoadDataBase(i);

                    Column column = new Column(name, Columns[0].Offset);//Новый столбец

                    ItemData[] itemDatas = new ItemData[Columns[0].GetCounts()];

                    for (int g = 0; g < itemDatas.Length; g++)//связывает ивенд дестроя и столбец
                    {
                        itemDatas[g] = new ItemData(g, " ");
                    }

                    column.SetDatas(itemDatas); //записываем пустые ячейки в новый столбец

                    Columns.Add(column);
                    DatabaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                }

                _AddColumn?.Invoke(name);
            }
        }

        /// <summary>
        /// Добавляет столбик и задет тип данных в столбике
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typeOfData"></param>
        public virtual void AddColumn(string name, TypeOfData typeOfData)
        {
            lock (Columns)
            {
                Settings.ColumnsCount += 1;
                for (int i = 0; i < Settings.CountClusters; i++)
                {
                    _LoadDataBase(i);

                    Column column = new Column(name, typeOfData, Columns[0].Offset);//Новый столбец

                    ItemData[] itemDatas = new ItemData[Columns[0].GetCounts()];

                    for (int g = 0; g < itemDatas.Length; g++)//связывает ивенд дестроя и столбец
                    {
                        itemDatas[g] = new ItemData(g, " ");
                    }

                    column.SetDatas(itemDatas); //записываем пустые ячейки данные в новый столбец

                    Columns.Add(column);
                    DatabaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                }
                _AddColumn?.Invoke(name);
            }
        }

        /// <summary>
        /// Добавляет столбик и задет тип данных в столбике
        /// </summary>
        public virtual void AddColumn(AColumn column)
        {
            AddColumn(column.Name, column.TypeOfData);
        }

        /// <summary>
        /// Копирует данные из левого столбца в правый. Важно, что копирует внутри самой базы данных
        /// </summary>
        /// <param name="leftColumn"></param>
        /// <param name="rightColumn"></param>
        public virtual void CloneTo(AColumn leftColumn, AColumn rightColumn)
        {
            lock (Columns)
            {
                var leftName = leftColumn.Name;
                var rightName = rightColumn.Name;

                if (this[leftName].TypeOfData != this[rightName].TypeOfData)
                {
                    rightColumn.ChangType(rightColumn.TypeOfData);
                    DatabaseSaver.SaveAllCluster(Settings, LoadedSector, Columns.ToArray());
                }

                for (int i = 1; i < Settings.CountClusters; i++)
                {
                    _LoadDataBase(i);
                    ItemData[] itemDatas = this[leftName].GetDatas();
                    this[rightName].SetDatas(itemDatas);
                    DatabaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                }
                _CloneColumn?.Invoke(leftName, rightName);
            }
        }

        /// <summary>
        /// Копирует данные из левого столбца в правый. Важно, что копирует внутри самой базы данных 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="rigth"></param>
        public virtual void CloneTo(string left, string rigth)
        {
            lock (Columns)
            {
                var rightColumn = this[rigth];
                var leftColumn = this[left];

                if (leftColumn.TypeOfData != rightColumn.TypeOfData)
                {
                    rightColumn.ChangType(leftColumn.TypeOfData);
                    DatabaseSaver.SaveAllCluster(Settings, LoadedSector, Columns.ToArray());
                }

                for (int i = 1; i < Settings.CountClusters; i++)
                {
                    _LoadDataBase(i);
                    ItemData[] itemDatas = this[left].GetDatas();
                    this[rigth].SetDatas(itemDatas);
                    DatabaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                }
                _CloneColumn?.Invoke(left, rigth);
            }
        }

        /// <summary>
        /// Отчищает отдельный столбец в указанном секторе/кластере или везде 
        /// </summary>
        /// <param name="column"></param>
        public virtual void ClearAllColumn(AColumn column, int inSector = -1)
        {
            if (inSector == -1)
            {
                for (int i = 1; i < Settings.CountClusters; i++)
                {
                    var _column = this[column.Name];
                    if (_column.TypeOfData == column.TypeOfData)
                    {
                        _LoadDataBase(i);
                        _column.ClearBoxes();
                        DatabaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                    }
                }
            }
            else
            {
                var _column = this[column.Name];
                if (_column.TypeOfData == column.TypeOfData)
                {
                    _LoadDataBase(inSector);
                    _column.ClearBoxes();
                    DatabaseSaver.SaveAllCluster(Settings, (uint)inSector, Columns.ToArray());
                }
            }
        }

        /// <summary>
        /// Отчищает отдельный столбец в указаном секторе/класторе или везде 
        /// </summary>
        /// <param name="columnName"></param>
        public virtual void ClearAllColumn(string columnName, int inSector = -1)
        {
            if (inSector == -1)
            {
                for (int i = 1; i < Settings.CountClusters; i++)
                {
                    var _column = this[columnName];
                    _LoadDataBase(i);
                    _column.ClearBoxes();
                    DatabaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                }
            }
            else
            {
                var _column = this[columnName];
                _LoadDataBase(inSector);
                _column.ClearBoxes();
                DatabaseSaver.SaveAllCluster(Settings, (uint)inSector, Columns.ToArray());
            }
        }
        /// <summary>
        /// Чистит всю базу / не производительная команда
        /// </summary>
        public virtual void ClearAllBase()
        {
            lock (Columns)
            {
                for (int i = 1; i < Settings.CountClusters; i++)
                {
                    _LoadDataBase(i);
                    foreach (Interfaces.AColumn t in Columns)
                    {
                        t.ClearBoxes();
                    }
                    DatabaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                }
                Settings.CountBuckets = 0;
            }
        }

        public virtual void RenameColumn(string name, string newName)
        {
            lock (Columns)
            {
                this[name].Name = newName;
                _myManager.SaveStatesDatabase(this);
            }
        }

        public virtual void RenameColumn(int name, string newName)
        {
            RenameColumn(Columns[name].Name, newName);
        }

        public virtual void RenameColumn(AColumn column, string newName)
        {
            RenameColumn(column.Name, newName);
        }

        #endregion

        #region Добавление/замена данных 

        /// <summary>
        /// Заменяет все элементы в указанном столбце на новые данные, довольно тяжелая операция 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="newData"></param>
        /// <param name="sectorID"></param>
        /// <param name="columnName"></param>
        public virtual void ChangeEverythingTo(string columnName, string param, string newData, int sectorID = -1)
        {
            lock (Columns)
            {
                if (sectorID == -1)
                {
                    for (int i = 1; i < Settings.CountClusters; i++)
                    {
                        _LoadAndChengeDataInCluster(i, columnName, param, newData);
                    }
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
        /// <param name="params"></param>
        /// <param name="newData"></param>
        private void _LoadAndChengeDataInCluster(int sector, string columnName, string @params, string newData)
        {
            _LoadDataBase(sector);
            foreach (AColumn column in Columns)
            {
                if (column.Name == columnName)
                {
                    var ids = column.FindIDs(@params);
                    foreach (var id in ids)
                    {
                        var itemData = new ItemData(id, newData);
                        column.SetDataByID(itemData);
                        _SetDataInColumn?.Invoke(columnName, itemData);
                    }
                }
            }
            DatabaseSaver.SaveAllCluster(Settings, (uint)sector, Columns.ToArray());
        }


        /// <summary>
        /// Заменяет строку
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="data"></param>
        public virtual void SetData(int ID, params string[] data)
        {
            uint SectorID = GetSectorByID((uint)ID);

            ReplayesDataBySectorAndID(SectorID, ID, data);
        }

        /// <summary>
        /// Заменяет строку c помошью DataLine 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="datas"></param>
        public virtual void SetData(IDataRow row)
        {
            SetData(row.ID, row.GetData());
        }

        public virtual void SetData<T>(T row) where T : IDataRow
        {
            SetData(row.ID, row.GetData());
        }
        /// <summary>
        /// Создает экземпляр IDataRow и заполняет им строчку
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ID"></param>
        public virtual void SetData<T>(int ID) where T : IDataRow, new()
        {
            var t = Activator.CreateInstance<T>();
            SetData(ID, t.GetData());
        }

        /// <summary>
        /// Заменяет строку
        /// </summary>
        /// <param name="data"></param>
        public virtual void SetData(params ItemData[] data)
        {
            List<string> _datas = new List<string>();

            foreach (var d in data)
            {
                _datas.Add(d.Data);
            }

            SetData(data[0].ID, _datas.ToArray());
        }

        /// <summary>
        /// В необходимой табличке происходит замена данных
        /// </summary>
        public virtual void SetDataInColumn(string columnName, int ID, string newData)
        {
            SetDataInColumn(columnName, new ItemData(ID, newData));
        }
        /// <summary>
        /// В необходимой табличке происходит замена данных в NewItemData укажите новые данные и id ячейки в которой нужно перезаписать данные
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="newItemData"></param>
        public virtual void SetDataInColumn(string columnName, ItemData newItemData)
        {
            lock (Columns)
            {
                uint SectorID = GetSectorByID((uint)newItemData.ID);

                _LoadDataBase((int)SectorID);

                this[columnName].SetDataByID(newItemData);

                DatabaseSaver.SaveAllCluster(Settings, SectorID, Columns.ToArray());
                _SetDataInColumn?.Invoke(columnName, newItemData);
            }
        }

        public virtual void SetDataInColumn(AColumn column, ItemData newItemData)
        {
            SetDataInColumn(column.Name, newItemData);
        }

        public virtual void SetDataInColumn(AColumn column, int ID, string newData)
        {
            SetDataInColumn(column.Name, new ItemData(ID, newData));
        }
        /// <summary>
        /// Добавляет данные в таблицу, важно чтобы длина поступающего массива была равна кол-ву столбцов  
        /// Ошибки: Exception($"Длина поступивших данных меньше кол-ва столбцов: {Columns.Count}")
        /// </summary>
        /// <param name="datas"></param>
        public virtual void AddData(params string[] datas)
        {
            if (datas?.Length == Columns.Count)
            {
                if (FreeIDs.Count == 0)
                {
                    uint SectorID = GetSectorByID(Settings.CountBuckets);//Опредиляем к какому сектору обратиться
                    AddBySectorAndID(SectorID, (int)Settings.CountBuckets, datas);
                }
                else
                {
                    uint FreeID = FreeIDs[0];
                    FreeIDs.Remove(FreeID);
                    uint SectorID = GetSectorByID(FreeID);
                    ReplayesDataBySectorAndID(SectorID, (int)FreeID, datas);
                }

            }
            else if (datas?.Length < Columns.Count)
            {
                throw new Exception(ExeptionLengthReceivedDataDoesNotMatchNumberOfColumns + Columns.Count);
            }
            else if (datas?.Length > Columns.Count)
            {
                throw new Exception(ExeptionLengthReceivedDataDoesNotMatchNumberOfColumns + Columns.Count);
            }
        }

        /// <summary>
        /// Добавляет данные в таблицу, важно чтобы длина поступающего массива была равна кол-ву столбцов  
        /// Ошибки: Exception($"Длина поступивших данных меньше кол-ва столбцов: {Columns.Count}")
        /// </summary>
        /// <param name="datas"></param>
        public virtual void AddData(IDataRow row)
        {
            AddData(row.GetData());
        }

        /// <summary>
        /// Добавляет данные в таблицу, важно чтобы длина поступающего массива была равна кол-ву столбцов  
        /// Ошибки: Exception($"Длина поступивших данных меньше кол-ва столбцов: {Columns.Count}")
        /// </summary>
        /// <param name="data"></param>
        public virtual void AddData(params object[] data)
        {
            List<string> strings = new List<string>();

            foreach (var d in data)
            {
                strings.Add(d.ToString());
            }

            AddData(strings.ToArray());
        }

        private void ReplayesDataBySectorAndID(uint sectorID, int ID, string[] data)
        {
            lock (Columns)
            {
                _LoadDataBase((int)sectorID);
                bool res = false;

                List<ItemData> itemDatas = new List<ItemData>();
                for (int i = 0; i < this.Columns.Count; i++)
                {
                    res = Columns[i].SetDataByID(new ItemData(ID, data[i]));
                    itemDatas.Add(new ItemData(ID, data[i]));
                }

                Settings.CountBuckets += 1;
                DatabaseReplayser.ReplayesElement(Settings, sectorID, itemDatas.ToArray());


                if (res)
                    _AddData?.Invoke(data, ID);
            }
        }
        /// <summary>
        /// Добавляет данные по сектору и id
        /// </summary>
        /// <param name="sectorID"></param>
        /// <param name="data"></param>
        private void AddBySectorAndID(uint sectorID, int ID, string[] data)
        {
            lock (Columns)
            {
                _LoadDataBase((int)sectorID);

                List<ItemData> itemDatas = new List<ItemData>();
                for (int i = 0; i < this.Columns.Count; i++)
                {
                    itemDatas.Add(new ItemData(ID, data[i]));
                    Columns[i].Push(data[i], (uint)ID);
                }
                Settings.CountBuckets += 1;
                DatabaseSaver.AddElement(Settings, sectorID, itemDatas.ToArray());


                _AddData?.Invoke(data, ID);
            }
        }
        #endregion

        #region Удаление данных
        /// <summary>
        /// Удаляет данные по введенному ID
        /// </summary>
        /// <param name="ID"></param>
        public virtual void RemoveDataByID(int ID)
        {
            lock (Columns)
            {
                uint SectorID = GetSectorByID((uint)ID);
                List<ItemData> ItemDatas = new List<ItemData>();

                var data = GetDataByID((int)ID);

                _LoadDataBase((int)SectorID);

                for (int i = 0; i < this.Columns.Count; i++)
                {
                    Columns[i].SetDataByID(new ItemData((int)ID, " "));
                    ItemDatas.Add(new ItemData((int)ID, " "));
                }
                FreeIDs.Add((uint)ID);
                DatabaseReplayser.ReplayesElement(Settings, SectorID, ItemDatas.ToArray());
                Settings.CountBuckets -= 1;


                _RemoveData?.Invoke(data, (int)ID);
            }
        }

        /// <summary>
        /// Удаляет все данные из базы подходящие по параметру.
        /// </summary>
        /// <param name="datas">Параметр</param>
        public virtual bool RemoveAllData(params string[] datas)
        {
            if (datas.Length == Columns.Count)
            {
                for (int i = 0; i < Settings.CountClusters; i++)
                {
                    _LoadDataBase(i);
                    List<bool> bools = new List<bool>();

                    for (int g = 0; g < this.Columns.Count; g++)
                    {
                        if (Columns[g].Pop(datas[g]))
                        {
                            bools.Add(true);
                        }
                    }

                    if (bools.Count == Columns.Count)
                    {
                        Settings.CountBuckets -= 1;
                        DatabaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());

                        string _datas = "";//для логирования

                        foreach (var d in datas)
                        {
                            _datas += d;
                        }

                        return true;
                    }
                    else
                    {
                        DatabaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                        return false;
                    }

                }
            }
            _RemoveDataByData?.Invoke(datas);
            return true;
        }

        /// <summary>
        /// Удаляет все данные из базы подходящие по параметру.
        /// </summary>
        public virtual bool RemoveAllData(IDataRow row)
        {
            return RemoveAllData(row.GetData());
        }

        /// <summary>
        /// Удаляет все данные из базы подходящие по параметру.
        /// </summary>
        public virtual bool RemoveAllData(params object[] datas)
        {
            List<string> strings = new List<string>();
            foreach (object data in datas)
            {
                strings.Add(data.ToString());
            }
            return RemoveAllData(strings);
        }
        /// <summary>
        /// Удаляет все данные по указанному списку ID
        /// </summary>
        /// <param name="IDs"></param>
        public virtual void RemoveDatasByIDs(int[] IDs)
        {
            foreach (var d in IDs)
            {
                RemoveDataByID(d);
            }
        }

        #endregion

        #region Сортировка/Получение данных по параметрам
        /// <summary>
        /// Отображает загруженный сектор в память. Если происходит ошибка возврат " " 
        /// </summary>
        /// <returns></returns>
        public virtual string PrintBase()
        {
            try
            {
                lock (Columns)
                {
                    lock (_ForPrint)
                    {
                        int l = Columns[0].GetCounts();

                        StringBuilder ColumnsBuilder = new StringBuilder();

                        ColumnsBuilder.Append("% Number % | ");
                        for (int g = 0; g < Columns.Count; g++)
                        {
                            ColumnsBuilder.Append("% " + Columns[g].Name + " % | ");
                        }

                        string CLText = ColumnsBuilder.ToString();
                        string lines = new string('-', CLText.Length);//Делаем линии длиной равной длине кол-ву столбцов

                        _ForPrint.Append($"Columns names:\n{lines}\n");
                        _ForPrint.Append(CLText);
                        _ForPrint.Append($"\n{lines}\n");

                        for (int id = 0; id < l; id++)
                        {
                            _ForPrint.Append(id.ToString() + " | ");
                            for (int g = 0; g < Columns.Count; g++)
                            {
                                _ForPrint.Append(Columns[g].FindDataByID(id) + " | ");
                            }

                            _ForPrint.Append($"\n{lines}\n");
                        }

                        string Text = _ForPrint.ToString();
                        _ForPrint.Clear();
                        return Text;
                    }
                }
            }
            catch
            {
                return " ";
            }
        }

        /// <summary>
        /// Сканирует всю БД в поисках подходящих строк 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual Row[] GetAllDataInBaseByColumnName(string columnName, string data)
        {
            lock (Columns)
            {
                List<Row> Boxes = new List<Row>();

                for (int i = 1; i < Settings.CountClusters + 1; i++)
                {
                    _LoadDataBase(i);

                    int[] ids = this[columnName].FindIDs(data);

                    for (int j = 0; j < ids.Length; j++)
                    {
                        Boxes.Add(new Row());
                        string[] strings = new string[Columns.Count];

                        for (int k = 0; k < Columns.Count; k++)
                        {
                            strings[k] = Columns[k].FindDataByID((int)ids[j]);
                        }
                        Boxes[j].Init(ids[j], strings);
                    }
                }
                return Boxes.ToArray();
            }
        }


        /// <summary>
        /// Сканирует всю БД в поисках подходящих строк 
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual Row[] GetAllDataInBaseByColumnName(AColumn column, string data)
        {
            return GetAllDataInBaseByColumnName(column.Name, data);
        }

        /// <summary>
        /// По введенным параметрам ищет данные в БД
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns">Табличка параметр</param>
        /// <param name="typesOfSearch">способ поиска данных</param>
        /// <param name="param">Данные от которых нужно операться</param>
        /// <param name="inSectro">Сектор в котором нужно искать данные(-1 - во всех)</param>
        /// <returns></returns>
        public virtual T[] SmartSearch<T>(AColumn[] columns, SearchType[] typesOfSearch, string[] param, int inSectro = -1)
            where T : IDataRow, new()
        {
            lock (this.Columns)
            {
                List<T> Boxes = new List<T>();
                List<List<int>> Search = new List<List<int>>();
                List<int> resultIDs = new List<int>();

                if (columns.Length != typesOfSearch.Length && columns.Length != param.Length)
                    throw new ArgumentException(ExeptionTheParametersDoNotMatchInQuantity);

                if (inSectro == -1)
                {
                    for (int i = 0; i < Settings.CountClusters; i++)
                    {
                        Boxes.AddRange(SmartSearch<T>(columns, typesOfSearch, param, i));
                    }
                }
                else
                {
                    _LoadDataBase(inSectro);
                    for (int j = 0; j < param.Length; j++)
                    {
                        var _colomn = this[columns[j].Name];

                        if (_colomn.TypeOfData == columns[j].TypeOfData)
                        {
                            List<int> IDs = new List<int>();
                            IDs = new SmartSearcher(columns[j], _colomn, typesOfSearch[j], param[j]).Search();
                            Search.Add(IDs);
                        }
                    }

                    for (int i = 0; i < Search.Count; i++)
                    {
                        if (Search.Count > i + 1)
                        {
                            int[] _result = Search[i].Intersect(Search[i + 1]).ToArray();
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


        /// <summary>
        /// Ищет первый id элемента по параметрам, если IsSector = -1, то ищет везде 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="data"></param>
        /// <param name="inSector"></param>
        /// <returns></returns>
        public virtual int GetIDByParams(string columnName, string data, int inSector = -1)
        {
            int result = -1;

            if (inSector == -1)
            {
                for (int i = 0; i < Settings.CountClusters; i++)
                {
                    result = GetIDByParams(columnName, data, i);

                    if (result != -1)
                    {
                        break;
                    }
                }

            }
            else
            {
                _LoadDataBase(inSector);

                result = this[columnName].FindID(data);
            }
            return result;
        }

        /// <summary>
        /// Ищет первый id элемента по параметрам, если IsSector = -1, то ищет везде 
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="data"></param>
        /// <param name="inSector"></param>
        /// <returns></returns>
        public virtual int GetIDByParams(AColumn column, string data, int inSector = -1)
        {
            return GetIDByParams(column.Name, data, inSector);
        }

        /// <summary>
        /// Находит строку данных по id
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="InSector"></param>
        /// <returns></returns>
        public virtual string[] GetDataByID(int ID)
        {
            lock (Columns)
            {
                List<string> strings = new List<string>();

                _LoadDataBase((int)GetSectorByID((uint)ID));

                foreach (var t in Columns)
                {
                    strings.Add(t.FindDataByID(ID));
                }
                return strings.ToArray();
            }
        }

        /// <summary>
        /// Возвращет строку по id в виде ItemData[]
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public virtual ItemData[] GetItemsDataByID(int ID)
        {
            List<ItemData> data = new List<ItemData>();
            foreach (var t in GetDataByID(ID))
            {
                data.Add(new ItemData(ID, t));
            }
            return data.ToArray();
        }

        public virtual T GetDataLineByID<T>(int ID) where T : IDataRow
        {
            var line = Activator.CreateInstance<T>();
            line.Init(ID, GetDataByID(ID));
            return line;
        }

        /// <summary>
        /// Ищет и возвращает первую строку подходящую под введенные параметры возврат через массив ячеек, если не находит => массив пустой
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="data"></param>
        /// <param name="inSector">Если -1, то ищет во всех сразу, иначе в загруженном</param>
        /// <returns></returns>
        public virtual ItemData[] GetDataInBaseByColumnName(string columnName, string data, int inSector = -1)
        {
            lock (Columns)
            {
                List<ItemData> _data = new List<ItemData>();

                bool Use = false;//маркер о том что поиск в {InSector} секторе уже был 

                for (int i = 0; i < Settings.CountClusters; i++)
                {
                    if (inSector != -1 && Use == true)
                    {
                        if (Use == true)
                        {
                            break;
                        }
                        else
                        {
                            Use = true;
                            _LoadDataBase(inSector);
                        }
                    }
                    else
                    {
                        _LoadDataBase(i);
                    }


                    int id = this[columnName].FindID(data);

                    if (id != -1)
                    {
                        foreach (AColumn column in Columns)
                        {
                            _data.Add(new ItemData(id, column.FindDataByID(id)));
                        }
                        return _data.ToArray();
                    }

                }

                return _data.ToArray();
            }
        }

        public virtual ItemData GetDataByParams(string columnName, int ID)
        {
            var Sector = GetSectorByID((uint)ID);
            _LoadDataBase((int)Sector);
            return new ItemData(ID, this[columnName].FindDataByID(ID));
        }

        public virtual ItemData GetDataByParams(AColumn column, int ID)
        {
            return GetDataByParams(column.Name, ID);
        }
        #endregion

        #region Индексаторы
        public virtual AColumn this[string columnName]
        {
            get
            {
                lock (Columns)
                {
                    foreach (Column Column in Columns)
                    {
                        if (Column.Name == columnName)
                        {
                            return Column;
                        }
                    }

                    throw new IndexOutOfRangeException(ExeptionThereIsNotColumn);
                }
            }
            protected set
            {
                lock (Columns)
                {
                    for (int i = 0; i < Columns.Count; i++)
                    {
                        if (Columns[i].Name == columnName)
                        {
                            Columns[i] = value; return;
                        }
                    }

                    throw new IndexOutOfRangeException(ExeptionThereIsNotColumn);
                }
            }
        }

        public virtual AColumn this[int index]
        {
            get
            {
                return Columns[index];
            }
            protected set
            {
                lock (Columns)
                {
                    Columns[index] = value;
                }
            }
        }
        #endregion

        public void InitManager(DatabaseManager databaseManager) { _myManager = _myManager == null ? databaseManager : _myManager; }

        public void Dispose()
        {
            DatabaseSaver.SaveAllCluster(Settings, LoadedSector, Columns.ToArray());
        }

        ~Database() => Dispose();
    }
}
