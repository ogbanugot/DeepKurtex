using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AI.ML.ANN;
using AI.ML.ANN.Enums;
using AI.ML.ANN.Trainers;

using Feedforward = AI.ML.ANN.Models.Acyclic;
using HiddenPerceptron = AI.ML.ANN.Neurons.Perceptron.Hidden;
using Logistic = AI.ML.ANN.Activation.Logistic;
using OutputPerceptron = AI.ML.ANN.Neurons.Perceptron.Output;
using Tanh = AI.ML.ANN.Activation.Tanh;

using Node = Foundation.Node;

namespace Tests
{
    partial class frmMain : Form
    {
        partial void test_ann_activation_logisticToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string log = "";
            int nofSmp = 100;

            double x = -1.5, s = (System.Math.Abs(x) * 2) / nofSmp, y = 0.0;

            Logistic lgs = new Logistic();
            lgs.Configure(new double?[] { -1.0, 2.0, 6.8 });
            lgs.Input = new double[1];

            for (int i = 0; i <= nofSmp; i++)
            {
                lgs.Input[0] = x;
                y = lgs.Next();
                log += "\nx = " + (x > 0.0 ? "+" : "") + x.ToString("0.000000");
                log += " y = " + (y > 0.0 ? "+" : "") + y.ToString("0.000000");

                x += s;
            }

            richTextBox.Text = log;
        }

        partial void test_ann_model_acyclicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string log = "";

            string c = "";
            c += "[neu=hiddenperceptron; act=tanh(a=0.5, b=0.03); fieldsize=2; nodes=2]";
            c += "[neu=outputperceptron; act=tanh(a = 0.2, b = 1.786); nodes=2]";

            Feedforward[] fnn = new Feedforward[2];

            fnn[0] = new Feedforward();
            fnn[0].Configure(c);

            Node[] input = new Node[]
            {
                new Node(new double?[] { 0.05, 0.00 }),
                new Node(new double?[] { 0.01, 0.00 })
            };

            fnn[0].Input = input;

            double[][] dataSet = new double[2][];
            dataSet[0] = new double[] { 0.05, 0.10 };
            dataSet[1] = new double[] { 0.01, 0.99 };

            richTextBox.Text = log;
        }

        partial void test_ann_neuron_hiddenPercpetronToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string log = "";
            int epochs = 1;
            int nofS = 2;
            Synapse s;
            double momentum = 0.5, learningRate = 0.08;

            string c = "";
            c += "activation=tanh(a=1.7159, b=0.6667); fieldsize=2";

            HiddenPerceptron[] h = new HiddenPerceptron[2];
            OutputPerceptron o = new OutputPerceptron();
            o.Configure(c);

            // instantiate with configuration string
            h[0] = new HiddenPerceptron();
            h[0].Configure(c);
            h[1] = new HiddenPerceptron();
            h[1].Configure(c);

            Node[] src0 = new Node[nofS];
            for (int i = 0; i < nofS; i++)
            {
                src0[i] = new Node(new double?[] { -1.0 + (i * 2.0), null });
            }

            // initialize inputs
            for (int i = 0; i < nofS; i++)
            {
                h[0].Source = src0[i];
                h[1].Source = src0[i];
            }

            // initialize inputs for output node
            for (int i = 0; i < nofS; i++)
                o.Source = h[i].Output;

            // initialize weights
            for (int i = 0; i <= nofS; i++)
            {
                h[0].Synapse[i].W = (Math.Daemon.Random.Next(6) + 1) * 0.35;
                h[0].Synapse[i].dW = 0.0;

                h[1].Synapse[i].W = (Math.Daemon.Random.Next(6) + 1) * 0.50;
                h[1].Synapse[i].dW = 0.0;

                o.Synapse[i].W = (Math.Daemon.Random.Next(6) + 1) * 0.20;
                o.Synapse[i].dW = 0.0;
            }

            double[][] trainingSet = new double[4][];

            trainingSet[0] = new double[] { 0.0, 0.0, 0.0 };
            trainingSet[1] = new double[] { 0.0, 1.0, 1.0 };
            trainingSet[2] = new double[] { 1.0, 0.0, 1.0 };
            trainingSet[3] = new double[] { 1.0, 1.0, 0.0 };

            for (int i = 0; i < epochs; i++)
            {
                log += "\n";

                for (int j = 0; j < trainingSet.Length; j++)
                {
                    ((double?[])src0[0].Element)[Global.Sig] = trainingSet[j][0];
                    ((double?[])src0[1].Element)[Global.Sig] = trainingSet[j][1];

                    // propagate signal
                    log += "\n\nPROPAGATE SIGNAL...///////////////////////////////////////";
                    h[0].Next(Propagate.Signal);
                    h[1].Next(Propagate.Signal);
                    o.Next(Propagate.Signal);
                    log += "\n\nepoch[" + i.ToString("00") + "]";
                    log += "\n\n" + h[0].ToString();
                    log += "\n" + h[1].ToString();
                    log += "\n" + o.ToString();

                    // set target output value
                    log += "\n\nSET TARGET...////////////////////////////////////////////";
                    ((double?[])h[0].Output.Element)[Global.Err] = 0.0;
                    ((double?[])h[1].Output.Element)[Global.Err] = 0.0;
                    ((double?[])o.Output.Element)[Global.Err] = trainingSet[j][2];
                    log += "\n\n" + h[0].ToString();
                    log += "\n" + h[1].ToString();
                    log += "\n" + o.ToString();

                    // propagate error
                    log += "\n\nPROPAGATE ERROR...///////////////////////////////////////";
                    o.Next(Propagate.Error);
                    h[0].Next(Propagate.Error);
                    h[1].Next(Propagate.Error);
                    log += "\n\n" + h[0].ToString();
                    log += "\n" + h[1].ToString();
                    log += "\n" + o.ToString();

                    // adjust weight
                    log += "\n\nADJUST WEIGHTS...///////////////////////////////////////";

                    for (int k = 0; k < o.Synapse.Count; k++)
                    {
                        s = o.Synapse[k];
                        s.dW = (momentum * s.dW) + (learningRate * o.Gradient * ((double?[])s.Source.Element)[Global.Sig].Value);
                        s.W += s.dW;
                    }
                    for (int k = 0; k < h[0].Synapse.Count; k++)
                    {
                        s = h[0].Synapse[k];
                        s.dW = (momentum * s.dW) + (learningRate * h[0].Gradient * ((double?[])s.Source.Element)[Global.Sig].Value);
                        s.W += s.dW;
                    }
                    for (int k = 0; k < h[1].Synapse.Count; k++)
                    {
                        s = h[1].Synapse[k];
                        s.dW = (momentum * s.dW) + (learningRate * h[1].Gradient * ((double?[])s.Source.Element)[Global.Sig].Value);
                        s.W += s.dW;
                    }

                    log += "\n\n" + h[0].ToString();
                    log += "\n" + h[1].ToString();
                    log += "\n" + o.ToString();
                }
            }

            richTextBox.Text = log;
        }

        /// <summary>
        /// tests output perceptron
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        partial void test_ann_neuron_outputPerceptronToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string log = "";

            int epochs = 1, nofS = 2;
            string c = "act=tanh(a=1.7159, b=0.6667); outputfieldsize=2";
            Synapse s;
            double momentum = 0.5, learningRate = 0.08;

            OutputPerceptron o = new OutputPerceptron();
            o.Configure(c);

            ((double?[])o.Output.Element)[Global.Err] = 1.0;

            Node[] source = new Node[nofS];
            for (int i = 0; i < nofS; i++)
                source[i] = new Node(new double?[] { 1.0, null });
            
            for (int i = 0; i < nofS; i++)
                o.Source = source[i];

            for (int i = 0; i <= nofS; i++)
            {
                o.Synapse[i].W = (Math.Daemon.Random.Next(6) + 1) * 0.1;
                o.Synapse[i].dW = 0.0;
            }

            double[][] trainingSet = new double[4][];

            trainingSet[0] = new double[] { 0.0, 0.0, 0.0 };
            trainingSet[1] = new double[] { 0.0, 1.0, 1.0 };
            trainingSet[2] = new double[] { 1.0, 0.0, 1.0 };
            trainingSet[3] = new double[] { 1.0, 1.0, 1.0 };

            for (int i = 0; i < epochs; i++)
            {
                log += "\n";

                for (int j = 0; j < trainingSet.Length; j++)
                {
                    ((double?[])source[0].Element)[Global.Sig] = trainingSet[j][0];
                    ((double?[])source[1].Element)[Global.Sig] = trainingSet[j][1];

                    // propagate signal
                    o.Next(Propagate.Signal);
                    log += "\n\nepoch[" + i.ToString("00") + "]";
                    log += "\n\npropagate signal...";
                    log += "\n" + o.ToString();

                    // set target output value
                    ((double?[])o.Output.Element)[Global.Err] = trainingSet[j][2];
                    log += "\n\nSet target...";
                    log += "\n" + o.ToString();

                    // propagate error
                    o.Next(Propagate.Error);
                    log += "\n\npropagate error...";
                    log += "\n" + o.ToString();

                    // adjust weight
                    for (int k = 0; k < o.Synapse.Count; k++)
                    {
                        s = o.Synapse[k];
                        s.dW = (momentum * s.dW) + (learningRate * o.Gradient * ((double?[])s.Source.Element)[Global.Sig].Value);
                        s.W += s.dW;
                    }
                    log += "\n\nadjust weights...";
                    log += "\n" + o.ToString();
                }
            }

            richTextBox.Text = log;
        }

        partial void test_ann_trainer_deltaRuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string log = "";
            int epochs = 10;
            double learningRate = 0.09, momentum = 0.6;

            Node[] input = new Node[]
            {
                new Node(new double?[] { 0.00, null }),
                new Node(new double?[] { 0.00, null })
            };

            string c = ""; // configuration string
            c += "[neu=hiddenperceptron; act=tanh(a=1.7159, b=0.6667); fieldsize=2; nodes=2]";
            c += "[neu=outputperceptron; act=tanh(a=1.7159, b=0.6667); nodes=1]";

            Feedforward[] fnn = new Feedforward[2];

            fnn[0] = new Feedforward();
            fnn[0].Configure(c);

            fnn[0].Input = input;

            // create or data set
            DataSet orDataSet = new DataSet(2, 1);

            orDataSet.Add(new double[] { -1.0, -1.0 }, new double[] { -1.0 });
            orDataSet.Add(new double[] { -1.0, +1.0 }, new double[] { +1.0 });
            orDataSet.Add(new double[] { +1.0, -1.0 }, new double[] { +1.0 });
            orDataSet.Add(new double[] { +1.0, +1.0 }, new double[] { +1.0 });

            // instantiate trainer
            DeltaRule dRule = new DeltaRule();
            dRule.Configure(fnn[0], epochs, orDataSet.Data, learningRate, momentum);

            //log += dRule.Next();

            dRule.Next();

            double[][] data = orDataSet.Data;
            for (int i = 0; i < orDataSet.Count; i++)
            {
                fnn[0].SetInput(data[i * 2]);
                fnn[0].PropagateSignal();
                log += "\n\n" + fnn[0].ToString();
            }

            fnn[1] = new Feedforward();
            fnn[1].Configure<Tanh>(new int[] { 2, 1 }, new double?[] { 1.7159, 0.6667 }, 2);

            fnn[1].Input = input;

            DataSet xrDataSet = new DataSet(2, 1);

            xrDataSet.Add(new double[] { -1.0, -1.0 }, new double[] { -1.0 });
            xrDataSet.Add(new double[] { -1.0, +1.0 }, new double[] { +1.0 });
            xrDataSet.Add(new double[] { +1.0, -1.0 }, new double[] { +1.0 });
            xrDataSet.Add(new double[] { +1.0, +1.0 }, new double[] { -1.0 });

            epochs = 200;
            learningRate = 0.1; momentum = 0.80;

            dRule.Configure(fnn[1], epochs, xrDataSet.Data, learningRate, momentum);

            //log += dRule.Next();

            dRule.Next();

            log += "\n\n////////////////////////////////////////////////////////////////////";

            data = xrDataSet.Data;
            for (int i = 0; i < xrDataSet.Count; i++)
            {
                fnn[1].SetInput(data[i * 2]);
                fnn[1].PropagateSignal();
                log += "\n\n" + fnn[1].ToString();
            }

            richTextBox.Text = log;
        }
    }
}