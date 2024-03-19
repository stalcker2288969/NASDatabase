using NASDatabase.Client.Utilities;
using NASDatabase.Data.DataTypesInColumn;
using NASDatabase.Server.Data;
using NASDatabase.Server.Data.DatabaseSettings;
using NASDatabase.Interfaces;
using System;
using System.Text.Json;
using NASDatabase.Server;
using NASDatabase.Client.Handleres.Base;
using System.Text;
using NASDatabase.Data;

namespace NASDatabase.Client
{
    /// <summary>
    /// Класс для работы с сервером/DataBaseSaver, DataBaseReplayser, DataBaseLoader, DataBaseLoger - не работают
    /// </summary>
    public class Client : Database
    {
        #region Ошибки
        public const string NoRightsExceptionText = "Отказ в действии! ";
        public const string ErrorOnServerExceptionText = "На сервере произошла ошибка, команда не может быть выполнена! ";
        public const string IncorrectDataWasReceivedFromServerExceptionText = "Были получены некорректные данные с сервера! "; 
        #endregion

        #region События 
        public Action Disconnect { protected get; set; }
        #endregion

        public readonly string Name;
        public readonly string Password;
        public readonly IDataConverter DataConverter;
        public ICommandWorker Worker { get; private set; }

        public CommandsFactory CommandsFactory { get; private set; }

        public Client(string name, string password, IDataConverter dataConverter) : base(0,default(DatabaseSettings))
        {
            this.Name = name;
            this.Password = password;
            this.DataConverter = dataConverter;
        }

        public Client(string name, string password) : this(name, password, new DataConverter()) { }

        private Client(int countColumn, DatabaseSettings settings, int loadedSector = 1) : base(countColumn, settings, loadedSector) { }

        #region Глобальное взаимодействие

        public DatabaseSettings LoadDataBaseSettings()
        {
            Worker.Push(BaseCommands.LoadDataBaseState);
            return JsonSerializer.Deserialize<DatabaseSettings>(NOTIFICATION());
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

        public override void ChangTypeInColumn(string nameColumn, TypeOfData newTypeOfData)
        {
            Worker.Push(BaseCommands.ChengTypeInColumn + BaseCommands.SEPARATION + nameColumn + BaseCommands.SEPARATION + newTypeOfData.GetType().Name);
            var msg = NOTIFICATION(); 
        }

        public override int[] GetAllIDsByParams(int numberColumn, string data)
        {
            return GetAllIDsByParams(this[numberColumn].Name, data);
        }

        public override int[] GetAllIDsByParams(string columnName, string data)
        {
            Worker.Push(BaseCommands.GetAllIDsByParams + BaseCommands.SEPARATION +
                columnName + BaseCommands.SEPARATION + data);
            
            return DataConverter.GetInts(NOTIFICATION());
        }

        public override void AddColumn(string columnName)
        {
            Worker.Push(BaseCommands.AddColumn + BaseCommands.SEPARATION + columnName 
                + BaseCommands.SEPARATION + DataTypesInColumns.Text.GetType().Name);
            
            NOTIFICATION();
            _AddColumn?.Invoke(columnName);
        }

        public override void AddColumn(string name, TypeOfData typeOfData)
        {
            Worker.Push(BaseCommands.AddColumn + BaseCommands.SEPARATION + name
                + BaseCommands.SEPARATION + typeOfData.GetType().Name);
            NOTIFICATION();
            _AddColumn?.Invoke(name);
        }

        public override void AddColumn(AColumn column)
        {
            AddColumn(column.Name);
        }

        public override void RemoveColumn(string columnName)
        {
            Worker.Push(BaseCommands.RemoveColumn + BaseCommands.SEPARATION + columnName);
            NOTIFICATION();
            _RemoveColumn?.Invoke(columnName);
        }

        public override void RemoveColumn(int numberOfColumn)
        {
            Worker.Push(BaseCommands.RemoveColumn + BaseCommands.SEPARATION + this[numberOfColumn].Name);
            NOTIFICATION();
            _RemoveColumn?.Invoke(this[numberOfColumn].Name);
        }

        public override void RemoveColumn(AColumn columnName)
        {
            RemoveColumn(columnName.Name);
        }

        public override void CloneTo(AColumn left, AColumn right)
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

        public override void ClearAllColumn(AColumn column, int inSector = -1)
        {
            ClearAllColumn(column.Name, inSector);
        }

        public override void ClearAllColumn(string columnName, int inSector = -1)
        {
            Worker.Push(BaseCommands.ClearColumn + BaseCommands.SEPARATION + columnName + BaseCommands.SEPARATION + inSector);
            NOTIFICATION();
            _ClearAllColumn?.Invoke(columnName, inSector);
        }

        public override void ClearAllBase()
        {
            Worker.Push(BaseCommands.ClearAllBase);
            NOTIFICATION();
            _ClearAllBase?.Invoke();
        }

        public override void RenameColumn(AColumn column, string newName)
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

        public override void ChangeEverythingTo(string columnName, string @params, string newData, int sectorID = -1)
        {
            Worker.Push(BaseCommands.ChangeEverythingTo + BaseCommands.SEPARATION + 
                columnName + BaseCommands.SEPARATION + @params + BaseCommands.SEPARATION +
                newData + BaseCommands.SEPARATION + sectorID);
            NOTIFICATION();           
        }

        public override void SetData(int ID, params string[] datas)
        {
            var bl = new Row();
            bl.Init(ID, datas);

            Worker.Push(BaseCommands.SetData + BaseCommands.SEPARATION + DataConverter.ParsDataLine(bl));
            NOTIFICATION();
        }

        public override void SetDataInColumn(string columnName, ItemData itemData)
        {
            Worker.Push(BaseCommands.SetDataInColumn + BaseCommands.SEPARATION + columnName + BaseCommands.SEPARATION + 
               DataConverter.ParsItemData(itemData));
            
            NOTIFICATION(); 
        }

        public override void AddData(params string[] datas)
        {
            var bl = new Row();
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
            var dl = DataConverter.GetDataLine<Row>(NOTIFICATION());   

            Worker.Push(BaseCommands.RemoveDataByID + BaseCommands.SEPARATION + ID);
            
            NOTIFICATION();
            _AddData?.Invoke(dl.GetData(), ID);
        }

        public override bool RemoveAllData(params string[] datas)
        {
            var bl = new Row();
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

        public override Row[] GetAllDataInBaseByColumnName(string columnName, string data)
        {
            Worker.Push(BaseCommands.GetAllDataInBaseByColumnName + BaseCommands.SEPARATION +
                columnName + BaseCommands.SEPARATION + data);
            
            var msg = NOTIFICATION();

            return DataConverter.GetDataLines<Row>(msg);
        }

        public override int GetIDByParams(string columnName, string data, int inSector = -1)
        {
            Worker.Push(BaseCommands.GetIDByParams + BaseCommands.SEPARATION + columnName + BaseCommands.SEPARATION +
                data + BaseCommands.SEPARATION + inSector);
            var msg = NOTIFICATION();
            return int.Parse(msg);
        }

        public override string[] GetDataByID(int ID)
        {
            Worker.Push(BaseCommands.GetDataByID + BaseCommands.SEPARATION + ID);
            return DataConverter.GetDataLine<Row>(NOTIFICATION()).GetData();
        }

        public override T[] SmartSearch<T>(AColumn[] columns, SearchType[] typesOfSearch, string[] @params, int inSectro = -1)
        {
            throw new NotImplementedException();
        }
        #endregion

        public void ConnectTo<T>(string IP, int Port) where T : ClientCommandsWorker
        {
            Worker = (T)typeof(T).GetConstructor(new Type[]
            { typeof(string), typeof(string), typeof(string), typeof(string) }).Invoke(new object[] 
            { IP, Port.ToString(), Name, Password });
            
            InitHandlers();

            Worker.Push(BaseCommands.Login + BaseCommands.SEPARATION + Name + BaseCommands.SEPARATION + Password);
            string response = NOTIFICATION();
            

            if (response == BaseCommands.Disconnect || response != BaseCommands.Connect)
            {
                throw new Exception($"Не удалось подключиться к БД! IP: {IP}, Port: {Port}, Name: {Name}, Password: {Password}");
            }

            
            Settings = LoadDataBaseSettings();
            LoadDataBaseColumnsState();
        }

        public void CloseConnection()
        {           
            Worker.Dispose();
            Disconnect?.Invoke();
        }

        private void InitHandlers()
        {
            CommandsFactory = new CommandsFactory();
            CommandsFactory.AddCommand(BaseCommands.HAVENOTPERM, new HAVENOTPERM());
            CommandsFactory.AddCommand(BaseCommands.ERROR, new ERROR());
            CommandsFactory.AddCommand(BaseCommands.Disconnect, new Disconnect());
        }

        /// <summary>
        /// Отправляет сообщение на сервер
        /// </summary>
        /// <param name="command"></param>
        public void PushCastomCommand(string command)
        {
            Worker.Push(BaseCommands.MSG + BaseCommands.SEPARATION + command);
        }

        public override AColumn this[string index]
        {
            get
            {
                LoadDataBaseColumnsState();
                return (AColumn)base[index];
            }
            protected set
            {
                LoadDataBaseColumnsState();
                base[index] = value;
            }
        }

        public override AColumn this[int index]
        {
            get
            {
                LoadDataBaseColumnsState();
                return (AColumn)base[index];
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
            string[] datas = res.Split(BaseCommands.SEPARATION.ToCharArray(), 
                StringSplitOptions.RemoveEmptyEntries);
           
            var sb = new StringBuilder();

            if (datas.Length > 1)
            {
                sb.Append(datas[1]);
                for (int i = 2; i < datas.Length; i++)
                {
                    sb.Append(BaseCommands.SEPARATION);
                    sb.Append(datas[i]);
                }
            }

            try
            {
                CommandsFactory[datas[0]].SetData(sb.ToString());
                CommandsFactory[datas[0]].Use();

            }
            catch { }

            return res;
        }
    }
}
