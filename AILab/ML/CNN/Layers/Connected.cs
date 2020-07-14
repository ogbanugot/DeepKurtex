using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Propagate = AI.ML.ANN.Enums.Propagate;

namespace AI.ML.CNN.Layers
{
    [Serializable]
	public class Connected : Layer
    {
        protected ANN.Neuron[] neurons = null;

        public Connected()
            : base() { }

        /// <summary>
        /// configures connected layer
        /// </summary>
        /// <param name="configuration">neu=outputperceptron; act=sigmoid(a=-1, b=2, c=6); nodes=6; fieldsize=2(def:2)</param>
        /// <returns></returns>
        public override Model.Unit Configure(string configuration)
        {
            string cfg = ANN.Global.Parser.RemoveWhiteSpaces(configuration);
            string[] a = ANN.Global.Parser.Split(cfg, ";"), b;
            int nofN = ANN.Global.Parser.Extract<int>(a, new string[] { "nodes" }, ANN.Global.Parser.Option.None, out b);
            string neu = ANN.Global.Parser.Extract<string>(b, new string[] { "neu", "neuron" }, ANN.Global.Parser.Option.None, out a);
            cfg = ANN.Global.Parser.Build(a, 0, ";");

            neurons = new ANN.Neuron[nofN];

            for (int i = 0; i < nofN; i++)
            {
                switch (neu)
                {
                    case "hiddenperceptron":
                        neurons[i] = new ANN.Neurons.Perceptron.Hidden(cfg);
                        break;
                    case "outputperceptron":
                        neurons[i] = new ANN.Neurons.Perceptron.Output(cfg);
                        break;
                }

                neurons[i].SetElement(0.0);
            }

            ANN.Neuron n;

            fmaps.Clear();

            fMap sfmap = Input[0];
            fMap tfmap = (fMap)new fMap()
                .Configure(neurons.Length, 1);

            for (int i = 0; i < neurons.Length; i++)
            {
                n = neurons[i];
                // 1. connect sources
                for (int j = 0; j < sfmap.Rows; j++)
                    n.Source = sfmap.GetElement(j, 0);
                // 2. set target into map
                tfmap.SetElement(i, 0, n.Output);
            }

            fmaps.Add(tfmap);

            return this;
        }

        public virtual Connected Configure<T>(params double?[] funcparams)
            where T : ANN.Function, new()
        {

            return this;
        }

        public ANN.Neuron[] Neurons
        {
            get
            {
                return neurons;
            }
        }

        public override void Next(Propagate p)
        {
            switch (p)
            {
                case Propagate.Error:
                    for (int i = 0; i < neurons.Length; i++)
                        neurons[i].Next(Propagate.Error);
                    break;

                case Propagate.Signal:
                    for (int i = 0; i < neurons.Length; i++)
                        neurons[i].Next(Propagate.Signal);
                    break;
            }
        }
    }
}
