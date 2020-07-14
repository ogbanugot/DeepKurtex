using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Tests
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            pictureBox.Height = 50;
            pictureBox.Width = 50;
            pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;

            Math.Daemon.Random = new Random();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save text
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "rtx files (*.rtf)|*.rtf|txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.Title = "Save RichText File";
            saveFileDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog.FileName != "")
            { 
                // Saves text in the appropriate doument format based upon the
                // file type selected in the dialog box.
                // NOTE that the FilterIndex property is one-based.
                switch (saveFileDialog.FilterIndex)
                {
                    case 1:
                        using (var sw = new StreamWriter(saveFileDialog.FileName, true))
                        {
                            sw.Write(richTextBox.Text);
                            sw.Close();
                        }

                        break;

                    case 2:
                        
                        break;

                    case 3:
                        
                        break;
                }
            }
        }

        #region TestCore Members

        partial void test_ai_adversarialSearch_tictactoe_boardToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_ai_core_domainToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_core_fDataToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_ai_core_graphToolStripMenuItem1_Click(object sender, EventArgs e);

        partial void test_ai_core_nodeToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_ai_core_queueToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_ai_core_stackToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_ai_core_treeToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_ai_nodes_npuzzleToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_ai_nodes_nqboardToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_ai_search_bestfirstToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_ai_uByteLoaderToolStripMenuItem_Click(object sender, EventArgs e);

        #endregion

        #region TestANN Members

        partial void test_ann_activation_logisticToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_ann_model_acyclicToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_ann_neuron_hiddenPercpetronToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_ann_neuron_outputPerceptronToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_ann_trainer_deltaRuleToolStripMenuItem_Click(object sender, EventArgs e);

        #endregion

        #region TestCNN Members

        partial void test_cnn_backprop_concatenationToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_cnn_backprop_modelToolStripMenuItem1_Click(object sender, EventArgs e);

        partial void test_cnn_depreciated_convolutionToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_cnn_depreciated_maxpoolToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_cnn_filters_concatenationToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_cnn_filters_connectedToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_cnn_filters_convolutionToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_cnn_filters_poolingToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_cnn_fMapToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_cnn_layers_concatenationToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_cnn_layers_connectedToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_cnn_layers_convolutionToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_cnn_layers_imageToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_cnn_layers_poolingToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_cnn_modelToolStripMenuItem_Click(object sender, EventArgs e);

        partial void Test_CNN_trainer_deltaRuleToolStripMenuItem_Click(object sender, EventArgs e);

        partial void Test_CNN_trainer_ADAMToolStripMenuItem_Click(object sender, EventArgs e);

        partial void cifarToolStripMenuItem_Click(object sender, EventArgs e);

        partial void testToolStripMenuItem1_Click(object sender, EventArgs e);


        #endregion

        #region TestGraphics Members

        partial void test_graphics_frameToolStripMenuItem_Click(object sender, EventArgs e);

        #endregion

        #region TestLDT Members

        partial void test_dt_id3_algorithmToolStripMenuItem_Click(object sender, EventArgs e);

        #endregion

        #region TestMath Members

        partial void test_math_directedEdgeToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_math_edgeToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_math_graphBuilderToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_math_graphToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_math_nodeToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_math_randomToolStripMenuItem_Click(object sender, EventArgs e);

        #endregion

        #region TestSearch Members

        partial void test_search_buzzerToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_search_crawlerToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_search_environmentToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_search_foragerToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_search_hopperToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_search_tictactoeToolStripMenuItem_Click(object sender, EventArgs e);

        partial void test_search_solutionToolStripMenuItem_Click(object sender, EventArgs e);

        #endregion

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Images (*.bmp;*.jpg;*.gif,*.png,*.tiff)|*.bmp;*.jpg;*.gif;*.png;*.tiff|All files (*.*)|*.*";
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Select Photos";

            DialogResult dr = openFileDialog.ShowDialog();

            if (dr == DialogResult.OK)
            {
                switch (openFileDialog.FilterIndex)
                {
                    case 1:
                        try
                        {
                            Bitmap src = new Bitmap(openFileDialog.FileName);
                            Bitmap bmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                            using (Graphics gfx = Graphics.FromImage(bmp))
                            {
                                gfx.DrawImage(src, 0, 0);
                            }

                            pictureBox.Image = bmp;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        break;

                    case 2:
                        // add code to open txt file here
                        break;
                }
            }
        }
        
    }
}