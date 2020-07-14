using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Graphics.Frame
{
    [Serializable]
	public class Model
    {
        public enum Visible
        {
            No, Yes
        }

        protected int? borderWidth = 1;
        protected int x, y, _X, _Y, vH, vW;

        protected IList<Pixel> border = new List<Pixel>();
        protected Color color;
        protected Visible? visibility = Visible.No;

        public Model(int X, int Y, int viewHeight, int viewWidth, int? borderWidth, Color color)
        {
            Configure(X, Y, viewHeight, viewWidth, borderWidth, color);
        }

        public IList<Pixel> Border
        {
            get { return border; }
        }

        public Bitmap Bitmap { get; set; }

        public int? BorderWidth
        {
            get { return borderWidth; }
        }

        public virtual Model Configure(int X, int Y, int viewHeight, int viewWidth, int? borderWidth, Color color)
        {
            this.border.Clear();

            this.borderWidth = borderWidth.Value;
            this.color = color;

            _X = X;
            _Y = Y;

            x = _X - borderWidth.Value;
            y = _Y - borderWidth.Value;

            int fW = viewWidth + (2 * borderWidth.Value);
            int fH = viewHeight + (2 * borderWidth.Value);

            int r, c;

            for (int i = 0; i < fH; i++)
            {
                for (int j = 0; j < fW; j++)
                {
                    r = x + i;
                    c = y + j;

                    if ((r < _X) || (c < _Y) || (r >= _X + viewHeight) || (c >= _Y + viewWidth))
                        border.Add(new Pixel(r, c, null));
                }
            }

            vH = viewHeight;
            vW = viewWidth;

            return this;
        }

        public int Height
        {
            get { return vH; }
        }

        public void Move(int? Sx, int? Sy)
        {
            Pixel p;

            for (int i = 0; i < border.Count; i++)
            {
                p = border[i];
                p.X += Sx.Value;
                p.Y += Sy.Value;
            }

            _X = border[0].X + borderWidth.Value;
            _Y = border[0].Y + borderWidth.Value;
        }

        public Color BorderColor
        {
            get { return color; }
            set { color = value; }
        }

        public int Width
        {
            get { return vW; }
        }

        public int X
        {
            get { return _X; }
        }

        public int Y
        {
            get { return _Y; }
        }

        [Serializable]
	public class Pixel
        {
            public Pixel(int X, int Y, Color? ImageColor)
            {
                this.ImageColor = (ImageColor == null ? Color.Black : ImageColor);

                this.X = X;
                this.Y = Y;
            }

            public Color? ImageColor { get; set; }

            public int X { get; set; }
            public int Y { get; set; }

        }

        public Visible? Visibility
        {
            get { return visibility; }
            set { visibility = value.Value; }
        }
    }
}
