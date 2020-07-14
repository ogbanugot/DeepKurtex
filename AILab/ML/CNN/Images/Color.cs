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
	public class Color : Image
    {
        public Color()
            : base() { }

        /// <summary>
        /// configure image feature map
        /// </summary>
        /// <param name="configuration">rows=100; cols=100; min=-1(def:1); max=1(def:1)</param>
        /// <returns></returns>
        public override Model.Unit Configure(string configuration)
        {
            base.Configure(configuration);

            fMap fmap;
            Foundation.Node node;

            // 1. set delta
            dq = (max.Value - min.Value) / 255;

            // 2. initialize fmaps
            for (int i = 0; i < 3; i++)
            {
                fmap = (fMap)new fMap()
                    .Configure(rows, cols);

                for (int j = 0; j < rows; j++)
                {
                    for (int k = 0; k < cols; k++)
                    {
                        node = new Foundation.Node(new double?[] { null, null });
                        fmap.SetElement(j, k, node);
                    }
                }

                fmaps.Add(fmap);
            }

            return this;
        }

        /// <summary>
        /// configure image feature maps
        /// </summary>
        /// <param name="rows">number of rows</param>
        /// <param name="cols">number of columns</param>
        /// <param name="min">minimum of dynamic range</param>
        /// <param name="max">maximum of dynamic range</param>
        /// <returns></returns>
        public override Image Configure(int rows, int cols, double? min, double? max)
        {
            fMap fmap;
            Foundation.Node node;

            // 0. set range
            this.max = (max == null ? +1.0 : max.Value);
            this.min = (min == null ? -1.0 : min.Value);

            this.cols = cols;
            this.rows = rows;

            // 1. set delta
            dq = (this.max.Value - this.min.Value) / 255;

            // 2. initialize fmaps
            for (int i = 0; i < 3; i++)
            {
                fmap = (fMap)new fMap()
                    .Configure(rows, cols);

                for (int j = 0; j < rows; j++)
                {
                    for (int k = 0; k < cols; k++)
                    {
                        node = new Foundation.Node(new double?[] { null, null });
                        fmap.SetElement(j, k, node);
                    }
                }

                fmaps.Add(fmap);
            }

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

                        // set red
                        n = fMapR.GetElement(i, j);
                        ((double?[])n.Element)[0] = convertFromRGB(c.R);
                        // set green
                        n = fMapG.GetElement(i, j);
                        ((double?[])n.Element)[0] = convertFromRGB(c.G);
                        // set blue;
                        n = fMapB.GetElement(i, j);
                        ((double?[])n.Element)[0] = convertFromRGB(c.B);
                    }
                }
            }
        }

        /// <summary>
        /// converts from grayscale to color pixel
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        protected override System.Drawing.Color CreateColor(int i, int j)
        {
            Foundation.Node n; double[] c = new double[3];

            for (int k = 0; k < 3; k++)
            {
                n = fmaps[k].GetElement(i, j);
                c[k] = ((double?[])n.Element)[0].Value;
            }

            return System.Drawing.Color.FromArgb(convertToRGB(c[0]), convertToRGB(c[1]), convertToRGB(c[2]));
        }

        /// <summary>
        /// blue feature map
        /// </summary>
        public fMap fMapB
        {
            get { return fmaps[2]; }
        }

        /// <summary>
        /// green feature map
        /// </summary>
        public fMap fMapG
        {
            get { return fmaps[1]; }
        }

        /// <summary>
        /// red feature map
        /// </summary>
        public fMap fMapR
        {
            get { return fmaps[0]; }
        }

        public override fData fData
        {
            set
            {
                int size = (int)System.Math.Sqrt(value.Data.Length/3);
                Foundation.Node n;
                if ((size != rows) || (size != cols))
                    throw new Exception();
                int c = 0;
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                    {
                        // set red
                        n = fMapR.GetElement(i, j);
                        ((double?[])n.Element)[0] = value.Data[c++];
                    }                        

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                    {
                        // set green
                        n = fMapG.GetElement(i, j);
                        ((double?[])n.Element)[0] = value.Data[c++];
                    }                        

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                    {
                        // set blue;
                        n = fMapB.GetElement(i, j);
                        ((double?[])n.Element)[0] = value.Data[c++];
                    }                                           
            }
        }

        public override string ToString()
        {
            double? c;
            Foundation.Node n; int disp = 100, size = 5;

            string[] label = new string[] { "\n\nRED....[5 x 5]", "\n\nGREEN....[5 x 5]", "\n\nBLUE....[5 x 5]" };
            string s = "";

            for (int i = 0; i < 3; i++)
            {
                s += label[i];

                for (int x = 0; x < size; x++)
                {
                    s += "\n";

                    for (int y = 0; y < size; y++)
                    {
                        n = fmaps[i].GetElement(x + disp, y + disp);
                        c = ((double?[])n.Element)[0];
                        s += (c == null ? "#" : c.Value.ToString("e4")) + ", ";
                    }

                    s = s.TrimEnd(' ', ',');
                }
            }

            return s;
        }
    }
}
