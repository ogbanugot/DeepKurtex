using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AI.ML.ANN;
using AI.ML.ANN.Enums;

namespace AI.ML.CNN.Layers
{
    public class Convolution : Layer
    {
        public Convolution() { }

        /// <summary>
        /// configures convolution layer
        /// </summary>
        /// <param name="configuration">activation=logistic(a=-1, b=2.0, c=5.0); depth=32; padding=2(def:0); kernelSize=7; stride=2(def:1); outputfieldSize=2</param>
        /// <returns></returns>
        public override Model.Unit Configure(string configuration)
        {
            string cfg = Global.Parser.RemoveWhiteSpaces(configuration);
            string[] a = Global.Parser.Split(cfg, ";");
            int? depth = Global.Parser.Extract<int?>(a, new string[] { "depth" }, Global.Parser.Option.StripDefaultToken, out string[] b);

            cfg = Global.Parser.Build(b, 0, ";");

            Filter fl;

            if (depth == null)
                throw new Exception();

            for (int i = 0; i < depth.Value; i++)
            {
                fl = new Filter();
                for (int j = 0; j < Input.Count; j++)
                    fl.Source = Input[j];

                fl.Configure(cfg);
                fmaps.Add(fl.Target);
                filters.Add(fl);
            }

            return this;
        }

        public virtual Layer Configure<T>(double?[] funcparams, int depth, int kernelSize, int? stride, int? outputfieldSize)
            where T : Function, new()
        {
            Filter fl;

            for (int i = 0; i < depth; i++)
            {
                fl = new Filter();
                for (int j = 0; j < Input.Count; j++)
                    fl.Source = Input[j];

                fl.Configure<T>(funcparams, kernelSize, stride, outputfieldSize);
                fmaps.Add(fl.Target);
                filters.Add(fl);
            }

            return this;
        }

        public CNN.Kernel[][] Kernels
        {
            get
            {
                CNN.Kernel[][] k = new CNN.Kernel[filters.Count][];
                for (int i = 0; i < k.Length; i++)
                    k[i] = ((Filter)filters[i]).Kernels;
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

        public class Filter : CNN.Filter
        {
            protected fMap bias = null;

            public Filter()
                : base() { }

            public override CNN.Filter Configure(string configuration)
            {
                bias = (fMap)new fMap()
                    .Configure(1, 1);
                bias.SetElement(0, 0, new Foundation.Node(new double?[] { 1.0, null }));

                string cfg = Global.Parser.RemoveWhiteSpaces(configuration);
                string[] a = Global.Parser.Split(cfg, ";"), b;
                padding = Global.Parser.Extract<int?>(a, new string[] { "kernelSize" }, Global.Parser.Option.StripDefaultToken, out b) / 2;

                kernel.Add(new Kernel(bias, 0, 1));
                base.Configure(cfg);

                return this;
            }

            public override CNN.Filter Configure<T>(double?[] funcparams, int kernelSize, int? stride, int? outputfieldSize)
            {
                bias = (fMap)new fMap()
                    .Configure(1, 1);
                bias.SetElement(0, 0, new Foundation.Node(new double?[] { 1.0, null }));

                padding = kernelSize / 2;

                Kernel k = new Kernel(bias, 0, 1);
                k.WeightCorrection = new double?[] { null };
                kernel.Add(k);

                base.Configure<T>(funcparams, kernelSize, stride, outputfieldSize);

                return this;
            }

            protected override void ConstructKernel()
            {
                Kernel k; double?[][] wc;

                for (int i = 0; i < source.Count; i++)
                {
                    k = new Kernel(source[i], padding, kernelSize.Value);

                    wc = new double?[kernelSize.Value][];
                    for (int j = 0; j < kernelSize; j++)
                        wc[j] = new double?[kernelSize.Value];
                    k.WeightCorrection = wc;

                    kernel.Add(k);
                }

                function.Input = new double[kernel.Count];
            }

            protected override void ConstructTargetMap()
            {
                int nr = ((rows.Value - kernelSize.Value + (2 * padding.Value)) / stride.Value) + 1;
                int nc = ((cols.Value - kernelSize.Value + (2 * padding.Value)) / stride.Value) + 1;

                double?[][] gradient = new double?[nr][];
                for (int i = 0; i < nr; i++)
                    gradient[i] = new double?[nc];

                Gradient = gradient;

                target = new fMap();
                target.Configure(nr, nc);

                for (int i = 0; i < nr; i++)
                    for (int j = 0; j < nc; j++)
                        target.SetElement(i, j, new Foundation.Node(new double?[outputfieldSize]));
            }

            public override object Gradient
            {
                get { return GetElement<double?[][]>(); }
                set { SetElement(value); }
            }

            public override void Next<T>(Propagate prop)
            {
                double x; double?[][] y; double?[][] c; Foundation.Node[] n;
                Synapse[,] s;

                switch (prop)
                {
                    case Propagate.Error:

                        // 0. set gradients
                        c = (double?[][])Gradient;
                        for (int i = 0; i < target.Rows; i++)
                        {
                            for (int j = 0; j < target.Columns; j++)
                            {
                                x = function.Next(((double?[])target.GetElement(i, j).Element)[Global.Sig].Value);
                                c[i][j] = x * ((double?[])target.GetElement(i, j).Element)[Global.Err].Value;
                            }
                        }

                        // 1. set weight correction in bias
                        x = 0.0;
                        for (int i = 0; i < c.Length; i++)
                            for (int j = 0; j < c[i].Length; j++)
                                x += c[i][j].Value;
                        ((Kernel)kernel[0]).WeightCorrection = x;

                        // 2. set weight correction in kernels
                        for (int i = 1; i < kernel.Count; i++)
                        {
                            y = (double?[][])((Kernel)kernel[i]).WeightCorrection;
                            // set weight corrections to zero
                            for (int j = 0; j < y.Length; j++)
                                for (int k = 0; k < y[j].Length; k++)
                                    y[j][k] = 0.0;

                            for (int j = 0; j < c.Length; j++)
                            {
                                for (int k = 0; k < c[j].Length; k++)
                                {
                                    x = c[j][k].Value;
                                    n = ((Kernel)kernel[i]).GetSourceNodes(j * stride.Value, k * stride.Value);
                                    for (int m = 0, u = 0; u < y.Length; u++)
                                    {
                                        for (int v = 0; v < y[u].Length; v++)
                                        {
                                            if (n[m] == null)
                                                continue;
                                            y[u][v] += x * ((double?[])n[m++].Element)[Global.Sig].Value;
                                        }
                                    }
                                }
                            }
                        }

                        // 3. propagate error to previous layer
                        for (int i = 1; i < kernel.Count; i++)
                        {
                            s = ((Kernel)kernel[i]).Synapse;

                            for (int j = 0; j < c.Length; j++)
                            {
                                for (int k = 0; k < c[j].Length; k++)
                                {
                                    x = c[j][k].Value;
                                    n = ((Kernel)kernel[i]).GetSourceNodes(j * stride.Value, k * stride.Value); // nodes is fMap
                                    for (int m = 0, u = 0; u < s.GetLength(0); u++)
                                    {
                                        for (int v = 0; v < s.GetLength(1); v++)
                                        {
                                            if (n[m] == null)
                                                continue;
                                            ((double?[])n[m++].Element)[Global.Err] += x * s[u, v].Weights[0].Value;
                                        }
                                    }
                                }
                            }
                        }

                        return;

                    case Propagate.Signal:

                        function.Input[0] = kernel[0].Output(0, 0)[0];

                        for (int i = 0; i < rows.Value; i += stride.Value)
                        {
                            for (int j = 0; j < cols.Value; j += stride.Value)
                            {
                                for (int k = 1; k < kernel.Count; k++)
                                    function.Input[k] = kernel[k].Output(i, j)[0];
                                Target.WriteAt<double?>(i / stride.Value, j / stride.Value, 0, function.Next());
                            }
                        }

                        return;
                }
            }
        }
    }
}