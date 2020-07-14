using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core.Collections
{
    [Serializable]
	public class Set<T> : Collection<T>
        where T : INode
    {
        public Set()
            : base() { }

        /// <summary>
        /// adds unique elements in array to set
        /// </summary>
        /// <param name="t">array of elements</param>
        public virtual void Add(params T[] t)
        {
            T x; int n = 0;

            for (int i = 0; i < t.Length; i++)
            {
                x = t[i];
                n = 0;

                for (int j = 0; j < c.Count; j++)
                {
                    if (x.IsEqual(c[j]) != 0)
                        ++n;
                }

                if (n == 0)
                    c.Add(x);
            }
        }
    }
}
