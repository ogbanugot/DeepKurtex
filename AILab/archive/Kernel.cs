using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AI.ML.ANN;

namespace AI.ML.CNN.Layers
{
    public class Kernel : CNN.Kernel
    {
        protected Synapse[,] synapse = null;

        public Kernel(fMap source, int? padding, int? size)
            : base(source, padding, size)
        {
            synapse = new Synapse[size.Value, size.Value];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    synapse[i, j] = new Synapse(null, Math.Daemon.NextGaussianDouble(0.0, 0.004)[0], 0.0);
        }

        public override double[] Output(int row, int col)
        {
            SetSourceNodes(row, col);

            double output = 0.0;

            for (int i = 0; i < synapse.GetLength(0); i++)
                for (int j = 0; j < synapse.GetLength(1); j++)
                    output += synapse[i, j].Output;

            return new double[] { output };
        }

        protected override void SetSourceNodes(int row, int col)
        {
            base.SetSourceNodes(row, col);

            for (int n = 0, r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                    synapse[r, c].Source = knode[n++];
        }

        public Synapse[,] Synapse
        {
            get { return synapse; }
        }

        public double?[][][] Weights
        {
            get
            {
                double?[][][] w = new double?[Synapse.GetLength(0)][][];
                for (int i = 0; i < w.Length; i++)
                {
                    w[i] = new double?[Synapse.GetLength(1)][];
                    for (int j = 0; j < w[i].Length; j++)
                        w[i][j] = Synapse[i, j].Weights;
                }

                return w;
            }
        }

        public virtual object WeightCorrection
        {
            get { return GetElement<double?[][]>(); }
            set { SetElement(value); }
        }
    }
}
