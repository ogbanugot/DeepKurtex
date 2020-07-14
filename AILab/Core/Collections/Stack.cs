using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core.Collections
{
    [Serializable]
	public class Stack<T> : Collection<T>
        where T : INode
    {
        public Stack()
            : base() { }

        public T Peek()
        {
            if (c.Count == 0)
                return default(T);
            return c[0];
        }

        public T Pop()
        {
            if (c.Count == 0)
                return default(T);
            T t = c[0];
            c.RemoveAt(0);
            return t;
        }

        public void Push(T t)
        {
            c.Insert(0, t);
        }
    }
}