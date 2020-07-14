using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI.ML.ANN.Enums;
using AI.ML.ANN;
using Foundation;
using GrayImage = AI.ML.CNN.Images.Gray;
using AI.ML.CNN;
using AI.ML.CNN.Layers;

namespace AI.ML.CNN.Trainers
{
    [Serializable]
	public class Verbose : Trainer
    {
        private double? learningRate = null, momentum = null;
        protected new string log = "";
        private double batchLoss, singleLoss;
        private double[] probs;
        private int? batchSize = null;
        private Loss lossfunc;
        double? beta1;
        double? beta2;
        private Connected hiddLyr = null;
        private Connected outpLyr = null;

        private Concatenation concLyr = null;
        private Convolution convLyr = null;
        private Convolution convLyr1 = null;
        private Pooling poolLyr = null;
        private GrayImage imagLyrG = null;

        IList<Core.fDataSet> batchImages = new List<Core.fDataSet>();
        public Verbose()
            : base() { }

        protected void AdjustWeights(Layer lyr)
        {            
            bool convtype = lyr.GetType().ToString() == "AI.ML.CNN.Layers.Convolution" ? true : false;
            bool conctype = lyr.GetType().ToString() == "AI.ML.CNN.Layers.Connected" ? true : false;

            switch (convtype)
            {
                case true:
                    CNN.Layers.Convolution convLyr = (CNN.Layers.Convolution)lyr;
                    double wv;
                    double ws, diff, gradient;
                    AI.ML.CNN.Layers.Convolution.Kernel krn;
                    double?[][][] weights;
                    for (int j = 0; j < convLyr.Filters.Count; j++)
                    {
                        krn = (AI.ML.CNN.Layers.Convolution.Kernel)((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[j]).Kernels[0];
                        weights = krn.Weights;
                        wv = 0;
                        ws = 0;
                        wv = (beta1.Value * wv) + (1 - beta1.Value) * (weights[0][0][2].Value / batchSize.Value);
                        ws = (beta2.Value * ws) + (1 - beta2.Value) * System.Math.Pow((weights[0][0][2].Value / batchSize.Value), 2); ;
                        gradient = learningRate.Value * (wv / System.Math.Sqrt(ws + 1e-7));
                        diff = weights[0][0][0].Value - gradient;
                        krn.Weights[0][0][0] = diff;


                        // j: indexing filters
                        for (int k = 1; k < ((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[j]).Kernels.Length; k++)
                        {
                            //k: indexing kernels
                            krn = (AI.ML.CNN.Layers.Convolution.Kernel)((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[j]).Kernels[k];
                            weights = krn.Weights;
                            for (int m = 0; m < weights.Length; m++)
                            {
                                for (int n = 0; n < weights[m].Length; n++)
                                {

                                    wv = 0;
                                    ws = 0;
                                    wv = (beta1.Value * wv) + (1 - beta1.Value) * (weights[m][n][2].Value / batchSize.Value);
                                    ws = (beta2.Value * ws) + (1 - beta2.Value) * System.Math.Pow((weights[m][n][2].Value / batchSize.Value), 2);
                                    gradient = learningRate.Value * (wv / System.Math.Sqrt(ws + 1e-7));
                                    diff = weights[m][n][0].Value - gradient;
                                    krn.Weights[m][n][0] = diff;
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
                                    wv = 0;
                                    ws = 0;
                                    wv = (beta1.Value * wv) + (1 - beta1.Value) * (syn.Weights[2].Value / batchSize.Value);
                                    ws = (beta2.Value * ws) + (1 - beta2.Value) * System.Math.Pow((syn.Weights[2].Value / batchSize.Value), 2);
                                    gradient = learningRate.Value * (wv / System.Math.Sqrt(ws + 1e-7));
                                    diff = syn.W.Value - gradient;
                                    syn.W = diff;
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

        public virtual Trainer Configure<T>(Model model, int? epochs, Core.fDataSet dataSet, double? learningRate, double? momentum, int? batchSize, double? beta1, double? beta2)
            where T : CNN.Loss, new()
        {
            this.batchSize = batchSize;
            List<Core.fData> dataList = (List<Core.fData>)dataSet.fData;
            int index = 0, count;
            while (index < dataList.Count())
            {
                count = (int)(dataList.Count() - index > batchSize ? batchSize : dataList.Count() - index);
                Core.fDataSet data = new Core.fDataSet();
                data.fData = dataList.GetRange(index, count);
                batchImages.Add(data);
                index += (int)batchSize;
            }
            base.Configure(model, epochs, batchImages[0]);

            // 0. assert learningRate and momentum
            if ((learningRate == null) || (momentum == null) || (beta1 == null) || (beta2 == null))
                throw new Exception();

            this.learningRate = learningRate;
            this.momentum = momentum;
            this.beta1 = beta1;
            this.beta2 = beta2;
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
            int[] batchRandomizer = new int[batchImages.Count()];
            int[] dataSetRandomizer = new int[nofSet];
            log += "\nTraining data...";
            imagLyrG = new GrayImage();
            imagLyrG.Configure(28, 28, -1.0, 1.0);
            imagLyrG.fData = batchImages[0].fData[0];

            convLyr = new Convolution();
            convLyr.Input = imagLyrG.Output;
            convLyr.Configure("depth=8; activation=relu(void); kernel=convolution(size = 5, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2");

            convLyr1 = new Convolution();
            convLyr1.Input = convLyr.Output;
            convLyr1.Configure("depth=8; activation=relu(void); kernel=convolution(size = 5, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2");

            poolLyr = new Pooling();
            poolLyr.Input = convLyr1.Output;
            poolLyr.Configure("kernel=maxpool(size=2, stride=2); outputfieldsize=2");

            concLyr = new Concatenation();
            concLyr.Input = poolLyr.Output;
            concLyr.Configure("outputfieldsize=2");

            // 1. initialize connection layers
            hiddLyr = new Connected();
            hiddLyr.Input = concLyr.Output;
            hiddLyr.Configure("neuron=hiddenperceptron; activation=relu(void); nodes=128; outputfieldsize=2(def:2)"); ;

            // 2. initialize connection layers
            outpLyr = new Connected();
            outpLyr.Input = hiddLyr.Output;
            outpLyr.Configure("neuron=outputperceptron; activation=linear(void); nodes=10; outputfieldsize=2(def:2)");

            for (int i = 0; i < Epochs; i++)
            {

                //log += "\n\n////////// Epoch[" + i + "]... ///////////////////////////////////////////////////////";

                Global.NextIntArray(0, nofSet, dataSetRandomizer);
                Global.NextIntArray(0, batchImages.Count(), batchRandomizer);
                // single epoch
                for (int j = 0; j < 100; j++) //batch
                {
                    //DisplayWeights();
                    batchLoss = 0;
                    for (int k = 0; k < 6; k++)
                    {
                        imagLyrG.fData = batchImages[batchRandomizer[j]].fData[dataSetRandomizer[k]];
                        convLyr.Next(Propagate.Signal);
                        convLyr1.Next(Propagate.Signal);
                        poolLyr.Next(Propagate.Signal);
                        concLyr.Next(Propagate.Signal);
                        hiddLyr.Next(Propagate.Signal);
                        outpLyr.Next(Propagate.Signal);

                        Node node;
                        probs = new double[outpLyr.Output[0].Rows];
                        for (int l = 0; l < batchImages[batchRandomizer[j]].fData[dataSetRandomizer[k]].Label.Length; l++)
                        {
                            node = outpLyr.Output[0].GetElement(l, 0);
                            probs[l] = (double)((double?[])node.Element)[Global.Sig];
                            ((double?[])node.Element)[Global.Err] = batchImages[batchRandomizer[j]].fData[dataSetRandomizer[k]].Label[l];
                        }

                        //Set softmax output
                        double[] softmaxOutput = Softmax(probs);
                        for (int l = 0; l < probs.Length; l++)
                        {
                            node = outpLyr.Output[0].GetElement(l, 0);
                            ((double?[])node.Element)[Global.Sig] = softmaxOutput[l];
                        }

                        singleLoss = lossfunc.GetLoss(softmaxOutput, batchImages[batchRandomizer[j]].fData[dataSetRandomizer[k]].Label);

                        // Calculate batch loss
                        batchLoss += singleLoss;

                        //4.4 propagate error and set new weights
                        outpLyr.Next(Propagate.Error);
                        hiddLyr.Next(Propagate.Error);
                        concLyr.Next(Propagate.Error);
                        poolLyr.Next(Propagate.Error);
                        convLyr1.Next(Propagate.Error);
                        convLyr.Next(Propagate.Error);

                        SaveError(outpLyr);
                        SaveError(hiddLyr);
                        SaveError(concLyr);
                        SaveError(convLyr1);
                        SaveError(convLyr);

                        //log += "\n" + Model.ToString();                           

                    }
                    // 4.5 adjust weights
                    AdjustWeights(outpLyr);
                    AdjustWeights(hiddLyr);
                    AdjustWeights(concLyr);
                    AdjustWeights(convLyr1);
                    AdjustWeights(convLyr);

                    //DisplayWeights();
                    ClearError(outpLyr);
                    ClearError(hiddLyr);
                    ClearError(concLyr);
                    ClearError(convLyr1);
                    ClearError(convLyr);

                    batchLoss = batchLoss / (double)batchSize;
                    //log += "\n" + Model.ToString();
                    Console.WriteLine("Epoch[" + i.ToString() + "]" + "Batch[" + j.ToString() + "]" + "Loss: " + batchLoss.ToString("e4"));
                    log += "\n\nEpoch[" + i.ToString() + "]" + "Batch[" + j.ToString() + "]" + "Loss: " + batchLoss.ToString("e4");
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
        protected void SaveError(Layer lyr)
        {            
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

        protected void ClearError(Layer lyr)
        {
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
