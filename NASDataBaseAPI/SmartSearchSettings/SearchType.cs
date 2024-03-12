using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDatabase.Data
{
    /// <summary>
    /// Тип поиска данных в базе
    /// </summary>
    public enum SearchType
    {
        /// <summary>
        /// > int/float/Time или длина строки для Text
        /// </summary>
        More,
        /// <summary>
        /// int/float/Time или длина строки для Text
        /// </summary>
        Less,
        /// <summary>
        /// !> int/float/Time или длина строки для Text
        /// </summary>
        NotMore,
        /// <summary>
        /// !< int/float/Time или длина строки для Text
        /// </summary>
        NotLess,
        /// <summary>
        /// == что угодно 
        /// </summary>
        Equally, 
        /// <summary>
        /// != что угодно
        /// </summary>
        NotEqually,
        /// <summary>
        /// >= int/float/Time или длина строки для Text
        /// </summary>
        MoreOrEqually,
        /// <summary>
        /// int/float/Time или длина строки для Text
        /// </summary>
        LessOrEqually,
        /// <summary>
        /// По первому символу
        /// </summary>
        StartWith,
        /// <summary>
        /// По последнему символу
        /// </summary>
        StopWith,
        /// <summary>
        /// На совподение символов в строке для Text! Если перечислить числа через запятую то может поискать на савпадение из списка в столбцах с числами. Для Boolean не работает.
        /// </summary>
        ByRange,
        /// <summary>
        /// Нет совпадание из списка
        /// </summary>
        NotInRange,
        /// <summary>
        /// Кратность числу
        /// </summary>
        Multiple,
        /// <summary>
        /// Отсутствие кратности
        /// </summary>
        NotMultiple,

    }
}
