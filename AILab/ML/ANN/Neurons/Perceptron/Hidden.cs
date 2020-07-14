using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.ANN.Neurons.Perceptron
{
    [Serializable]
	public class Hidden : Neuron
    {
        public Hidden()
         : base() { }

        public Hidden(string configuration)
            : base(configuration) { }

        /// <summary>
        /// propagate error
        /// </summary>
        protected override void propErr()
        {
            Synapse s;

            double u = function.Next(((double?[])Output.Element)[Global.Sig].Value);
            double v = ((double?[])Output.Element)[Global.Err].Value;

            Gradient = u * v;

            s = Synapse[0];
            s.dW = Gradient.Value;
            for (int i = 1; i < Synapse.Count; i++)
            {
                s = Synapse[i];
                if (((double?[])s.Source.Element)[Global.Err] == null)
                    ((double?[])s.Source.Element)[Global.Err] = 0;

                ((double?[])s.Source.Element)[Global.Err] += Gradient.Value * s.W.Value;
                s.dW = Gradient.Value * ((double?[])s.Source.Element)[Global.Sig].Value;
            }
        }
    }
}