using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Core
{
    [Serializable]
	public class Domain
    {
        protected object[] elements = null;

        protected int[] nofE = null;
        protected int[] indx = null;

        protected string[] tags = null;

        public Domain(int[] nofE)
        {
            construct(nofE);
        }

        public Domain(int[] nofE, object[] elements)
        {
            construct(nofE);

            if (this.elements.Length != elements.Length)
                throw new Exception();

            for (int i = 0; i < this.elements.Length; i++)
                this.elements[i] = elements[i];
        }

        private void construct(int[] nofE)
        {
            indx = new int[nofE.Length];
            this.nofE = new int[nofE.Length];

            int e = 0;

            for (int i = 0; i < nofE.Length; i++)
            {
                indx[i] = e;

                e += nofE[i];
                this.nofE[i] = nofE[i];
            }

            elements = new object[e];
        }

        public object[] Elements
        {
            get { return elements; }
            set { elements = value; }
        }

        public T[] GetElements<T>(int dimension)
        {
            int c = nofE[dimension];
            int i = indx[dimension];

            T[] e = new T[c];

            for (int j = 0; j < c; j++)
                e[j] = (T)elements[j + i];

            return e;
        }

        public T[] GetElements<T>(string tag)
        {
            int? i = getIndex(tag);

            if (i == null)
                throw new Exception();

            return GetElements<T>(i.Value);
        }

        private int? getIndex(string tag)
        {
            if (tags == null)
                return null;

            for (int i = 0; i < tags.Length; i++)
                if (tag.CompareTo(tags[i]) == 0)
                    return i;

            return null;
        }

        public int NofD
        {
            get { return nofE.Length; }
        }

        public void SetElements<T>(int dimension, T[] elements)
        {
            int c = nofE[dimension];
            int d = indx[dimension];

            if (c != elements.Length)
                throw new Exception();

            object[] b = new object[c];

            for (int i = 0; i < c; i++)
                this.elements[i + d] = elements[i];
        }

        public object[] this[int i]
        {
            get
            {
                int c = nofE[i];
                int j = indx[i];

                object[] e = new object[c];

                for (int k = 0; k < c; k++)
                    e[k] = elements[k + i];

                return e;
            }
            set
            {
                int c = nofE[i];
                int j = indx[i];

                for (int k = 0; k < c; k++)
                    elements[k + i] = value[k];
            }
        }

        public object[] this[string tag]
        {
            get
            {
                int? i = getIndex(tag);

                if (i == null)
                    throw new Exception();

                int c = nofE[i.Value];
                int d = indx[i.Value];

                object[] b = new object[c];

                for (int j = 0; j < c; j++)
                    b[j] = elements[j + d];

                return b;
            }
            set
            {
                int? i = getIndex(tag);

                if (i == null)
                    throw new Exception();

                int c = nofE[i.Value];
                int d = indx[i.Value];

                object[] b = new object[c];

                for (int j = 0; j < c; j++)
                    elements[j + d] = value[j];
            }
        }

        public string[] Tags
        {
            get { return tags; }
            set
            {
                if (nofE.Length != value.Length)
                    throw new Exception();

                tags = new string[value.Length];

                for (int i = 0; i < tags.Length; i++)
                    tags[i] = value[i];
            }
        }

        public override string ToString()
        {
            string s = "";

            for (int n = 0, i = 0; i < nofE.Length; i++)
            {
                s += "[";
                s += (tags == null ? "#: " : tags[i].ToUpper() + ": ");
                for (int j = 0; j < nofE[i]; j++)
                    s += elements[n++].ToString() + ", ";
                s = s.TrimEnd(new char[] { ' ', ',' });
                s += "]";
            }

            return s;
        }
    }
}