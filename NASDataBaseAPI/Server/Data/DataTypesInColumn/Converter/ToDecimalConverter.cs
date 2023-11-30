using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.DataTypesInColumn.Converter
{
    public class ToDecimalConverter : IDataTypeConverter<decimal>
    {
        public decimal Convert(string value)
        {
            return System.Convert.ToDecimal(value);
        }

        public decimal Convert(object value)
        {
            return System.Convert.ToDecimal(value);
        }

        public bool TryConvert(object value)
        {
            try
            {
                System.Convert.ToDecimal(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
