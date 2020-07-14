using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core
{
    [Serializable]
	public class Node<T> : INode
    {
        protected T[] d = null; // domain: optional
        protected T[] t = null; // value

        public Node() { }

        public Node(int size)
        {
            t = new T[size];
        }

        public virtual string ArrayToString()
        {
            string str = "";
            for (int i = 0; i < t.Length; i++)
                str += t[i].ToString() + ", ";
            str = str.TrimEnd(',', ' ');
            return str;
        }

        public virtual object Clone()
        {
            Node<T> clone = new Node<T>();
            clone.Configure(t);
            return clone;
        }

        public virtual Node<T> Configure(T[] t)
        {
            this.t = new T[t.Length];
            for (int i = 0; i < t.Length; i++)
                this.t[i] = t[i];
            return this;
        }

        public virtual double? Cost { get; set; }

        public virtual T[] Domain
        {
            get { return d; }
            set
            {
                d = new T[value.Length];
                for (int i = 0; i < value.Length; i++)
                    d[i] = value[i];
            }
        }

        public virtual double? Fitness { get; set; }

        public virtual double? Heuristic { get; set; }

        public int? ID { get; set; }

        /// <summary>
        /// determines if equal to n
        /// </summary>
        /// <param name="n"></param>
        /// <returns>number of mismatches</returns>
        public virtual int IsEqual(INode n)
        {
            T[] a = ((Node<T>)n).ToArray();

            if (a.Length != t.Length)
                throw new Exception();

            int c = 0;

            for (int i = 0; i < t.Length; i++)
            {
                if (!t[i].Equals(a[i]))
                    ++c;
            }

            return c;
        } 

        public virtual int Size
        {
            get { return t.Length; }
        }

        public T this[int i]
        {
            get { return t[i]; }
            set { t[i] = value; }
        }

        public T[] ToArray()
        {
            T[] a = new T[t.Length];
            for (int i = 0; i < t.Length; i++)
                a[i] = t[i];
            return a;
        }

        public T[,] ToMatrix(int nrow, int ncol)
        {
            if ((nrow * ncol) != t.Length)
                throw new Exception();

            T[,] m = new T[nrow, ncol];

            for (int c = 0, i = 0; i < nrow; i++)
            {
                for (int j = 0; j < ncol; j++)
                    m[i, j] = t[c++];
            }

            return m;
        }

        public override string ToString()
        {
            string s = "";

            s += "ID:" + (ID == null ? "#" : ID.Value.ToString()) + "; ";
            s += "Fitness:" + (Fitness == null ? "#" : Fitness.Value.ToString("00.000000")) + "; ";
            s += "Size:" + Size + "; ";

            s += "Domain:";

            if (d != null)
            {
                for (int i = 0; i < d.Length; i++)
                    s += d[i] + ", ";
                s = s.TrimEnd(' ', ',');
                s += "; ";
            }
            else
            {
                s += "#; ";
            }

            s += "Coord:";

            if (t != null)
            {
                for (int i = 0; i < t.Length; i++)
                    s += t[i] + ", ";
                s = s.TrimEnd(' ', ',');
            }
            else
            {
                s += "#";
            }

            return s;
        }
    }
}