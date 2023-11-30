using NASDataBaseAPI.Data.DataTypesInColumn;
using NASDataBaseAPI.Server.Data.DataTypesInColumn.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.DataTypesInColumn.Types
{
    public class DataTypeTime : DataType
    {
        private IDataTypeConverter<DateTime> converter = new ToTimeConverter();
        public DataTypeTime(string Name) : base(Name)
        {
        }

        public override object Convert(object value) => converter.Convert(value);

        public override bool TryConvert(object value) { return converter.TryConvert(value); }
    }
}
