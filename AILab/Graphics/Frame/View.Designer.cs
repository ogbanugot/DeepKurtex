namespace AI.Graphics.Frame
{
    partial class View
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnNext = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.grpBoxMovement = new System.Windows.Forms.GroupBox();
            this.btnRightDown = new System.Windows.Forms.Button();
            this.btnUpRight = new System.Windows.Forms.Button();
            this.btnDownLeft = new System.Windows.Forms.Button();
            this.btnLeftUp = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.grpBoxStepSize = new System.Windows.Forms.GroupBox();
            this.btnSetAsDefault = new System.Windows.Forms.Button();
            this.txtBoxYStepSize = new System.Windows.Forms.TextBox();
            this.lblYAxis = new System.Windows.Forms.Label();
            this.lblXAxis = new System.Windows.Forms.Label();
            this.txtBoxXStepSize = new System.Windows.Forms.TextBox();
            this.grpBoxVisibility = new System.Windows.Forms.GroupBox();
            this.btnResize = new System.Windows.Forms.Button();
            this.txtBoxResize = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbBoxBorderSize = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbBoxBorderColor = new System.Windows.Forms.ComboBox();
            this.btnShow = new System.Windows.Forms.Button();
            this.magBox = new System.Windows.Forms.PictureBox();
            this.grpBoxMovement.SuspendLayout();
            this.grpBoxStepSize.SuspendLayout();
            this.grpBoxVisibility.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.magBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(60, 73);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(50, 50);
            this.btnNext.TabIndex = 0;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(60, 21);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(50, 50);
            this.btnUp.TabIndex = 1;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(60, 125);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(50, 50);
            this.btnDown.TabIndex = 2;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // grpBoxMovement
            // 
            this.grpBoxMovement.Controls.Add(this.btnRightDown);
            this.grpBoxMovement.Controls.Add(this.btnUpRight);
            this.grpBoxMovement.Controls.Add(this.btnDownLeft);
            this.grpBoxMovement.Controls.Add(this.btnLeftUp);
            this.grpBoxMovement.Controls.Add(this.btnLeft);
            this.grpBoxMovement.Controls.Add(this.btnRight);
            this.grpBoxMovement.Controls.Add(this.btnUp);
            this.grpBoxMovement.Controls.Add(this.btnDown);
            this.grpBoxMovement.Controls.Add(this.btnNext);
            this.grpBoxMovement.Location = new System.Drawing.Point(198, 376);
            this.grpBoxMovement.Name = "grpBoxMovement";
            this.grpBoxMovement.Size = new System.Drawing.Size(170, 182);
            this.grpBoxMovement.TabIndex = 3;
            this.grpBoxMovement.TabStop = false;
            this.grpBoxMovement.Text = "Movement";
            // 
            // btnRightDown
            // 
            this.btnRightDown.Location = new System.Drawing.Point(113, 125);
            this.btnRightDown.Name = "btnRightDown";
            this.btnRightDown.Size = new System.Drawing.Size(50, 50);
            this.btnRightDown.TabIndex = 8;
            this.btnRightDown.Text = "R+D";
            this.btnRightDown.UseVisualStyleBackColor = true;
            this.btnRightDown.Click += new System.EventHandler(this.btnRightDown_Click);
            // 
            // btnUpRight
            // 
            this.btnUpRight.Location = new System.Drawing.Point(113, 21);
            this.btnUpRight.Name = "btnUpRight";
            this.btnUpRight.Size = new System.Drawing.Size(50, 50);
            this.btnUpRight.TabIndex = 7;
            this.btnUpRight.Text = "U+R";
            this.btnUpRight.UseVisualStyleBackColor = true;
            this.btnUpRight.Click += new System.EventHandler(this.btnUpRight_Click);
            // 
            // btnDownLeft
            // 
            this.btnDownLeft.Location = new System.Drawing.Point(7, 125);
            this.btnDownLeft.Name = "btnDownLeft";
            this.btnDownLeft.Size = new System.Drawing.Size(50, 49);
            this.btnDownLeft.TabIndex = 6;
            this.btnDownLeft.Text = "D+L";
            this.btnDownLeft.UseVisualStyleBackColor = true;
            this.btnDownLeft.Click += new System.EventHandler(this.btnDownLeft_Click);
            // 
            // btnLeftUp
            // 
            this.btnLeftUp.Location = new System.Drawing.Point(7, 21);
            this.btnLeftUp.Name = "btnLeftUp";
            this.btnLeftUp.Size = new System.Drawing.Size(50, 50);
            this.btnLeftUp.TabIndex = 5;
            this.btnLeftUp.Text = "L+U";
            this.btnLeftUp.UseVisualStyleBackColor = true;
            this.btnLeftUp.Click += new System.EventHandler(this.btnLeftUp_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(7, 73);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(50, 50);
            this.btnLeft.TabIndex = 4;
            this.btnLeft.Text = "Left";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(113, 73);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(50, 50);
            this.btnRight.TabIndex = 4;
            this.btnRight.Text = "Right";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // grpBoxStepSize
            // 
            this.grpBoxStepSize.Controls.Add(this.btnSetAsDefault);
            this.grpBoxStepSize.Controls.Add(this.txtBoxYStepSize);
            this.grpBoxStepSize.Controls.Add(this.lblYAxis);
            this.grpBoxStepSize.Controls.Add(this.lblXAxis);
            this.grpBoxStepSize.Controls.Add(this.txtBoxXStepSize);
            this.grpBoxStepSize.Location = new System.Drawing.Point(5, 460);
            this.grpBoxStepSize.Name = "grpBoxStepSize";
            this.grpBoxStepSize.Size = new System.Drawing.Size(91, 96);
            this.grpBoxStepSize.TabIndex = 4;
            this.grpBoxStepSize.TabStop = false;
            this.grpBoxStepSize.Text = "Step Size";
            // 
            // btnSetAsDefault
            // 
            this.btnSetAsDefault.Location = new System.Drawing.Point(10, 66);
            this.btnSetAsDefault.Name = "btnSetAsDefault";
            this.btnSetAsDefault.Size = new System.Drawing.Size(74, 24);
            this.btnSetAsDefault.TabIndex = 4;
            this.btnSetAsDefault.Text = "Set Default";
            this.btnSetAsDefault.UseVisualStyleBackColor = true;
            this.btnSetAsDefault.Click += new System.EventHandler(this.btnSetAsDefault_Click);
            // 
            // txtBoxYStepSize
            // 
            this.txtBoxYStepSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBoxYStepSize.Location = new System.Drawing.Point(45, 43);
            this.txtBoxYStepSize.Name = "txtBoxYStepSize";
            this.txtBoxYStepSize.Size = new System.Drawing.Size(39, 20);
            this.txtBoxYStepSize.TabIndex = 3;
            this.txtBoxYStepSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblYAxis
            // 
            this.lblYAxis.AutoSize = true;
            this.lblYAxis.Location = new System.Drawing.Point(7, 46);
            this.lblYAxis.Name = "lblYAxis";
            this.lblYAxis.Size = new System.Drawing.Size(35, 13);
            this.lblYAxis.TabIndex = 2;
            this.lblYAxis.Text = "Y-axis";
            // 
            // lblXAxis
            // 
            this.lblXAxis.AutoSize = true;
            this.lblXAxis.Location = new System.Drawing.Point(7, 23);
            this.lblXAxis.Name = "lblXAxis";
            this.lblXAxis.Size = new System.Drawing.Size(35, 13);
            this.lblXAxis.TabIndex = 1;
            this.lblXAxis.Text = "X-axis";
            // 
            // txtBoxXStepSize
            // 
            this.txtBoxXStepSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBoxXStepSize.Location = new System.Drawing.Point(45, 20);
            this.txtBoxXStepSize.Name = "txtBoxXStepSize";
            this.txtBoxXStepSize.Size = new System.Drawing.Size(39, 20);
            this.txtBoxXStepSize.TabIndex = 0;
            this.txtBoxXStepSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // grpBoxVisibility
            // 
            this.grpBoxVisibility.Controls.Add(this.btnResize);
            this.grpBoxVisibility.Controls.Add(this.txtBoxResize);
            this.grpBoxVisibility.Controls.Add(this.label2);
            this.grpBoxVisibility.Controls.Add(this.cmbBoxBorderSize);
            this.grpBoxVisibility.Controls.Add(this.label1);
            this.grpBoxVisibility.Controls.Add(this.cmbBoxBorderColor);
            this.grpBoxVisibility.Controls.Add(this.btnShow);
            this.grpBoxVisibility.Location = new System.Drawing.Point(102, 376);
            this.grpBoxVisibility.Name = "grpBoxVisibility";
            this.grpBoxVisibility.Size = new System.Drawing.Size(90, 182);
            this.grpBoxVisibility.TabIndex = 5;
            this.grpBoxVisibility.TabStop = false;
            this.grpBoxVisibility.Text = "Visibility";
            // 
            // btnResize
            // 
            this.btnResize.Location = new System.Drawing.Point(6, 38);
            this.btnResize.Name = "btnResize";
            this.btnResize.Size = new System.Drawing.Size(78, 23);
            this.btnResize.TabIndex = 11;
            this.btnResize.Text = "Resize";
            this.btnResize.UseVisualStyleBackColor = true;
            this.btnResize.Click += new System.EventHandler(this.btnResize_Click);
            // 
            // txtBoxResize
            // 
            this.txtBoxResize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBoxResize.Location = new System.Drawing.Point(6, 16);
            this.txtBoxResize.Name = "txtBoxResize";
            this.txtBoxResize.Size = new System.Drawing.Size(78, 20);
            this.txtBoxResize.TabIndex = 10;
            this.txtBoxResize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Border Width";
            // 
            // cmbBoxBorderSize
            // 
            this.cmbBoxBorderSize.FormattingEnabled = true;
            this.cmbBoxBorderSize.Location = new System.Drawing.Point(6, 84);
            this.cmbBoxBorderSize.Name = "cmbBoxBorderSize";
            this.cmbBoxBorderSize.Size = new System.Drawing.Size(78, 21);
            this.cmbBoxBorderSize.TabIndex = 8;
            this.cmbBoxBorderSize.SelectedIndexChanged += new System.EventHandler(this.cmbBoxBorderSize_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Border Color";
            // 
            // cmbBoxBorderColor
            // 
            this.cmbBoxBorderColor.FormattingEnabled = true;
            this.cmbBoxBorderColor.Location = new System.Drawing.Point(6, 126);
            this.cmbBoxBorderColor.Name = "cmbBoxBorderColor";
            this.cmbBoxBorderColor.Size = new System.Drawing.Size(78, 21);
            this.cmbBoxBorderColor.TabIndex = 6;
            this.cmbBoxBorderColor.SelectedIndexChanged += new System.EventHandler(this.cmbBoxBorderColor_SelectedIndexChanged);
            // 
            // btnShow
            // 
            this.btnShow.Location = new System.Drawing.Point(6, 151);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(78, 23);
            this.btnShow.TabIndex = 0;
            this.btnShow.Text = "Show";
            this.btnShow.UseVisualStyleBackColor = true;
            this.btnShow.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // magBox
            // 
            this.magBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.magBox.Location = new System.Drawing.Point(7, 6);
            this.magBox.Name = "magBox";
            this.magBox.Size = new System.Drawing.Size(360, 360);
            this.magBox.TabIndex = 6;
            this.magBox.TabStop = false;
            // 
            // View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 562);
            this.Controls.Add(this.magBox);
            this.Controls.Add(this.grpBoxVisibility);
            this.Controls.Add(this.grpBoxStepSize);
            this.Controls.Add(this.grpBoxMovement);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "View";
            this.Opacity = 0.8D;
            this.Text = "View";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.view_FormClosed);
            this.grpBoxMovement.ResumeLayout(false);
            this.grpBoxStepSize.ResumeLayout(false);
            this.grpBoxStepSize.PerformLayout();
            this.grpBoxVisibility.ResumeLayout(false);
            this.grpBoxVisibility.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.magBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.GroupBox grpBoxMovement;
        private System.Windows.Forms.Button btnRightDown;
        private System.Windows.Forms.Button btnUpRight;
        private System.Windows.Forms.Button btnDownLeft;
        private System.Windows.Forms.Button btnLeftUp;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.GroupBox grpBoxStepSize;
        private System.Windows.Forms.TextBox txtBoxXStepSize;
        private System.Windows.Forms.Label lblYAxis;
        private System.Windows.Forms.Label lblXAxis;
        private System.Windows.Forms.Button btnSetAsDefault;
        private System.Windows.Forms.TextBox txtBoxYStepSize;
        private System.Windows.Forms.GroupBox grpBoxVisibility;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbBoxBorderColor;
        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbBoxBorderSize;
        private System.Windows.Forms.Button btnResize;
        private System.Windows.Forms.TextBox txtBoxResize;
        private System.Windows.Forms.PictureBox magBox;
    }
}