using NASDataBaseAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.SmartSort
{
    public class SortingManager
    {
        /// <summary>
        /// Сортирует данные в зависимости от параметров(работае только Ascending и Descending
        /// </summary>
        /// <param name="itemDatas"></param>
        /// <param name="sortType"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        public static List<ItemData> Sort(List<ItemData> itemDatas, SortType sortType, string Params = " ")
        {
            List<ItemData> result = new List<ItemData>();

            switch (sortType)
            {
                case SortType.Ascending:
                    result.AddRange(new Ascending().SortItemsData(itemDatas.ToArray()));
                    break;
                case SortType.Descending:
                    result.AddRange(new Descending().SortItemsData(itemDatas.ToArray()));
                    break;                
            }
            return result;
        }
    }
}
