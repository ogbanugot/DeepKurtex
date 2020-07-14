using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Node = Foundation.Node;

namespace AI.ML.ANN
{
    [Serializable]
    public abstract class Model
    {
        protected Neuron[][] neurons;

        protected Node[] input;
        protected Node[] output;

        public Model()
        {

        }

        /// <summary>
        /// get layer of neurons with zero index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Neuron[] GetLayer(int index)
        {
            return neurons[index];
        }

        public Node[] Input
        {
            get { return input; }
            set
            {
                input = value;

                for (int i = 0; i < neurons[0].Length; i++)
                {
                    for (int j = 0; j < value.Length; j++)
                        neurons[0][i].Source = value[j];
                }
            }
        }

        public Neuron[][] Neurons
        {
            get { return neurons; }
        }

        public Node[] Output
        {
            get { return output; }
        }

        /// <summary>
        /// propagates error backwards
        /// </summary>
        public virtual void PropagateError()
        {
            for (int i = neurons.Length - 1; i >= 0; i--)
                for (int j = 0; j < neurons[i].Length; j++)
                    neurons[i][j].Next(Enums.Propagate.Error);
        }

        /// <summary>
        /// propagates signal forward
        /// </summary>
        public virtual void PropagateSignal()
        {
            for (int i = 0; i < neurons.Length; i++)
                for (int j = 0; j < neurons[i].Length; j++)
                    neurons[i][j].Next(Enums.Propagate.Signal);
        }

        public abstract void ResetOutputField();

        public virtual void SetInput(double[] input)
        {
            // test for mismatch
            for (int i = 0; i < input.Length; i++)
                ((double?[])this.input[i].Element)[Global.Sig] = input[i];
        }

        public virtual void SetTarget(double[] target)
        {
            // test for mismatch
            for (int i = 0; i < output.Length; i++)
                ((double?[])output[i].Element)[Global.Err] = target[i];
        }

        public override string ToString()
        {
            string s = "";

            for (int i = 0; i < neurons.Length; i++)
            {
                s += "\n> Layer[" + i.ToString("00") + "]...";

                for (int j = 0; j < neurons[i].Length; j++)
                {
                    s += "\nneuron[" + j.ToString("00") + "]\n";
                    s += neurons[i][j].ToString();
                }
            }

            return s;
        }
    }
}