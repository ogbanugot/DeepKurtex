using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.ANN.Activation
{
    [Serializable]
	public class Minpool : Function
    {
        public Minpool()
            : base() { }

        public override double? Inverse()
        {
            throw new NotImplementedException();
        }

        public override double Next()
        {
            double min = double.MaxValue;
            for (int i = 0; i < Input.Length; i++)
                min = System.Math.Min(Input[i], min);
            return min;
        }

        public override double Next(double y)
        {
            throw new NotImplementedException();
        }
    }
}
