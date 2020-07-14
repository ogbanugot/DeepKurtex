using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AI.Graphics.Frame
{
    public enum Move { D, DL, L, LU, U, UR, R, RD }

    [Serializable]
	public class Control
    {
        private PictureBox pictureBox = null;
        private Model model;
        private IList<Color> borderColors = new List<Color>();

        public Control() { }

        public Button ButtonShow { get; set; }

        public IList<Color> BorderColor
        {
            get { return borderColors; }
        }

        public ComboBox ComboBoxBorderWidth { get; set; }

        public ComboBox ComboBoxColors { get; set; }

        public void Change(string shapeAttribute)
        {
            if (model.Visibility == Model.Visible.Yes)
                PaintStrip();

            switch (shapeAttribute)
            {
                case "BorderColor":
                    model.BorderColor = borderColors[ComboBoxColors.SelectedIndex];
                    break;
                    
                case "BorderWidth":
                    model.Configure(model.X, model.Y, model.Height, model.Width, ComboBoxBorderWidth.SelectedIndex + 1, model.BorderColor);
                    break;

                case "Size":
                    // 1. get size parameters
                    string[] a = TextBoxResize.Text.Split(new char[] { ' ', ';', ',' }, StringSplitOptions.RemoveEmptyEntries), b;
                    int h = 20, w = 20;
                    for (int i = 0; i < a.Length; i++)
                    {
                        b = a[i].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        switch (b[0])
                        {
                            case "h":
                                h = int.Parse(b[1]);
                                break;
                            case "w":
                                w = int.Parse(b[1]);
                                break;
                            default:
                                throw new Exception();
                        }
                    }

                    // 2. configure model
                    model.Configure(model.X, model.Y, h, w, model.BorderWidth + 1, model.BorderColor);
                    break;
            }

            CopyStrip();

            if (model.Visibility == Model.Visible.Yes)
                PaintBorder();
        }

        public void Close()
        {
            if (model.Visibility == Model.Visible.Yes)
                PaintStrip();
        }

        protected void CopyStrip()
        {
            Model.Pixel pixel;

            for (int i = 0; i < model.Border.Count; i++)
            {
                pixel = model.Border[i];
                if ((pixel.X < 0) || (pixel.Y < 0) || (pixel.X >= pictureBox.Image.Height) || (pixel.Y >= pictureBox.Image.Height))
                {
                    pixel.ImageColor = null;
                    continue;
                }
                pixel.ImageColor = ((Bitmap)pictureBox.Image).GetPixel(pixel.X, pixel.Y);
            }
        }

        public int? GetColorIndex(Color borderColor)
        {
            for (int i = 0; i < borderColors.Count; i++)
                if (borderColor == borderColors[i])
                    return i;
            return null;
        }

        public void Initialize(int height, int width, int borderwidth, Color borderColor, int stepSize, params int[] initialPosition)
        {
            // initialize colors
            borderColors = new Color[] {
                Color.Black, Color.Aquamarine, Color.Blue, Color.DarkCyan, Color.Green, Color.Red, Color.Gray };
            if (GetColorIndex(borderColor) == null)
                borderColors.Add(borderColor);

            // instantiate model
            model = new Model(0, 0, height, width, borderwidth, borderColor);
            model.Visibility = Model.Visible.No;

            // initialize step size
            Sx = stepSize;
            Sy = stepSize;
        }

        public Model Model
        {
            get { return model; }
        }

        public void Move(Move? direction)
        {
            if (model.Visibility == Model.Visible.Yes)
                PaintStrip();

            int?[] stepSize = null;

            if (direction == null)
                model.Move(Sx, Sy);
            else
            {
                stepSize = StepSize;

                if ((stepSize[0].Value != Sx.Value) || (stepSize[1].Value != Sy.Value))
                    StepSize = new int?[] { Sx.Value, Sy.Value };

                switch (direction.Value)
                {
                    case Frame.Move.D:
                        model.Move(0, stepSize[1].Value);
                        break;
                    case Frame.Move.DL:
                        model.Move(0 - stepSize[0].Value, stepSize[1].Value);
                        break;
                    case Frame.Move.L:
                        model.Move(0 - stepSize[0].Value, 0);
                        break;
                    case Frame.Move.LU:
                        model.Move(0 - stepSize[0].Value, 0 - stepSize[1].Value);
                        break;
                    case Frame.Move.U:
                        model.Move(0, 0 - stepSize[1].Value);
                        break;
                    case Frame.Move.UR:
                        model.Move(stepSize[0].Value, 0 - stepSize[1].Value);
                        break;
                    case Frame.Move.R:
                        model.Move(stepSize[0].Value, 0);
                        break;
                    case Frame.Move.RD:
                        model.Move(stepSize[0].Value, stepSize[1].Value);
                        break;
                }
            }

            CopyStrip();

            if (model.Visibility == Model.Visible.Yes)
                PaintBorder();
        }

        protected void PaintBorder()
        {
            Model.Pixel pixel;
            Bitmap bitmap = (Bitmap)pictureBox.Image;

            for (int i = 0; i < model.Border.Count; i++)
            {
                pixel = model.Border[i];
                if ((pixel.X < 0) || (pixel.Y < 0) || (pixel.X >= pictureBox.Image.Height) || (pixel.Y >= pictureBox.Image.Width))
                    continue;
                bitmap.SetPixel(pixel.X, pixel.Y, model.BorderColor);
            }

            pictureBox.Refresh();
        }

        protected void PaintStrip()
        {
            Model.Pixel pixel;
            Bitmap bitmap = (Bitmap)pictureBox.Image;

            for (int i = 0; i < model.Border.Count; i++)
            {
                pixel = model.Border[i];
                if ((pixel.X < 0) || (pixel.Y < 0) || (pixel.X >= pictureBox.Image.Height) || (pixel.Y >= pictureBox.Image.Height))
                    continue;
                bitmap.SetPixel(pixel.X, pixel.Y, pixel.ImageColor.Value);
            }

            pictureBox.Refresh();
        }

        public PictureBox PictureBox
        {
            set { pictureBox = value; }
        }

        public void SetDefaultSteps()
        {
            int?[] steps = StepSize;

            Sx = steps[0].Value;
            Sy = steps[1].Value;
        }

        public void Show()
        {
            switch (ButtonShow.Text)
            {
                case "Hide":
                    PaintStrip();
                    model.Visibility = Model.Visible.No;
                    ButtonShow.Text = "Show";
                    break;

                case "Show":
                    CopyStrip();
                    PaintBorder();
                    model.Visibility = Model.Visible.Yes;
                    ButtonShow.Text = "Hide";
                    break;
            }
        }

        public int?[] StepSize
        {
            get
            {
                int? x = int.Parse(TextBoxSteps[0].Text);
                int? y = int.Parse(TextBoxSteps[1].Text);

                return new int?[] { x, y };
            }
            set
            {
                TextBoxSteps[0].Text = value[0].Value.ToString();
                TextBoxSteps[1].Text = value[1].Value.ToString();
            }
        }

        public int? Sx { get; set; }

        public int? Sy { get; set; }

        public TextBox[] TextBoxSteps { get; set; }

        public TextBox TextBoxResize { get; set; }
    }
}