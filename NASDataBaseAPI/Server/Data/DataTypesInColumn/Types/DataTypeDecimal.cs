using NASDataBaseAPI.Data.DataTypesInColumn;
using NASDataBaseAPI.Server.Data.DataTypesInColumn.Converter;


namespace NASDataBaseAPI.Server.Data.DataTypesInColumn.Types
{
    public class DataTypeDecimal : DataType
    {
        private IDataTypeConverter<decimal> converter = new ToDecimalConverter();
        public DataTypeDecimal(string Name) : base(Name)
        {
        }

        public DataTypeDecimal() { }

        public override object Convert(object value)
        {
            return converter.Convert(value);
        }
        public override bool TryConvert(object value)
        {
            return converter.TryConvert(value);
        }
    }
}
