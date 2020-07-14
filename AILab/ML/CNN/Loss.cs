using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.CNN
{
    [Serializable]
    public abstract class Loss
    {
        public Loss() { }

        public abstract double GetLoss(double[] probs, double[] label);

    }
}
