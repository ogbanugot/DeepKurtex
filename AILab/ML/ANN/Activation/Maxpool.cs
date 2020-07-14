using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.ANN.Activation
{
    [Serializable]
	public class Maxpool : Function
    {
        public Maxpool()
            : base() { }

        public override double? Inverse()
        {
            throw new NotImplementedException();
        }

        public override double Next()
        {
            double max = double.MinValue;
            for (int i = 0; i < Input.Length; i++)
                max = System.Math.Max(Input[i], max);
            return max;
        }

        public override double Next(double y)
        {
            throw new NotImplementedException();
        }
    }
}
