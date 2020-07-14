using AI.ML.ANN.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AI.ML.ANN;

namespace AI.ML.CNN.Layers
{
    [Serializable]
	public class Convolution : Layer
    {
        public Convolution()
            : base()
        {
            // construct convolution layer object
        }

        /// <summary>
        /// configures convolution layer with string
        /// </summary>
        /// <param name="configuration">depth=32; activation=logistic(a=-1, b=2.0, c=5.0); kernel=convolution(size=7, stride=2, padding=2, weightfieldsize=2);
        /// outputfieldsize=2(def:2)</param>
        /// <returns></returns>
        public override Model.Unit Configure(string configuration)
        {
            // convolution parameters: depth
            string cfg = Global.Parser.RemoveWhiteSpaces(configuration);
            string[] a = Global.Parser.Split(cfg, ";");
            string dpt = Global.Parser.Extract<string>(a, new string[] { "depth" }, Global.Parser.Option.StripDefaultToken, out string[] b);
            int depth = int.Parse(dpt);
            cfg = Global.Parser.Build(b, 0, ";");

            Filter flt;
            
            for (int i = 0; i < depth; i++)
            {
                // filter parameters: activation function, kernel and outputfieldsize
                // activation function parameters: a, b, c...
                // kernel parameters: size, stride, padding, weightfieldsize
                flt = new Filter();
                for (int j = 0; j < Input.Count; j++)
                    flt.Source = Input[j];
                flt.Configure(cfg);
                fmaps.Add(flt.Target);
                filters.Add(flt);
            }

            return this;
        }

        public virtual Convolution Configure<T>(int depth, double?[] funcparams, int kernelsize, int? kernelstride, int? kernelpadding, 
            int? kernelweightfieldsize, int? outputfieldsize)
            where T : Function, new()
        {
            Filter flt;

            for (int i = 0; i < depth; i++)
            {
                flt = new Filter();
                for (int j = 0; j < Input.Count; j++)
                    flt.Source = Input[j];
                flt.Configure<T>(funcparams, kernelsize, kernelstride, kernelpadding, kernelweightfieldsize, outputfieldsize);
                fmaps.Add(flt.Target);
                filters.Add(flt);
            }

            return this;
        }

        public CNN.Kernel[][] Kernels
        {
            get
            {
                CNN.Kernel[][] k = new CNN.Kernel[filters.Count][];
                for (int i = 0; i < k.Length; i++)
                    k[i] = filters[i].Kernels;
                return k;
            }
        }

        public double?[][][][][] Weights
        {
            get
            {
                int n; Kernel k;

                double?[][][][][] w = new double?[filters.Count][][][][];

                for (int i = 0; i < w.Length; i++)
                {
                    n = ((Filter)filters[i]).Kernels.Length;
                    w[i] = new double?[n][][][];
                    for (int j = 0; j < n; j++)
                    {
                        k = (Kernel)((Filter)filters[i]).Kernels[j];
                        w[i][j] = k.Weights;
                    }
                }

                return w;
            }
        }

        [Serializable]
	public class Filter : CNN.Filter
        {
            protected fMap bias = null;
            protected double?[][] grad = null;

            public Filter()
                : base() { }

            public override CNN.Filter Configure(string configuration)
            {
                bias = (fMap)new fMap()
                    .Configure(1, 1);
                bias.SetElement(0, 0, new Foundation.Node(new double?[] { 1.0, null }));
                string cfg = Global.Parser.RemoveWhiteSpaces(configuration);
                string[] a = Global.Parser.Split(cfg, ";");
                cfg = Global.Parser.Extract<string>(a, new string[] { "ker", "kernel" }, ANN.Global.Parser.Option.None, out string[] b);
                a = Global.Parser.Split(cfg, "(", ")");
                if (a[0].CompareTo("convolution") != 0)
                    throw new Exception();
                a = Global.Parser.Split(a[1], ",");
                cfg = Global.Parser.Extract<string>(a, new string[] { "weightfieldsize" }, Global.Parser.Option.None, out b);
                Kernel kernel = new Kernel();
                kernel.Configure(1, 1, 0, 0, int.Parse(cfg));
                kernel.Source = bias;
                kernels.Add(kernel);

                base.Configure(configuration);

                function.Input = new double[kernels.Count];

                grad = new double?[target.Rows][];
                for (int i = 0; i < target.Rows; i++)
                    grad[i] = new double?[target.Columns];

                SetElement(grad);

                return this;
            }

            public virtual Filter Configure<T>(double?[] funcparams, int kernelsize, int? kernelstride, int? kernelpadding,
                int? kernelweightfieldsize, int? outputfieldsize)  
                where T : Function, new()
            {
                // 0. construct bias
                bias = (fMap)new fMap()
                    .Configure(1, 1);
                bias.SetElement(0, 0, new Foundation.Node(new double?[] { 1.0, null }));
                Kernel kernel = new Kernel();
                kernel.Configure(1, 1, 0, 0, kernelweightfieldsize == null ? 2 : kernelweightfieldsize.Value,true);
                kernel.Source = bias;
                kernels.Add(kernel);

                // 1. construct kernels
                for (int i = 0; i < sources.Count; i++)
                {
                    kernel = new Kernel();
                    kernel.Configure(kernelsize, kernelsize, kernelstride.Value, kernelpadding.Value, kernelweightfieldsize.Value);
                    kernel.Source = sources[i];
                    kernels.Add(kernel);
                }

                // 2. construct function
                function = new T();
                function.Configure(funcparams);
                function.Input = new double[kernels.Count];

                // 3. construct target
                int rows = ((sources[0].Rows - kernel.Rows + (2 * kernel.Padding)) / kernel.Stride) + 1;
                int cols = ((sources[0].Columns - kernel.Columns + (2 * kernel.Padding)) / kernel.Stride) + 1;
                target = new fMap();
                target.Configure<double?>(rows, cols, outputfieldsize == null ? 2 : outputfieldsize.Value);

                grad = new double?[target.Rows][];
                for (int i = 0; i < target.Rows; i++)
                    grad[i] = new double?[target.Columns];

                SetElement(grad);

                return this;
            }

            public double?[][] Gradient
            {
                get { return GetElement<double?[][]>(); }
            }

            public override void Next<T>(Propagate prop)
            {
                switch (prop)
                {
                    case Propagate.Error:
                        // 0. set gradients
                        double x;
                        for (int i = 0; i < target.Rows; i++)
                        {
                            for (int j = 0; j < target.Columns; j++)
                            {
                                x = function.Next(((double?[])target.GetElement(i, j).Element)[Global.Sig].Value);
                                grad[i][j] = x * ((double?[])target.GetElement(i, j).Element)[Global.Err].Value;
                            }
                        }
                        // 1. 
                        // 1a. compute bias weight
                        x = 0.0;
                        for (int i = 0; i < grad.Length; i++)
                            for (int j = 0; j < grad[i].Length; j++)
                                x += grad[i][j].Value;
                        ((Kernel)kernels[0]).WeightCorrection[0][0] = x;
                        // 1b. backprop error
                        for (int i = 0; i < grad.Length; i++)
                        {
                            for (int j = 0; j < grad[i].Length; j++)
                            {
                                for (int k = 1; k < kernels.Count; k++)
                                    kernels[k].Next(grad[i][j].Value, i, j);
                            }
                        }
                        break;

                    case Propagate.Signal:
                        function.Input[0] = kernels[0].Next(0, 0);

                        for (int i = 0; i < target.Rows; i++)
                        {
                            for (int j = 0; j < target.Columns; j++)
                            {
                                for (int k = 1; k < kernels.Count; k++)
                                    function.Input[k] = kernels[k].Next(i, j);
                                target.WriteAt<double?>(i, j, Global.Sig, function.Next());
                            }
                        }
                        break;
                }
            }
        }

        [Serializable]
	public class Kernel : CNN.Kernel
        {
            protected double?[][][] wg;
            protected double?[][] wc;
            Random random = new Random();

            public Kernel() { }

            public override CNN.Kernel Configure(string configuration)
            {
                string cfg = Global.Parser.RemoveWhiteSpaces(configuration);
                string[] a = Global.Parser.Split(cfg, ",");
                string wfs = Global.Parser.Extract<string>(a, new string[] { "weightfieldsize" }, Global.Parser.Option.StripDefaultToken, out string[] b);

                base.Configure(b[0]);

                int ws = wfs == "" ? 2 : int.Parse(wfs);
                wg = new double?[rows][][];

                wc = new double?[rows][];

                for (int i = 0; i < rows; i++)
                {
                    // set weight correction
                    wc[i] = new double?[cols];
                    // set weights
                    wg[i] = new double?[cols][];
                    for (int j = 0; j < cols; j++)
                    {
                        wg[i][j] = new double?[ws];
                        wg[i][j][0] = RandomStandard(0.00001);
                        wg[i][j][1] = 0.0;
                        wg[i][j][2] = 0.0;
                    }
                }
                //Initialize weight correction to zero
                for (int u = 0; u < this.rows; u++)
                {
                    for (int v = 0; v < this.cols; v++)
                    {
                        wc[u][v] = 0;
                    }
                }
                SetElement(wc);

                return this;
            }

            public override CNN.Kernel Configure(int rows, int cols, int? stride, int? padding)
            {
                return Configure(rows, cols, stride, padding, 2);
            }

            public virtual Kernel Configure(int rows, int cols, int? stride, int? padding, int? weightfieldsize)
            {
                base.Configure(rows, cols, stride.Value, padding.Value);

                int ws = weightfieldsize == null ? 2 : weightfieldsize.Value;
                wg = new double?[this.rows][][];

                wc = new double?[this.rows][];

                for (int i = 0; i < this.rows; i++)
                {
                    // set weight correction
                    wc[i] = new double?[this.cols];
                    // set weights
                    wg[i] = new double?[this.cols][];
                    for (int j = 0; j < cols; j++)
                    {
                        wg[i][j] = new double?[ws];
                        wg[i][j][0] = RandomStandard(0.1);
                        wg[i][j][1] = 0.0;
                        wg[i][j][2] = 0.0;
                    }
                }
                //Initialize weight correction to zero
                for (int u = 0; u < this.rows; u++)
                {
                    for (int v = 0; v < this.cols; v++)
                    {
                        wc[u][v] = 0;
                    }
                }
                SetElement(wc);

                return this;
            }
                                   
            public virtual Kernel Configure(int rows, int cols, int? stride, int? padding, int? weightfieldsize, bool bias)
            {
                base.Configure(rows, cols, stride.Value, padding.Value);

                int ws = weightfieldsize == null ? 2 : weightfieldsize.Value;
                wg = new double?[this.rows][][];

                wc = new double?[this.rows][];

                for (int i = 0; i < this.rows; i++)
                {
                    // set weight correction
                    wc[i] = new double?[this.cols];
                    // set weights
                    wg[i] = new double?[this.cols][];
                    for (int j = 0; j < cols; j++)
                    {
                        wg[i][j] = new double?[ws];
                        wg[i][j][0] = 0.0;
                        wg[i][j][1] = 0.0;
                        wg[i][j][2] = 0.0;
                    }
                }
                for (int u = 0; u < this.rows; u++)
                {
                    for (int v = 0; v < this.cols; v++)
                    {
                        wc[u][v] = 0;
                    }
                }

                SetElement(wc);

                return this;
            }

            public double RandomStandard(double scale)
        {
            double u1 = 1.0 - random.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - random.NextDouble();
            //double u1 = 1.0 - Math.Daemon.NextDoubleArray(1, 0, 1)[0]; //uniform(0,1] random doubles
            //double u2 = 1.0 - Math.Daemon.NextDoubleArray(1, 0, 1)[0];
            double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Sin(2.0 * System.Math.PI * u2); //random normal(0,1)
            return randStdNormal * scale;
        }


            public override double Next(int row, int col)
            {
                GetSourceNodes(row, col);
                double sum = 0.0, p;

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (sources[i][j] == null)
                            continue;
                        p = sources[i][j].GetElement<double?[]>()[Global.Sig].Value;
                        sum += p * wg[i][j][Global.Sig].Value;
                    }
                }

                return sum;
            }

            public override void Next(double y, int row, int col)
            {
                GetSourceNodes(row, col);
                double p;
                
                // 0. compute weight correction
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (sources[i][j] == null)
                            continue;
                        p = ((double?[])sources[i][j].Element)[Global.Sig].Value;
                        wc[i][j] = wc[i][j].Value + (y * p);
                    }
                }

                // 1. propagate error
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (sources[i][j] == null)
                            continue;
                        p = ((double?[])sources[i][j].Element)[Global.Err].Value;
                        ((double?[])sources[i][j].Element)[Global.Err] = (y * wg[i][j][Global.Sig].Value) + p;
                    }
                }
            }

            public double?[][][] Weights
            {
                get { return wg; }
                set { wg = value; }
            }

            public double?[][] WeightCorrection
            {
                get { return GetElement<double?[][]>(); }
            }

            public override string ToString()
            {
                string s = "weights";
                double d;

                for (int i = 0; i < wg.Length; i++)
                {
                    s += "\n";
                    for (int j = 0; j < wg[i].Length; j++)
                    {
                        s += "[";
                        for (int k = 0; k < wg[i][j].Length; k++)
                        {
                            if (wg[i][j][k] == null)
                                s += "?#.### ";
                            else
                            {
                                d = System.Math.Round(wg[i][j][k].Value, 3);
                                if (d >= 0.0)
                                    s += "+";
                                s += d.ToString("0.000");
                                s += " ";
                            }
                        }
                        s += "]";
                    }
                }

                s += "\n\nweight correction";
                for (int i = 0; i < wc.Length; i++)
                {
                    s += "\n";
                    for (int j = 0; j < wc[i].Length; j++)
                    {
                        s += "[";
                        if (wc[i][j] == null)
                            s += "?#.### ";
                        else
                        {
                            d = System.Math.Round(wc[i][j].Value, 3);
                            if (d >= 0.0)
                                s += "+";
                            s += d.ToString("0.000");
                            s += " ";
                        }
                        s += "]";
                    }
                }

                return s;
            }
        }
    }
}