using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.CNN.Kernels
{
    [Serializable]
	public class Avgpool : Kernel
    {
        public Avgpool() { }

        public override object Clone()
        {
            Avgpool clone = new Avgpool();
            clone.Configure(rows, cols, stride.Value, padding.Value);
            return clone;
        }

        public override void Next(double y, int row, int col)
        {
            double x = y / (rows * cols), z;
            GetSourceNodes(row, col);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (sources[i][j] == null)
                        continue;
                    z = ((double?[])sources[i][j].Element)[ANN.Global.Err].Value;
                    ((double?[])sources[i][j].Element)[ANN.Global.Err] = x + z;
                }
            }
        }

        public override double Next(int row, int col)
        {
            GetSourceNodes(row, col);
            double sum = 0.0;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    sum += sources[i][j] == null ? 0.0 : sources[i][j].GetElement<double?[]>()[ANN.Global.Sig].Value;
            sum /= rows * cols;

            return sum;
        }
    }
}
