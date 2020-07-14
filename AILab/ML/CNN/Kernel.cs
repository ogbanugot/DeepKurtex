using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Node = Foundation.Node;

namespace AI.ML.CNN
{
    [Serializable]
    public abstract class Kernel : Node
    {
        protected int cols, rows;
        protected int? padding, stride;

        protected Node[][] sources;

        public Kernel() { }

        /// <summary>
        /// configures string
        /// </summary>
        /// <param name="configuration">size=7, stride=2(def:1), padding=1(def:0)</param>
        /// <returns></returns
        public virtual Kernel Configure(string configuration)
        {
            string cfg = ANN.Global.Parser.RemoveWhiteSpaces(configuration);
            string[] a = ANN.Global.Parser.Split(cfg, ","), b;

            padding = 0; stride = 1;

            for (int i = 0; i < a.Length; i++)
            {
                a[i] = ANN.Global.Parser.StripDefaultToken(a[i]);
                b = ANN.Global.Parser.Split(a[i], "=");

                switch (b[0])
                {
                    case "padding":
                        padding = int.Parse(b[1]);
                        break;
                    case "size":
                        cols = int.Parse(b[1]);
                        rows = cols;
                        break;
                    case "stride":
                        stride = int.Parse(b[1]);
                        break;
                }
            }

            sources = new Node[rows][];
            for (int i = 0; i < rows; i++)
                sources[i] = new Node[cols];

            return this;
        }

        /// <summary>
        /// constructs a kernel obejct
        /// </summary>
        /// <param name="padding">size of padding</param>
        /// <param name="size">size of kernel</param>
        public virtual Kernel Configure(int rows, int cols, int? stride, int? padding)
        {
            this.cols = cols;
            this.rows = rows;

            this.padding = padding == null ? 0 : padding.Value;
            this.stride = stride == null ? 1 : stride.Value;

            sources = new Node[this.rows][];
            for (int i = 0; i < this.rows; i++)
                sources[i] = new Node[this.cols];

            return this;
        }

        public int Columns
        {
            get { return cols; }
        }

        protected void GetSourceNodes(int row, int col)
        {
            int u = (row * stride.Value) - padding.Value;
            int v = (col * stride.Value) - padding.Value;
            int x, y;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    x = i + u;
                    y = j + v;

                    sources[i][j] = (x < 0) || (x >= Source.Rows) || (y < 0) || (y >= Source.Columns) ? null : Source.GetElement(x, y);
                }
            }
        }

        public static Kernel MakeNew(string name)
        {
            switch (name)
            {
                case "avgp":
                case "avgpool":
                case "Avgpool":
                    return new Kernels.Avgpool();

                case "conv":
                case "convolution":
                case "Convolution":
                    return new Layers.Convolution.Kernel();

                case "maxp":
                case "maxpool":
                case "Maxpool":
                    return new Kernels.Maxpool();

                case "minp":
                case "minpool":
                case "Minpool":
                    return new Kernels.Minpool();                
            }

            throw new Exception();
        }

        /// <summary>
        /// get the next output value
        /// </summary>
        /// <param name="row">row index of target feature map</param>
        /// <param name="col">col index of target feature map</param>
        /// <returns></returns>
        public abstract double Next(int row, int col);

        public abstract void Next(double y, int row, int col);

        public int Padding
        {
            get { return padding.Value; }
        }

        public int Rows
        {
            get { return rows; }
        }

        public fMap Source { get; set; }

        public int Stride
        {
            get { return stride.Value; }
        }

        public Node[][] SourceNodes
        {
            get { return sources; }
        }

        /// <summary>
        /// returns string value of source nodes
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public string SourceNodesToString(int field)
        {
            string s = "";
            double? x;

            for (int i = 0; i < sources.Length; i++)
            {
                s += "\n";
                for (int j = 0; j < sources[i].Length; j++)
                {
                    if (sources[i][j] == null)
                        s += "+0.000 ";
                    else
                    {
                        x = ((double?[])sources[i][j].Element)[field];
                        if (0.0 <= x.Value)
                            s += "+";
                        s += x.Value.ToString("0.000");
                        s += " ";
                    }
                }
            }

            return s;
        }
    }
}