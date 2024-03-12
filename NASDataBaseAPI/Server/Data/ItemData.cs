using NASDatabase.Interfaces;
using System;


namespace NASDatabase.Server.Data
{
    /// <summary>
    /// Тип представляющий из себя ячейку в столбцах базы  
    /// </summary>
    public struct ItemData : IItemData
    {
        public int ID { get; private set; }
        public string Data { get; private set; }

        public ItemData(int ID, string Data)
        {
            this.Data = Data;
            this.ID = ID;
        }

        public static bool operator ==(ItemData left, ItemData right)
        {
            return left.Data == right.Data && left.ID == right.ID;
        }

        public static bool operator !=(ItemData left, ItemData right)
        {
            return left.Data != right.Data && left.ID != right.ID;
        }

        public static bool operator >(ItemData left, ItemData right)
        {
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
