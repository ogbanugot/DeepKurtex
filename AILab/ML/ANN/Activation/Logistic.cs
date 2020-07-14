using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.ANN.Activation
{
    [Serializable]
	public class Logistic : Function
    {
        public Logistic()
            : base() { }

        public override double Next(double y)
        {
            //if (Field == null)
            //    throw new Exception();

            double x = System.Math.Exp(0.0 - y);
            double z = System.Math.Pow(1.0 + x, 2.0);

            //Field = null;

            return x / z;
        }

        public override double? Inverse()
        {
            throw new NotImplementedException();
        }

        public override double Next()
        {
            Field = 0.0;
            for (int i = 0; i < Input.Length; i++)
                    Field += Input[i];

            double sigm = 1.0 + System.Math.Exp(-1.0 * constants[2].Value * Field.Value);
            sigm = constants[1].Value / sigm;
            return constants[0].Value + sigm;
        }
    }
}