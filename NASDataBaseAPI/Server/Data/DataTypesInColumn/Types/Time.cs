using NASDatabase.Data.DataTypesInColumn;
using NASDatabase.Server.Data.DataTypesInColumn.Converter;
using System;

namespace NASDatabase.Server.Data.DataTypesInColumn.Types
{
    public class Time : TypeOfData
    {
        private IDataTypeConverter<DateTime> converter = new ToTimeConverter();


        public override object Convert(object value) => converter.Convert(value);

        public override bool CanConvert(object value) => converter.TryConvert(value); 

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
            return x != y;
        }

        public override string GetBaseValue()
        {
            return default(DateTime).ToString();
        }
    }
}
