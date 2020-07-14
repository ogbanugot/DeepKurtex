using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AI.ML.ANN.Enums;
using AI.ML.ANN;

namespace AI.ML.CNN
{
    public abstract class Filter : Foundation.Node
    {
        protected Function function;
        protected IList<Kernel> kernels = new List<Kernel>();
        protected IList<fMap> sources = new List<fMap>();
        protected fMap target = null;

        protected int outputfieldsize = 2;

        public Filter()
            : base()
        {
            // construct filter object
        }

        /// <summary>
        /// configure filter
        /// </summary>
        /// <param name="configuration">activation=logistic(a=-1, b=2.0, c=5.0); kernel=avgpool(size=7, padding=2, stride=2); outputfieldsize=2</param>
        /// <returns></returns>
        public virtual Filter Configure(string configuration)
        {
            // 0. initialize strings
            string cfg = Global.Parser.RemoveWhiteSpaces(configuration), c;
            string[] a = Global.Parser.Split(cfg, ";"), d = null;
            string[] e = new string[1];
            Kernel kernel;

            // 1. construct function
            c = Global.Parser.Extract<string>(a, new string[] { "act", "activation" }, Global.Parser.Option.None, out string[] b);
            if (c != default)
            {
                d = Global.Parser.Split(c, "(", ")");
                function = Function.MakeNew(d[0]);
                function.Configure(d[1]);
            }

            // 2. construct kernel
            c = Global.Parser.Extract<string>(b, new string[] { "ker", "kernel" }, Global.Parser.Option.None, out a);
            if (c != default)
            {
                d = Global.Parser.Split(c, "(", ")");
                for (int i = 0; i < sources.Count; i++)
                {
                    kernel = Kernel.MakeNew(d[0]);
                    //e[1] = d[1];
                    kernel.Configure(d[1]);
                    kernel.Source = sources[i];
                    kernels.Add(kernel);
                }
            }

            // 3. construct targetMap
            kernel = Kernel.MakeNew(d[0]);
            kernel.Configure(d[1]);

            // 4. get outputfieldsize of fmap
            for (int i = 0; i < a.Length; i++)
            {
                d = Global.Parser.Split(a[i], "=");
                switch (d[0])
                {
                    case "outputfieldsize":
                        outputfieldsize = int.Parse(d[1]);
                        break;
                }
            }

            // size = (((src.Size - kernel.Size) + (2 * kernel.Padding)) / kernel.Stride) + 1;
            int rows = ((sources[0].Rows - kernel.Rows + (2 * kernel.Padding)) / kernel.Stride) + 1;
            int cols = ((sources[0].Columns - kernel.Columns + (2 * kernel.Padding)) / kernel.Stride) + 1;
            target = new fMap();
            target.Configure<double?>(rows, cols, outputfieldsize);

            // 5. return object
            return this;
        }

        public Kernel[] Kernels
        {
            get { return kernels.ToArray(); }
        }

        public abstract void Next<T>(Propagate prop);

        public fMap Source
        {
            set { sources.Add(value); }
        }

        public virtual fMap Target
        {
            get { return target; }
        }
    }
}