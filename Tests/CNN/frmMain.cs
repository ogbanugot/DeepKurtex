using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

using AI.Core;
using AI.Core.Collections;
using AI.Search.Informed;

using AI.ML.ANN.Enums;
using AI.ML.CNN;
using AI.ML.CNN.Layers;

using ColorImage = AI.ML.CNN.Images.Color;
using GrayImage = AI.ML.CNN.Images.Gray;
using Image = AI.ML.CNN.Image;
using Node = Foundation.Node;
using ReLU = AI.ML.ANN.Activation.ReLU;
using Linear = AI.ML.ANN.Activation.Linear;
using System.Drawing;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using AI.ML.ANN;
using System.IO;
using Newtonsoft.Json;


namespace Tests
{
    [System.Runtime.InteropServices.Guid("FD95BC14-D0BF-4068-8645-350613CAA99E")]
    partial class frmMain : Form
    {
        private Connected hiddLyr = null;
        private Connected outpLyr = null;

        private ColorImage imagLyr = null;
        private Concatenation concLyr = null;
        private Convolution convLyr = null;
        private Convolution convLyr1 = null;
        private Pooling poolLyr = null;
        private GrayImage imagLyrG = null;

        private int? batchSize = null;
        private double beta1 = 0.95;
        private double beta2 = 0.99;
        private double? learningRate = 0.1;

        private AI.ML.CNN.Model model = null;

        /// <summary>
        /// initializes the connection layer
        /// </summary>
        private void InitializeConcLayer()
        {
            // 0. initialize pooling layer
            InitializePoolLayer();

            // 1. configure convolution layer
            concLyr = new Concatenation();
            concLyr.Input = poolLyr.Output;
            concLyr.Configure("outputfieldsize=2");
        }

        private void InitializeConvLayer()
        {
            // 0. initialize image layer
            InitializeImage();

            // 1. create configuration staring for convolution layer
            string cfg = "depth=8; activation=logistic(a=-1, b=2.0, c=5.0);";
            cfg += "kernel=convolution(size=7, stride=2, padding=3, weightfieldsize=2);";
            cfg += "outputfieldsize=3";

            // 2. configure convolution layer
            convLyr = new Convolution();
            convLyr.Input = imagLyr.Output;
            convLyr.Configure(cfg);
        }

        private void InitializeImage()
        {
            Bitmap src = new Bitmap(@"C:\Users\Ugot\Documents\AILab\images.png");
            Bitmap bmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.DrawImage(src, 0, 0);
            }

            imagLyr = new ColorImage();
            imagLyr.Configure(225, 225, -1.0, 1.0);
            imagLyr.Bitmap = bmp;
        }

        private void InitializeModel()
        {
            // 0. build configuration string
            string fileName = @"C:\Users\Ebun Fasina\Documents\Visual Studio 2019\AILab\images.png";

            // 1. set test image
            Bitmap src = new Bitmap(fileName);
            Bitmap bmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.DrawImage(src, 0, 0);
            }

            string cfg = "";

            cfg += "imag[image=gray; rows=225; cols=225; min=-1(def:1); max=1(def:1)]";
            cfg += "conv[depth=6; activation=tanh(a=1.7159, b=0.6667);";
            cfg += "kernel=convolution(size=7, stride=2, padding=3, weightfieldsize=2);";
            cfg += "outputfieldsize=2]";
            cfg += "pool[kernel=avgpool(size=4, stride=2, padding=0); outputfieldsize=2]";
            cfg += "conv[depth=12; activation=tanh(a=1.7159, b=0.6667);";
            cfg += "kernel=convolution(size=7, stride=2, padding=3, weightfieldsize=2);";
            cfg += "outputfieldsize=2]";
            cfg += "pool[kernel=avgpool(size=4, stride=2, padding=0); outputfieldsize=2]";
            cfg += "conc[outputfieldsize=2]";
            cfg += "conn[neuron=hiddenperceptron; activation=tanh(a=1.7159, b=0.6667); nodes=16; outputfieldsize=2(def:2)]";
            cfg += "conn[neuron=outputperceptron; activation=tanh(a=1.7159, b=0.6667); nodes=10; outputfieldsize=2(def:2)]";

            // 2. construct model
            model = new AI.ML.CNN.Model();
            model.Configure(cfg);
            GrayImage inputlayer = (GrayImage)model.Input;

            inputlayer.Bitmap = Image.ConvertImageToGray(bmp);
        }

        partial void test_cnn_backprop_concatenationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string log = "";

            // 0.
            InitializeModel();
            // 1.
            model.Next(Propagate.Signal);
            Layer lyr = model.Output;
            // 2. set target value
            double[] target = new double[] { 0.0, 1.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 1.0 };
            Node node;
            for (int i = 0; i < target.Length; i++)
            {
                node = lyr.Output[0].GetElement(i, 0);
                ((double?[])node.Element)[Global.Err] = target[i];
            }

            for (int i = 0; i < 3; i++)
                ((Layer)model[7 - i]).Next(Propagate.Error);

            for (int i = 0; i < model[4].Output.Count; i++)
            {
                log += "\n\n2nd Pooling Layer fMap[" + i + "]...";
                log += "\n" + model[4].Output[i].ToSubString(0, 0, model[4].Output[i].Rows, model[4].Output[i].Columns);
            }

            richTextBox.Text = log;
        }
        
        partial void test_cnn_backprop_modelToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int layer = 0;

            string log = "";

            // 0.
            InitializeModel();
            // 1.
            model.Next(Propagate.Signal);
            Layer lyr = model.Output;
            // 2. set target value
            double[] target = new double[] { 0.0, 1.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 1.0 };
            Node node;
            for (int i = 0; i < target.Length; i++)
            {
                node = lyr.Output[0].GetElement(i, 0);
                ((double?[])node.Element)[Global.Err] = target[i];
            }

            for (int i = model.Layers.Length - 1; i > layer; i--)
                ((Layer)model[i]).Next(Propagate.Error);

            for (int i = 0; i < 1; i++) //model[layer].fMaps.Count
            {
                log += "\n\nLayer " + "[" + layer + "] fMap[" + i + "]...";
                log += "\n" + model[layer].Output[i].ToSubString(0, 0, model[layer].Output[i].Rows, model[layer].Output[i].Columns);
            }

            richTextBox.Text = log;
        }
        
        partial void test_cnn_depreciated_convolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const int size = 28; double d = 2.0 / 255.0, x;
            Node node;
            fMap src = new fMap();
            src.Configure<double?>(size, size, 2);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    node = src.GetElement(i, j);
                    x = Math.Daemon.Random.Next(0, 256);
                    x = System.Math.Round(-1.0 + (x * d), 3);
                    node.SetElement(new double?[] { x, null });
                }
            }

            string log = "\n\n";
            log += src.ToSubString(0, 0, 10, 10);

            Convolution.Kernel cnv = new Convolution.Kernel();
            cnv.Configure("kernelsize=5; stride=1(def:1); padding=0(def:0); weightfieldsize=3(def:2)");
            cnv.Source = src;

            int trgsize = (((src.Rows - cnv.Rows) + (2 * cnv.Padding)) / cnv.Stride) + 1;
            fMap trg = new fMap();
            trg.Configure<double?>(trgsize, trgsize, 2);

            log += "\n\nKERNEL CONFIGURATION...";
            log += "\n" + cnv.ToString();
            
            for (int i = 0; i < trg.Rows; i++)
            {
                for (int j = 0; j < trg.Columns; j++)
                {
                    x = cnv.Next(i, j);
                    trg.WriteAt<double?>(i, j, Global.Sig, x);
                    log += "\n\nOutput: " + x;
                    log += cnv.SourceNodesToString(Global.Sig);
                }
            }

            // set error of source
            for (int i = 0; i < src.Rows; i++)
            {
                for (int j = 0; j < src.Columns; j++)
                {
                    node = src.GetElement(i, j);
                    ((double?[])node.Element)[Global.Err] = 0.0;
                }
            }

            // set weightcorrection to zero
            double?[][] wc = cnv.WeightCorrection;
            for (int i = 0; i < wc.Length; i++)
                for (int j = 0; j < wc[i].Length; j++)
                    wc[i][j] = 0.0;

            for (int i = 0; i < trg.Rows; i++)
                for (int j = 0; j < trg.Columns; j++)
                    cnv.Next(Math.Daemon.Random.NextDouble(), i, j);

            log += "\n\nKERNEL CONFIGURATION...(after back prop)";
            log += "\n" + cnv.ToString();

            richTextBox.Text = log;
        }

        partial void test_cnn_depreciated_maxpoolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const int size = 28; double d = 2.0/255.0, x;
            Node node;
            fMap src = new fMap();
            src.Configure<double?>(size, size, 2);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    node = src.GetElement(i, j);
                    x = Math.Daemon.Random.Next(0, 256);
                    x = System.Math.Round(-1.0 + (x * d), 3);
                    node.SetElement(new double?[] { x, null });
                }
            }

            string log = "\n\n";
            log += src.ToSubString(0, 0, 10, 10);

            AI.ML.CNN.Kernels.Maxpool mxp = new AI.ML.CNN.Kernels.Maxpool();
            mxp.Configure("size=5, stride=1, padding=0");
            mxp.Source = src;

            // size = (((src.Size - kernel.Size) + (2 * kernel.Padding)) / kernel.Stride) + 1;
            int trgsize = (((src.Rows - mxp.Rows) + (2 * mxp.Padding)) / mxp.Stride) + 1;
            fMap trg = new fMap();
            trg.Configure<double?>(trgsize, trgsize, 2);

            for (int i = 0; i < trg.Rows; i++)
            {
                for (int j = 0; j < trg.Columns; j++)
                {
                    x = mxp.Next(i, j);
                    trg.WriteAt<double?>(i, j, Global.Sig, x);
                    log += "\n\nOutput: " + x;
                    log += mxp.SourceNodesToString(Global.Sig);
                }
            }

            for (int i = 0; i < trg.Rows; i++)
            {
                for (int j = 0; j < trg.Columns; j++)
                {
                    mxp.Next(Math.Daemon.Random.NextDouble(), i, j);
                }
            }

            log += "\n\n";
            log += src.ToSubString(0, 0, 10, 10);

            richTextBox.Text = log;
        }

        private void InitializePoolLayer()
        {
            // 0. initialize convolution layer
            InitializeConvLayer();

            // 1. configure pooling layer
            poolLyr = new Pooling();
            poolLyr.Input = convLyr.Output;
            poolLyr.Configure("kernel=avgpool(size=4; stride=2; padding=0); outputfieldsize=2");
        }

        partial void test_cnn_filters_concatenationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int? outputfieldsize = 2;

            // 0. initialize pooling layer
            InitializePoolLayer();

            // 1. instantiate and initialize filter
            Concatenation.Filter cfilter = new Concatenation.Filter();
            cfilter.Source = poolLyr.Output[0];
            cfilter.Configure(outputfieldsize);

            fMap target = cfilter.Target;

            convLyr.Next(Propagate.Signal);
            poolLyr.Next(Propagate.Signal);
            cfilter.Next<double?>(Propagate.Signal);

            richTextBox.Text = target.ToSubString(1000, 0, 200, 1);
        }

        partial void test_cnn_filters_connectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const int nofi = 20; // number of inputs
            double?[] funcparam = new double?[] { -1.0, 2.0, 1.5 };

            // 0. initialize concatenated layer
            InitializeConcLayer();

            AI.ML.ANN.Neuron neuron = new AI.ML.ANN.Neurons.Perceptron.Hidden();

            neuron.Configure<AI.ML.ANN.Activation.Logistic>(funcparam, 2);

            for (int i = 0; i < nofi; i++)
                neuron.Source = concLyr.Output[0].GetElement(i, 0);

            // 1. drive all layers
            convLyr.Next(Propagate.Signal);
            poolLyr.Next(Propagate.Signal);
            concLyr.Next(Propagate.Signal);

            neuron.Next(Propagate.Signal);

            richTextBox.Text = "\nOutput of neuron: " + ((double?[])neuron.Output.Element)[0].Value.ToString();
        }

        partial void test_cnn_filters_convolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 0. initialize image layer
            InitializeImage();
 
            // 1. configuration string of convolution layer
            string cfg = "activation=logistic(a=-1.5, b=2.0, c=5.0);";
            cfg += "kernel=convolution(size=7, stride=2, padding=3, weightfieldsize=2); outputfieldsize=2";

            // 2. configure convolution filter
            Convolution.Filter filter = new Convolution.Filter();
            for (int i = 0; i < imagLyr.Output.Count; i++)
                filter.Source = imagLyr.Output[i];
            filter.Configure(cfg);

            // 3. propagate signal
            filter.Next<double?>(Propagate.Signal);

            richTextBox.Text = filter.Target.ToString();
        }

        partial void test_cnn_filters_poolingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 0. initialize image layer
            InitializeImage();

            // 1. configure convolution filter
            string cfg = "activation=logistic(a=-1, b=2.0, c=5.0); padding=3(def:0); kernelsize=7; stride=2(def:1); outputfieldsize=2";
            Convolution.Filter conv = new Convolution.Filter();
            for (int i = 0; i < imagLyr.Output.Count; i++)
                conv.Source = imagLyr.Output[i];
            conv.Configure(cfg);

            // 2. configure pooling filter
            cfg = "act=avgpool(void); kernelsize=4; stride=2(def:1); outputfieldsize=2";
            Pooling.Filter flt = new Pooling.Filter();
            flt.Source = conv.Target;
            flt.Configure(cfg);

            conv.Next<double?>(Propagate.Signal);
            flt.Next<double?>(Propagate.Signal);

            richTextBox.Text = flt.Target.ToString();
        }

        partial void test_cnn_fMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "";

            richTextBox.Text = s;
        }

        partial void test_cnn_layers_concatenationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string log = "\n"; int? outputfieldsize = 2;

            // 0. initialize pooling layer
            InitializePoolLayer();

            concLyr = new Concatenation();
            concLyr.Input = poolLyr.Output;
            concLyr.Configure(outputfieldsize);

            convLyr.Next(Propagate.Signal);
            poolLyr.Next(Propagate.Signal);
            concLyr.Next(Propagate.Signal);

            log += concLyr.Output[0].ToSubString(1000, 0, 500, 1);

            richTextBox.Text = log;
        }

        partial void test_cnn_layers_connectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 0. initialize concatenated layer
            InitializeConcLayer();

            // 1. initialize connection layers
            hiddLyr = new Connected();
            hiddLyr.Input = concLyr.Output;
            hiddLyr.Configure("neuron=hiddenperceptron; activation=logistic(a=-1, b=2.0, c=1.5); nodes=8; outputfieldsize=2(def:2)");

            // 2. initialize connection layers
            outpLyr = new Connected();
            outpLyr.Input = hiddLyr.Output;
            outpLyr.Configure("neuron=outputperceptron; activation=tanh(a=-1, b=2.0, c=1.5); nodes=4; fieldsize=2(def:2)");

            // 3. propagate signals
            convLyr.Next(Propagate.Signal);
            poolLyr.Next(Propagate.Signal);
            concLyr.Next(Propagate.Signal);
            hiddLyr.Next(Propagate.Signal);
            outpLyr.Next(Propagate.Signal);

            // 4.
            string log = "";

            log += "\n\nHIDDEN LAYER...\n\n";

            for (int i = 0; i < hiddLyr.Output.Count; i++)
                log += "\n" + hiddLyr.Output[i].ToSubString(0, 0, 8, 1);

            log += "\n\nOUTPUT LAYER...\n\n";

            for (int i = 0; i < outpLyr.Output.Count; i++)
                log += "\n" + outpLyr.Output[i].ToSubString(0, 0, 4, 1);

            richTextBox.Text = log;
        }

        protected void AdjustWeights(Layer lyr)
        {

            bool convtype = lyr.GetType().ToString() == "AI.ML.CNN.Layers.Convolution" ? true : false;
            bool conctype = lyr.GetType().ToString() == "AI.ML.CNN.Layers.Connected" ? true : false;

            switch (convtype)
            {
                case true:
                    Convolution convLyr = (Convolution)lyr;
                    double wv;
                    double ws, diff, gradient;
                    AI.ML.CNN.Layers.Convolution.Kernel krn;
                    double?[][][] weights;
                    double?[][] wc;
                    for (int j = 0; j < convLyr.Filters.Count; j++)
                    {

                        if (batchSize == null)
                        {
                            krn = (AI.ML.CNN.Layers.Convolution.Kernel)((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[j]).Kernels[0];
                            wc = krn.WeightCorrection;
                            krn.Weights[0][0][0] += learningRate.Value * wc[0][0];
                            //dW = (Momentum.Value * krn.Weights[0][0][2]) + (LearningRate.Value * wc[0][0]);
                            //krn.Weights[0][0][2] = dW;
                            //krn.Weights[0][0][0] += dW;
                        }
                        else
                        {
                            krn = (AI.ML.CNN.Layers.Convolution.Kernel)((AI.ML.CNN.Layers.Convolution.Filter)convLyr.Filters[j]).Kernels[0];
                            weights = krn.Weights;
                            wv = 0;
                            ws = 0;
                            wv = (beta1 * wv) + (1 - beta1) * (weights[0][0][2].Value / batchSize.Value);
                            ws = (beta2 * ws) + (1 - beta2) * System.Math.Pow((weights[0][0][2].Value / batchSize.Value), 2); ;
                            gradient = learningRate.Value * (wv / System.Math.Sqrt(ws + 1e-7));
                            diff = weights[0][0][0].Value - gradient;
                            krn.Weights[0][0][0] = diff;

                        }
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
                                    if (batchSize == null)
                                    {
                                        //dW = (Momentum.Value * krn.Weights[m][n][2]) + (LearningRate.Value * wc[m][n]);
                                        //krn.Weights[m][n][2] = dW;
                                        krn.Weights[m][n][0] += learningRate.Value * wc[m][n];
                                    }
                                    else
                                    {
                                        wv = 0;
                                        ws = 0;
                                        wv = (beta1 * wv) + (1 - beta1) * (weights[m][n][2].Value / batchSize.Value);
                                        ws = (beta2 * ws) + (1 - beta2) * System.Math.Pow((weights[m][n][2].Value / batchSize.Value), 2);
                                        gradient = learningRate.Value * (wv / System.Math.Sqrt(ws + 1e-7));
                                        diff = weights[m][n][0].Value - gradient;
                                        krn.Weights[m][n][0] = diff;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case false:
                    switch (conctype)
                    {
                        case true:
                            Connected connLyr = (Connected)lyr;
                            Neuron n; Synapse syn;
                            for (int j = 0; j < connLyr.Neurons.Length; j++)
                            {
                                n = connLyr.Neurons[j];
                                for (int k = 0; k < n.Synapse.Count; k++)
                                {
                                    if (batchSize == null)
                                    {
                                        syn = n.Synapse[k];
                                        syn.W -= learningRate * syn.dW;
                                    }
                                    else
                                    {
                                        syn = n.Synapse[k];
                                        wv = 0;
                                        ws = 0;
                                        wv = (beta1 * wv) + (1 - beta1) * (syn.Weights[2].Value / batchSize.Value);
                                        ws = (beta2 * ws) + (1 - beta2) * System.Math.Pow((syn.Weights[2].Value / batchSize.Value), 2);
                                        gradient = learningRate.Value * (wv / System.Math.Sqrt(ws + 1e-7));
                                        diff = syn.W.Value - gradient;
                                        syn.W = diff;
                                    }
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

        private double CrossEntropyLoss(double predicted, double target)
        {
            if (target == 1)
                return -System.Math.Log(predicted);
            else
                return -System.Math.Log(1 - predicted);
        }

        protected double CategoricalCrossEntropyLoss(double[] probs, double[] label)
        {
            if (probs.Count() != label.Count())
                throw new Exception("Size mismatch: Label and probs should be the same ");

            double sum = 0.0;
            double logProb, lbl;

            for (int i = 0; i < probs.Count(); i++)
            {
                logProb = System.Math.Log(probs[i]);
                lbl = label[i];
                sum += lbl * logProb;
            }
            return -sum;
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

        partial void test_cnn_layers_convolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string log = "";

            double loss = 0;
            double[] probs;
            double[] softmaxOutput;
            string dataFileName = @"C:\Users\ogban\Documents\mnist\train-images.idx3-ubyte";
            string labelFileName = @"C:\Users\ogban\Documents\mnist\train-labels.idx1-ubyte";
            string cfg = "";
            List<fData> dataList = (List<fData>)UByteLoader.ReadGrayImage(dataFileName, 1, 0.0, 1.0, labelFileName, 0.0, 1.0);

            fDataSet dataSet = new fDataSet();
            dataSet.fData.Add(dataList[0]);
            cfg += "imag[image=gray; rows=28; cols=28; min=-1(def:1); max=1(def:1)]";
            cfg += "conv[depth=6; activation=logistic(a=-1.0, b=2.0, c=6.8); kernel=convolution(size = 5, stride = 1, padding = 2, weightfieldsize = 3); outputfieldSize =2]";
            cfg += "pool[kernel=avgpool(size=2, stride=2); outputfieldsize=2]";
            cfg += "conv[depth=16; activation=logistic(a=-1.0, b=2.0, c=6.8); kernel=convolution(size = 5, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            cfg += "pool[kernel=avgpool(size=2, stride=2); outputfieldsize=2]";
            cfg += "conc[outputfieldsize=2]";
            cfg += "conn[neuron=hiddenperceptron; activation=logistic(a=-1.0, b=2.0, c=6.8); nodes=120; outputfieldsize=2(def:2)]";
            cfg += "conn[neuron=hiddenperceptron; activation=logistic(a=-1.0, b=2.0, c=6.8); nodes=84; outputfieldsize=2(def:2)]";
            cfg += "conn[neuron=outputperceptron; activation=linear(void); nodes=10; outputfieldsize=2(def:2)]";

            //cfg += "imag[image=gray; rows=28; cols=28; min=-1(def:1); max=1(def:1)]";
            //cfg += "conv[depth=8; activation=relu(void); kernel=convolution(size = 5, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "conv[depth=8; activation=relu(void); kernel=convolution(size = 5, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "pool[kernel=maxpool(size=2, stride=2); outputfieldsize=2]";
            //cfg += "conc[outputfieldsize=2]";
            //cfg += "conn[neuron=hiddenperceptron; activation=relu(void); nodes=128; outputfieldsize=2(def:2)]";
            //cfg += "conn[neuron=outputperceptron; activation=linear(void); nodes=10; outputfieldsize=2(def:2)]";


            int[] dataSetRandomizer = new int[dataSet.fData.Count];
            Global.NextIntArray(0, dataSet.fData.Count, dataSetRandomizer);

            for (int m = 0; m < dataSet.fData.Count; m++)
            {
                for (int i = 0; i < 50; i++)
                {
                    GrayImage inputlayer = (GrayImage)model.Input;

                    inputlayer.fData = dataSet.fData[m];
                    //log += "\n\nModel: " + Model.ToString();

                    // 4.1 propagate signal
                    //DisplayWeights();
                    model.Next(Propagate.Signal);
                    //log += "\n\nModel: " + Model.ToString();

                    // 4.2 set targets
                    Layer lyr = model.Output;
                    Node node;
                    probs = new double[lyr.Output[0].Rows];
                    for (int l = 0; l < dataSet.fData[m].Label.Length; l++)
                    {
                        node = lyr.Output[0].GetElement(l, 0);
                        probs[l] = (double)((double?[])node.Element)[Global.Sig];
                        ((double?[])node.Element)[Global.Err] = dataSet.fData[m].Label[l];
                    }

                    //Set softmax output
                    //softmaxOutput = Softmax(probs);
                    //for (int l = 0; l < probs.Length; l++)
                    //{
                    //    node = lyr.Output[0].GetElement(l, 0);
                    //    ((double?[])node.Element)[Global.Sig] = softmaxOutput[l];
                    //}

                    // 4.4 propagate error and set new weights
                    model.Next(Propagate.Error);

                    // 4.5 adjust weights
                    for (int j = 1; j < model.Layers.Length; j++)
                    {
                        AdjustWeights((Layer)model.Layers[j]);

                    }
                    //4.6 Calculate loss
                    loss = CategoricalCrossEntropyLoss(probs, dataSet.fData[m].Label);

                    log += "\n\nEpoch[" + i.ToString() + "]" + "Image[" + m.ToString() + "]" + "Loss: " + loss.ToString("e4");
                    Console.WriteLine("Epoch[" + i.ToString() + "]" + "Image[" + m.ToString() + "]" + "Loss: " + loss.ToString("e4"));                   
                }
            }

            int[] predict = new int[dataSet.fData.Count];
            int[] target = new int[dataSet.fData.Count];
            for (int m = 0; m < dataSet.fData.Count; m++)
            {
                probs = model.Predict(dataSet.fData[m]);
                softmaxOutput = Softmax(probs);
                double maxValue = softmaxOutput.Max();
                predict[m] = softmaxOutput.ToList().IndexOf(maxValue);
                target[m] = dataSet.fData[m].DecodeLabel;
                log += "\n Target: " + target[m].ToString() + "\t" + "Predict: " + predict[m].ToString();


            }
            double acc = accuracy(target, predict);
            log += "\n Accuracy: " + acc.ToString() + "%";

            ModelSerializer modelSerializer = new ModelSerializer();

            string modelpath = modelSerializer.Serialize(cfg, model, "cnnmodelLeNet");
            log += "\n" + modelpath;
            string datapath = dataSet.Serilizer("datatest28.txt");
            log += "\n" + datapath;            

            //string path = @"C:\Users\ogban\Documents\AILab\Tests\bin\Debug\cnnmodel1.txt";
            //ModelSerializer modelSerializer = new ModelSerializer();
            //model = modelSerializer.DeserializeCNN(path);
            //fDataSet testdataset = new fDataSet();
            //testdataset.Deserializer(@"C: \Users\ogban\Documents\AILab\Tests\bin\Debug\datatest.txt");
            //int[] predict = new int[testdataset.fData.Count];
            //int[] target = new int[testdataset.fData.Count];
            //for (int m = 0; m < testdataset.fData.Count; m++)
            //{
            //    probs = model.Predict(testdataset.fData[m]);
            //    softmaxOutput = Softmax(probs);
            //    double maxValue = softmaxOutput.Max();
            //    predict[m] = softmaxOutput.ToList().IndexOf(maxValue);
            //    target[m] = testdataset.fData[m].DecodeLabel;
            //    log += "\n Target: " + target[m].ToString() + "\t" + "Predict: " + predict[m].ToString();
            //}
            //double acc = accuracy(target, predict);
            //log += "\n Accuracy: " + acc.ToString() + "%";

            richTextBox.Text = log;
        }

        partial void cifarToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string log = "";

            double loss = 0;
            double[] probs;
            double[] softmaxOutput;
            string dataFileName = @"C:\Users\ogban\Documents\mnist\cifar\cifar-10-batches-bin\data_batch_1.bin";
            string cfg = "";
            List<fData> dataList = (List<fData>)UByteLoader.ReadColorImage(dataFileName, 1, 0.0, 1.0, 0.0, 1.0);

            fDataSet dataSet = new fDataSet();
            dataSet.fData.Add(dataList[0]);
            //cfg += "imag[image=color; rows=32; cols=32; min=-1(def:1); max=1(def:1)]";
            //cfg += "conv[depth=8; activation=relu(void); kernel=convolution(size = 5, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "conv[depth=8; activation=relu(void); kernel=convolution(size = 5, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "pool[kernel=maxpool(size=2, stride=2); outputfieldsize=2]";
            //cfg += "conc[outputfieldsize=2]";
            //cfg += "conn[neuron=hiddenperceptron; activation=relu(void); nodes=128; outputfieldsize=2(def:2)]";
            //cfg += "conn[neuron=outputperceptron; activation=linear(void); nodes=10; outputfieldsize=2(def:2)]";

            cfg += "imag[image=color; rows=32; cols=32; min=-1(def:1); max=1(def:1)]";
            cfg += "conv[depth=6; activation=logistic(a=-1.0, b=2.0, c=6.8); kernel=convolution(size = 5, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            cfg += "pool[kernel=avgpool(size=2, stride=2); outputfieldsize=2]";
            cfg += "conv[depth=16; activation=logistic(a=-1.0, b=2.0, c=6.8); kernel=convolution(size = 5, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            cfg += "pool[kernel=avgpool(size=2, stride=2); outputfieldsize=2]";
            cfg += "conc[outputfieldsize=2]";
            cfg += "conn[neuron=hiddenperceptron; activation=logistic(a=-1.0, b=2.0, c=6.8); nodes=120; outputfieldsize=2(def:2)]";
            cfg += "conn[neuron=hiddenperceptron; activation=logistic(a=-1.0, b=2.0, c=6.8); nodes=84; outputfieldsize=2(def:2)]";
            cfg += "conn[neuron=outputperceptron; activation=linear(void); nodes=10; outputfieldsize=2(def:2)]";

            // 1. construct model
            model = new AI.ML.CNN.Model();
            model.Configure(cfg);            

            int[] dataSetRandomizer = new int[dataSet.fData.Count];
            Global.NextIntArray(0, dataSet.fData.Count, dataSetRandomizer);

            for (int m = 0; m < dataSet.fData.Count; m++)
            {
                for (int i = 0; i < 50; i++)
                {
                    ColorImage inputlayer = (ColorImage)model.Input;

                    inputlayer.fData = dataSet.fData[m];
                    //log += "\n\nModel: " + Model.ToString();

                    // 4.1 propagate signal
                    //DisplayWeights();
                    model.Next(Propagate.Signal);
                    //log += "\n\nModel: " + Model.ToString();

                    // 4.2 set targets
                    Layer lyr = model.Output;
                    Node node;
                    probs = new double[lyr.Output[0].Rows];
                    for (int l = 0; l < dataSet.fData[m].Label.Length; l++)
                    {
                        node = lyr.Output[0].GetElement(l, 0);
                        probs[l] = (double)((double?[])node.Element)[Global.Sig];
                        ((double?[])node.Element)[Global.Err] = dataSet.fData[m].Label[l];
                    }

                    //Set softmax output
                    //softmaxOutput = Softmax(probs);
                    //for (int l = 0; l < probs.Length; l++)
                    //{
                    //    node = lyr.Output[0].GetElement(l, 0);
                    //    ((double?[])node.Element)[Global.Sig] = softmaxOutput[l];
                    //}

                    // 4.4 propagate error and set new weights
                    model.Next(Propagate.Error);

                    // 4.5 adjust weights
                    for (int j = 1; j < model.Layers.Length; j++)
                    {
                        AdjustWeights((Layer)model.Layers[j]);

                    }
                    //4.6 Calculate loss
                    loss = CategoricalCrossEntropyLoss(probs, dataSet.fData[m].Label);

                    log += "\n\nEpoch[" + i.ToString() + "]" + "Image[" + m.ToString() + "]" + "Loss: " + loss.ToString("e4");
                    Console.WriteLine("Epoch[" + i.ToString() + "]" + "Image[" + m.ToString() + "]" + "Loss: " + loss.ToString("e4"));                    
                }
            }

            int[] predict = new int[dataSet.fData.Count];
            int[] target = new int[dataSet.fData.Count];
            for (int m = 0; m < dataSet.fData.Count; m++)
            {
                probs = model.Predict(dataSet.fData[m]);
                softmaxOutput = Softmax(probs);
                double maxValue = softmaxOutput.Max();
                predict[m] = softmaxOutput.ToList().IndexOf(maxValue);
                target[m] = dataSet.fData[m].DecodeLabel;
                log += "\n Target: " + target[m].ToString() + "\t" + "Predict: " + predict[m].ToString();

            }
            double acc = accuracy(target, predict);
            log += "\n Accuracy: " + acc.ToString() + "%";

            ModelSerializer modelSerializer = new ModelSerializer();

            string modelpath = modelSerializer.Serialize(cfg, model, "cnnLeNetCifarmodel");
            log += "\n" + modelpath;
            string datapath = dataSet.Serilizer("cifardatatest32.txt");
            log += "\n" + datapath;           

            richTextBox.Text = log;
        }


        public double accuracy(int[] target, int[] predict)
        {
            if (target.Length != predict.Length)
                throw new Exception("size mismatch between target and predict vector");
            double correct = 0;
            for (int i =0; i<target.Length; i++)
            {
                if (target[i] == predict[i])
                    correct++;
            }
            return correct / target.Length * 100;
        }

        partial void test_cnn_layers_imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorImage image = new ColorImage();

            if (pictureBox.Image == null)
                return;

            image.Bitmap = (Bitmap)pictureBox.Image;

            richTextBox.Text = image.ToString();

            Bitmap b = Image.ConvertImageToGray((Bitmap)pictureBox.Image);
            pictureBox.Image = b;
            pictureBox.Refresh();
        }

        partial void test_cnn_layers_poolingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int? stride = 2, padding = 0, outputfieldsize = 2;
            const int size = 4, x = 20, y = 20, xlen = 20, ylen = 20;
            string log = "";
            double?[] funcparams = new double?[] { null };

            // 0. initialize image layer
            InitializeConvLayer();

            poolLyr = new Pooling();
            poolLyr.Input = convLyr.Output;
            poolLyr.Configure<AI.ML.CNN.Kernels.Maxpool>(size, stride, padding, outputfieldsize);

            // 1. propagate signal
            convLyr.Next(Propagate.Signal);
            poolLyr.Next(Propagate.Signal);

            // 2. display maps
            for (int i = 0; i < poolLyr.Output.Count; i++)
            {
                log += "\n\nfMAP[" + i.ToString() + "]";
                log += "\n\n" + poolLyr.Output[i].ToSubString(x, y, xlen, ylen);
            }

            richTextBox.Text = log;

            //int depth = 3, kernelSize = 5, kernelStride = 3, kerenlPadding = 0, kernelWeightFieldSize = 2, OutputFieldSize = 2, nodes=10;
            //List<fData> dataList = (List<fData>)UByteLoader.ReadColorImage(dataFileName, 1, 0.0, 1.0, 0.0, 1.0);

            //// 1. construct model
            //imagLyrG = new GrayImage();
            //imagLyrG.Configure(28, 28, -1.0, 1.0);
            //imagLyrG.fData = dataList[0];

            //convLyr = new Convolution();
            //convLyr.Input = imagLyrG.Output;
            //convLyr.Configure<ReLU>(depth, funcparams, kernelSize, kernelStride, kerenlPadding, kernelWeightFieldSize, OutputFieldSize);

            //convLyr1 = new Convolution();
            //convLyr1.Input = convLyr.Output;
            //convLyr.Configure<ReLU>(depth, funcparams, kernelSize, kernelStride, kerenlPadding, kernelWeightFieldSize, OutputFieldSize);

            //poolLyr = new Pooling();
            //poolLyr.Input = convLyr1.Output;
            //poolLyr.Configure<AI.ML.CNN.Kernels.Maxpool>(kernelSize, kernelStride, kerenlPadding, OutputFieldSize);

            //concLyr = new Concatenation();
            //concLyr.Input = poolLyr.Output;
            //concLyr.Configure(OutputFieldSize);

            //// 1. initialize connection layers
            //hiddLyr = new Connected();
            //hiddLyr.Input = concLyr.Output;
            //hiddLyr.Configure<ReLU>(nodes, outputfieldsize);

            //outpLyr = new Connected();
            //outpLyr.Input = hiddLyr.Output;
            //hiddLyr.Configure<Linear>(nodes, outputfieldsize);


        }

        partial void test_cnn_modelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 0. build configuration string
            string fileName = @"C:\Users\Ugot\Documents\AILab\images.png";

            // 1. set test image
            Bitmap src = new Bitmap(fileName);
            Bitmap bmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.DrawImage(src, 0, 0);
            }

            string cfg = ""; // cnn configuration string

            // 2a. add configuration string of image
            cfg += "imag[image=gray; rows=225; cols=225; min=-1(def:1); max=1(def:1)]";

            // 2b. add configuration string of first convolution layer
            cfg += "conv[depth=1; activation=tanh(a=1.7159, b=0.6667);";
            cfg += "kernel=convolution(size=7, stride=2, padding=3, weightfieldsize=2);";
            cfg += "outputfieldsize=2]";
            
            // 2c. add configuration string of first pooling layer
            cfg += "pool[kernel=avgpool(size=4; stride=2; padding=0); outputfieldsize=2]";

            // 2d. add configurationstring of second convolution layer
            cfg += "conv[depth=1; activation=tanh(a=1.7159, b=0.6667);";
            cfg += "kernel=convolution(size=7, stride=2, padding=3, weightfieldsize=2);";
            cfg += "outputfieldsize=2]";

            // 2e. add configuration string of second pooling layer
            cfg += "pool[kernel=avgpool(size=4, stride=2); outputfieldsize=2]";

            // 2f. add configuration string of concatenation layer
            cfg += "conc[outputfieldsize=2]";

            // 2g. add configuration string of first fully connected (hidden) layer
            cfg += "conn[neuron=hiddenperceptron; activation=tanh(a=1.7159, b=0.6667); nodes=2; outputfieldsize=2(def:2)]";

            // 2h. add confoguration string of second fully connected (output) layer
            cfg += "conn[neuron=outputperceptron; activation=tanh(a=1.7159, b=0.6667); nodes=1; outputfieldsize=2(def:2)]";

            string log = "";

            // 3. construct model
            model = new AI.ML.CNN.Model();
            model.Configure(cfg);
            GrayImage inputlayer = (GrayImage)model.Input;

            inputlayer.Bitmap = Image.ConvertImageToGray(bmp);

            // 4. drive signals
            model.Next(Propagate.Signal);

            //for (int i = 0; i < model[4].Output.Count; i++)
            //{
            //    log += "\n\n2nd Pooling Layer fMap[" + i + "]...";
            //    log += "\n" + model[4].Output[i].ToSubString(0, 0, model[4].Output[i].Rows, model[4].Output[i].Columns);
            //}

            //// 5. log output
            //log += "\n\nConcatenation Layer...\n";
            //log += "\n" + model[5].Output[0].ToSubString(0, 0, model[5].Output[0].Rows, 1);
            //log += "\n\nHidden Connection Layer...\n";
            //log += "\n" + model[6].Output[0].ToSubString(0, 0, model[6].Output[0].Rows, 1);
            log += "\n\nOutput Connection Layer...\n";
            log += "\n" + model.Output.Output[0].ToSubString(0, 0, model.Output.Output[0].Rows, 1);

            richTextBox.Text = log;
        }

        partial void Test_CNN_trainer_deltaRuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string path = @"C:\Users\ogban\Documents\AILab\Tests\bin\Debug\cnnmodel.txt";
            //string log = "";
            //double[] probs;
            //double[] softmaxOutput;
            ////2. Load dataset
            //string dataFileName = @"C:\Users\ogban\Documents\mnist\train-images.idx3-ubyte";
            //string labelFileName = @"C:\Users\ogban\Documents\mnist\train-labels.idx1-ubyte";
            //List<fData> dataList = (List<fData>)UByteLoader.ReadGrayImage(dataFileName, 1, 0.0, 1.0, labelFileName, 0.0, 1.0);

            //fDataSet dataSet = new fDataSet();
            //dataSet.fData = dataList;

            //AI.Core.ModelSerializer modelSerializer = new ModelSerializer();

            //model = modelSerializer.DeserializeCNN(path);

            //probs = model.Predict(dataSet.fData[0]);
            //softmaxOutput = Softmax(probs);

            //for (int i = 0; i < softmaxOutput.Length; i++)
            //    log += "\n" + softmaxOutput[i].ToString();

            //log += "\n" + dataSet.fData[0].DecodeLabel;           

            //richTextBox.Text = log;

            string log = "";
            int epochs = 100;
            double learningRate = 0.5, momentum = 0.6;
            string cfg = "";

            //cfg += "imag[image=gray; rows=28; cols=28; min=-1(def:1); max=1(def:1)]";
            //cfg += "conv[activation=tanh(a=1.7159, b=0.6667); depth=6; kernelSize=5; stride=1(def:1); outputfieldSize=2]";
            //cfg += "pool[activation=avgpool(void); kernelSize=2; stride=2(def:1); outputfieldSize=2]";
            //cfg += "conv[activation=tanh(a=1.7159, b=0.6667); depth=12; kernelSize=5; stride=1(def:1); outputfieldSize=2]";
            //cfg += "pool[activation=avgpool(void); kernelSize=2; stride=2(def:1); outputfieldSize=2]";
            //cfg += "conc[outputfieldSize=2]";
            //cfg += "conn[neuron=hiddenperceptron; activation=tanh(a=1.7159, b=0.6667); nodes=16; outputfieldSize=2(def:2)]";
            //cfg += "conn[neuron=outputperceptron; activation=tanh(a=1.7159, b=0.6667); nodes=10; outputfieldSize=2(def:2)]";

            //cfg += "imag[image=gray; rows=28; cols=28; min=-1(def:1); max=1(def:1)]";
            //cfg += "conv[depth=1; activation=relu(void); kernel=convolution(size = 5, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "conv[depth=1; activation=relu(void); kernel=convolution(size = 5, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "pool[kernel=maxpool(size=2, stride=2); outputfieldsize=2]";
            //cfg += "conc[outputfieldsize=2]";
            //cfg += "conn[neuron=hiddenperceptron; activation=relu(void); nodes=10; outputfieldsize=2(def:2)]";
            //cfg += "conn[neuron=outputperceptron; activation=linear(void); nodes=10; outputfieldsize=2(def:2)]";

            //gray LeNet
            cfg += "imag[image=gray; rows=28; cols=28; min=-1(def:1); max=1(def:1)]";
            cfg += "conv[depth=6; activation=logistic(a=-1.0, b=2.0, c=6.8); kernel=convolution(size = 5, stride = 1, padding = 2, weightfieldsize = 3); outputfieldSize =2]";
            cfg += "pool[kernel=avgpool(size=2, stride=2); outputfieldsize=2]";
            cfg += "conv[depth=16; activation=logistic(a=-1.0, b=2.0, c=6.8); kernel=convolution(size = 5, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            cfg += "pool[kernel=avgpool(size=2, stride=2); outputfieldsize=2]";
            cfg += "conc[outputfieldsize=2]";
            cfg += "conn[neuron=hiddenperceptron; activation=logistic(a=-1.0, b=2.0, c=6.8); nodes=120; outputfieldsize=2(def:2)]";
            cfg += "conn[neuron=hiddenperceptron; activation=logistic(a=-1.0, b=2.0, c=6.8); nodes=84; outputfieldsize=2(def:2)]";
            cfg += "conn[neuron=outputperceptron; activation=linear(void); nodes=10; outputfieldsize=2(def:2)]";

            //color LeNet
            //cfg += "imag[image=color; rows=32; cols=32; min=-1(def:1); max=1(def:1)]";
            //cfg += "conv[depth=6; activation=logistic(a=-1.0, b=2.0, c=6.8); kernel=convolution(size = 5, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "pool[kernel=avgpool(size=2, stride=2); outputfieldsize=2]";
            //cfg += "conv[depth=16; activation=logistic(a=-1.0, b=2.0, c=6.8); kernel=convolution(size = 5, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "pool[kernel=avgpool(size=2, stride=2); outputfieldsize=2]";
            //cfg += "conc[outputfieldsize=2]";
            //cfg += "conn[neuron=hiddenperceptron; activation=logistic(a=-1.0, b=2.0, c=6.8); nodes=120; outputfieldsize=2(def:2)]";
            //cfg += "conn[neuron=hiddenperceptron; activation=logistic(a=-1.0, b=2.0, c=6.8); nodes=84; outputfieldsize=2(def:2)]";
            //cfg += "conn[neuron=outputperceptron; activation=linear(void); nodes=10; outputfieldsize=2(def:2)]";

            //string dataFileName = @"C:\Users\ogban\Documents\mnist\cifar\cifar-10-batches-bin\data_batch_1.bin";
            //string dataFileName1 = @"C:\Users\AzureUser\Documents\datasetLab\cifar\cifar-10-batches-bin\data_batch_1.bin";
            //string dataFileName2 = @"C:\Users\AzureUser\Documents\datasetLab\cifar\cifar-10-batches-bin\data_batch_2.bin";
            //string dataFileName3 = @"C:\Users\AzureUser\Documents\datasetLab\cifar\cifar-10-batches-bin\data_batch_3.bin";
            //string dataFileName4 = @"C:\Users\AzureUser\Documents\datasetLab\cifar\cifar-10-batches-bin\data_batch_4.bin";
            //string dataFileName5 = @"C:\Users\AzureUser\Documents\datasetLab\cifar\cifar-10-batches-bin\data_batch_5.bin";
            //List<fData> dataList1 = (List<fData>)UByteLoader.ReadColorImage(dataFileName1, null, 0.0, 1.0, 0.0, 1.0);
            //List<fData> dataList2 = (List<fData>)UByteLoader.ReadColorImage(dataFileName2, null, 0.0, 1.0, 0.0, 1.0);
            //List<fData> dataList3 = (List<fData>)UByteLoader.ReadColorImage(dataFileName3, null, 0.0, 1.0, 0.0, 1.0);
            //List<fData> dataList4 = (List<fData>)UByteLoader.ReadColorImage(dataFileName4, null, 0.0, 1.0, 0.0, 1.0);
            //List<fData> dataList5 = (List<fData>)UByteLoader.ReadColorImage(dataFileName5, null, 0.0, 1.0, 0.0, 1.0);
            //List<fData> dataList = new List<fData>();
            //dataList.AddRange(dataList1);
            //dataList.AddRange(dataList2);
            //dataList.AddRange(dataList3);
            //dataList.AddRange(dataList4);
            //dataList.AddRange(dataList5);

            // 1. construct model
            model = new AI.ML.CNN.Model();
            model.Configure(cfg);

            //2. Load dataset
            string dataFileName = @"C:\Users\ogban\Documents\mnist\train-images.idx3-ubyte";
            string labelFileName = @"C:\Users\ogban\Documents\mnist\train-labels.idx1-ubyte";

            List<fData> dataList = (List<fData>)UByteLoader.ReadGrayImage(dataFileName, null, 0.0, 1.0, labelFileName, 0.0, 1.0);
            //List<fData> dataList = (List<fData>)UByteLoader.ReadColorImage(dataFileName, null, 0.0, 1.0, 0.0, 1.0);

            fDataSet dataSet = new fDataSet();
            dataSet.fData =  dataList;
            //for (int i = 0; i < 10; i++)
            //{
            //    dataSet.fData.Add(dataList[i]);

            //}
            // 3. instantiate trainer
            AI.ML.CNN.Trainers.DeltaRule deltaRule = new AI.ML.CNN.Trainers.DeltaRule();
            deltaRule.Configure<AI.ML.CNN.Lossfunc.CategoricalCrossEntropy>(model, epochs, dataSet, learningRate, momentum);
            log += deltaRule.Next();
            richTextBox.Text = log;
        }

        partial void Test_CNN_trainer_ADAMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string log = "";
            int epochs = 100, batchSize = 256;
            double? beta1 = 0.95;
            double? beta2 = 0.99;
            double learningRate = 0.001, momentum = 0.6;
            string cfg = "";

            //cfg += "imag[image=color; rows=32; cols=32; min=-1(def:1); max=1(def:1)]";
            //cfg += "conv[activation=tanh(a=1.7159, b=0.6667); depth=6; kernelSize=5; stride=1(def:1); outputfieldSize=2]";
            //cfg += "pool[activation=avgpool(void); kernelSize=2; stride=2(def:1); outputfieldSize=2]";
            //cfg += "conv[activation=tanh(a=1.7159, b=0.6667); depth=12; kernelSize=5; stride=1(def:1); outputfieldSize=2]";
            //cfg += "pool[activation=avgpool(void); kernelSize=2; stride=2(def:1); outputfieldSize=2]";
            //cfg += "conc[outputfieldSize=2]";
            //cfg += "conn[neuron=hiddenperceptron; activation=tanh(a=1.7159, b=0.6667); nodes=16; outputfieldSize=2(def:2)]";
            //cfg += "conn[neuron=outputperceptron; activation=tanh(a=1.7159, b=0.6667); nodes=10; outputfieldSize=2(def:2)]";

            //gray LeNet
            cfg += "imag[image=gray; rows=28; cols=28; min=-1(def:1); max=1(def:1)]";
            cfg += "conv[depth=6; activation=logistic(a=-1.0, b=2.0, c=6.8); kernel=convolution(size = 5, stride = 1, padding = 2, weightfieldsize = 3); outputfieldSize =2]";
            cfg += "pool[kernel=avgpool(size=2, stride=2); outputfieldsize=2]";
            cfg += "conv[depth=16; activation=logistic(a=-1.0, b=2.0, c=6.8); kernel=convolution(size = 5, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            cfg += "pool[kernel=avgpool(size=2, stride=2); outputfieldsize=2]";
            cfg += "conc[outputfieldsize=2]";
            cfg += "conn[neuron=hiddenperceptron; activation=logistic(a=-1.0, b=2.0, c=6.8); nodes=120; outputfieldsize=2(def:2)]";
            cfg += "conn[neuron=hiddenperceptron; activation=logistic(a=-1.0, b=2.0, c=6.8); nodes=84; outputfieldsize=2(def:2)]";
            cfg += "conn[neuron=outputperceptron; activation=linear(void); nodes=10; outputfieldsize=2(def:2)]";

            //color LeNet
            //cfg += "imag[image=color; rows=32; cols=32; min=-1(def:1); max=1(def:1)]";
            //cfg += "conv[depth=6; activation=logistic(a=-1.0, b=2.0, c=6.8); kernel=convolution(size = 5, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "pool[kernel=avgpool(size=2, stride=2); outputfieldsize=2]";
            //cfg += "conv[depth=16; activation=logistic(a=-1.0, b=2.0, c=6.8); kernel=convolution(size = 5, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "pool[kernel=avgpool(size=2, stride=2); outputfieldsize=2]";
            //cfg += "conc[outputfieldsize=2]";
            //cfg += "conn[neuron=hiddenperceptron; activation=logistic(a=-1.0, b=2.0, c=6.8); nodes=120; outputfieldsize=2(def:2)]";
            //cfg += "conn[neuron=hiddenperceptron; activation=logistic(a=-1.0, b=2.0, c=6.8); nodes=84; outputfieldsize=2(def:2)]";
            //cfg += "conn[neuron=outputperceptron; activation=linear(void); nodes=10; outputfieldsize=2(def:2)]";

            //string dataFileName = @"C:\Users\ogban\Documents\mnist\cifar\cifar-10-batches-bin\data_batch_1.bin";


            //VGG13
            //cfg += "imag[image=gray; rows=28; cols=28; min=-1(def:1); max=1(def:1)]";
            //cfg += "conv[depth=64; activation=relu(void); kernel=convolution(size = 3, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "conv[depth=64; activation=relu(void); kernel=convolution(size = 3, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "pool[kernel=maxpool(size=2, stride=1); outputfieldsize=2]";
            //cfg += "conv[depth=128; activation=relu(void); kernel=convolution(size = 3, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "conv[depth=128; activation=relu(void); kernel=convolution(size = 3, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "pool[kernel=maxpool(size=2, stride=1); outputfieldsize=2]";
            //cfg += "conv[depth=256; activation=relu(void); kernel=convolution(size = 3, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "conv[depth=256; activation=relu(void); kernel=convolution(size = 3, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "pool[kernel=maxpool(size=2, stride=1); outputfieldsize=2]";
            //cfg += "conv[depth=512; activation=relu(void); kernel=convolution(size = 3, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "conv[depth=512; activation=relu(void); kernel=convolution(size = 3, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "pool[kernel=maxpool(size=2, stride=1); outputfieldsize=2]";
            //cfg += "conv[depth=512; activation=relu(void); kernel=convolution(size = 3, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "conv[depth=512; activation=relu(void); kernel=convolution(size = 3, stride = 1, padding = 0, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "pool[kernel=maxpool(size=2, stride=1); outputfieldsize=2]";
            //cfg += "conc[outputfieldsize=2]";
            //cfg += "conn[neuron=hiddenperceptron; activation=relu(void); nodes=4096; outputfieldsize=2(def:2)]";
            //cfg += "conn[neuron=hiddenperceptron; activation=relu(void); nodes=4096; outputfieldsize=2(def:2)]";
            //cfg += "conn[neuron=hiddenperceptron; activation=relu(void); nodes=1000; outputfieldsize=2(def:2)]";
            //cfg += "conn[neuron=outputperceptron; activation=linear(void); nodes=10; outputfieldsize=2(def:2)]";

            //AlexNet
            //cfg += "imag[image=gray; rows=227; cols=227; min=-1(def:1); max=1(def:1)]";
            //cfg += "conv[depth=96; activation=relu(void); kernel=convolution(size = 11, stride = 4, padding = 1, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "pool[kernel=maxpool(size=2, stride=2); outputfieldsize=2]";
            //cfg += "conv[depth=256; activation=relu(void); kernel=convolution(size = 5, stride = 1, padding = 2, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "pool[kernel=maxpool(size=2, stride=2); outputfieldsize=2]";
            //cfg += "conv[depth=384; activation=relu(void); kernel=convolution(size = 3, stride = 1, padding = 1, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "conv[depth=384; activation=relu(void); kernel=convolution(size = 3, stride = 1, padding = 1, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "conv[depth=256; activation=relu(void); kernel=convolution(size = 3, stride = 1, padding = 1, weightfieldsize = 3); outputfieldSize =2]";
            //cfg += "pool[kernel=maxpool(size=2, stride=2); outputfieldsize=2]";
            //cfg += "conc[outputfieldsize=2]";
            //cfg += "conn[neuron=hiddenperceptron; activation=relu(void); nodes=4096; outputfieldsize=2(def:2)]";
            //cfg += "conn[neuron=hiddenperceptron; activation=relu(void); nodes=4096; outputfieldsize=2(def:2)]";
            //cfg += "conn[neuron=outputperceptron; activation=linear(void); nodes=10; outputfieldsize=2(def:2)]";


            // 1. construct model
            model = new AI.ML.CNN.Model();
            model.Configure(cfg);

            //2. Load dataset
            //string dataFileName = @"C:\Users\ogban\Documents\mnist\cifar\cifar-10-batches-bin\data_batch_1.bin";
            string dataFileName = @"C:\Users\ogban\Documents\mnist\train-images.idx3-ubyte";
            string labelFileName = @"C:\Users\ogban\Documents\mnist\train-labels.idx1-ubyte";

            List<fData> dataList = (List<fData>)UByteLoader.ReadGrayImage(dataFileName, null, 0.0, 1.0,labelFileName, 0.0, 1.0);

            fDataSet dataSet = new fDataSet();
            dataSet.fData =  dataList;
            //for (int i=0; i<1; i++)
            //{
            //    dataSet.fData.Add(dataList[i]);
            //}
            //int[] input_size = new int[] { 227, 227 };
            //dataSet.Resize(input_size, 3);

            // 3. instantiate trainer
            AI.ML.CNN.Trainers.ADAM adam = new AI.ML.CNN.Trainers.ADAM();
            adam.Configure<AI.ML.CNN.Lossfunc.CategoricalCrossEntropy>(model, epochs, dataSet, learningRate, momentum, batchSize, beta1, beta2);

            //adam.Next();
            log += adam.Next();

            ModelSerializer modelSerializer = new ModelSerializer();

            modelSerializer.Serialize(cfg, model, "cnnMNIstModel");
            dataSet.Serilizer("MNISTtestdata.txt");

            //AI.ML.CNN.Trainers.Verbose verbose = new AI.ML.CNN.Trainers.Verbose();
            //verbose.Configure<AI.ML.CNN.Lossfunc.CategoricalCrossEntropy>(model, epochs, dataSet, learningRate, momentum, batchSize, beta1, beta2);

            //log += verbose.Next();

            richTextBox.Text = log;
        }

        partial void testToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string log = "";
            string path = @"C:\Users\ogban\Documents\AILab\Tests\bin\Debug\cnnMNIstModel.txt";
            ModelSerializer modelSerializer = new ModelSerializer();
            model = modelSerializer.DeserializeCNN(path);
            fDataSet testdataset = new fDataSet();
            testdataset.Deserializer(@"C: \Users\ogban\Documents\AILab\Tests\bin\Debug\MNISTtestdata.txt");
            int[] predict = new int[testdataset.fData.Count];
            int[] target = new int[testdataset.fData.Count];
            for (int m = 0; m < testdataset.fData.Count; m++)
            {
                double[] probs = model.Predict(testdataset.fData[m]);
                double[] softmaxOutput = Softmax(probs);
                double maxValue = softmaxOutput.Max();
                predict[m] = softmaxOutput.ToList().IndexOf(maxValue);
                target[m] = testdataset.fData[m].DecodeLabel;
                log += "\n Target: " + target[m].ToString() + "\t" + "Predict: " + predict[m].ToString();
            }
            double acc = accuracy(target, predict);
            log += "\n Accuracy: " + acc.ToString() + "%";
            richTextBox.Text = log;
        }

    }
}