using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace NASDatabase.Client
{
    /// <summary>
    /// Предоставляет интерфейс запросов от клиента к БД 
    /// </summary>
    public static class BaseCommands
    {
        public const string SEPARATION = ":";
        public const string HAVENOTPERM = "HaveNotPerm";
        public const string ERROR = "ErrorInCommand";
        public const string DONE = "Done";

        public const string LoadDataBaseState = "LoadDataBaseState";
        public const string LoadDataBaseColumnsState = "LoadDataBaseColumnsState";

        public const string ChengTypeInColumn = "Com_ChengTypeInColumn";
        public const string Registration = "Com_Registration";
        public const string Login  = "Com_Login";
        public const string Connect = "Com_Connect";
        public const string Disconnect  = "Com_Disconnect";
        public const string AddData = "Com_AddData";
        public const string RemoveDataByID = "Com_RemoveDataByID";
        public const string SetDataInColumn = "Com_SetDataInColumn";
        public const string SetData = "Com_SetData";
        public const string GetDataByID = "Com_GetDataByID";
        public const string GetIDByParams = "Com_GetIDByParams";
        public const string AddColumn = "Com_AddColumn";
        public const string CloneColumn = "Com_CloneTo";
        public const string ClearColumn = "Com_ClearColumn";
        public const string ClearAllBase = "Com_ClearAllBase";
        public const string SetTypeOfColumn = "Com_SetTypeOfColumn";
        public const string RemoveColumn = "Com_RemoveColumn";
        public const string RemoveAllData = "Com_RemoveAllData";
        public const string LoadClusterToMe = "Com_LoadClusterToMe";
        public const string SmartSearch = "Com_SmartSearch";
        public const string PrintBase = "Com_PrintBase";
        public const string GetDataInBaseByColumnName = "Com_GetDataByColumnName";
        public const string GetAllDataInBaseByColumnName = "Com_GetAllDataInBaseByColumnName";
        public const string FindID = "Com_FindID";
        public const string FindDataByID = "Com_FindDataByID";
        public const string RenameColumn = "Com_RenameColumn";
        public const string GetAllIDsByParams = "Com_GetAllIDsByParams";
        public const string ChangeEverythingTo = "Com_ChangeEverythingTo";

        public const string MSG = "Com_MSG";
    }   
}
