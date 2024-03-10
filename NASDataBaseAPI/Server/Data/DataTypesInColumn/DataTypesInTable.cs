using NASDataBaseAPI.Server.Data.DataTypesInColumn.Types;
using t = NASDataBaseAPI.Server.Data.DataTypesInColumn.Types;
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
        public static TypeOfData Int { get; private set; } = new Int();
        public static TypeOfData SemicolonNumbers { get; private set; } = new t.Decimal();
        public static TypeOfData Text { get; private set; } = new Text();
        public static TypeOfData Boolean { get; private set; } = new Bool();
        public static TypeOfData Time { get; private set; } = new Time();

        public static TypeOfData GetType(string typeName)
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

        /// <summary>
        /// Возвращает все зарегистрированные типы в сборке 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static TypeOfData[] GetRegisterTypesOfData()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;

            Assembly[] assemblies = currentDomain.GetAssemblies();           

            var baseType = typeof(TypeOfData);

            List<TypeOfData> dataTypes = new List<TypeOfData>();
            
            foreach(var assemb in assemblies)
            {
                var Types = assemb.GetTypes();
                var derivedTypes = Types.Where(type => baseType.IsAssignableFrom(type) && type != baseType);

                foreach (var type in derivedTypes)
                {
                    try
                    {
                        dataTypes.Add(Activator.CreateInstance(type) as TypeOfData);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("При сборе информации о всех типах в сборке обнаружена ошибка! |" + ex.Message, ex);
                    }
                }
            }
            
            return dataTypes.ToArray();
        }

        public static TypeOfData GetTypeOf<T>(T value)
        {
            var types = GetRegisterTypesOfData();
            foreach (var DT in types)
            { 
                if (DT.CanConvert(value.ToString()))
                {
                    return DT;
                }
            }

            throw new Exception("У указанного обьекта нет своего DataType");
        }

        public static TypeOfData GetTypeOfDataByClassName(string className)
        {
            // Получаем текущий домен приложения
            AppDomain currentDomain = AppDomain.CurrentDomain;

            // Получаем все загруженные сборки в текущем домене
            Assembly[] assemblies = currentDomain.GetAssemblies();

            // Перебираем все сборки
            foreach (Assembly assembly in assemblies)
            {
                // Ищем тип в текущей сборке с указанным именем
                Type targetType = assembly.GetTypes().FirstOrDefault(type => type.Name == className && typeof(TypeOfData).IsAssignableFrom(type));

                // Если тип найден, создаем экземпляр и возвращаем его
                if (targetType != null)
                {
                    return (TypeOfData)Activator.CreateInstance(targetType);
                }
            }

            throw new Exception("В сборке не обнаружен искомый тип данных!");
        }
    }

    /// <summary>
    /// Базовый класс для дипов данных
    /// </summary>
    public abstract class TypeOfData
    {
        public string Name { get => this.GetType().Name; }
            
        public abstract object Convert(object value);

        public abstract bool CanConvert(object value);

        public abstract bool More(string value1, string value2);
        public abstract bool Less(string value1, string value2);
        public abstract bool Equal(string value1, string value2);
        public abstract bool NotEqual(string value1, string value2);
        public virtual bool IsBaseValue(string value) => Equal(value, GetBaseValue());
        public abstract string GetBaseValue();


        public virtual bool Multiple(string value1, string value2) { throw new NotImplementedException(); }
        public virtual string Share(string value1, string value2) { throw new NotImplementedException(); }
        public virtual string Mult(string value1, string value2) { throw new NotImplementedException(); }
    }
}
