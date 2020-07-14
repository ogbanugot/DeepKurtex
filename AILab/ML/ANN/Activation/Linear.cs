using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.ANN.Activation
{
    public class Linear : Function
    {
        public Linear()
            : base() { }

        public override double? Inverse()
        {
            throw new NotImplementedException();
        }

        public override double Next()
        {
            double linear = 0.0;
            for (int i = 0; i < Input.Length; i++)
                linear += Input[i];
            return linear;
        }

        public override double Next(double y)
        {
            return 1;
        }
    }
}
