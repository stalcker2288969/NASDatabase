using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.SmartSort
{
    /// <summary>
    /// Тип сортировки данных
    /// </summary>
    public enum SortType
    {
        /// <summary>
        /// По возростанию если это Int/Float или Boolean(true/false),если Text => По длине строки
        /// </summary>
        Ascending,
        /// <summary>
        /// По уменьшению если это Int/Float или Boolean(true/false),если Text => По длине строки
        /// </summary>
        Descending,
        /// <summary>
        /// Вначале элементы из списка будут идти в указанной последовательности
        /// </summary>
        ByRange,
        /// <summary>
        /// Вначале все элементы не из списка, а потом с конца списка пойдут элементы
        /// </summary>
        NotInRange
    }
}
