using NASDataBaseAPI.Client.Utilities;
using NASDataBaseAPI.Data.DataTypesInColumn;
using NASDataBaseAPI.Server.Data;
using NASDataBaseAPI.Server.Data.DataBaseSettings;
using NASDataBaseAPI.Interfaces;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Client
{
    /// <summary>
    /// Класс для работы с сервером/DataBaseSaver, DataBaseReplayser, DataBaseLoader, DataBaseLoger - не работают
    /// </summary>
    public class Client : DataBase
    {
        #region Ошибки
        public const string NoRightsExceptionText = "Отказ в действии! ";
        public const string ErrorOnServerExceptionText = "На сервере произошла ошибка, команда не может быть выполнена! ";
        public const string IncorrectDataWasReceivedFromServerExceptionText = "Были получены некорректные данные с сервера! "; 
        #endregion

        #region События 
        public Action Disconnect { protected get; set; }
        #endregion

        public string Name { get; set; }
        public string Password { get; set; }
        public IDataConverter DataConverter { get; set; } = new DataConverter();
        public ICommandWorker Worker { get; private set; }

        public Client(string Name, string Password) : base(0,default(DataBaseSettings))
        {
            this.Name = Name;
            this.Password = Password;
        }

        private Client(int countColumn, DataBaseSettings settings, int loadedSector = 1) : base(countColumn, settings, loadedSector) { }

        #region Глобальное взаимодействие

        public DataBaseSettings LoadDataBaseSettings()
        {
            Worker.Push(BaseCommands.LoadDataBaseState);
            return JsonSerializer.Deserialize<DataBaseSettings>(NOTIFICATION());
        }

        public void LoadDataBaseColumnsState()
        {
            Worker.Push(BaseCommands.LoadDataBaseColumnsState);
            string msg = NOTIFICATION();
            var d = msg.Split(BaseCommands.SEPARATION.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            lock (Columns)
            {
                Columns.Clear();

                foreach (var d2 in d)
                {
                    Columns.Add(DataConverter.GetColumn<Column>(d2));
                }
            }           
        }

        public override void ChengTypeInColumn(string NameColumn, DataType NewDataType)
        {
            Worker.Push(BaseCommands.ChengTypeInColumn + BaseCommands.SEPARATION + NameColumn + BaseCommands.SEPARATION + NewDataType.GetType().Name);
            var msg = NOTIFICATION(); 
        }

        public override int[] GetAllIDsByParams(int NumberColumn, string data)
        {
            return GetAllIDsByParams(this[NumberColumn].Name, data);
        }

        public override int[] GetAllIDsByParams(string ColumnName, string data)
        {
            Worker.Push(BaseCommands.GetAllIDsByParams + BaseCommands.SEPARATION +
                ColumnName + BaseCommands.SEPARATION + data);
            
            return DataConverter.GetInts(NOTIFICATION());
        }

        public override void AddColumn(string Name)
        {
            Worker.Push(BaseCommands.AddColumn + BaseCommands.SEPARATION + Name 
                + BaseCommands.SEPARATION + DataTypesInColumns.Text.GetType().Name);
            
            NOTIFICATION();
            _AddColumn?.Invoke(Name);
        }

        public override void AddColumn(string Name, DataType dataType)
        {
            Worker.Push(BaseCommands.AddColumn + BaseCommands.SEPARATION + Name
                + BaseCommands.SEPARATION + dataType.GetType().Name);
            NOTIFICATION();
            _AddColumn?.Invoke(Name);
        }

        public override void AddColumn(IColumn column)
        {
            AddColumn(column.Name);
        }

        public override void RemoveColumn(string ColumnName)
        {
            Worker.Push(BaseCommands.RemoveColumn + BaseCommands.SEPARATION + ColumnName);
            NOTIFICATION();
            _RemoveColumn?.Invoke(ColumnName);
        }

        public override void RemoveColumn(int NumberOFColumn)
        {
            Worker.Push(BaseCommands.RemoveColumn + BaseCommands.SEPARATION + this[NumberOFColumn].Name);
            NOTIFICATION();
            _RemoveColumn?.Invoke(this[NumberOFColumn].Name);
        }

        public override void RemoveColumn(IColumn ColumnName)
        {
            RemoveColumn(ColumnName.Name);
        }

        public override void CloneTo(IColumn left, IColumn right)
        {
            CloneTo(left.Name, right.Name);
        }

        public override void CloneTo(string left, string right)
        {
            Worker.Push(BaseCommands.CloneColumn + BaseCommands.SEPARATION + left
                + BaseCommands.SEPARATION + right);
            NOTIFICATION();
            _CloneColumn?.Invoke(left, right);
        }

        public override void ClearAllColumn(IColumn column, int InSector = -1)
        {
            ClearAllColumn(column.Name, InSector);
        }

        public override void ClearAllColumn(string ColumnName, int InSector = -1)
        {
            Worker.Push(BaseCommands.ClearColumn + BaseCommands.SEPARATION + ColumnName + BaseCommands.SEPARATION + InSector);
            NOTIFICATION();
            _ClearAllColumn?.Invoke(ColumnName, InSector);
        }

        public override void ClearAllBase()
        {
            Worker.Push(BaseCommands.ClearAllBase);
            NOTIFICATION();
            _ClearAllBase?.Invoke();
        }

        public override void RenameColumn(IColumn column, string newName)
        {
            RenameColumn(column.Name, newName);
        }

        public override void RenameColumn(int n, string newName)
        {           
            RenameColumn(this[n].Name, newName);  
        }

        public override void RenameColumn(string name, string newName)
        {
            Worker.Push(BaseCommands.RenameColumn + BaseCommands.SEPARATION + name + BaseCommands.SEPARATION + newName);
            NOTIFICATION();
            _RenameColumn?.Invoke(name, newName);
        }

        #endregion

        #region Добавление/замена данных 

        public override void ChangeEverythingTo(string ColumnName, string Params, string New, int SectorID = -1)
        {
            Worker.Push(BaseCommands.ChangeEverythingTo + BaseCommands.SEPARATION + 
                ColumnName + BaseCommands.SEPARATION + Params + BaseCommands.SEPARATION +
                New + BaseCommands.SEPARATION + SectorID);
            NOTIFICATION();           
        }

        public override void SetData(int ID, params string[] datas)
        {
            var bl = new BaseLine();
            bl.Init(ID, datas);

            Worker.Push(BaseCommands.SetData + BaseCommands.SEPARATION + DataConverter.ParsDataLine(bl));
            NOTIFICATION();
        }

        public override void SetDataInColumn(string ColumnName, ItemData itemData)
        {
            Worker.Push(BaseCommands.SetDataInColumn + BaseCommands.SEPARATION + ColumnName + BaseCommands.SEPARATION + 
               DataConverter.ParsItemData(itemData));
            
            NOTIFICATION(); 
        }

        public override void AddData(params string[] datas)
        {
            var bl = new BaseLine();
            bl.Init(-1, datas);

            Worker.Push(BaseCommands.AddData + BaseCommands.SEPARATION + DataConverter.ParsDataLine(bl));

            var msg = NOTIFICATION();

            try
            {
                _AddData?.Invoke(datas, int.Parse(msg));
            }
            catch 
            {
                throw new Exception(IncorrectDataWasReceivedFromServerExceptionText);
            }
        }
        #endregion
        
        #region Удаление данных
        public override void RemoveDataByID(int ID)
        {
            Worker.Push(BaseCommands.GetDataByID + BaseCommands.SEPARATION + ID);
            var dl = DataConverter.GetDataLine<BaseLine>(NOTIFICATION());   

            Worker.Push(BaseCommands.RemoveDataByID + BaseCommands.SEPARATION + ID);
            
            NOTIFICATION();
            _AddData?.Invoke(dl.GetData(), ID);
        }

        public override bool RemoveAllData(params string[] datas)
        {
            var bl = new BaseLine();
            bl.Init(-1, datas);
            Worker.Push(BaseCommands.RemoveAllData + BaseCommands.SEPARATION + DataConverter.ParsDataLine(bl));
            var msg = NOTIFICATION();
            
            try
            {
                return bool.Parse(msg);
            }
            catch
            {
                throw new Exception(IncorrectDataWasReceivedFromServerExceptionText);
            }
        }

        public override void RemoveDatasByIDs(int[] IDs)
        {
            base.RemoveDatasByIDs(IDs);
        }
        #endregion

        #region Сортировка/получение данных по параметрам

        public override string PrintBase()
        {
            Worker.Push(BaseCommands.PrintBase);
            return NOTIFICATION(); 
        }

        public override BaseLine[] GetAllDataInBaseByColumnName(string ColumnName, string data)
        {
            Worker.Push(BaseCommands.GetAllDataInBaseByColumnName + BaseCommands.SEPARATION +
                ColumnName + BaseCommands.SEPARATION + data);
            var msg = NOTIFICATION();

            return DataConverter.GetDataLines<BaseLine>(msg);
        }

        public override int GetIDByParams(string ColumnName, string Data, int InSector = -1)
        {
            Worker.Push(BaseCommands.GetIDByParams + BaseCommands.SEPARATION + ColumnName + BaseCommands.SEPARATION +
                Data + BaseCommands.SEPARATION + InSector);
            var msg = NOTIFICATION();
            return int.Parse(msg);
        }

        public override string[] GetDataByID(int ID)
        {
            Worker.Push(BaseCommands.GetDataByID + BaseCommands.SEPARATION + ID);
            return DataConverter.GetDataLine<BaseLine>(NOTIFICATION()).GetData();
        }

        #endregion

        public void ConnectTo<T>(string IP, int Port) where T : ClientCommandsWorker
        {
            Worker = (T)typeof(T).GetConstructor(new Type[]
            { typeof(string), typeof(string), typeof(string), typeof(string) }).Invoke(new object[] 
            { IP, Port.ToString(), Name, Password });

            Worker.Push(BaseCommands.Login + BaseCommands.SEPARATION + Name + BaseCommands.SEPARATION + Password);
            string response = NOTIFICATION();
            

            if (response == BaseCommands.Disconnect || response != BaseCommands.Connect)
            {
                throw new Exception($"Не удалось подключиться к БД! IP: {IP}, Port: {Port}, Name: {Name}, Password: {Password}");
            }

            //Task.Run(() => HandlerCommandsFromServer());

            Settings = LoadDataBaseSettings();
            LoadDataBaseColumnsState();
        }

        public void CloseConnection()
        {           
            Worker.Dispose();
            Disconnect?.Invoke();
        }
        /// <summary>
        /// Отправляет сообщение на сервер
        /// </summary>
        /// <param name="Command"></param>
        public void PushCastomCommand(string Command)
        {
            Worker.Push(BaseCommands.MSG + BaseCommands.SEPARATION + Command);
        }

        public override IColumn this[string index]
        {
            get
            {
                LoadDataBaseColumnsState();
                return (IColumn)base[index];
            }
            protected set
            {
                LoadDataBaseColumnsState();
                base[index] = value;
            }
        }

        public override IColumn this[int index]
        {
            get
            {
                LoadDataBaseColumnsState();
                return (IColumn)base[index];
            }
            protected set
            {
                LoadDataBaseColumnsState();
                base[index] = value;
            }
        }

        private string NOTIFICATION()
        {
            string res = Worker.Listen();
            if (res.Split(BaseCommands.SEPARATION.ToCharArray())[0] == BaseCommands.HAVENOTPERM)
            {
                throw new ArgumentException(NoRightsExceptionText + res.Split(BaseCommands.SEPARATION.ToCharArray())[1]);
            }
            else if (res.Split(BaseCommands.SEPARATION.ToCharArray())[0] == BaseCommands.ERROR)
            {
                throw new ArgumentException(ErrorOnServerExceptionText
                    + res.Split(BaseCommands.SEPARATION.ToCharArray())[1]);
            }

            return res;
        }

        private void HandlerCommandsFromServer()
        {
            
        }
    }
}
