using NASDataBaseAPI.Data.DataTypesInColumn;
using NASDataBaseAPI.Server.Data.DataTypesInColumn.Converter;


namespace NASDataBaseAPI.Server.Data.DataTypesInColumn.Types
{
    public class Int : TypeOfData
    {
        private IDataTypeConverter<int> converter = new ToIntConverter();
        
        public override object Convert(object value)
        {
            return converter.Convert(value);
        }

        public override bool Equal(string value1, string value2)
        {
            if (CanConvert(value1) && CanConvert(value2))
                return value1 == value2;
            else
                return false;
        }

        public override string GetBaseValue()
        {
            return "0";
        }

        public override bool Less(string value1, string value2)
        {
            var x = converter.Convert(value1);
            var y = converter.Convert(value2);
            return x < y;
        }

        public override bool More(string value1, string value2)
        {
            var x = converter.Convert(value1);
            var y = converter.Convert(value2);
            return x > y;
        }

        public override bool NotEqual(string value1, string value2)
        {
            return !Equal(value1, value2);
        }

        public override bool CanConvert(object value)
        {
            return converter.TryConvert(value);
        }

        public override bool Multiple(string value1, string value2)
        {
            var x = converter.Convert(value1);
            var y = converter.Convert(value2);
            return x % y == default(int);
        }

        public override string Mult(string value1, string value2)
        {
            return (converter.Convert(value1) * converter.Convert(value2)).ToString();
        }

        public override string Share(string value1, string value2)
        {
            return (converter.Convert(value1) / converter.Convert(value2)).ToString();
        }
    }
}
