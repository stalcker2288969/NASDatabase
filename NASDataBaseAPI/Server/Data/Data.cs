using NASDataBaseAPI.Data;
using NASDataBaseAPI.Server.Data.DataBaseSettings;
using NASDataBaseAPI.Server.Data.Interfases;
using NASDataBaseAPI.SmartSearchSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace NASDataBaseAPI.Data
{
    public class DataBase
    {
        #region Events
        /// <summary>
        /// Где было удоление
        /// </summary>
        public event Action<int> _RemoveDataByID;
        /// <summary>
        /// что
        /// </summary>
        public event Action<string[]> _RemoveDataByData;
        /// <summary>
        /// что/куда
        /// </summary>
        public event Action<string[], int> _AddData;
        /// <summary>
        /// Имя
        /// </summary>
        public event Action<string> _AddTable;
        /// <summary>
        /// Имя
        /// </summary>
        public event Action<string> _RemoveTable;
        #endregion

        public List<Table> tables;

        public List<uint> freeIDs = new List<uint>();

        public DataBaseSettings settings;
        public IDataBaseSaver DataBaseSaver;
        public ILoger DataBaseLoger;
        
        public uint LoadedSector { get; private set; } = 0;

        private StringBuilder _stringBuilder = new StringBuilder();

        public DataBase(int countTables, DataBaseSettings settings)
        {
            this.tables = new List<Table>();
            this.settings = settings;
            for (int i = 0; i < countTables; i++)
            {
                tables.Add(new Table(i.ToString()));
            }
            
        }

        public DataBase(List<Table> tables, DataBaseSettings settings)
        {
            this.tables = tables;
            this.settings = settings;
            
        }

        #region Глобальное взаимодействое
        /// <summary>
        /// Получает айдишики все строк по параметрам
        /// </summary>
        /// <param name="nameTable"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int[] GetAllIDsByParams(string nameTable, string data)
        {
            List<int> IDs = new List<int>();

            for (int i = 0; i < settings.CountClusters; i++)
            {
                tables.Clear();
                tables.AddRange(DataBaseSaver.LoadCluster(settings.Path, (uint)i+1, settings.Key));

                for (int g = 0; g < this.tables.Count; g++)
                {
                    foreach (Table table in tables)
                    {
                        if (table.Name == nameTable)
                        {
                            IDs.AddRange(table.FindeIDs(data) ?? new int[0]);
                            DataBaseLoger.Log($"Get all IDs by {nameTable} and {data} in {i} cluester");
                            break;
                        }
                    }
                }
            }

            return IDs.ToArray();
        }

        /// <summary>
        /// Удоляет столбец
        /// </summary>
        /// <param name="TableName"></param>
        public void RemoveTable(string TableName)
        {
            for (int i = 0; i < settings.CountClusters; i++)
            {
                tables.Clear();
                tables.AddRange(DataBaseSaver.LoadCluster(settings.Path, (uint)i+1, settings.Key));
                foreach (Table t in tables)
                {
                    if (t.Name == TableName)
                    {
                        t.ClearBoxes();
                    }
                }
                DataBaseSaver.SaveAllCluster(settings, (uint)i, tables.ToArray());
            }

            DataBaseLoger.Log($"Delited table {TableName}");
            settings.TablesCount -= 1;
            _RemoveTable?.Invoke(TableName);           
        }

        /// <summary>
        /// Добавляет новый столбец. Процедура очень не продуктивная не рекаминдуется тк ее кпд O(n) 
        /// </summary>
        /// <param name="Name"></param>
        public void AddTable(string Name)
        {
            settings.TablesCount += 1;
            for (int i = 0; i < settings.CountClusters; i++)
            {
                tables.Clear();
                tables.AddRange(DataBaseSaver.LoadCluster(settings.Path, (uint)i+1, settings.Key));

                Table table = new Table(Name);//Новый столбец

                ItemData[] itemDatas = new ItemData[tables[0].GetCounts()];

                for (int g = 0; g < itemDatas.Length; g++)//связывает ивенд дестроя и столбец
                {
                    itemDatas[g] = new ItemData(g, " ");
                }

                table.SetDatas(itemDatas); //записываем пустые ячейки в новый столбец

                tables.Add(table);                
                DataBaseSaver.SaveAllCluster(settings, (uint)i, tables.ToArray());
            }

            DataBaseLoger.Log($"Add table {Name}");           
            _AddTable?.Invoke(Name);           
        }

        /// <summary>
        /// Добавляет столбик и задет тип данных в столбике
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="dataType"></param>
        public void AddTable(string Name, DataType dataType)
        {
            settings.TablesCount += 1;
            for (int i = 0; i < settings.CountClusters; i++)
            {
                tables.Clear();
                tables.AddRange(DataBaseSaver.LoadCluster(settings.Path, (uint)i + 1, settings.Key));

                Table table = new Table(Name, dataType, tables[0].OffSet);//Новый столбец

                ItemData[] itemDatas = new ItemData[tables[0].GetCounts()];

                for (int g = 0; g < itemDatas.Length; g++)//связывает ивенд дестроя и столбец
                {
                    itemDatas[g] = new ItemData(g, " ");
                }

                table.SetDatas(itemDatas); //записываем пустые ячейки данные в новый столбец

                tables.Add(table);               
                DataBaseSaver.SaveAllCluster(settings, (uint)i, tables.ToArray());
            }
            DataBaseLoger.Log($"Add table {Name}|{dataType}");
            _AddTable?.Invoke(Name);           
        }

        /// <summary>
        /// Копирует данные из левого столбца в правый
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public void CloneTo(Table left, Table right)
        {
            for (int i = 1; i < settings.CountClusters; i++)
            {
                tables.Clear();
                tables.AddRange(DataBaseSaver.LoadCluster(settings.Path, (uint)i, settings.Key));

                ItemData[] itemDatas = left.GetDatas();
                right.SetDatas(itemDatas);
                DataBaseSaver.SaveAllCluster(settings, (uint)i, tables.ToArray());
            }
            DataBaseLoger.Log($"{left.Name} clone to {right.Name}");
        }
        /// <summary>
        /// Чистит всю базу / не производительная команда
        /// </summary>
        public void ClearAllBase()
        {
            for (int i = 1; i < settings.CountClusters; i++)
            {
                tables.Clear();
                tables.AddRange(DataBaseSaver.LoadCluster(settings.Path, (uint)i, settings.Key));

                foreach (Table t in tables)
                {
                    t.ClearBoxes();
                }
                DataBaseSaver.SaveAllCluster(settings, (uint)i, tables.ToArray());
            }
            DataBaseLoger.Log($"!Clear all base!");
            settings.CountBuckets = 0;
        }
        #endregion

        #region Добавление/замена данных 
        /// <summary>
        /// В необходимой табличке происходит добавление данных в NewItemData укажите новые данные и id ячейки в которой нужно перезаписать данные
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="NewItemData"></param>
        public void SetDataInTable(string tableName, ItemData NewItemData)
        {

            uint SectorID = ((uint)NewItemData.IDInTable / settings.CountBucketsInSector) + 1;

            if (SectorID != LoadedSector)
            {
                tables.Clear();
                tables.AddRange(DataBaseSaver.LoadCluster(settings.Path, SectorID, settings.Key));
                LoadedSector = SectorID;
            }

            for (int i = 0; i < this.tables.Count; i++)
            {
                if (tables[i].Name == tableName)
                {
                    tables[i].SetDataByID(NewItemData); break;
                }
            }
            DataBaseLoger.Log($"Set {NewItemData.Data} in {tableName} ID:{NewItemData.IDInTable}");
            DataBaseSaver.SaveAllCluster(settings, SectorID, tables.ToArray());
        }
        /// <summary>
        /// Добавляет данные в таблюцу, важно чтобы длина поступающего массива была равна кол-ву столбцов
        /// </summary>
        /// <param name="datas"></param>
        public void AddData(string[] datas)
        {
            if (datas.Length == tables.Count)
            {
                if (freeIDs.Count == 0)
                {
                    uint SectorID = (settings.CountBuckets / settings.CountBucketsInSector) + 1;//Опредиляем к каому сектору обратиться
                    AddBySectorAndID(SectorID, (int)settings.CountBuckets, datas);
                }
                else
                {
                    uint FreeID = freeIDs[0];
                    freeIDs.Remove(FreeID);
                    uint SectorID = (FreeID / settings.CountBucketsInSector) + 1;
                    ReplayesDataBySectorAndID(SectorID, (int)FreeID, datas);
                }

                string MSG = "Add data:";
                for (int i = 0; i < datas.Length; i++)
                {
                    MSG += datas[i] + "|";
                }

                DataBaseLoger.Log(MSG);
            }
            else
            {
                throw new Exception($"Длина поступивших данных меньше кол-ва столбцов: {tables.Count}");
            }
        }

        private void ReplayesDataBySectorAndID(uint SectorID, int ID, string[] datas)
        {
            if (SectorID != LoadedSector)
            {
                tables.Clear();
                tables.AddRange(DataBaseSaver.LoadCluster(settings.Path, SectorID, settings.Key));
                LoadedSector = SectorID;
            }

            List<ItemData> itemDatas = new List<ItemData>();
            for (int i = 0; i < this.tables.Count; i++)
            {
                tables[i].SetDataByID(new ItemData(ID, datas[i]));
                itemDatas.Add(new ItemData(ID, datas[i]));
            }
  
            settings.CountBuckets += 1;
            DataBaseSaver.ReplayesElement(settings, SectorID, itemDatas.ToArray());

            DataBaseLoger.Log($"Replayes element in {SectorID} Cluster | {ID}");

            _AddData?.Invoke(datas, ID);
        }
        /// <summary>
        /// Добавляет данные по сектору и id
        /// </summary>
        /// <param name="SectorID"></param>
        /// <param name="datas"></param>
        private void AddBySectorAndID(uint SectorID, int ID, string[] datas)
        {
            if (SectorID != LoadedSector)
            {
                tables.Clear();
                tables.AddRange(DataBaseSaver.LoadCluster(settings.Path, SectorID, settings.Key));
                LoadedSector = SectorID;
            }

            List<ItemData> itemDatas = new List<ItemData>();
            for(int i = 0; i < this.tables.Count; i++)
            {
                itemDatas.Add(new ItemData(ID, datas[i]));
                tables[i].Push(datas[i], ID);
            }
            settings.CountBuckets += 1;
            DataBaseSaver.AddElement(settings, SectorID, itemDatas.ToArray());

            DataBaseLoger.Log($"Add element in {SectorID} Cluster | {ID}");

            _AddData?.Invoke(datas, ID);
        }
        #endregion

        #region Удоление данных
        /// <summary>
        /// Удоляет данные по введенному ID
        /// </summary>
        /// <param name="ID"></param>
        public void RemoveDataByID(uint ID)
        {
            uint SectorID = (ID / settings.CountBucketsInSector) + 1;
            List<ItemData> ItemDatas = new List<ItemData>();
            if (SectorID != LoadedSector)
            {
                tables.Clear();
                tables.AddRange(DataBaseSaver.LoadCluster(settings.Path, SectorID, settings.Key));
            }
            for (int i = 0; i < this.tables.Count; i++)
            {
                tables[i].SetDataByID(new ItemData((int)ID, " "));
                ItemDatas.Add(new ItemData((int)ID, " "));
            }
            freeIDs.Add(ID);
            DataBaseSaver.ReplayesElement(settings, SectorID, ItemDatas.ToArray());
            settings.CountBuckets -= 1;

            DataBaseLoger.Log($"Remove element in {SectorID} Cluster | {ID}");

            _RemoveDataByID?.Invoke((int)ID);
        }

        /// <summary>
        /// Очуне(!ОЧЕНЬ!) долгая процедура => не рекомендуется  
        /// </summary>
        /// <param name="datas"></param>
        public bool RemoveData(string[] datas)
        {
            bool result = false;
            if (datas.Length == tables.Count)
            {
                for (int i = 0; i < settings.CountClusters; i++)
                {
                    tables.Clear();
                    tables.AddRange(DataBaseSaver.LoadCluster(settings.Path, (uint)i, settings.Key));
                    List<bool> bools = new List<bool>();

                    for (int g = 0; g < this.tables.Count; g++)
                    {
                        if (tables[g].Pop(datas[g]))
                        {
                            bools.Add(true);
                        }
                    }
                    if (bools.Count == tables.Count)
                    {
                        settings.CountBuckets -= 1;
                        DataBaseSaver.SaveAllCluster(settings, (uint)i, tables.ToArray());

                        string _datas = "";//для логирования

                        foreach(var d in datas)
                        {
                            _datas += d;
                        }

                        DataBaseLoger.Log($"Remove element in {i} Cluster | {_datas} ");

                        _RemoveDataByData?.Invoke(datas);
                        return true;
                    }
                }                
            }
            return result;
        }
        #endregion

        #region Сортировка/получение данных по параметрам
        /// <summary>
        /// Отображает загруженный сектор в память. Если происходит ошибка возврат " " 
        /// </summary>
        /// <returns></returns>
        public string PrintBase()
        {
            try
            {
                lock (_stringBuilder)
                {
                    int l = tables[0].GetCounts();

                    _stringBuilder.Append("Tables names:\n---------------------------------------------\n");
                    _stringBuilder.Append("% Number % | ");
                    for (int g = 0; g < tables.Count; g++)
                    {
                        _stringBuilder.Append("% " + tables[g].Name + " % | ");
                    }
                    _stringBuilder.Append("\n---------------------------------------------\n");

                    for (int id = 0; id < l; id++)
                    {
                        _stringBuilder.Append(id.ToString() + " | ");
                        for (int g = 0; g < tables.Count; g++)
                        {
                            _stringBuilder.Append(tables[g].FindeDataByID(id) + " | ");
                        }

                        _stringBuilder.Append("\n---------------------------------------------\n");
                    }

                    string Text = _stringBuilder.ToString();
                    _stringBuilder.Clear();
                    return Text;
                }
            }
            catch
            {
                return " ";
            }
        }

        /// <summary>
        /// Сканирует всю БД в поисках подходящих строк / тоже не самый подходящий способ для скана данных 
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<List<ItemData>> GetAllDataInBaseByTableName(string TableName, string data)
        {
            List<List<ItemData>> Boxes = new List<List<ItemData>>();

            for (int i = 1; i < settings.CountClusters + 1; i++)
            {
                tables.Clear();
                tables.AddRange(DataBaseSaver.LoadCluster(settings.Path, (uint)i, settings.Key));


                int[] id = new int[0];

                foreach (Table table in tables)
                {
                    if (table.Name == TableName)
                    {
                        id = table.FindeIDs(data);
                        break;
                    }
                }

                for (int g1 = 0; g1 < id.Length; g1++)
                {
                    Boxes.Add(new List<ItemData>());
                    foreach (var t in tables)
                    {
                        Boxes[g1].Add(new ItemData(id[g1], t.FindeDataByID(id[g1])));
                    }
                }

            }

            return Boxes;
        }

        /// <summary>
        /// По введенным параметрам ищет данные в БД
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Tables">Табличка параметр</param>
        /// <param name="searchTypes">способ поиска данных</param>
        /// <param name="Params">Данные от которых нужно операться</param>
        /// <param name="InSectro">Сектор в котором нужно искать данные(-1 - во всех)</param>
        /// <returns></returns>
        public List<List<ItemData>> SmartSearch(Table[] Tables, SearchType[] searchTypes, string[] Params, int InSectro = -1)
        {
            List<List<ItemData>> Boxes = new List<List<ItemData>>();
            List<List<int>> Search = new List<List<int>>();
            List<int> resultIDs = new List<int>();

            if (Tables.Length != searchTypes.Length && Tables.Length != Params.Length)
                return null;

            if (InSectro == -1)
            {
                for (int i = 0; i < settings.CountClusters; i++)
                {
                    tables.Clear();
                    tables.AddRange(DataBaseSaver.LoadCluster(settings.Path, (uint)i, settings.Key));

                    Boxes.Add(SmartSearch(Tables, searchTypes, Params, i)[0]);
                }
            }
            else
            {
                tables.Clear();
                tables.AddRange(DataBaseSaver.LoadCluster(settings.Path, (uint)InSectro, settings.Key));

                for (int g = 0; g < Params.Length; g++)
                {
                    for (int i = 0; i < tables.Count; i++)
                    {
                        if (Tables[g] == tables[i])
                        {
                            List<int> IDs = new List<int>();
                            IDs = new SmartSearcher(Tables[g], tables[i], searchTypes[g], Params[g]).Search();
                            Search.Add(IDs);
                        }
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
                    foreach (var t in tables)
                    {
                        data.Add(new ItemData(resultIDs[i], t.FindeDataByID(resultIDs[i])));
                    }
                    Boxes.Add(data);
                }

            }
            return Boxes;
        }

        public string[] GetDataByID(int ID)
        {
            List<string> strings = new List<string>();
            foreach(var t in tables)
            {
                strings.Add(t.FindeDataByID(ID));
            }
            return strings.ToArray();
        }

        /// <summary>
        /// Ищит и возвращает первую строку подходящую под введенные параметры возврат через массив ячеек, если ненаходит => массив пустой
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ItemData[] GetDataInBaseByTableName(string TableName, string data)
        {
            List<ItemData> Boxes = new List<ItemData>();
            for (int i = 0; i < settings.CountClusters; i++)
            {
                tables.Clear();
                tables.AddRange(DataBaseSaver.LoadCluster(settings.Path, (uint)i, settings.Key));

                int id = 0;
                foreach (Table table in tables)
                {
                    if (table.Name == TableName)
                    {
                        id = table.FindeID(data);
                        if (id != -1)
                        {
                            foreach (Table table1 in tables)
                            {
                                Boxes.Add(new ItemData(id, table1.FindeDataByID(id)));
                            }
                            return Boxes.ToArray();
                        }
                        break;
                    }
                }
            }

            return null;
        }
#endregion



        public Table this[string index]
        {
            get
            {
                foreach (Table table in tables)
                {
                    if (table.Name == index)
                    {
                        return table;
                    }
                }
                return null;
            }
            set
            {
                for (int i = 0; i < tables.Count; i++)
                {
                    if (tables[i].Name == index)
                    {
                        tables[i] = value; break;
                    }
                }
            }
        }

        public Table this[int index]
        {
            get
            {               
                return tables[index];
            }
            set
            {
                tables[index] = value; 
            }
        }
    }
}
