using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AI.Graphics.Frame
{
    public partial class View : Form
    {
        private AI.Graphics.Frame.Control control = null;

        public View()
        {
            InitializeComponent();
        }

        public AI.Graphics.Frame.Control Control
        {
            set
            {
                value.ButtonShow = btnShow;

                // 1. set and initialize color combobox
                value.ComboBoxColors = cmbBoxBorderColor;
                cmbBoxBorderColor.Items.Clear();
                for (int i = 0; i < value.BorderColor.Count; i++)
                    cmbBoxBorderColor.Items.Add(value.BorderColor[i].ToString());
                cmbBoxBorderColor.SelectedIndex = value.GetColorIndex(value.Model.BorderColor).Value;

                // 2. set and initialize bordersize combobox
                value.ComboBoxBorderWidth = cmbBoxBorderSize;
                for (int i = 1; i <= 5; i++)
                    cmbBoxBorderSize.Items.Add(i.ToString());
                cmbBoxBorderSize.SelectedIndex = value.Model.BorderWidth.Value - 1;

                // 3. set and initialize step size textboxes
                value.TextBoxSteps = new TextBox[] { txtBoxXStepSize, txtBoxYStepSize };
                txtBoxXStepSize.Text = value.Sx.ToString();
                txtBoxYStepSize.Text = value.Sy.ToString();

                // 4. set and initialize resize textbox
                value.TextBoxResize = txtBoxResize;
                txtBoxResize.Text = "h:" + value.Model.Height.ToString() + "; w:" + value.Model.Width.ToString();

                control = value;
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            control.Move(Frame.Move.D);
        }

        private void btnDownLeft_Click(object sender, EventArgs e)
        {
            control.Move(Frame.Move.DL);
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            control.Move(Frame.Move.L);
        }

        private void btnLeftUp_Click(object sender, EventArgs e)
        {
            control.Move(Frame.Move.LU);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            control.Move(null);
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            control.Move(Frame.Move.R);
        }

        private void btnRightDown_Click(object sender, EventArgs e)
        {
            control.Move(Frame.Move.RD);
        }

        private void btnSetAsDefault_Click(object sender, EventArgs e)
        {
            control.SetDefaultSteps();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            control.Move(Frame.Move.U);
        }

        private void btnUpRight_Click(object sender, EventArgs e)
        {
            control.Move(Frame.Move.UR);
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            control.Show();
        }

        private void view_FormClosed(object sender, FormClosedEventArgs e)
        {
            control.Close();
        }

        private void cmbBoxBorderColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (control == null)
                return;
            control.Change("BorderColor");
        }

        private void cmbBoxBorderSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (control == null)
                return;
            control.Change("BorderWidth");
        }

        private void btnResize_Click(object sender, EventArgs e)
        {
            control.Change("Size");
        }
    }
}