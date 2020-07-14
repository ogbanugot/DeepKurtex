using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using AI.Core;

namespace AI.ML.CNN.Images
{
    [Serializable]
	public class Gray : Image
    {
        public Gray()
            : base() { }

        public override Model.Unit Configure(string configuration)
        {
            base.Configure(configuration);

            Foundation.Node node;

            // 1. set delta
            dq = (max.Value - min.Value) / 255;

            // 2. initialize fmap
            fMap fmap = (fMap)new fMap()
                .Configure(rows, cols);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    node = new Foundation.Node(new double?[] { null, null });
                    fmap.SetElement(i, j, node);
                }
            }

            fmaps.Add(fmap);

            return this;
        }

        public override Image Configure(int rows, int cols, double? min, double? max)
        {
            Foundation.Node node;

            // 0. set range
            this.max = (max == null ? +1.0 : max.Value);
            this.min = (min == null ? -1.0 : min.Value);

            this.cols = cols;
            this.rows = rows;

            // 1. set delta
            dq = (max.Value - min.Value) / 255;

            // 2. initialize fmap
            fMap fmap = (fMap)new fMap()
                .Configure(rows, cols);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    node = new Foundation.Node(new double?[] { null, null });
                    fmap.SetElement(i, j, node);
                }
            }

            fmaps.Add(fmap);

            return this;
        }

        public override Bitmap Bitmap 
        { 
            get => base.Bitmap;
            set
            {
                if ((value.Height != rows) || (value.Width != cols))
                    throw new Exception();

                System.Drawing.Color c; Foundation.Node n;

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        c = value.GetPixel(i, j);
                        if ((c.R != c.G) && (c.R != c.B))
                            throw new Exception();
                        n = Output[0].GetElement(i, j);
                        ((double?[])n.Element)[0] = convertFromRGB(c.R);
                    }
                }
            }
        }

        public override fData fData
        {
            set
            {
                int size = (int)System.Math.Sqrt(value.Data.Length);

                if ((size != rows) || (size != cols))
                    throw new Exception();
                Foundation.Node n;
                for (int c = 0, i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        n = Output[0].GetElement(i, j);
                        ((double?[])n.Element)[0] = value.Data[c++];
                    }
                }
            }
        }

        protected override System.Drawing.Color CreateColor(int i, int j)
        {
            Foundation.Node n; double c;

            n = fmaps[0].GetElement(i, j);
            c = ((double?[])n.Element)[0].Value;

            return System.Drawing.Color.FromArgb(convertToRGB(c), convertToRGB(c), convertToRGB(c));

        }

        public override string ToString()
        {
            double? c;
            Foundation.Node n; int disp = 100, size = 5;

            string s = "\n\nGRAY....[5 x 5]\n";

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    n = fmaps[0].GetElement(i + disp, j + disp);
                    c = ((double?[])n.Element)[0];
                    s += (c == null ? "#" : c.Value.ToString("e4")) + ", ";
                }

                s = s.TrimEnd(' ', ',');
            }

            return s;
        }
    }
}