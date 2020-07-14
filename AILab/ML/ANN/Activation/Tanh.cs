using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.ANN.Activation
{
    [Serializable]
	public class Tanh : Function
    {
        public Tanh()
            : base() { }

        public override Function Configure(double?[] constants)
        {
            this.constants[0] = constants[0];
            this.constants[1] = constants[1];
            this.constants[2] = this["b"] / this["a"];

            return this;
        }

        public override Function Configure(string configuration)
        {
            base.Configure(configuration);
            constants[2] = this["b"] / this["a"];

            return this;
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
            double a = this["b"] * Field.Value;
            return this["a"] * System.Math.Tanh(a);
        }

        public override double Next(double y)
        {
            double d = this["c"] * (this["a"] - y) * (this["a"] + y);
            return d;
        }
    }
}