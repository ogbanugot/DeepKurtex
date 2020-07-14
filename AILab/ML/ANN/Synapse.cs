using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Foundation;

namespace AI.ML.ANN
{
    [Serializable]
	public class Synapse
    {
        private Node source;
        private double?[] weight;

        public Synapse(Node source, params double?[] weight)
        {
            this.source = source;
            if (weight.Length < 2)
                throw new Exception();
            this.weight = weight;
        }

        public double Output
        {
            get
            {
                double src = (source == null ? 0.0 : ((double?[])source.Element)[Global.Sig].Value);
                return src * W.Value;
            }
        }

        public Node Source
        {
            get { return source; }
            set { source = value; }
        }

        /// <summary>
        /// weight
        /// </summary>
        public double? W
        {
            get { return weight[0]; }
            set { weight[0] = value.Value; }
        }

        public double?[] Weights
        {
            get { return weight; }
        }

        /// <summary>
        /// weight change (delta W)
        /// </summary>
        public double? dW
        {
            get { return weight[1]; }
            set { weight[1] = value.Value; }
        }

        public virtual double? this[int i]
        {
            get { return weight[i]; }
            set { weight[i] = value.Value; }
        }
    }
}
