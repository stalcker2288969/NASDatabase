using NASDataBaseAPI.Data.DataTypesInColumn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Server.Data.DataTypesInColumn.Converter
{
    /// <summary>
    /// Смотрит можно ли по пути получить данные
    /// </summary>
    public class ToFileConverter : IDataTypeConverter<string>
    {
        public string Convert(string value)
        {
            throw new Exception("Не реализованно");
        }

        public string Convert(object value)
        {
            throw new Exception("Не реализованно");
        }

        public bool TryConvert(object value)
        {
            try
            {
                throw new Exception("Не реализованно");
                return true;
            }
            catch { return false; }
        }
    }
}
