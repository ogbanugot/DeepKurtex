using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI.Core;
using AI.ML.ANN;
using AI.ML.ANN.Enums;
using Foundation;
using GrayImage = AI.ML.CNN.Images.Gray;
using ColorImage = AI.ML.CNN.Images.Color;
using System.Diagnostics;


namespace AI.ML.CNN.Trainers
{
    [Serializable]
	public class DeltaRule : Trainer
    {
        private double? learningRate = null, momentum = null;
        protected new string log = "";
        private double loss;
        private double[] probs;
        private Loss lossfunc;
        
        public DeltaRule()
            : base() { }

        protected void AdjustWeights()
        {
            for (int i = 1; i < Model.Layers.Length; i++)
            {
                Model.Unit lyr;
                lyr = Model.Layers[i];
                bool convtype = lyr.GetType().ToString() == "AI.ML.CNN.Layers.Convolution" ? true : false;
                bool conctype = lyr.GetType().ToString() == "AI.ML.CNN.Layers.Connected" ? true : false;

                switch (convtype)
                {
                    case true:
                        CNN.Layers.Convolution convLyr = (CNN.Layers.Convolution)lyr;                        
                        AI.ML.CNN.Layers.Convolution.Kernel krn;
                        double?[][][] weights;
                        double?[][] wc;
                        for (int j = 0; j < convLyr.Filters.Count; j++)
                        {

                            krn = (AI.ML.CNN.Layers.Convolution.Kernel)((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[j]).Kernels[0];
                            wc = krn.WeightCorrection;
                            krn.Weights[0][0][0] += learningRate.Value * wc[0][0];

                            //dW = (Momentum.Value * krn.Weights[0][0][2]) + (LearningRate.Value * wc[0][0]);
                            //krn.Weights[0][0][2] = dW;
                            //krn.Weights[0][0][0] += dW;                           

                            // j: indexing filters
                            for (int k = 1; k < ((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[j]).Kernels.Length; k++)
                            {
                                //k: indexing kernels
                                krn = (AI.ML.CNN.Layers.Convolution.Kernel)((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[j]).Kernels[k];
                                weights = krn.Weights;
                                wc = krn.WeightCorrection;
                                for (int m = 0; m < weights.Length; m++)
                                {
                                    for (int n = 0; n < weights[m].Length; n++)
                                    {
                                        
                                        //dW = (Momentum.Value * krn.Weights[m][n][2]) + (LearningRate.Value * wc[m][n]);
                                        //krn.Weights[m][n][2] = dW;
                                        krn.Weights[m][n][0] += learningRate.Value * wc[m][n];                                        
                                    }
                                }
                            }
                        }
                        break;
                    case false:
                        switch (conctype)
                        {
                            case true:
                                CNN.Layers.Connected connLyr = (CNN.Layers.Connected)lyr;
                                Neuron n; Synapse syn;
                                for (int j = 0; j < connLyr.Neurons.Length; j++)
                                {
                                    n = connLyr.Neurons[j];
                                    for (int k = 0; k < n.Synapse.Count; k++)
                                    {
                                        syn = n.Synapse[k];
                                        syn.W -= learningRate * syn.dW;                                        
                                    }
                                }
                                break;

                            case false:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }


        public virtual Trainer Configure<T>(Model model, int? epochs, fDataSet dataSet, double? learningRate, double? momentum)
            where T : CNN.Loss, new() 
        {
            base.Configure(model, epochs, dataSet);

            // 0. assert learningRate and momentum
            if ((learningRate == null) || (momentum == null))
                throw new Exception();

            this.learningRate = learningRate;
            this.momentum = momentum;
            this.lossfunc = new T(); 
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

        public override string Next()
        {
            // 0. validate parameters
            if ((learningRate == null) || (momentum == null))
                throw new Exception();

            // 3. configure randomizer...
            int[] dataSetRandomizer = new int[nofSet];

            log += "\nTraining data...";
            // 4. train epoch by epoch...
            for (int i = 0; i < Epochs; i++)
            {
                Console.WriteLine("Start");

                log += "\n\n////////// Epoch[" + i + "]... ///////////////////////////////////////////////////////";

                Global.NextIntArray(0, nofSet, dataSetRandomizer);

                //log += "\n\nDataSet randomizer sequence: ";
                //for (int s = 0; s < dataSetRandomizer.Length; s++)
                //    log += dataSetRandomizer[s] + ", ";
                //log = log.TrimEnd(new char[] { ',', ' ' });

                // single epoch
                double mean_loss = 0;
                for (int j = 0; j < nofSet; j++)
                {                 

                    if (Model.Input.Output.Count == 3)
                    {
                        ColorImage inputlayer = (ColorImage)Model.Input;
                        inputlayer.fData = DataSet.fData[dataSetRandomizer[j]];
                    }
                    else
                    {
                        GrayImage inputlayer = (GrayImage)Model.Input;
                        inputlayer.fData = DataSet.fData[dataSetRandomizer[j]];
                    }
                   
                    //log += "\n\nModel: " + Model.ToString();

                    // 4.1 propagate signal
                    //DisplayWeights();
                    Model.Next(Propagate.Signal);
                    //log += "\n\nModel: " + Model.ToString();

                    // 4.2 set targets
                    Layer lyr = Model.Output;
                    Node node;
                    probs = new double[lyr.Output[0].Rows];
                    for (int l = 0; l < DataSet.fData[dataSetRandomizer[j]].Label.Length; l++)
                    {
                        node = lyr.Output[0].GetElement(l, 0);
                        probs[l] = (double)((double?[])node.Element)[Global.Sig];
                        ((double?[])node.Element)[Global.Err] = DataSet.fData[dataSetRandomizer[j]].Label[l];
                    }

                    //Set softmax output
                    double[] softmaxOutput = Softmax(probs);
                    for (int l = 0; l < probs.Length; l++)
                    {
                        node = lyr.Output[0].GetElement(l, 0);
                        ((double?[])node.Element)[Global.Sig] = softmaxOutput[l];
                    }

                    // 4.4 propagate error and set new weights
                    Model.Next(Propagate.Error);

                    // 4.5 adjust weights
                    AdjustWeights();

                    //4.6 Calculate loss
                    loss = lossfunc.GetLoss(softmaxOutput, DataSet.fData[dataSetRandomizer[j]].Label);
                    mean_loss += loss; 
                    //log += "\n\nEpoch[" + i.ToString() + "]" + "Image[" + j.ToString() + "]" + "Loss: " + loss.ToString("e4");
                    Console.WriteLine("Epoch[" + i.ToString() + "]" + "Batch[" + j.ToString() + "]" + "Loss: " + loss.ToString("e4"));
                    //log += "\n" + Model.ToString();
                }
                Console.WriteLine("Epoch[" + i.ToString() + "]" + "Loss: " + (mean_loss/nofSet).ToString("e4"));
            }
            return log;
        }

        public string NextVerbose()
        {
            // 0. validate parameters
            if ((learningRate == null) || (momentum == null))
                throw new Exception();

            // 3. configure randomizer...
            int[] dataSetRandomizer = new int[nofSet];

            log = "\nTraining data...";

            // 4. train epoch by epoch...
            for (int i = 0; i < Epochs; i++)
            {

                log += "\n\n////////// Epoch[" + i + "]... ///////////////////////////////////////////////////////";

                Global.NextIntArray(0, nofSet, dataSetRandomizer);

                //log += "\n\nDataSet randomizer sequence: ";
                //for (int s = 0; s < dataSetRandomizer.Length; s++)
                //    log += dataSetRandomizer[s] + ", ";
                //log = log.TrimEnd(new char[] { ',', ' ' });

                // single epoch
                for (int j = 0; j < nofSet; j++)
                {
                    // 4.0 set inputs
                    GrayImage inputlayer = (GrayImage)Model.Input;

                    inputlayer.fData = DataSet.fData[dataSetRandomizer[j]];
                    //log += "\n\nModel: " + Model.ToString();

                    // 4.1 propagate signal
                    //DisplayWeights();
                    Model.Next(Propagate.Signal);
                    //log += "\n\nModel: " + Model.ToString();

                    // 4.2 set targets
                    Layer lyr = Model.Output;
                    Node node;
                    probs = new double[lyr.Output[0].Rows];
                    for (int l = 0; l < DataSet.fData[dataSetRandomizer[j]].Label.Length; l++)
                    {
                        node = lyr.Output[0].GetElement(l, 0);
                        probs[l] = (double)((double?[])node.Element)[Global.Sig];
                        ((double?[])node.Element)[Global.Err] = DataSet.fData[dataSetRandomizer[j]].Label[l];
                    }

                    //Set softmax output
                    double[] softmaxOutput = Softmax(probs);
                    for (int l = 0; l < probs.Length; l++)
                    {
                        node = lyr.Output[0].GetElement(l, 0);
                        ((double?[])node.Element)[Global.Sig] = softmaxOutput[l];
                    }

                    // 4.4 propagate error and set new weights
                    Model.Next(Propagate.Error);

                    // 4.5 adjust weights
                    AdjustWeights();

                    //4.6 Calculate loss
                    loss = lossfunc.GetLoss(softmaxOutput, DataSet.fData[dataSetRandomizer[j]].Label);
                    log = "\n\nEpoch[" + i.ToString() + "]" + "Image[" + j.ToString() + "]" + "Loss: " + loss.ToString("e4");
                    //Console.WriteLine("Epoch[" + i.ToString() + "]" + "Batch[" + j.ToString() + "]" + "Loss: " + loss.ToString("e4"));
                    //log += "\n" + Model.ToString();
                }
            }
            return log;
        }

        protected void DisplayWeights()
        {
            int fltCount, krnLength;
            for (int i = 1; i < 3; i++)
            {
                Model.Unit lyr;
                lyr = Model.Layers[i];
                AI.ML.CNN.Layers.Convolution convLyr = (AI.ML.CNN.Layers.Convolution)lyr;
                double?[][][][][] w = convLyr.Weights;
                AI.ML.CNN.Layers.Convolution.Kernel krn;
                fltCount = convLyr.Filters.Count;
                for (int q = 0; q < fltCount; q++)
                {
                    krnLength = ((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[q]).Kernels.Length;
                    // i: indexing filters
                    for (int j = 0; j < krnLength; j++)
                    {
                        // j: indexing kernels
                        log += "\n\nkernel[" + q.ToString("00") + "][" + j.ToString("00") + "]...";
                        krn = (AI.ML.CNN.Layers.Convolution.Kernel)((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[q]).Kernels[j];
                        log += "\n";
                        for (int k = 0; k < krn.Weights.Length; k++)
                        {
                            for (int m = 0; m < krn.Weights[k].Length; m++)
                            {
                                //for (int n = 0; n < krn.Weights[k][m].Length; n++)
                                log += krn.Weights[k][m][0].ToString() + " ";
                            }
                        }
                    }
                }
            }

        }
        protected void SaveError()
        {

            for (int i = 1; i < Model.Layers.Length; i++)
            {
                Model.Unit lyr;
                lyr = Model.Layers[i];
                bool convtype = lyr.GetType().ToString() == "AI.ML.CNN.Layers.Convolution" ? true : false;
                bool conctype = lyr.GetType().ToString() == "AI.ML.CNN.Layers.Connected" ? true : false;

                switch (convtype)
                {
                    case true:
                        Layers.Convolution convLyr = (Layers.Convolution)lyr;
                        Layers.Convolution.Kernel krn;
                        for (int j = 0; j < convLyr.Filters.Count; j++)
                        {
                            krn = (AI.ML.CNN.Layers.Convolution.Kernel)((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[j]).Kernels[0];
                            //bwc = (double?)krn.Element;
                            krn.Weights[0][0][2] += krn.WeightCorrection[0][0].Value;
                            // j: indexing filters
                            for (int k = 1; k < ((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[j]).Kernels.Length; k++)
                            {
                                //k: indexing kernels
                                krn = (AI.ML.CNN.Layers.Convolution.Kernel)((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[j]).Kernels[k];
                                for (int m = 0; m < krn.Weights.Length; m++)
                                {
                                    for (int n = 0; n < krn.Weights.Length; n++)
                                    {
                                        krn.Weights[m][n][2] += krn.WeightCorrection[m][n].Value;
                                    }
                                }
                            }
                        }
                        break;
                    case false:
                        switch (conctype)
                        {
                            case true:
                                Layers.Connected connLyr = (Layers.Connected)lyr;
                                Neuron n; Synapse syn;
                                for (int j = 0; j < connLyr.Neurons.Length; j++)
                                {
                                    n = connLyr.Neurons[j];
                                    n.Synapse[0].Weights[2] += n.Synapse[0].dW.Value;
                                    for (int k = 1; k < n.Synapse.Count; k++)
                                    {
                                        syn = n.Synapse[k];
                                        syn.Weights[2] += syn.dW.Value;
                                    }
                                }
                                break;

                            case false:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        protected void ClearError()
        {

            for (int i = 1; i < Model.Layers.Length; i++)
            {
                Model.Unit lyr;
                lyr = Model.Layers[i];
                bool convtype = lyr.GetType().ToString() == "AI.ML.CNN.Layers.Convolution" ? true : false;
                bool conctype = lyr.GetType().ToString() == "AI.ML.CNN.Layers.Connected" ? true : false;

                switch (convtype)
                {
                    case true:
                        Layers.Convolution convLyr = (Layers.Convolution)lyr;
                        Layers.Convolution.Kernel krn;
                        for (int j = 0; j < convLyr.Filters.Count; j++)
                        {
                            krn = (AI.ML.CNN.Layers.Convolution.Kernel)((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[j]).Kernels[0];
                            krn.Weights[0][0][2] = 0;
                            // j: indexing filters
                            for (int k = 1; k < ((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[j]).Kernels.Length; k++)
                            {
                                //k: indexing kernels
                                krn = (AI.ML.CNN.Layers.Convolution.Kernel)((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[j]).Kernels[k];
                                for (int m = 0; m < krn.Weights.Length; m++)
                                {
                                    for (int n = 0; n < krn.Weights.Length; n++)
                                    {
                                        krn.Weights[m][n][2] = 0;
                                    }
                                }
                            }
                        }
                        break;
                    case false:
                        switch (conctype)
                        {
                            case true:
                                Layers.Connected connLyr = (Layers.Connected)lyr;
                                Neuron n; Synapse syn;
                                for (int j = 0; j < connLyr.Neurons.Length; j++)
                                {
                                    n = connLyr.Neurons[j];
                                    n.Synapse[0].Weights[2] = 0;
                                    for (int k = 1; k < n.Synapse.Count; k++)
                                    {
                                        syn = n.Synapse[k];
                                        syn.Weights[2] = 0;
                                    }
                                }
                                break;

                            case false:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        public double[] Softmax(double[] output)
        {
            double[] vs = new double[output.Length];
            double[] softmax = new double[output.Length];

            for (int i = 0; i < output.Length; i++)
            {
                vs[i] = System.Math.Exp(output[i]);
            }
            var total = vs.Sum();
            for (int i = 0; i < vs.Length; i++)
            {
                softmax[i] = vs[i] / total;
            }
            return softmax;
        }
    }
}
