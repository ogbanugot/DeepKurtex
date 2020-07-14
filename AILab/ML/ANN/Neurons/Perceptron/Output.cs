using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.ANN.Neurons.Perceptron
{
    [Serializable]
	public class Output : Neuron
    {
        public Output()
         : base() { }

        public Output(string configuration)
            : base(configuration) { }

        /// <summary>
        /// propagate error
        /// </summary>
        protected override void propErr()
        {
            double drv = function.Next(((double?[])Output.Element)[Global.Sig].Value);
            double?[] ele = (double?[])Output.Element;

            Gradient = (ele[Global.Sig].Value - ele[Global.Err].Value) * drv;

            Synapse s; double v;

            s = Synapse[0];
            s.dW = Gradient.Value;
            for (int i = 1; i < Synapse.Count; i++)
            {
                s = Synapse[i];
                //v = ((double?[])s.Source.Element)[Global.Sig].Value + (Gradient.Value * s.W.Value);
                v = Gradient.Value * s.W.Value;
                if (((double?[])s.Source.Element)[Global.Err] == null)
                    ((double?[])s.Source.Element)[Global.Err] = 0;
                ((double?[])s.Source.Element)[Global.Err] += v;
                s.dW = Gradient.Value * ((double?[])s.Source.Element)[Global.Sig].Value;
            }
        }
    }
}