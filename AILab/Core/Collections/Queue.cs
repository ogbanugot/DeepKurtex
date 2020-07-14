using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core.Collections
{ 
    [Serializable]
	public class Queue<T> : Collection<T>
        where T : INode
    {
        protected Enums.OrderPriorityBy? p = null;

        public Queue(Enums.OrderPriorityBy? p)
        {
            this.p = p;
        }

        public T Dequeue()
        {
            if (c.Count == 0)
                return default(T);
            T t = c[0];
            c.RemoveAt(0);
            return t;
        }

        public void Enqueue(T t)
        {
            if (IsFull() == true)
                throw new Exception();

            int? j = null;

            if (p != null)
            {
                int i = 0;

                switch (p.Value)
                {
                    case Enums.OrderPriorityBy.Ascending: // higher values have higher priority
                        
                        for (; i < c.Count; i++)
                        {
                            if (c[i].Fitness.Value < t.Fitness.Value)
                            {
                                j = i;
                                break;
                            }
                        }

                        break;

                    case Enums.OrderPriorityBy.Descending: // lower values have higher priority

                        for (; i < c.Count; i++)
                        {
                            if (c[i].Fitness.Value > t.Fitness.Value)
                            {
                                j = i;
                                break;
                            }
                        }

                        break;
                }
            }

            if ((j == null) || (p == null))
            {
                c.Add(t);
            }
            else
            {
                c.Insert(j.Value, t);
            }
        }

        public T Peek()
        {
            if (c.Count == 0)
                return default(T);
            return c[0];
        }
    }
}
