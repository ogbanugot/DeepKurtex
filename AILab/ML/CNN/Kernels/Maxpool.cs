using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.CNN.Kernels
{
    [Serializable]
	public class Maxpool : Kernel
    {
        public Maxpool() { }

        public override object Clone()
        {
            Maxpool clone = new Maxpool();
            clone.Configure(rows, cols, stride.Value, padding.Value);
            return clone;
        }

        public override void Next(double y, int row, int col)
        {
            double max = double.MinValue, x; int m = 0, n = 0;
            GetSourceNodes(row, col);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (sources[i][j] == null)
                        continue;
                    x = sources[i][j].GetElement<double?[]>()[ANN.Global.Sig].Value;
                    if (max < x)
                    {
                        max = x;
                        m = i; n = j;
                    }
                }
            }

            max = ((double?[])sources[m][n].Element)[ANN.Global.Err].Value + y;
            ((double?[])sources[m][n].Element)[ANN.Global.Err] = max;
        }

        public override double Next(int row, int col)
        {
            double max = double.MinValue;
            GetSourceNodes(row, col);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (sources[i][j] == null)
                        continue;
                    max = System.Math.Max(max, sources[i][j].GetElement<double?[]>()[ANN.Global.Sig].Value);
                }
            }

            return max;
        }
    }
}