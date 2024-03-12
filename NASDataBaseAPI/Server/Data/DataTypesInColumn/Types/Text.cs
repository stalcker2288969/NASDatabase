using NASDatabase.Data.DataTypesInColumn;
using NASDatabase.Server.Data.DataTypesInColumn.Converter;


namespace NASDatabase.Server.Data.DataTypesInColumn.Types
{
    public class Text : TypeOfData
    {
        private IDataTypeConverter<string> converter = new ToStringConverter();

        public override object Convert(object value)
        {
            return converter.Convert(value);
        }
        public override bool CanConvert(object value)
        {
            return converter.TryConvert(value);
        }

        public override bool More(string value1, string value2)
        {
            return value1.Length > value2.Length;
        }

        public override bool Less(string value1, string value2)
        {
            return value1.Length < value2.Length;
        }

        public override bool Equal(string value1, string value2)
        {
            return value1 == value2;
        }

        public override bool NotEqual(string value1, string value2)
        {
            return value1 != value2;
        }

        public override string GetBaseValue()
        {
            return string.Empty;
        }

        public override bool Multiple(string value1, string value2)
        {
            return value1.Length % (int)new Int().Convert(value2) == 0;
        }
    }
}
