﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.ANN.Activation
{
    [Serializable]
	public class ReLU : Function
    {
        public ReLU()
            : base() { }

        public override double? Inverse()
        {
            throw new NotImplementedException();
        }

        public override double Next()
        {
            double relu = 0.0;
            for (int i = 0; i < Input.Length; i++)
                relu += Input[i];
            return (relu >= 0.0 ? relu : 0.0);
        }

        public override double Next(double y)
        {
            return (y >= 0.0 ? 1.0 : 0.0);
        }
    }
}
