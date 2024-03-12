using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDatabase.Server.Data.DataTypesInColumn.Converter
{
    public class ToBoolConverter : IDataTypeConverter<bool>
    {
        public bool TryConvert(object value)
        {
            try
            {
                System.Convert.ToBoolean(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        bool IDataTypeConverter<bool>.Convert(string value)
        {
            return System.Convert.ToBoolean(value);
        }

        bool IDataTypeConverter<bool>.Convert(object value)
        {
            return System.Convert.ToBoolean(value);
        }
    }
}
