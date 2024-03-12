using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDatabase.Server.Data.DataTypesInColumn.Converter
{
    public class ToIntConverter : IDataTypeConverter<int>
    {
        public int Convert(string value)
        {
            return System.Convert.ToInt32(value);
        }

        public int Convert(object value)
        {
            return System.Convert.ToInt32(value);
        }

        public bool TryConvert(object value)
        {
            try
            {
                System.Convert.ToInt32(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
