using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.ANN.Activation
{
    [Serializable]
	public class Signum : Function
    {
        public Signum()
            : base() { }

        public override double? Inverse()
        {
            throw new NotImplementedException();
        }

        public override double Next()
        {
            Field = 0.0;
            for (int i = 0; i < Input.Length; i++)
                Field += Input[i];
            return (Field < 0 ? -1.0 : 1.0);
        }

        public override double Next(double y)
        {
            throw new NotImplementedException();
        }
    }
}
