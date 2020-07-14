using AI.ML.ANN;
using System;
using System.Collections.Generic;
using System.Linq;
using AI.ML.ANN.Enums;
using Foundation;
using GrayImage = AI.ML.CNN.Images.Gray;
using ColorImage = AI.ML.CNN.Images.Color;


namespace AI.ML.CNN.Trainers
{
    [Serializable]
	public class ADAM : Trainer
    {
        private double? learningRate = null, momentum = null;
        protected new string log = "";
        private double batchLoss, singleLoss;
        private double[] probs;
        private int? batchSize = null;
        private Loss lossfunc;
        double? beta1;
        double? beta2;
        IList<Core.fDataSet> batchImages = new List<Core.fDataSet>();
        public ADAM()
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
            for (int i = 0; i < Epochs; i++)
            {

                //log += "\n\n////////// Epoch[" + i + "]... ///////////////////////////////////////////////////////";

                Global.NextIntArray(0, nofSet, dataSetRandomizer);
                Global.NextIntArray(0, batchImages.Count(), batchRandomizer);
                // single epoch
                for (int j = 0; j < batchImages.Count; j++) //batch
                {
                    //DisplayWeights();
                    batchLoss = 0;
                    for (int k = 0; k < nofSet; k++)
                    {
                        if(Model.Input.Output.Count == 3)
                        {
                            ColorImage inputlayer = (ColorImage)Model.Input;
                            inputlayer.fData = batchImages[batchRandomizer[j]].fData[dataSetRandomizer[k]];
                        }
                        else
                        {
                            GrayImage inputlayer = (GrayImage)Model.Input;
                            inputlayer.fData = batchImages[batchRandomizer[j]].fData[dataSetRandomizer[k]];
                        }

                        //log += "\n\nModel: " + Model.ToString();
                        //DisplayWeights();
                        // 4.1 propagate signal
                        Model.Next(Propagate.Signal);
                        //log += "\n\nModel: " + Model.ToString();

                        // 4.2 set targets
                        Layer lyr = Model.Output;
                        Node node;
                        probs = new double[lyr.Output[0].Rows];
                        for (int l = 0; l < batchImages[batchRandomizer[j]].fData[dataSetRandomizer[k]].Label.Length; l++)
                        {
                            node = lyr.Output[0].GetElement(l, 0);
                            probs[l] = (double)((double?[])node.Element)[Global.Sig];
                            ((double?[])node.Element)[Global.Err] = batchImages[batchRandomizer[j]].fData[dataSetRandomizer[k]].Label[l];
                        }
                        // Set softmax output 
                        double[] softmaxOutput = Softmax(probs);
                        for (int l = 0; l < probs.Length; l++)
                        {
                            node = lyr.Output[0].GetElement(l, 0);
                            ((double?[])node.Element)[Global.Sig] = softmaxOutput[l];
                        }
                        singleLoss = lossfunc.GetLoss(softmaxOutput, batchImages[batchRandomizer[j]].fData[dataSetRandomizer[k]].Label);

                        // Calculate batch loss
                        batchLoss += singleLoss;

                        //4.4 propagate error and set new weights
                        Model.Next(Propagate.Error);

                        SaveError();
                        //log += "\n" + Model.ToString();                           

                    }
                    // 4.5 adjust weights
                    AdjustWeights();
                    //DisplayWeights();
                    ClearError();
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
