using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AI.ML.ANN;
using AI.ML.ANN.Enums;

namespace AI.ML.CNN.Layers
{
    public class Pooling : Layer
    {
        public Pooling()
            : base() { }

        public override Model.Unit Configure(string configuration)
        {
            string cfg = Global.Parser.RemoveWhiteSpaces(configuration);

            Filter pl;

            for (int i = 0; i < Input.Count; i++)
            {
                pl = new Filter();
                pl.Source = Input[i];
                pl.Configure(cfg);

                fmaps.Add(pl.Target);

                filters.Add(pl);
            }

            return this;
        }

        public virtual Layer Configure<T>(double?[] funcparams, int kernelSize, int? stride, int? outputfieldSize)
            where T : ANN.Function, new()
        {
            Filter pl;

            for (int i = 0; i < Input.Count; i++)
            {
                pl = new Filter();
                pl.Source = Input[i];
                pl.Configure<T>(funcparams, kernelSize, stride, outputfieldSize);

                fmaps.Add(pl.Target);

                filters.Add(pl);
            }

            return this;
        }

        public class Filter : CNN.Filter
        {
            public Filter()
                : base() { }
            public override CNN.Filter Configure(string configuration)
            {
                base.Configure(configuration);

                return this;
            }

            public override CNN.Filter Configure<T>(double?[] funcparams, int kernelSize, int? stride, int? outputfieldSize)
            {
                base.Configure<T>(funcparams, kernelSize, stride, outputfieldSize);

                return this;
            }

            protected override void ConstructKernel()
            {
                for (int i = 0; i < source.Count; i++)
                    kernel.Add(new CNN.Kernel(source[i], padding, kernelSize.Value));

                function.Input = new double[kernelSize.Value * kernelSize.Value];
            }

            protected override void ConstructTargetMap()
            {
                int nr = ((rows.Value - kernelSize.Value) / stride.Value) + 1;
                int nc = ((cols.Value - kernelSize.Value) / stride.Value) + 1;

                target = new fMap();
                target.Configure(nr, nc);

                for (int i = 0; i < nr; i++)
                    for (int j = 0; j < nc; j++)
                        target.SetElement(i, j, new Foundation.Node(new double?[outputfieldSize]));
            }

            public override object Gradient
            {
                get { return null; }
                set { ; }
            }

            public override void Next<T>(Propagate prop)
            {
                Foundation.Node[] n;

                switch (prop)
                {
                    case Propagate.Error:

                        double x, y;

                        for (int i = 0; i < target.Rows; i++)
                        {
                            for (int j = 0; j < target.Columns; j++)
                            {
                                x = ((double?[])target.GetElement(i, j).Element)[Global.Err].Value;
                                y = function.Next(x);
                                n = kernel[0].GetSourceNodes(i * stride.Value, j * stride.Value);
                                for (int c = 0, u = 0; u < kernelSize.Value; u++)
                                    for (int v = 0; v < kernelSize.Value; v++)
                                        ((double?[])n[c++].Element)[Global.Err] += y;
                            }
                        }

                        return;

                    case Propagate.Signal:

                        int nr = rows.Value - kernelSize.Value;
                        int nc = cols.Value - kernelSize.Value;
                        double[] output;
                        for (int i = 0; i <= nr; i += stride.Value)
                        {
                            for (int j = 0; j <= nc; j += stride.Value)
                            {
                                output = kernel[0].Output(i, j);
                                for (int k = 0; k < output.Length; k++)
                                    function.Input[k] = output[k];
                                Target.WriteAt<double?>(i / stride.Value, j / stride.Value, 0, function.Next());
                            }
                        }

                        return;
                }
            }
        }
    }
}