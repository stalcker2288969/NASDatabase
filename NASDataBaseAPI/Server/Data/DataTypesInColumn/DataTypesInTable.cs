using NASDataBaseAPI.Server.Data.DataTypesInColumn.Types;
using NASDataBaseAPI.Server.Data.DataTypesInColumn.Converter;
using System;
using System.IO;

namespace NASDataBaseAPI.Data.DataTypesInColumn
{
    /// <summary>
    /// Предоставляет базовый список типов данных для работы БД 
    /// </summary>
    public static class DataTypesInColumns
    {
        public static DataType Int { get; private set; } = new DataTypeInt("Int");
        public static DataType SemicolonNumbers { get; private set; } = new DataTypeDecimal("Float");
        public static DataType Text { get; private set; } = new DataTypeText("Text");
        public static DataType Boolean { get; private set; } = new DataTypeBool("Boolean");
        public static DataType Time { get; private set; } = new DataTypeTime("Time");
        public static DataType File { get; private set; } = new DataTypeFile("File");

        public static DataType GetType(string typeName)
        {
            switch (typeName)
            {
                case "Int":
                    return DataTypesInColumns.Int;
                case "Boolean":
                    return DataTypesInColumns.Boolean;
                case "Text":
                    return DataTypesInColumns.Text;
                case "Float":
                    return DataTypesInColumns.SemicolonNumbers;
                case "Time":
                    return DataTypesInColumns.Time;
                case "File":
                    return DataTypesInColumns.File;
                default:
                    return DataTypesInColumns.Text;
            }
        }
    }

    /// <summary>
    /// Базовый класс для дипов данных
    /// </summary>
    public class DataType
    {
        public string Name { get; private set; }
       
        public DataType(string Name)
        {
            this.Name = Name;
        }

        /// <summary>
        /// По умолчанию не работает 
        /// </summary>
        public virtual object Convert(object value)
        {
            return value;
        }

        /// <summary>
        /// По умолчанию не работает  
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool TryConvert(object value)
        {
            return false;
        }
    }
}
