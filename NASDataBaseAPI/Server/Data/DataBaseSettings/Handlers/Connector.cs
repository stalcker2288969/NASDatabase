using NASDataBaseAPI.Server.Data.DataBaseSettings.Handlers;
using NASDataBaseAPI.Server.Data.Interfases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.DataBaseSettings
{
    /// <summary>
    /// Предостовляет удобный интерфейс связи баз данных 
    /// </summary>
    public class Connector : IConnector<DataBase>
    {
        public DataBase DB1 { get; protected set; }
        public DataBase DB2 { get; protected set; }

        protected List<Handler<DataBase>> handlers = new List<Handler<DataBase>>();

        private event Action<object, object> _OnAddData;
        private event Action<object, object> _OnRemoveData;
        private event Action<object, object> _OnRemoveDataByData;
        private event Action<object, object> _OnAddColumn;
        private event Action<object, object> _OnRemoveColumn;
        private event Action<object, object> _OnLoadedNewSector;
        private event Action<object, object> _OnCloneColumn;
        private event Action<object, object> _OnClearAllColumn;

        public Connector(DataBase DB1, DataBase DB2)
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
        }

        public virtual void AddConectionByHandler(Handler<DataBase> Handler)
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
            }
        }
        
        public virtual void DestroyConectionByHandler(Handler<DataBase> Handler)
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
            }
            Handler.OnDestory();
            handlers.Remove(Handler);
        }

        #region Реакция на изменения

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

        protected void OnClearAllColumn(string name)
        {
            _OnClearAllColumn(name, "");
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
