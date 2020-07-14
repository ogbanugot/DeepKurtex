using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.CNN.Lossfunc
{
    [Serializable]
	public class CategoricalCrossEntropy : CNN.Loss
    {
        public override double GetLoss(double[] probs, double[] label)
        {
            if (probs.Count() != label.Count())
                throw new Exception("Size mismatch: Label and probs should be the same ");
            double sum = 0.0;
            double logProb, lbl;

            for (int i = 0; i < probs.Count(); i++)
            {
                logProb = System.Math.Log(probs[i]);
                lbl = label[i];
                sum += lbl * logProb;
            }
            return -sum;
        }
    }
}
