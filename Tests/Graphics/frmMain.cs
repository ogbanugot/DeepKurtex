using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AI.Graphics.Frame;

namespace Tests
{
    partial class frmMain : Form
    {
        partial void test_graphics_frameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const int height = 20, width = 20, borderWdith = 2, stepSize = 5, xpos = 30, ypos = 30;
            // 1. instantiate control
            AI.Graphics.Frame.Control c = new AI.Graphics.Frame.Control();
            c.Initialize(height, width, borderWdith, Color.Black, stepSize, xpos, ypos);
            c.PictureBox = pictureBox;

            // 2. instantiate view
            AI.Graphics.Frame.View v = new AI.Graphics.Frame.View();
            v.Control = c;

            v.ShowDialog();
        }
    }
}