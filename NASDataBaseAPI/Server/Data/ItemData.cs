using NASDataBaseAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Data
{
    public class ItemData
    {
        public int IDInTable { get;private set; }
        public string Data { get; private set; }

        public ItemData(int IDInTable, string Data)
        {
            this.Data = Data;
            this.IDInTable = IDInTable;
        }

        public static bool operator ==(ItemData left, ItemData right)
        {
            return left?.Data == right?.Data && left?.IDInTable == right?.IDInTable;
        }

        public static bool operator !=(ItemData left, ItemData right)
        {
            return left?.Data != right?.Data && left?.IDInTable != right?.IDInTable;
        }

        public static bool operator >(ItemData left, ItemData right)
        {
            bool result = false;
            int x;
            int y;
            decimal x1;
            decimal y1;
            DateTime dateTime1;
            DateTime dateTime2;
            bool B1;
            bool B2;
            if(int.TryParse(left.Data,out x) && int.TryParse(right.Data,out y))
            {
                return x > y;
            }
            else if(decimal.TryParse(left.Data,out x1) && decimal.TryParse(right.Data, out y1))
            {
                return x1 > y1;
            }
            else if(DateTime.TryParse(left.Data, out dateTime1) && DateTime.TryParse(right.Data,out dateTime2))
            {
                return dateTime1 > dateTime2;
            }
            else if(bool.TryParse(left.Data, out B1) && bool.TryParse(right.Data, out B2))
            {
                return Convert.ToInt32(B1) > Convert.ToInt32(B2);
            }
            else
            {
                return left.Data.Length > right.Data.Length;
            }
        }

        public static bool operator <(ItemData left, ItemData right)
        {
            bool result = false;
            int x;
            int y;
            decimal x1;
            decimal y1;
            DateTime dateTime1;
            DateTime dateTime2;
            bool B1;
            bool B2;
            if (int.TryParse(left.Data, out x) && int.TryParse(right.Data, out y))
            {
                return x < y;
            }
            else if (decimal.TryParse(left.Data, out x1) && decimal.TryParse(right.Data, out y1))
            {
                return x1 < y1;
            }
            else if (DateTime.TryParse(left.Data, out dateTime1) && DateTime.TryParse(right.Data, out dateTime2))
            {
                return dateTime1 < dateTime2;
            }
            else if (bool.TryParse(left.Data, out B1) && bool.TryParse(right.Data, out B2))
            {
                return Convert.ToInt32(B1) < Convert.ToInt32(B2);
            }
            else
            {
                return left.Data.Length < right.Data.Length;
            }
        }

        public override string ToString()
        {
            return Data;
        }
    }
}
