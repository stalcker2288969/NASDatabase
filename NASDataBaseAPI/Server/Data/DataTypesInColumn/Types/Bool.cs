using NASDatabase.Data.DataTypesInColumn;
using NASDatabase.Server.Data.DataTypesInColumn.Converter;


namespace NASDatabase.Server.Data.DataTypesInColumn.Types
{
    public class Bool : TypeOfData
    {
        private IDataTypeConverter<bool> converter = new ToBoolConverter();       

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
            return new Int().More(value1, value2);
        }

        public override bool Less(string value1, string value2)
        {
            return new Int().Less(value1, value2);
        }

        public override bool Equal(string value1, string value2)
        {
            return new Int().Equal(value1, value2);
        }

        public override bool NotEqual(string value1, string value2)
        {
            return new Int().NotEqual(value1, value2);
        }

        public override string GetBaseValue()
        {
            return bool.FalseString;
        }
    }
}
