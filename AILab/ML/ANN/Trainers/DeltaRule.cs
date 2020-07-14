using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.ANN.Trainers
{
    [Serializable]
	public class DeltaRule : Trainer
    {
        private double? learningRate = null, momentum = null;

        public DeltaRule()
            : base() { }

        protected void adjustWeights()
        {
            Neuron n; Synapse s;

            for (int i = 0; i < neurons.Length; i++)
            {
                for (int j = 0; j < neurons[i].Length; j++)
                {
                    n = neurons[i][j];
                    for (int k = 0; k < n.Synapse.Count; k++)
                    {
                        s = n.Synapse[k];
                        s.dW = (Momentum * s.dW) + (LearningRate * n.Gradient * ((double?[])s.Source.Element)[Global.Sig].Value);
                        s.W += s.dW;
                    }
                }
            }
        }

        public virtual Trainer Configure(Model model, int? epochs, double[][] dataSet, double? learningRate, double? momentum)
        {
            base.Configure(model, epochs, dataSet);

            // 0. assert learningRate and momentum
            if ((learningRate == null) || (momentum == null))
                throw new Exception();

            this.learningRate = learningRate;
            this.momentum = momentum;

            return this;
        }

        public double? LearningRate
        {
            get { return learningRate; }
        }

        public double? Momentum
        {
            get { return momentum; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="epochs"></param>
        /// <param name="dataSet"></param>
        public override string Next()
        {
            string log = "";

            // 0. validate parameters
            if ((learningRate == null) || (momentum == null))
                throw new Exception();

            // 2. initialize weight
            setWeights();

            // 3. configure randomizer...
            int[] dataSetRandomizer = new int[nofSet];

            log = "\nTraining data...";

            // 4. train epoch by epoch...
            for (int i = 0; i < Epochs; i++)
            {

                log += "\n\n////////// Epoch[" + i + "]... ///////////////////////////////////////////////////////";

                Global.NextIntArray(0, nofSet, dataSetRandomizer);

                log += "\n\nDataSet randomizer sequence: ";
                for (int s = 0; s < dataSetRandomizer.Length; s++)
                    log += dataSetRandomizer[s] + ", ";
                log = log.TrimEnd(new char[] { ',', ' ' });

                // single epoch
                for (int j = 0; j < nofSet; j++)
                {
                    // 4.0 set inputs
                    Model.SetInput(DataSet[dataSetRandomizer[j] * 2]);
                    log += "\n\nModel: " + Model.ToString();
                    // 4.1 propagate signal
                    Model.PropagateSignal();
                    log += "\n\nModel: " + Model.ToString();

                    // 4.2 reset output fields
                    Model.ResetOutputField();
                    log += "\n\nModel: " + Model.ToString();

                    // 4.3 set targets
                    Model.SetTarget(DataSet[(dataSetRandomizer[j] * 2) + 1]);

                    // 4.4 propagate signals and set new weights
                    Model.PropagateError();

                    // 4.5 adjust weights
                    adjustWeights();

                    log += "\n" + Model.ToString();
                }
            }

            return log;
        }

        protected virtual void setWeights()
        {
            Synapse s;

            for (int i = 0; i < neurons.Length; i++)
            {
                for (int j = 0; j < neurons[i].Length; j++)
                {
                    for (int k = 0; k < neurons[i][j].Synapse.Count; k++)
                    {
                        s = neurons[i][j].Synapse[k];
                        s.W = Math.Daemon.NextGaussianDouble(0.0, 0.5)[0];
                        s.dW = 0.0;
                    }
                }
            }
        }
    }
}