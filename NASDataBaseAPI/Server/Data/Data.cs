using NASDataBaseAPI.Data;
using NASDataBaseAPI.Data.DataTypesInColumn;
using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.Server.Data.DataBaseSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NASDataBaseAPI.Server.Data
{
    public class DataBase : IDisposable
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
        #endregion

        public List<IColumn> Columns { get; protected set; }

        public List<uint> FreeIDs { get; protected set; } = new List<uint>();

        public DataBaseSettings.DataBaseSettings Settings;
        public DataBaseServer DataBaseServer;


        public IDataBaseSaver<IColumn> DataBaseSaver;
        public IDataBaseReplayser DataBaseReplayser;
        public IDataBaseLoader<IColumn> DataBaseLoader;
        public ILoger DataBaseLoger;

        private DataBaseManager _myManager;

        public uint LoadedSector { get; private set; } = 1;

        private StringBuilder _ForPrint = new StringBuilder();

        #region Конструкторы

        public DataBase(int countColumn, DataBaseSettings.DataBaseSettings settings, int loadedSector = 1)
        {
            Columns = new List<IColumn>();
            this.Settings = settings;
            SetLoadedSector((int)loadedSector);
            for (int i = 0; i < countColumn; i++)
            {
                Columns.Add(new Column(i.ToString()));
            }
        }

        public DataBase(List<IColumn> Column, DataBaseSettings.DataBaseSettings settings, int loadedSector = 1)
        {
            Columns = Column;
            this.Settings = settings;
            SetLoadedSector((int)loadedSector);
        }

        /// <summary>
        /// Изменяет тип мода сохранения данных на безопасный
        /// </summary>
        public virtual void EnableSafeMode()
        {
            lock (this)
            {
                if (Settings.SaveMod != true)
                {
                    Settings = new DataBaseSettings.DataBaseSettings(Settings, true);
                }

                DataBaseSaver = _myManager._dataBaseSavers[Convert.ToInt32(true)];
                DataBaseLoader = _myManager._dataBaseSavers[(int)Convert.ToInt32(true)];
                DataBaseReplayser = _myManager._dataBaseSavers[((int)Convert.ToInt32(true))];
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
                    Settings = new DataBaseSettings.DataBaseSettings(Settings, false);
                }
                DataBaseSaver = _myManager._dataBaseSavers[Convert.ToInt32(false)];
                DataBaseLoader = _myManager._dataBaseSavers[(int)Convert.ToInt32(false)];
                DataBaseReplayser = _myManager._dataBaseSavers[((int)Convert.ToInt32(false))];
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
        /// <param name="NewSectorsID"></param>
        protected void SetLoadedSector(int NewSectorsID)
        {
            LoadedSector = (uint)NewSectorsID; 
            Settings.CountClusters = LoadedSector - 1 == Settings.CountClusters ? LoadedSector : Settings.CountClusters;
            _LoadedNewSector?.Invoke(NewSectorsID);
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
                Columns.AddRange((IEnumerable<IColumn>)DataBaseLoader.LoadCluster(Settings.Path, (uint)newSector, Settings.Key));
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
                    DataBaseLoger.Log($"Get all IDs by {nameColumn} and {data} in {i} cluester");
                }

                return IDs.ToArray();
            }
        }

        public virtual int[] GetAllIDsByParams(int NumberColumn, string data)
        {
            return GetAllIDsByParams(Columns[NumberColumn].Name, data);
        }
        /// <summary>
        /// Изменяет тип в указанном столбце 
        /// </summary>
        /// <param name="NameColumn"></param>
        /// <param name="DataType"></param>
        public virtual void ChengTypeInColumn(string NameColumn, DataType DataType)
        {
            lock (Columns)
            {
                for(int i = 0; i < Settings.CountClusters; i++)
                {
                    _LoadDataBase(i);
                    foreach (var j in Columns)
                    {
                        j.ChangType(DataType);
                    }
                    DataBaseSaver.SaveAllCluster(Settings, LoadedSector, Columns.ToArray());
                }     
            }
        }

        public void ChengTypeInColumn(IColumn Column, DataType DataType)
        {
            ChengTypeInColumn(Column.Name, DataType);
        }

        public void ChengTypeInColumn(int column, DataType DataType)
        {
            ChengTypeInColumn(Columns[column].Name, DataType);
        }
        /// <summary>
        /// Удаляет столбец
        /// </summary>
        /// <param name="ColumnName"></param>
        public virtual void RemoveColumn(string ColumnName)
        {
            lock (Columns)
            {
                for (int i = 0; i < Settings.CountClusters; i++)
                {
                    _LoadDataBase(i);
                    this[ColumnName].ClearBoxes();

                    DataBaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                }

                DataBaseLoger.Log($"Delited table {ColumnName}");
                Settings.ColumnsCount -= 1;
                _RemoveColumn?.Invoke(ColumnName);
            }
        }

        public virtual void RemoveColumn(int NumberOFColumn)
        {
            RemoveColumn(Columns[NumberOFColumn].Name);
        }

        public virtual void RemoveColumn(IColumn ColumnName)
        {
            RemoveColumn(ColumnName.Name);
        }

        /// <summary>
        /// Добавляет новый столбец. Процедура очень не продуктивная
        /// </summary>
        /// <param name="Name"></param>
        public virtual void AddColumn(string Name)
        {
            lock (Columns)
            {
                Settings.ColumnsCount += 1;
                for (int i = 0; i < Settings.CountClusters; i++)
                {
                    _LoadDataBase(i);

                    IColumn table = new Column(Name, Columns[0].OffSet);//Новый столбец

                    ItemData[] itemDatas = new ItemData[Columns[0].GetCounts()];

                    for (int g = 0; g < itemDatas.Length; g++)//связывает ивенд дестроя и столбец
                    {
                        itemDatas[g] = new ItemData(g, " ");
                    }

                    table.SetDatas(itemDatas); //записываем пустые ячейки в новый столбец

                    Columns.Add(table);
                    DataBaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                }

                DataBaseLoger.Log($"Add table {Name}");
                _AddColumn?.Invoke(Name);
            }
        }

        /// <summary>
        /// Добавляет столбик и задет тип данных в столбике
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="dataType"></param>
        public virtual void AddColumn(string Name, DataType dataType)
        {
            lock (Columns)
            {
                Settings.ColumnsCount += 1;
                for (int i = 0; i < Settings.CountClusters; i++)
                {
                    _LoadDataBase(i);

                    IColumn table = new Column(Name, dataType, Columns[0].OffSet);//Новый столбец

                    ItemData[] itemDatas = new ItemData[Columns[0].GetCounts()];

                    for (int g = 0; g < itemDatas.Length; g++)//связывает ивенд дестроя и столбец
                    {
                        itemDatas[g] = new ItemData(g, " ");
                    }

                    table.SetDatas(itemDatas); //записываем пустые ячейки данные в новый столбец

                    Columns.Add(table);
                    DataBaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                }
                DataBaseLoger.Log($"Add table {Name}|{dataType}");
                _AddColumn?.Invoke(Name);
            }
        }

        /// <summary>
        /// Добавляет столбик и задет тип данных в столбике
        /// </summary>
        public virtual void AddColumn(IColumn column)
        {
            AddColumn(column.Name, column.DataType);
        }

        /// <summary>
        /// Копирует данные из левого столбца в правый. Важно, что копирует внутри самой базы данных
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public virtual void CloneTo(IColumn left, IColumn right)
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

                for (int i = 1; i < Settings.CountClusters; i++)
                {
                    _LoadDataBase(i);
                    ItemData[] itemDatas = this[leftName].GetDatas();
                    this[rightName].SetDatas(itemDatas);
                    DataBaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                }
                DataBaseLoger.Log($"{left.Name} clone to {right.Name}");
                _CloneColumn?.Invoke(leftName, rightName);
            }
        }

        /// <summary>
        /// Копирует данные из левого столбца в правый. Важно, что копирует внутри самой базы данных 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public virtual void CloneTo(string left, string right)
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

                for (int i = 1; i < Settings.CountClusters; i++)
                {
                    _LoadDataBase(i);
                    ItemData[] itemDatas = this[left].GetDatas();
                    this[right].SetDatas(itemDatas);
                    DataBaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                }
                DataBaseLoger.Log($"{left} clone to {right}");
                _CloneColumn?.Invoke(left, right);
            }
        }

        /// <summary>
        /// Отчищает отдельный столбец в указаном секторе/класторе или везде 
        /// </summary>
        /// <param name="column"></param>
        public virtual void ClearAllColumn(IColumn column, int InSector = -1)
        {
            if (InSector == -1)
            {
                for (int i = 1; i < Settings.CountClusters; i++)
                {
                    var _column = this[column.Name];
                    if (_column.DataType == column.DataType)
                    {
                        _LoadDataBase(i);
                        _column.ClearBoxes();
                        DataBaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                        DataBaseLoger.Log($"Clearing a column in a sector {i}");
                    }
                }
            }
            else
            {
                var _column = this[column.Name];
                if (_column.DataType == column.DataType)
                {
                    _LoadDataBase(InSector);
                    _column.ClearBoxes();
                    DataBaseSaver.SaveAllCluster(Settings, (uint)InSector, Columns.ToArray());
                    DataBaseLoger.Log($"Clearing a column in a sector {InSector}");
                }
            }
        }

        /// <summary>
        /// Отчищает отдельный столбец в указаном секторе/класторе или везде 
        /// </summary>
        /// <param name="ColumnName"></param>
        public virtual void ClearAllColumn(string ColumnName, int InSector = -1)
        {
            if (InSector == -1)
            {
                for (int i = 1; i < Settings.CountClusters; i++)
                {
                    var _column = this[ColumnName];
                    _LoadDataBase(i);
                    _column.ClearBoxes();
                    DataBaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                }
            }
            else
            {
                var _column = this[ColumnName];
                _LoadDataBase(InSector);
                _column.ClearBoxes();
                DataBaseSaver.SaveAllCluster(Settings, (uint)InSector, Columns.ToArray());
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
                    foreach (IColumn t in Columns)
                    {
                        t.ClearBoxes();
                    }
                    DataBaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
                }
                DataBaseLoger.Log($"!Clear all base!");
                Settings.CountBuckets = 0;
            }
        }

        public virtual void RenameColumn(string name, string newName)
        {
            lock (Columns)
            {
                this[name].Name = newName;
                _myManager.SaveStatesDataBase(this);
            }
        }

        public virtual void RenameColumn(int name, string newName)
        {
            RenameColumn(Columns[name].Name, newName);
        }

        public virtual void RenameColumn(IColumn column, string newName)
        {
            RenameColumn(column.Name, newName);
        }

        #endregion

        #region Добавление/замена данных 

        /// <summary>
        /// Заменяет все элементы в указанном столбце на новые данные, довольно тяжелая операция 
        /// </summary>
        /// <param name="Params"></param>
        /// <param name="New"></param>
        /// <param name="SectorID"></param>
        /// <param name="ColumnName"></param>
        public virtual void ChangeEverythingTo(string ColumnName, string Params, string New, int SectorID = -1)
        {
            lock (Columns)
            {
                if (SectorID == -1)
                {
                    for (int i = 1; i < Settings.CountClusters; i++)
                    {
                        _LoadAndChengeDataInCluster(i, ColumnName, Params, New);
                    }
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
            _LoadDataBase(sector);
            foreach (IColumn t in Columns)
            {
                if (t.Name == ColumnName)
                {
                    var ids = t.FindIDs(Params);
                    foreach (var id in ids)
                    {
                        var itemData = new ItemData(id, New);
                        t.SetDataByID(itemData);
                        _SetDataInColumn?.Invoke(ColumnName, itemData);
                    }
                }
            }
            DataBaseSaver.SaveAllCluster(Settings, (uint)sector, Columns.ToArray());
        }


        /// <summary>
        /// Заменяет строку
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="datas"></param>
        public virtual void SetData(int ID, params string[] datas)
        {
            uint SectorID = GetSectorByID((uint)ID);

            ReplayesDataBySectorAndID(SectorID, ID, datas);
        }

        /// <summary>
        /// Заменяет строку c помошью DataLine 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="datas"></param>
        public virtual void SetData(IDataLine dataline)
        {
            SetData(dataline.ID, dataline.GetData());
        }

        public virtual void SetData<T>(T dataline) where T : IDataLine
        {
            SetData(dataline.ID, dataline.GetData());
        }

        public virtual void SetData<T>(int ID) where T : IDataLine
        {
            var t = Activator.CreateInstance<T>();
            SetData(ID, t.GetData());
        }

        /// <summary>
        /// Заменяет строку
        /// </summary>
        /// <param name="datas"></param>
        public virtual void SetData(params ItemData[] datas)
        {
            List<string> _datas = new List<string>();

            foreach (var d in datas)
            {
                _datas.Add(d.Data);
            }

            SetData(datas[0].ID, _datas.ToArray());
        }

        /// <summary>
        /// В необходимой табличке происходит добавление данных
        /// </summary>
        public virtual void SetDataInColumn(string ColumnName, int ID, string NewData)
        {
            SetDataInColumn(ColumnName, new ItemData(ID, NewData));           
        }
        /// <summary>
        /// В необходимой табличке происходит добавление данных в NewItemData укажите новые данные и id ячейки в которой нужно перезаписать данные
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="NewItemData"></param>
        public virtual void SetDataInColumn(string ColumnName, ItemData NewItemData)
        {
            lock (Columns)
            {
                uint SectorID = GetSectorByID((uint)NewItemData.ID);

                _LoadDataBase((int)SectorID);

                this[ColumnName].SetDataByID(NewItemData);

                DataBaseLoger.Log($"Set {NewItemData.Data} in {ColumnName} ID:{NewItemData.ID}");
                DataBaseSaver.SaveAllCluster(Settings, SectorID, Columns.ToArray());
                _SetDataInColumn?.Invoke(ColumnName, NewItemData);
            }
        }

        public virtual void SetDataInColumn(IColumn column, ItemData NewItemData)
        {
            SetDataInColumn(column.Name, NewItemData);
        }

        public virtual void SetDataInColumn(IColumn column, int ID, string NewData)
        {
            SetDataInColumn(column.Name, new ItemData(ID, NewData));
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

                if (Settings.Logs)
                {
                    string MSG = "Add data:";
                    for (int i = 0; i < datas.Length; i++)
                    {
                        MSG += datas[i] + "|";
                    }

                    DataBaseLoger.Log(MSG);
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
        public virtual void AddData(IDataLine dataline)
        {
            AddData(dataline.GetData());
        }

        /// <summary>
        /// Добавляет данные в таблицу, важно чтобы длина поступающего массива была равна кол-ву столбцов  
        /// Ошибки: Exception($"Длина поступивших данных меньше кол-ва столбцов: {Columns.Count}")
        /// </summary>
        /// <param name="datas"></param>
        public virtual void AddData(params object[] datas)
        {
            List<string> strings = new List<string>();

            foreach (var d in datas)
            {
                strings.Add(d.ToString());
            }

            AddData(strings.ToArray());
        }

        private void ReplayesDataBySectorAndID(uint SectorID, int ID, string[] datas)
        {
            lock (Columns)
            {
                _LoadDataBase((int)SectorID);

                List<ItemData> itemDatas = new List<ItemData>();
                for (int i = 0; i < this.Columns.Count; i++)
                {
                    Columns[i].SetDataByID(new ItemData(ID, datas[i]));
                    itemDatas.Add(new ItemData(ID, datas[i]));
                }

                Settings.CountBuckets += 1;
                DataBaseReplayser.ReplayesElement(Settings, SectorID, itemDatas.ToArray());

                DataBaseLoger.Log($"Replayes element in {SectorID} Cluster | {ID}");

                _AddData?.Invoke(datas, ID);
            }
        }
        /// <summary>
        /// Добавляет данные по сектору и id
        /// </summary>
        /// <param name="SectorID"></param>
        /// <param name="datas"></param>
        private void AddBySectorAndID(uint SectorID, int ID, string[] datas)
        {
            lock (Columns)
            {
                _LoadDataBase((int)SectorID);

                List<ItemData> itemDatas = new List<ItemData>();
                for (int i = 0; i < this.Columns.Count; i++)
                {
                    itemDatas.Add(new ItemData(ID, datas[i]));
                    Columns[i].Push(datas[i], (uint)ID);
                }
                Settings.CountBuckets += 1;
                DataBaseSaver.AddElement(Settings, SectorID, itemDatas.ToArray());

                DataBaseLoger.Log($"Add element in {SectorID} Cluster | {ID}");

                _AddData?.Invoke(datas, ID);
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
                DataBaseReplayser.ReplayesElement(Settings, SectorID, ItemDatas.ToArray());
                Settings.CountBuckets -= 1;

                DataBaseLoger.Log($"Remove element in {SectorID} Cluster | {ID}");

                _RemoveData?.Invoke(data, (int)ID);
            }
        }

        /// <summary>
        /// Удаляет все данные из базы подходящие по параметру. /*Хуйня какаето */
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
                        DataBaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());

                        string _datas = "";//для логирования

                        foreach (var d in datas)
                        {
                            _datas += d;
                        }

                        DataBaseLoger.Log($"Remove element in {i} Cluster | {_datas} ");

                        return true;
                    }
                    else
                    {
                        DataBaseSaver.SaveAllCluster(Settings, (uint)i, Columns.ToArray());
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
        public virtual bool RemoveAllData(IDataLine dataline)
        {
            return RemoveAllData(dataline.GetData());
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

        #region Сортировка/получение данных по параметрам
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
        /// <param name="ColumnName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual BaseLine[] GetAllDataInBaseByColumnName(string ColumnName, string data)
        {
            lock (Columns)
            {
                List<BaseLine> Boxes = new List<BaseLine>();

                for (int i = 1; i < Settings.CountClusters + 1; i++)
                {
                    _LoadDataBase(i);

                    int[] ids = this[ColumnName].FindIDs(data);

                    for (int j = 0; j < ids.Length; j++)
                    {
                        Boxes.Add(new BaseLine());
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
        public virtual BaseLine[] GetAllDataInBaseByColumnName(IColumn Column, string data)
        {
            return GetAllDataInBaseByColumnName(Column.Name, data);
        }

        /// <summary>
        /// По введенным параметрам ищет данные в БД
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns">Табличка параметр</param>
        /// <param name="searchTypes">способ поиска данных</param>
        /// <param name="Params">Данные от которых нужно операться</param>
        /// <param name="InSectro">Сектор в котором нужно искать данные(-1 - во всех)</param>
        /// <returns></returns>
        public virtual List<List<ItemData>> SmartSearch(IColumn[] columns, SearchType[] searchTypes, string[] Params, int InSectro = -1)
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
                    for (int i = 0; i < Settings.CountClusters; i++)
                    {
                        Boxes.Add(SmartSearch(columns, searchTypes, Params, i)[0]);
                    }
                }
                else
                {
                    _LoadDataBase(InSectro);
                    for (int j = 0; j < Params.Length; j++)
                    {
                        var _colomn = this[columns[j].Name];

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
                        foreach (var t in Columns)
                        {
                            data.Add(new ItemData(resultIDs[i], t.FindDataByID(resultIDs[i])));
                        }
                        Boxes.Add(data);
                    }
                }

                return Boxes;
            }
        }


        /// <summary>
        /// Ищет первый id элемента по параметрам, если IsSector = -1, то ищет везде 
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="Data"></param>
        /// <param name="InSector"></param>
        /// <returns></returns>
        public virtual int GetIDByParams(string ColumnName, string Data, int InSector = -1)
        {
            int result = -1;

            if (InSector == -1)
            {
                for (int i = 0; i < Settings.CountClusters; i++)
                {
                    result = GetIDByParams(ColumnName, Data, i);

                    if (result != -1)
                    {
                        break;
                    }
                }

            }
            else
            {
                _LoadDataBase(InSector);

                result = this[ColumnName].FindID(Data);
            }
            return result;
        }

        /// <summary>
        /// Ищет первый id элемента по параметрам, если IsSector = -1, то ищет везде 
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="Data"></param>
        /// <param name="InSector"></param>
        /// <returns></returns>
        public virtual int GetIDByParams(IColumn column, string Data, int InSector = -1)
        {
            return GetIDByParams(column.Name, Data, InSector);
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
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual ItemData[] GetItemsDataByID(int id)
        {
            List<ItemData> data = new List<ItemData>();
            foreach (var t in Columns)
            {
                data.Add(new ItemData(id, t.FindDataByID(id)));
            }
            return data.ToArray();
        }

        public virtual T GetDataLineByID<T>(int id) where T : IDataLine
        {
            var line = Activator.CreateInstance<T>();
            line.Init(id, GetDataByID(id));
            return line;
        }

        /// <summary>
        /// Ищет и возвращает первую строку подходящую под введенные параметры возврат через массив ячеек, если не находит => массив пустой
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="data"></param>
        /// <param name="InSector">Если -1, то ищет во всех сразу, иначе в загруженном</param>
        /// <returns></returns>
        public virtual ItemData[] GetDataInBaseByColumnName(string ColumnName, string data, int InSector = -1)
        {
            lock (Columns)
            {
                List<ItemData> _data = new List<ItemData>();

                bool Use = false;//маркер о том что поиск в {InSector} секторе уже был 

                for (int i = 0; i < Settings.CountClusters; i++)
                {
                    if (InSector != -1 && Use == true)
                    {
                        if (Use == true)
                        {
                            break;
                        }
                        else
                        {
                            Use = true;
                            _LoadDataBase(InSector);
                        }
                    }
                    else
                    {
                        _LoadDataBase(i);
                    }


                    int id = this[ColumnName].FindID(data);

                    if (id != -1)
                    {
                        foreach (IColumn table1 in Columns)
                        {
                            _data.Add(new ItemData(id, table1.FindDataByID(id)));
                        }
                        return _data.ToArray();
                    }

                }

                return _data.ToArray();
            }
        }

        public virtual ItemData GetDataByParams(string ColumnName, int ID)
        {
            var Sector = GetSectorByID((uint)ID);
            _LoadDataBase((int)Sector);
            return new ItemData(ID, this[ColumnName].FindDataByID(ID));
        }

        public virtual ItemData GetDataByParams(IColumn column, int ID)
        {
            return GetDataByParams(column.Name, ID);
        }
        #endregion

        #region Индексаторы
        public virtual IColumn this[string columnName]
        {
            get
            {
                lock (Columns)
                {
                    foreach (IColumn Column in Columns)
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

        public virtual IColumn this[int index]
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
    
        public void InitManager(DataBaseManager dataBaseManager) { _myManager = _myManager == null ? dataBaseManager : _myManager; }

        public void Dispose()
        {

        }
    }

}
