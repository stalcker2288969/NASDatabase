using NASDataBaseAPI.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server
{
    /// <summary>
    /// Сканирует получиные данные от клиента и оповещает всю программу о командах
    /// </summary>
    public class ParserCommands
    {
        #region Ивенты
        /// <summary>
        /// Данные которые добавляются
        /// </summary>
        public event Action<string[]> OnAddData;
        /// <summary>
        /// ID удоляемого картежа
        /// </summary>
        public event Action<int> OnRemoveData;
        /// <summary>
        /// Данные от аккаунта(login,password)
        /// </summary>
        public event Action<string[]> OnRegistration;
        /// <summary>
        /// Того кто покидает(login,password)
        /// </summary>
        public event Action<string[]> OnDisconnect;
        /// <summary>
        /// Вход в акаунт(login,password)
        /// </summary>
        public event Action<string[]> OnLogin;
        /// <summary>
        /// Има таблички и данные(id/data)
        /// </summary>
        public event Action<string[]> SetDataInTable;     
        /// <summary>
        /// Имя столбца и Тип
        /// </summary>
        public event Action<string[]> AddTable;
        /// <summary>
        /// Имя таблички
        /// </summary>
        public event Action<string> RemoveTable;
        /// <summary>
        /// Название/тип
        /// </summary>
        public event Action<string[]> SetTypeToTable;
        /// <summary>
        /// Параметры поиска
        /// </summary>
        public event Action<string[]> SmartSearch;
        /// <summary>
        /// Запрос
        /// </summary>
        public event Action PrintBase;
        /// <summary>
        /// Имя столбца и данные в столбце
        /// </summary>
        public event Action<string[]> GetDataInBaseByTableName;
        /// <summary>
        /// Имя столбца и данные в столбце
        /// </summary>
        public event Action<string[]> GetAllDataInBaseByTableName;

        public event Action<string[]> UnknownСommand;
        #endregion
        /// <summary>
        /// Определяет что за каманда и оповещает программу
        /// </summary>
        /// <param name="command"></param>
        public void ParsCommand(string command)
        {
            string[] Params = command.Split(':');
            if (Params[0] == BaseCommands.AddData)
            {
                OnAddData?.Invoke(Params);
            }
            else if (Params[0] == BaseCommands.AddColumn)
            {
                AddTable?.Invoke(Params);
            }
            else if (Params[0] == BaseCommands.Login)
            {
                OnLogin?.Invoke(Params);
            }
            else if (Params[0] == BaseCommands.Disconnect)
            {
                OnDisconnect?.Invoke(Params);
            }
            else if (Params[0] == BaseCommands.Registration)
            {
                OnRegistration?.Invoke(Params);
            }
            else if (Params[0] == BaseCommands.PrintBase)
            {
                PrintBase?.Invoke();
            }
            else if (Params[0] == BaseCommands.RemoveTable)
            {
                RemoveTable?.Invoke(Params[1]);
            }
            else if (Params[0] == BaseCommands.SetDataInColumn)
            {
                SetDataInTable?.Invoke(Params);
            }
            else if (Params[0] == BaseCommands.SetTypeToColumn)
            {
                SetTypeToTable?.Invoke(Params);
            }
            else if (Params[0] == BaseCommands.RemoveDataByID)
            {
                OnRemoveData?.Invoke(int.Parse(Params[1]));
            }
            else if (Params[0] == BaseCommands.SmartSearch)
            {
                SmartSearch?.Invoke(Params);
            }
            else if (Params[0] == BaseCommands.GetAllDataInBaseByColumnName)
            {
                GetAllDataInBaseByTableName?.Invoke(Params);
            }
            else if (Params[0] == BaseCommands.GetDataInBaseByColumnName)
            {
                GetDataInBaseByTableName?.Invoke(Params);
            }
            else
            {
                UnknownСommand?.Invoke(Params);
            }
        }
    }
}
