using System;
using System.IO;

namespace NASDataBaseAPI.Data
{
    /// <summary>
    /// Предоставляет базовый список типов данных для работы БД 
    /// </summary>
    public static class DataTypesInTable
    {
        public static DataType Int { get; private set; } = new DataTypeInt("Int");
        public static DataType SemicolonNumbers { get; private set; } = new DataTypeDecimal("Float");
        public static DataType Text { get; private set; } = new DataTypeText("Text");
        public static DataType Boolean { get; private set; } = new DataTypeBool("Boolean");
        public static DataType Time { get; private set; } = new DataTypeTime("Time");
        public static DataType File { get; private set; } = new DataTypeFile("File");
    }

    public class DataType
    {
        public string Name { get; private set; }
       
        public DataType(string Name)
        {
            this.Name = Name;
        }

        /// <summary>
        /// По умолчанию не работате
        /// </summary>
        public virtual object Convert(object value)
        {
            return value;
        }

        /// <summary>
        /// По умолчанию не работате
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool TryConvert(object value)
        {
            return false;
        }

    }

    public class DataTypeFile : DataType
    {
        private IDataTypeConverter<string> converter = new ToFileConverter();
        public DataTypeFile(string Name) : base(Name)
        {
        }

        public override object Convert(object value) => converter.Convert(value);

        public override bool TryConvert(object value) { return converter.TryConvert(value); }
    }

    public class DataTypeTime : DataType
    {
        private IDataTypeConverter<DateTime> converter = new ToTimeConverter();
        public DataTypeTime(string Name) : base(Name)
        {
        }

        public override object Convert(object value) => converter.Convert(value);

        public override bool TryConvert(object value) { return converter.TryConvert(value); }
    }

    public class DataTypeInt : DataType
    {
        private IDataTypeConverter<int> converter = new ToIntConverter();
        public DataTypeInt(string Name) : base(Name)
        {
        }
        public override object Convert(object value)
        {
            return converter.Convert(value);
        }
        public override bool TryConvert(object value)
        {
            return converter.TryConvert(value);
        }
    }

    public class DataTypeBool : DataType
    {
        private IDataTypeConverter<bool> converter = new ToBoolConverter();
        public DataTypeBool(string Name) : base(Name)
        {
        }
        public override object Convert(object value)
        {
            return converter.Convert(value);
        }
        public override bool TryConvert(object value)
        {
            return converter.TryConvert(value);
        }
    }

    public class DataTypeText : DataType
    {
        private IDataTypeConverter<string> converter = new ToStringConverter();
        public DataTypeText(string Name) : base(Name)
        {
        }
        public override object Convert(object value)
        {
            return converter.Convert(value);
        }
        public override bool TryConvert(object value)
        {
            return converter.TryConvert(value);
        }
    }

    public class DataTypeDecimal : DataType
    {
        private IDataTypeConverter<decimal> converter = new ToDecimalConverter();
        public DataTypeDecimal(string Name) : base(Name)
        {
        }
        public override object Convert(object value)
        {
            return converter.Convert(value);
        }
        public override bool TryConvert(object value)
        {
            return converter.TryConvert(value);
        }
    }

    /// <summary>
    /// Смотрит можно ли по пути получить данные
    /// </summary>
    public class ToFileConverter : IDataTypeConverter<string>
    {
        public string Convert(string value)
        {
            return File.ReadAllText(value);
        }

        public string Convert(object value)
        {
            return File.ReadAllText(value.ToString());
        }

        public bool TryConvert(object value)
        {
            try
            {
                File.ReadAllText(value.ToString());
                return true;
            }
            catch { return false; }
        }
    }


    public class ToTimeConverter : IDataTypeConverter<DateTime>
    {
        public DateTime Convert(string value)
        {
            return DateTime.Parse(value);
        }

        public DateTime Convert(object value)
        {
            return DateTime.Parse(value.ToString());
        }

        public bool TryConvert(object value)
        {
            return DateTime.TryParse(value.ToString(), out DateTime result);
        }
    }

    public class ToDecimalConverter : IDataTypeConverter<decimal>
    {
        public decimal Convert(string value)
        {
            return System.Convert.ToDecimal(value);
        }

        public decimal Convert(object value)
        {
            return System.Convert.ToDecimal(value);
        }

        public bool TryConvert(object value)
        {
            try
            {
                System.Convert.ToDecimal(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    public class ToBoolConverter : IDataTypeConverter<bool>
    {
        public bool TryConvert(object value)
        {
            try
            {
                System.Convert.ToBoolean(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        bool IDataTypeConverter<bool>.Convert(string value)
        {
            return System.Convert.ToBoolean(value);
        }

        bool IDataTypeConverter<bool>.Convert(object value)
        {
            return System.Convert.ToBoolean(value);
        }
    }
    public class ToIntConverter : IDataTypeConverter<int>
    {
        public int Convert(string value)
        {
            return System.Convert.ToInt32(value);
        }

        public int Convert(object value)
        {
            return System.Convert.ToInt32(value);
        }

        public bool TryConvert(object value)
        {
            try
            {
                System.Convert.ToInt32(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class ToStringConverter : IDataTypeConverter<string>
    {
        public string Convert(string value)
        {
            return value;
        }

        public string Convert(object value)
        {
            return value.ToString();
        }

        public bool TryConvert(object value)
        {
            return true;
        }
    }


    public interface IDataTypeConverter<T>
    {
        bool TryConvert(object value);
        T Convert(string value);
        T Convert(object value);
    }
}
