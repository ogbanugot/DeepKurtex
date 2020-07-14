using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.CNN.Lossfunc
{
    [Serializable]
	public class MeanSquaredError : CNN.Loss
    {
        public override double GetLoss(double[] probs, double[] label)
        {
            if (probs.Count() != label.Count())
                throw new Exception("Size mismatch: Label and probs should be the same ");
            double count = probs.Count();
            double sum = 0.0, diff, diffSquared, mse;
            for (int i = 0; i < probs.Count(); i++)
            {
                diff = label[i] - probs[i];
                diffSquared = System.Math.Pow(diff, 2);
                sum += diffSquared;
            }
            double N = 1 / count;
            mse = N * sum;
            return mse;
        }
    }
}
