using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.CNN
{
    [Serializable]
    public abstract class Matrix<T>
        where T : new()
    {
        protected T[][] m;
        protected int rows, cols;

        public Matrix() { }

        public virtual Matrix<T> Configure(int rows, int cols)
        {
            m = new T[rows][];

            for (int i = 0; i < rows; i++)
                m[i] = new T[cols];

            this.rows = rows;
            this.cols = cols; 

            return this;
        }

        public int Columns
        {
            get { return cols; }
        }

        public T GetElement(int i, int j)
        {
            return m[i][j];
        }

        public int Rows
        {
            get { return rows; }
        }

        public void SetElement(int i, int j, T element)
        {
            m[i][j] = element;
        }
    }
}