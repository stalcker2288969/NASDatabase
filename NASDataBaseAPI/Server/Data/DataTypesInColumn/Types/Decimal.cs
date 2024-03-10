using NASDataBaseAPI.Data.DataTypesInColumn;
using NASDataBaseAPI.Server.Data.DataTypesInColumn.Converter;


namespace NASDataBaseAPI.Server.Data.DataTypesInColumn.Types
{
    public class Decimal : TypeOfData
    {
        private IDataTypeConverter<decimal> converter = new ToDecimalConverter();

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
            var x = converter.Convert(value1);
            var y = converter.Convert(value2);
            return x > y;
        }

        public override bool Less(string value1, string value2)
        {
            var x = converter.Convert(value1);
            var y = converter.Convert(value2);
            return x < y;
        }

        public override bool Equal(string value1, string value2)
        {
            var x = converter.Convert(value1);
            var y = converter.Convert(value2);
            return x == y;
        }

        public override bool NotEqual(string value1, string value2)
        {
            var x = converter.Convert(value1);
            var y = converter.Convert(value2);
            return x!=y;
        }

        public override string GetBaseValue()
        {
            return default(decimal).ToString();
        }

        public override bool Multiple(string value1, string value2)
        {
            var x = converter.Convert(value1);
            var y = converter.Convert(value2);
            return x % y == default(decimal);
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
