using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core
{
    public abstract class Collection<T>
        where T : INode
    {
        protected IList<T> c = new List<T>();
        protected int? capacity = null;

        public Collection() { }

        public Collection(int capacity)
        {
            this.capacity = capacity;
        }

        public int? Capacity
        {
            get { return capacity; }
        }

        public void Clear()
        {
            c.Clear();
        }

        public int? Contains(T t)
        {
            for (int i = 0; i < c.Count; i++)
            {
                if (t.IsEqual(c[i]) == 0)
                    return i;
            }

            return null;
        }

        public int Count
        {
            get { return c.Count; }
        }

        public bool IsEmpty()
        {
            if (c.Count == 0)
                return true;
            return false;
        }

        public bool IsFull()
        {
            if (Capacity == null)
                return false;
            if (c.Count > Capacity.Value)
                throw new Exception();
            return (c.Count < Capacity.Value ? false : true);
        }

        public void Remove(T t)
        {
            c.Remove(t);
        }

        public T this[int i]
        {
            get { return c[i]; }
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < c.Count; i++)
                s += "\n[" + i.ToString("00") + "]: " + c[i].ToString();
            return s;
        }
    }
}
