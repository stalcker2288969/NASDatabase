using NASDataBaseAPI.Server.Data.Modules.Handlers;
using NASDataBaseAPI.Interfaces;
using System;
using System.Collections.Generic;

namespace NASDataBaseAPI.Server.Data.Modules
{
    /// <summary>
    /// Предостовляет удобный интерфейс связи баз данных 
    /// </summary>
    public class Connector<T1,T2> : IConnector<T1, T2> where T1 : DataBase where T2 : DataBase
    {
        public T1 DB1 { get; protected set; }
        public T2 DB2 { get; protected set; }

        protected List<Handler<T1,T2>> handlers = new List<Handler<T1,T2>>();

        private event Action<object, object> _OnAddData;
        private event Action<object, object> _OnRemoveData;
        private event Action<object, object> _OnRemoveDataByData;
        private event Action<object, object> _OnAddColumn;
        private event Action<object, object> _OnRemoveColumn;
        private event Action<object, object> _OnLoadedNewSector;
        private event Action<object, object> _OnCloneColumn;
        private event Action<object, object> _OnClearAllColumn;
        private event Action<object, object> _OnClearAllBase;
        private event Action<object, object> _OnRenameColumn;
        private event Action<object, object> _OnSetDataInColumn;

        public Connector(T1 DB1, T2 DB2)
        {
            if(DB1 == DB2)
            {
                throw new ArgumentException("Переданные базы данные равны, что не допустимо во избежание ошибок!");
            }

            this.DB1 = DB1;
            this.DB2 = DB2;

            DB1._AddData += OnAddData;
            DB1._RemoveData += OnRemoveData;
            DB1._RemoveDataByData += OnRemoveDataByData;
            DB1._AddColumn += OnAddColumn;
            DB1._RemoveColumn += OnRemoveColumn;
            DB1._LoadedNewSector += OnLoadedNewSector;
            DB1._CloneColumn += OnCloneColumn;
            DB1._ClearAllColumn += OnClearAllColumn;
            DB1._RenameColumn += OnRenameColumn;
            DB1._ClearAllBase += OnClearBase;
            DB1._SetDataInColumn += OnSetDataInColumn;
        }


        public virtual void AddConectionByHandler(Handler<T1, T2> Handler)
        {
            Handler.Init(DB1, DB2);
            handlers.Add(Handler);

            switch (Handler.Type)
            {
                case DataBaseEventType.AddData:                    
                    _OnAddData += Handler.Work;
                    break;
                case DataBaseEventType.RemoveData:
                    _OnRemoveData += Handler.Work;
                    break;
                case DataBaseEventType.RemoveDataByData:
                    _OnRemoveDataByData += Handler.Work;
                    break;
                case DataBaseEventType.AddColumn:
                    _OnAddColumn += Handler.Work;
                    break;
                case DataBaseEventType.RemoveColumn:
                    _OnRemoveColumn += Handler.Work;
                    break;
                case DataBaseEventType.LoadedNewSector:
                    _OnLoadedNewSector += Handler.Work;
                    break;
                case DataBaseEventType.CloneColumn:
                    _OnCloneColumn += Handler.Work;
                    break;
                case DataBaseEventType.ClearAllColumn:
                    _OnClearAllColumn += Handler.Work;
                    break;
                case DataBaseEventType.ClearAllBase:
                    _OnClearAllBase += Handler.Work;
                    break;
                case DataBaseEventType.RenamedColumn:
                    _OnRenameColumn += Handler.Work;
                    break;
                case DataBaseEventType.SetDataInColumn:
                    _OnSetDataInColumn += Handler.Work;
                    break;
            }
        }
        
        public virtual void DestroyConectionByHandler(Handler<T1, T2> Handler)
        {
            switch (Handler.Type)
            {
                case DataBaseEventType.AddData:
                    _OnAddData -= Handler.Work;
                    break;
                case DataBaseEventType.RemoveData:
                    _OnRemoveData -= Handler.Work;
                    break;
                case DataBaseEventType.RemoveDataByData:
                    _OnRemoveDataByData -= Handler.Work;
                    break;
                case DataBaseEventType.AddColumn:
                    _OnAddColumn -= Handler.Work;
                    break;
                case DataBaseEventType.RemoveColumn:
                    _OnRemoveColumn -= Handler.Work;
                    break;
                case DataBaseEventType.LoadedNewSector:
                    _OnLoadedNewSector -= Handler.Work;
                    break;
                case DataBaseEventType.CloneColumn:
                    _OnCloneColumn -= Handler.Work;
                    break;
                case DataBaseEventType.ClearAllColumn:
                    _OnClearAllColumn -= Handler.Work;
                    break;
                case DataBaseEventType.ClearAllBase:
                    _OnClearAllBase -= Handler.Work;
                    break;
                case DataBaseEventType.RenamedColumn:
                    _OnRenameColumn -= Handler.Work;
                    break;
                case DataBaseEventType.SetDataInColumn:
                    _OnSetDataInColumn -= Handler.Work; 
                    break;
            }
            Handler.OnDestory();
            handlers.Remove(Handler);
        }

        #region Реакция на изменения
        private void OnSetDataInColumn(string arg1, ItemData data)
        {
            _OnSetDataInColumn?.Invoke(arg1, data);
        }

        protected void OnAddData(string[] datas, int id)
        {
            _OnAddData?.Invoke(datas,id);
        }

        protected void OnRemoveData(string[] datas, int id)
        {
            _OnRemoveData?.Invoke(datas, id);
        }

        protected void OnRemoveDataByData(string[] datas)
        {
            _OnRemoveDataByData?.Invoke(datas, "");
        }

        protected void OnAddColumn(string name)
        {
            _OnAddColumn?.Invoke(name, "");
        }

        protected void OnRemoveColumn(string name)
        {
            _OnRemoveColumn?.Invoke(name, "");
        }

        protected void OnLoadedNewSector(int sector)
        {
            _OnLoadedNewSector?.Invoke(sector, "");
        }

        protected void OnCloneColumn(string name, string name2)
        {
            _OnCloneColumn?.Invoke(name, name2);
        }

        protected void OnClearAllColumn(string name, int sector)
        {
            _OnClearAllColumn(name, sector);
        }

        protected void OnClearBase()
        {
            _OnClearAllBase?.Invoke(null, null); 
        }

        protected void OnRenameColumn(string name, string newName)
        {
            _OnRenameColumn?.Invoke(name, newName);
        }
        #endregion

        ~Connector()
        {
            DB1._AddData -= OnAddData;
            DB1._RemoveData -= OnRemoveData;
            DB1._RemoveDataByData -= OnRemoveDataByData;
            DB1._AddColumn -= OnAddColumn;
            DB1._RemoveColumn -= OnRemoveColumn;
            DB1._LoadedNewSector -= OnLoadedNewSector;
            DB1._CloneColumn -= OnCloneColumn;
            DB1._ClearAllColumn -= OnClearAllColumn;
            DB1._RenameColumn -= OnRenameColumn;
            DB1._ClearAllBase -= OnClearBase;
            DB1._SetDataInColumn -= OnSetDataInColumn;

            if (handlers.Count > 0)
            {
                for(int i = 0; i < handlers.Count; i++)
                {
                    DestroyConectionByHandler(handlers[i]);
                }           
            }
        }
    }
}
