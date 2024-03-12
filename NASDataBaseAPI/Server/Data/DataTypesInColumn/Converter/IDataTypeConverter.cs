using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDatabase.Server.Data.DataTypesInColumn.Converter
{
    public interface IDataTypeConverter<T>
    {
        bool TryConvert(object value);
        T Convert(string value);
        T Convert(object value);
    }
}
