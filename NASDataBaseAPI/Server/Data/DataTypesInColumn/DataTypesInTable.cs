using NASDataBaseAPI.Server.Data.DataTypesInColumn.Types;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;


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
                default:
                    return DataTypesInColumns.Text;
            }
        }

        //Возвращает все зарегистрированные DataTypes в сборке | не протестированно  
        public static DataType[] GetRegisterDataTypes(string Namespace = "NASDataBaseAPI")
        {
            Assembly assembly = Assembly.Load(Namespace);

            var Types = assembly.GetTypes();

            var baseType = typeof(DataType);

            var derivedTypes = Types.Where(type => baseType.IsAssignableFrom(type) && type != baseType);

            List<DataType> dataTypes = new List<DataType>();
            
            foreach (var type in derivedTypes)
            {
                try
                {
                    dataTypes.Add(Activator.CreateInstance(type) as DataType);
                }
                catch(Exception ex) 
                {
                    throw new Exception("При сборе информации о всех типах в сборке обнаружена ошибка! |" + ex.Message, ex);
                }               
            }

            return dataTypes.ToArray();
        }

        public static DataType GetTypeOfDataByClassName(string className)
        {
            // Получаем текущий домен приложения
            AppDomain currentDomain = AppDomain.CurrentDomain;

            // Получаем все загруженные сборки в текущем домене
            Assembly[] assemblies = currentDomain.GetAssemblies();

            // Перебираем все сборки
            foreach (Assembly assembly in assemblies)
            {
                // Ищем тип в текущей сборке с указанным именем
                Type targetType = assembly.GetTypes().FirstOrDefault(type => type.Name == className && typeof(DataType).IsAssignableFrom(type));

                // Если тип найден, создаем экземпляр и возвращаем его
                if (targetType != null)
                {
                    return (DataType)Activator.CreateInstance(targetType);
                }
            }

            throw new Exception("В сборке не обнаружен искомый тип данных!");
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

        public DataType()
        {
            this.Name = "%None%";
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
