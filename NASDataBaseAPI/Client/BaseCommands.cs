using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Client
{
    /// <summary>
    /// Предостовляет интерфейс запросов от клиента к БД
    /// </summary>
    public static class BaseCommands
    {
        public static string Registration { get; } = "Com_Registration:";
        public static string Login { get; } = "Com_Login:";
        public static string Disconnect { get; } = "Com_Disconnect:";

        public static string AddData { get; } = "Com_ADD:";
        public static string RemoveDataByID { get; } = "Com_RemID:";
        public static string SetDataInTable { get; } = "Com_SetDataInTable:";
        public static string AddTable { get; } = "Com_AddTable:";
        public static string SetTypeToTable { get; } = "Com_SetTypeToTable:";
        public static string RemoveTable { get; } = "Com_RemoveTable:";

        public static string SmartSearch { get; } = "Com_SmartSearch:";
        public static string PrintBase { get; } = "Com_PrintBase";
        public static string GetDataInBaseByTableName { get; } = "Com_GetDataByTableName:";
        public static string GetAllDataInBaseByTableName { get; } = "Com_GetAllDataInBaseByTableName:";
    }
}
