namespace AcccesImagges
{
    partial class FrmMain
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
            this.PicOrig = new System.Windows.Forms.PictureBox();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LblTable = new System.Windows.Forms.Label();
            this.BtnConvert = new System.Windows.Forms.Button();
            this.LblElapsed = new System.Windows.Forms.Label();
            this.LblTotalTime = new System.Windows.Forms.Label();
            this.LblStartTime = new System.Windows.Forms.Label();
            this.LblStart = new System.Windows.Forms.Label();
            this.LblTotal = new System.Windows.Forms.Label();
            this.LblOf = new System.Windows.Forms.Label();
            this.LblCount = new System.Windows.Forms.Label();
            this.LblOrig = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PicOrig)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // PicOrig
            // 
            this.PicOrig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PicOrig.Location = new System.Drawing.Point(0, 0);
            this.PicOrig.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PicOrig.Name = "PicOrig";
            this.PicOrig.Size = new System.Drawing.Size(472, 372);
            this.PicOrig.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PicOrig.TabIndex = 2;
            this.PicOrig.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.LblTable);
            this.panel1.Controls.Add(this.BtnConvert);
            this.panel1.Controls.Add(this.LblElapsed);
            this.panel1.Controls.Add(this.LblTotalTime);
            this.panel1.Controls.Add(this.LblStartTime);
            this.panel1.Controls.Add(this.LblStart);
            this.panel1.Controls.Add(this.LblTotal);
            this.panel1.Controls.Add(this.LblOf);
            this.panel1.Controls.Add(this.LblCount);
            this.panel1.Controls.Add(this.LblOrig);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(472, 57);
            this.panel1.TabIndex = 13;
            // 
            // LblTable
            // 
            this.LblTable.AutoSize = true;
            this.LblTable.Location = new System.Drawing.Point(8, 34);
            this.LblTable.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LblTable.Name = "LblTable";
            this.LblTable.Size = new System.Drawing.Size(37, 13);
            this.LblTable.TabIndex = 23;
            this.LblTable.Text = "Table:";
            this.LblTable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BtnConvert
            // 
            this.BtnConvert.Location = new System.Drawing.Point(334, 8);
            this.BtnConvert.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.BtnConvert.Name = "BtnConvert";
            this.BtnConvert.Size = new System.Drawing.Size(65, 24);
            this.BtnConvert.TabIndex = 21;
            this.BtnConvert.Text = "Export";
            this.BtnConvert.UseVisualStyleBackColor = true;
            this.BtnConvert.Click += new System.EventHandler(this.BtnConvert_Click);
            // 
            // LblElapsed
            // 
            this.LblElapsed.AutoSize = true;
            this.LblElapsed.Location = new System.Drawing.Point(227, 13);
            this.LblElapsed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LblElapsed.Name = "LblElapsed";
            this.LblElapsed.Size = new System.Drawing.Size(48, 13);
            this.LblElapsed.TabIndex = 20;
            this.LblElapsed.Text = "Elapsed:";
            this.LblElapsed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LblTotalTime
            // 
            this.LblTotalTime.AutoSize = true;
            this.LblTotalTime.Location = new System.Drawing.Point(279, 13);
            this.LblTotalTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LblTotalTime.Name = "LblTotalTime";
            this.LblTotalTime.Size = new System.Drawing.Size(51, 13);
            this.LblTotalTime.TabIndex = 19;
            this.LblTotalTime.Text = "hh:mm:ss";
            this.LblTotalTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LblStartTime
            // 
            this.LblStartTime.AutoSize = true;
            this.LblStartTime.Location = new System.Drawing.Point(174, 13);
            this.LblStartTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LblStartTime.Name = "LblStartTime";
            this.LblStartTime.Size = new System.Drawing.Size(35, 13);
            this.LblStartTime.TabIndex = 18;
            this.LblStartTime.Text = "Start: ";
            this.LblStartTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LblStart
            // 
            this.LblStart.AutoSize = true;
            this.LblStart.Location = new System.Drawing.Point(142, 13);
            this.LblStart.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LblStart.Name = "LblStart";
            this.LblStart.Size = new System.Drawing.Size(35, 13);
            this.LblStart.TabIndex = 17;
            this.LblStart.Text = "Start: ";
            this.LblStart.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LblTotal
            // 
            this.LblTotal.AutoSize = true;
            this.LblTotal.Location = new System.Drawing.Point(92, 13);
            this.LblTotal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LblTotal.Name = "LblTotal";
            this.LblTotal.Size = new System.Drawing.Size(37, 13);
            this.LblTotal.TabIndex = 16;
            this.LblTotal.Text = "10000";
            this.LblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LblOf
            // 
            this.LblOf.AutoSize = true;
            this.LblOf.Location = new System.Drawing.Point(77, 13);
            this.LblOf.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LblOf.Name = "LblOf";
            this.LblOf.Size = new System.Drawing.Size(16, 13);
            this.LblOf.TabIndex = 15;
            this.LblOf.Text = "of";
            this.LblOf.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LblCount
            // 
            this.LblCount.AutoSize = true;
            this.LblCount.Location = new System.Drawing.Point(43, 13);
            this.LblCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LblCount.Name = "LblCount";
            this.LblCount.Size = new System.Drawing.Size(37, 13);
            this.LblCount.TabIndex = 14;
            this.LblCount.Text = "10000";
            this.LblCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LblOrig
            // 
            this.LblOrig.AutoSize = true;
            this.LblOrig.Location = new System.Drawing.Point(8, 13);
            this.LblOrig.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LblOrig.Name = "LblOrig";
            this.LblOrig.Size = new System.Drawing.Size(36, 13);
            this.LblOrig.TabIndex = 13;
            this.LblOrig.Text = "Image";
            this.LblOrig.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 372);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.PicOrig);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Image Conversion";
            ((System.ComponentModel.ISupportInitialize)(this.PicOrig)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox PicOrig;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button BtnConvert;
        private System.Windows.Forms.Label LblElapsed;
        private System.Windows.Forms.Label LblTotalTime;
        private System.Windows.Forms.Label LblStartTime;
        private System.Windows.Forms.Label LblStart;
        private System.Windows.Forms.Label LblTotal;
        private System.Windows.Forms.Label LblOf;
        private System.Windows.Forms.Label LblCount;
        private System.Windows.Forms.Label LblOrig;
        private System.Windows.Forms.Label LblTable;
    }
}

