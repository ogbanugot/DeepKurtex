using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hidden = AI.ML.ANN.Neurons.Perceptron.Hidden;
using Node = Foundation.Node;
using Output = AI.ML.ANN.Neurons.Perceptron.Output;

namespace AI.ML.ANN.Models
{
    [Serializable]
	public class Acyclic : Model
    {
        public Acyclic()
            : base() { }

        public Acyclic(string configuration)
            : base()
        {
            Configure(configuration);
        }

        /// <summary>
        /// configures acyclic ANN
        /// </summary>
        /// <typeparam name="T">activation function</typeparam>
        /// <param name="layersize">number of nodes in each layer</param>
        /// <param name="funcparams">activation function parameters</param>
        /// <param name="outputfieldsize">fieldsize of output nodes</param>
        /// <returns>acyclic ANN</returns>
        public virtual Acyclic Configure<T>(int[] layersize, double?[] funcparams, int outputfieldsize)
            where T : Function, new()
        {
            int m = layersize.Length - 1, n;

            // 1. instantiates hidden neurons

            neurons = new Neuron[layersize.Length][];

            for (int i = 0; i < m; i++)
            {
                neurons[i] = new Neuron[layersize[i]];
                for (int j = 0; j < layersize[i]; j++)
                    neurons[i][j] = new Hidden()
                        .Configure<T>(funcparams, outputfieldsize);
            }

            // 2. instantiates output neurons

            neurons[m] = new Neuron[layersize[m]];
            for (int j = 0; j < layersize[m]; j++)
                neurons[m][j] = new Output()
                    .Configure<T>(funcparams, outputfieldsize);

            // 3. connect neurons

            for (int i = 1; i <= m; i++)
            {
                for (int j = 0; j < layersize[i]; j++)
                {
                    n = layersize[i - 1];
                    for (int k = 0; k < n; k++)
                        neurons[i][j].Source = neurons[i - 1][k].Output;
                }
            }

            // 4. instantiate outputs

            Neuron[] outn = neurons.Last();
            output = new Node[outn.Length];
            for (int i = 0; i < outn.Length; i++)
                output[i] = outn[i].Output;

            return this;
        }

        public virtual Acyclic Configure(string configuration)
        {
            // "[neu=hiddenperceptron; act=sigmoid(a=0.34, b=0.45); fieldsize = 2; nodes=2]" //
            string a = "";
            string[] c = configuration.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < c.Length; i++)
                a += c[i];
            char[] del = new char[] { '[', ']' };
            c = a.Split(del, StringSplitOptions.RemoveEmptyEntries);
            string neuron; int n;

            neurons = new Neuron[c.Length][];

            a = getNeuronConfiguration(c[0], out neuron, out n);

            neurons[0] = new Neuron[n];

            for (int i = 0; i < n; i++)
            {
                neurons[0][i] = getNeuron(neuron);
                neurons[0][i].Configure(a);
            }

            for (int i = 1; i < c.Length; i++)
            {
                a = getNeuronConfiguration(c[i], out neuron, out n);

                neurons[i] = new Neuron[n];

                for (int j = 0; j < n; j++)
                {
                    neurons[i][j] = getNeuron(neuron);
                    neurons[i][j].Configure(a);

                    for (int k = 0; k < neurons[i - 1].Length; k++)
                        neurons[i][j].Source = neurons[i - 1][k].Output;
                }
            }

            // set output;
            Neuron[] outN = neurons.Last();
            output = new Node[outN.Length];
            for (int i = 0; i < outN.Length; i++)
                output[i] = outN[i].Output;

            return this;
        }

        protected Neuron getNeuron(string configuration)
        {
            switch (configuration)
            {
                case "hiddenperceptron":
                    return new Neurons.Perceptron.Hidden();
                case "outputperceptron":
                    return new Neurons.Perceptron.Output();
            }

            throw new Exception();
        } 

        protected string getNeuronConfiguration(string configuration, out string neuron, out int n)
        {
            // 0. extract type of neuron
            string[] c = configuration.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string[] l = c.First().Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
            if ((l[0].CompareTo("neu") != 0) && (l[0].CompareTo("neuron") != 0))
                throw new Exception();
            neuron = l[1];

            // 1. extract number of nodes
            l = c.Last().Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
            if (l[0].CompareTo("nodes") != 0)
                throw new Exception();
            n = int.Parse(l[1]);

            // 2. reconstitute neuron configuration string
            string s = "";
            for (int i = 1; i < c.Length - 1; i++)
                s += c[i] + ";";
            s = s.TrimEnd(';');

            return s;
        }

        public static Acyclic Parse(string acyclic)
        {
            return new Acyclic(acyclic);
        }

        /// <summary>
        /// resets output field
        /// </summary>
        public override void ResetOutputField()
        {
            for (int i = 0; i < neurons.Length; i++)
                for (int j = 0; j < neurons[i].Length; j++)
                    ((double?[])neurons[i][j].Output.Element)[Global.Err] = 0.0;

            // output layer to null
            Neuron[] outputLayer = neurons.Last();
            for (int i = 0; i < outputLayer.Length; i++)
                ((double?[])outputLayer[i].Output.Element)[Global.Err] = null;
        }
    }
}