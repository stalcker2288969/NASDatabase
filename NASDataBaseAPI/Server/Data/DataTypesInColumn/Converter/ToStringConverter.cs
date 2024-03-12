using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDatabase.Server.Data.DataTypesInColumn.Converter
{
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
            return value.ToString() != "" ? true : false;
        }
    }
}
