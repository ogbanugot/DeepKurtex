using System;
using System.Collections.Generic;

using System.Drawing;

using AI.ML.ANN;

namespace AI.ML.CNN
{
    [Serializable]
    public abstract class Image : Model.Unit
    {
        protected int cols, rows;

        protected double? min = null, max = null;
        protected double dq;

        public Image()
            : base() { }

        public virtual Bitmap Bitmap
        {
            get
            {
                Bitmap b = new Bitmap(cols, rows);
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        b.SetPixel(i, j, CreateColor(i, j));
                return b;
            }
            set => throw new NotImplementedException();
        }

        public abstract AI.Core.fData fData { set; }

        public void Clear()
        {
            for (int i = 0; i < fmaps.Count; i++)
                fmaps[i].Clear<double?>();
        }

        public int Columns
        {
            get { return cols; }
        }

        public override Model.Unit Configure(string configuration)
        {
            // 0. get tokens
            string cfg = Global.Parser.RemoveWhiteSpaces(configuration);
            string[] a = Global.Parser.Split(cfg, ";"), b;
            
            // 1. set defaults
            min = -1; max = 1;

            // 2. initialize parameters
            for (int i = 0; i < a.Length; i++)
            {
                b = Global.Parser.Split(a[i], "=");

                switch (b[0])
                {
                    case "cols":
                        cols = int.Parse(b[1]);
                        break;

                    case "rows":
                        rows = int.Parse(b[1]);
                        break;

                    case "max":
                        b[1] = Global.Parser.StripDefaultToken(b[1]);
                        max = int.Parse(b[1]);
                        break;

                    case "min":
                        b[1] = Global.Parser.StripDefaultToken(b[1]);
                        min = int.Parse(b[1]);
                        break;
                }
            }

            return this;
        }

        public abstract Image Configure(int rows, int cols, double? min, double? max);

        public static Bitmap ConvertImageToGray(Bitmap image)
        {
            double w, y, u, v, uv; Foundation.Node n;

            double[] b = new double[5];
            double[] g = new double[5];
            double[] r = new double[5];

            Images.Color imap = new Images.Color();
            imap.Configure(image.Height, image.Width, null, null);
            imap.Bitmap = image;

            for (int i = 0; i < imap.Rows; i++)
            {
                for (int j = 0; j < imap.Columns; j++)
                {
                    n = imap.fMapB.GetElement(i, j);
                    b[0] = ((double?[])n.Element)[0].Value;
                    n = imap.fMapG.GetElement(i, j);
                    g[0] = ((double?[])n.Element)[0].Value;
                    n = imap.fMapR.GetElement(i, j);
                    r[0] = ((double?[])n.Element)[0].Value;

                    if ((b[0] == 1.0) && (g[0] == 1.0) && (r[0] == 1.0))
                        continue;

                    if ((b[0] == -1.0) && (g[0] == -1.0) && (r[0] == -1.0))
                        continue;

                    y = (0.299 * r[0]) + (0.587 * g[0]) + (0.114 * b[0]);
                    u = (b[0] - y) * 0.565;
                    v = (r[0] - y) * 0.713;

                    uv = u + v;

                    r[1] = r[0] * 0.299;
                    r[2] = r[0] * 0.587;
                    r[3] = r[0] * 0.114;
                    g[1] = g[0] * 0.299;
                    g[2] = g[0] * 0.587;
                    g[3] = g[0] * 0.114;
                    b[1] = b[0] * 0.299;
                    b[2] = b[0] * 0.587;
                    b[3] = b[0] * 0.114;

                    r[4] = (r[1] + r[2] + r[3]) / 3.0;
                    g[4] = (g[1] + g[2] + g[3]) / 3.0;
                    b[4] = (b[1] + b[2] + b[3]) / 3.0;

                    w = (r[4] + g[4] + b[4] + uv) / 4.0;

                    n = imap.fMapB.GetElement(i, j);
                    ((double?[])n.Element)[0] = w;
                    n = imap.fMapG.GetElement(i, j);
                    ((double?[])n.Element)[0] = w;
                    n = imap.fMapR.GetElement(i, j);
                    ((double?[])n.Element)[0] = w;
                }
            }

            return imap.Bitmap;
        }

        protected double convertFromRGB(int v)
        {
            return (v * dq) + min.Value;
        }

        protected int convertToRGB(double v)
        {
            return (int)((v - min.Value) / dq);
        }

        protected abstract Color CreateColor(int i, int j);

        public int Rows
        {
            get { return rows; }
        }
    }
}