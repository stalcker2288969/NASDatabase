using NASDataBaseAPI.Data.DataTypesInColumn;
using NASDataBaseAPI.Server.Data.DataTypesInColumn.Converter;


namespace NASDataBaseAPI.Server.Data.DataTypesInColumn.Types
{
    public class DataTypeText : DataType
    {
        private IDataTypeConverter<string> converter = new ToStringConverter();
        public DataTypeText(string Name) : base(Name)
        {
        }

        public DataTypeText() { }

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
