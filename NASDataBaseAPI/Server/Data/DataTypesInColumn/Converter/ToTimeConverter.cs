using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.DataTypesInColumn.Converter
{
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
}
