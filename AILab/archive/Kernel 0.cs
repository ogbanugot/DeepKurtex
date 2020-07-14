using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Node = Foundation.Node;

namespace AI.ML.CNN
{
    public class Kernel : Node
    {
        protected fMap source;
        protected int? padding, size;
        protected Node[] knode;

        public Kernel(fMap source, int? padding, int? size)
        {
            this.padding = (padding == null ? 0 : padding.Value);
            this.size = size;
            this.source = source;
            knode = new Node[size.Value * size.Value];
        }

        public virtual double[] Output(int row, int col)
        {
            SetSourceNodes(row, col);

            double[] output = new double[knode.Length];

            for (int i = 0; i < knode.Length; i++)
                output[i] = ((double?[])knode[i].Element)[ANN.Global.Sig].Value;

            return output;
        }

        public Node[] GetSourceNodes(int row, int col)
        {
            SetSourceNodes(row, col);
            return knode;
        }

        protected virtual void SetSourceNodes(int row, int col)
        {
            int x, y;

            for (int n = 0, r = 0; r < size; r++)
            {
                x = row - padding.Value + r;

                for (int c = 0; c < size; c++)
                {
                    y = col - padding.Value + c;

                    if ((x < 0) || (x >= source.Rows) || (y < 0) || (y >= source.Columns))
                        knode[n++] = null;
                    else
                        knode[n++] = source.GetElement(x, y);
                }
            }
        }
    }
}
