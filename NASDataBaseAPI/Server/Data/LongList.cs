using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NASDataBaseAPI.Data
{
    internal class ULongList<T>
    {
        public ulong Count { get; private set; } = 0;

        private ulong _count = 1;

        private T[] values;

        public ULongList(ulong Count) 
        {
            values = new T[Count];
            this.Count = Count;
        }

        public ULongList(T[] Values)
        {
            this.values = Values;
            this.Count = (ulong)Values.Length;           
        }

        public void Add(T item)
        {
            
            //T[] values1 = values;
            //if (Count+1 > _count)
            //{
            //    values = new T[values.Length * 2];
            //}
            //else
            //{
            //    values = new T[values.Length];
            //}

            //for (ulong i = 0; i < values1.Length; i++)
            //{

            //}

        }
    }
}
