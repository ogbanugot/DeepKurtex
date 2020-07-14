using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.CNN.Kernels
{
    [Serializable]
	public class Minpool : Kernel
    {
        public Minpool() { }

        public override object Clone()
        {
            Minpool clone = new Minpool();
            clone.Configure(rows, cols, stride.Value, padding.Value);
            return clone;
        }

        public override void Next(double y, int row, int col)
        {
            double min = double.MaxValue, x; int m = 0, n = 0;
            GetSourceNodes(row, col);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (sources[i][j] == null)
                        continue;
                    x = sources[i][j].GetElement<double?[]>()[ANN.Global.Sig].Value;
                    if (min > x)
                    {
                        min = x;
                        m = i; n = j;
                    }
                }
            }

            min = ((double?[])sources[m][n].Element)[ANN.Global.Err].Value + y;
            ((double?[])sources[m][n].Element)[ANN.Global.Err] = min;
        }

        public override double Next(int row, int col)
        {
            double min = double.MaxValue;
            GetSourceNodes(row, col);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (sources[i][j] == null)
                        continue;
                    min = System.Math.Min(min, sources[i][j].GetElement<double?[]>()[ANN.Global.Sig].Value);
                }
            }

            return min;
        }
    }
}
