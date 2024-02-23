using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Client
{
    /// <summary>
    /// Предоставляет интерфейс запросов от клиента к БД 
    /// </summary>
    public static class BaseCommands
    {
        public const string Registration = "Com_Registration";
        public const string Login  = "Com_Login";
        public const string Disconnect  = "Com_Disconnect";
        public const string AddData = "Com_ADD";
        public const string RemoveDataByID = "Com_RemID";
        public const string SetDataInColumn = "Com_SetDataInColumn";
        public const string AddColumn = "Com_AddColumn";
        public const string SetTypeToColumn = "Com_SetTypeToColumn";
        public const string RemoveColumn = "Com_RemoveColumn";
        public const string LoadClusterToMe = "Com_LoadClusterToMe";
        public const string SmartSearch = "Com_SmartSearch";
        public const string PrintBase = "Com_PrintBase";
        public const string GetDataInBaseByColumnName = "Com_GetDataByColumnName";
        public const string GetAllDataInBaseByColumnName = "Com_GetAllDataInBaseByColumnName";
        public const string FindID = "Com_FindID";
        public const string FindDataByID = "Com_FindDataByID";
    }   
}
