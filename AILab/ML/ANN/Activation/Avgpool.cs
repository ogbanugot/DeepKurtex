using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.ANN.Activation
{
    [Serializable]
	public class Avgpool : Function
    {
        public Avgpool()
            : base() { }

        public override double? Inverse()
        {
            throw new NotImplementedException();
        }

        public override double Next()
        {
            double sum = 0.0;
            for (int i = 0; i < Input.Length; i++)
                sum = Input[i];
            return sum / Input.Length;
        }

        public override double Next(double y)
        {
            return y / Input.Length;
        }
    }
}