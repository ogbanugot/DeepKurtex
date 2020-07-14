using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AI.ML.ANN.Enums;
using AI.ML.ANN;

namespace AI.ML.CNN.Layers
{
    public abstract class Filter : CNN.Filter
    {
        protected Function function;

        protected IList<CNN.Kernel> kernel = new List<CNN.Kernel>();

        protected int? kernelSize;
        protected int? padding = 0;
        protected int? stride = null;

        public Filter() { }

        /// <summary>
        /// configures filter object with configuration string
        /// parameters with default values: padding = 0; stride = 1; outputfieldSize = 2;
        /// </summary>
        /// <param name="configuration">activation=logistic(a=-1, b=2.0, c=5.0); padding=2; kernelSize=7; stride=2; outputfieldSize=2</param>
        /// <returns>filter</returns>
        public override CNN.Filter Configure(string configuration)
        {
            string cfg = Global.Parser.RemoveWhiteSpaces(configuration);

            string[] a = Global.Parser.Split(cfg, ";");
            string c = Global.Parser.Extract<string>(a, new string[] { "act", "activation" }, Global.Parser.Option.None, out string[] b);
            string[] d = Global.Parser.Split(c, "(", ")");

            function = Function.MakeNew(d[0]);
            function.Configure(d[1]);

            stride = 1;
            outputfieldSize = 2;

            for (int i = 0; i < b.Length; i++)
            {
                a = Global.Parser.Split(b[i], "=");
                a[1] = Global.Parser.StripDefaultToken(a[1]);
                int u = int.Parse(a[1]);
                switch (a[0])
                {
                    case "kernelSize":
                        kernelSize = u;
                        continue;
                    case "outputfieldSize":
                        outputfieldSize = u;
                        continue;
                    case "stride":
                        stride = u;
                        continue;
                }
            }

            // 0. assert source
            if (source == null)
                throw new Exception();

            // 1. set rows and columns
            rows = source[0].Rows;
            cols = source[0].Columns;

            for (int i = 1; i < source.Count; i++)
                if ((source[i].Rows != rows.Value) || (source[i].Columns != cols.Value))
                    throw new Exception();

            // 2. construct kernel
            ConstructKernel();

            // 5. construct target map
            ConstructTargetMap();

            return this;
        }

        public virtual Filter Configure<T>(double?[] funcparams, int kernelSize, int? stride, int? outputfieldSize)
            where T : Function, new()
        {
            function = new T()
                .Configure(funcparams);

            this.kernelSize = kernelSize;
            this.outputfieldSize = outputfieldSize.Value;
            this.stride = (stride == null ? 1 : stride.Value);

            // 0. assert source
            if (source == null)
                throw new Exception();

            // 1. set rows and columns
            rows = source[0].Rows;
            cols = source[0].Columns;

            for (int i = 1; i < source.Count; i++)
                if ((source[i].Rows != rows.Value) || (source[i].Columns != cols.Value))
                    throw new Exception();

            // 2. construct kernel
            ConstructKernel();

            // 5. construct target map
            ConstructTargetMap();

            return this;
        }

        protected abstract void ConstructKernel();

        public abstract object Gradient { get; set; }

        public CNN.Kernel[] Kernels
        {
            get { return kernel.ToArray(); }
        }

        public double?[][][][] Weights
        {
            get
            {
                double?[][][][] w = new double?[kernel.Count][][][];
                for (int i = 0; i < w.Length; i++)
                    w[i] = ((Kernel)kernel[i]).Weights;
                return w;
            }
        }
    }
}