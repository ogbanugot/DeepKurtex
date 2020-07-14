using Math.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.CNN
{
    [Serializable]
	public class fMap : Matrix<Foundation.Node>
    {
        private string formatString = "0.00";

        public fMap()
            : base() { }

        public virtual Matrix<Foundation.Node> Configure<T>(int rows, int cols, int fieldsize)
        {
            base.Configure(rows, cols);
            T[] field;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    field = new T[fieldsize];
                    m[i][j] = new Foundation.Node(field);
                }
            }

            return this;
        }

        /// <summary>
        /// clear all elements contained in nodes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Clear<T>()
        {
            int n = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    n = ((T[])m[i][j].Element).Length;
                    for (int k = 0; k < n; k++)
                        ((T[])m[i][j].Element)[k] = default;
                }
            }
        }

        public void Set<T>(int index, T t)
        {
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                    ((T[])m[i][j].Element)[index] = t;
        }

        public fMap Initialize(string formatString)
        {
            this.formatString = formatString;
            return this;
        }

        public T Read<T>(int row, int col)
        {
            return m[row][col].GetElement<T>();
        }

        public T ReadAt<T>(int row, int col, int index)
        {
            return ((T[])m[row][col].Element)[index];
        }

        public string ToSubString(int x, int y, int xsize, int ysize)
        {
            string s = ""; object e; Array a;

            int rows = x + xsize;
            int cols = y + ysize;
            if ((Columns < cols) || (Rows < rows))
                throw new Exception();
            double? v;

            for (int i = x; i < rows; i++)
            {
                s += "\n";

                for (int j = y; j < cols; j++)
                {
                    e = m[i][j].Element;
                    if (e is Array)
                    {
                        a = (Array)e;
                        s += "[";
                        for (int k = 0; k < a.Length; k++)
                        {
                            if (a.GetValue(k) == null)
                            {
                                s += "#";
                                continue;
                            }
                            v = (double?)a.GetValue(k);
                            if (v.Value >= 0.0)
                                s += "+";
                            s += ((double)a.GetValue(k)).ToString("e4") + ", ";
                        }
                        s = s.TrimEnd(' ', ',');
                        s += "]";

                    }
                    else
                    {
                        s += "[" + m[i][j].Element.ToString() + "]";
                    }
                }
            }

            return s;
        }

        public override string ToString()
        {
            string s = ""; object e; Array a;

            for (int i = 0; i < Rows; i++)
            {
                s += "\n";

                for (int j = 0; j < Columns; j++)
                {
                    e = m[i][j].Element;
                    if (e is Array)
                    {
                        a = (Array)e;
                        s += "[";
                        for (int k = 0; k < a.Length; k++)
                            s += (a.GetValue(k) == null ? "#" : a.GetValue(k).ToString()) + ", ";
                        s = s.TrimEnd(' ', ',');
                        s += "]";

                    }
                    else
                    {
                        s += "[" + m[i][j].Element.ToString() + "]";
                    }
                }
            }

            return s;
        }

        public void Write<T>(int row, int col, T d)
        {
            if ((row >= Rows) || (col >= Columns))
                throw new Exception();
            m[row][col].SetElement(d);
        }

        /// <summary>
        /// write to indexed location in node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">row of node</param>
        /// <param name="col">col of node</param>
        /// <param name="index">location of element in array in node</param>
        /// <param name="d"></param>
        public void WriteAt<T>(int row, int col, int index, T d)
        {
            if ((row >= Rows) || (col >= Columns))
                throw new Exception();
            ((T[])m[row][col].Element)[index] = d;
        }
    }
}