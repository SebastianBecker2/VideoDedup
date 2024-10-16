namespace VideoDedupClient.Controls.ResolutionSettings
{
    partial class ResolutionSettingsCtrl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox4 = new GroupBox();
            label9 = new Label();
            NumThumbnailViewCount = new NumericUpDown();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NumThumbnailViewCount).BeginInit();
            SuspendLayout();
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(label9);
            groupBox4.Controls.Add(NumThumbnailViewCount);
            groupBox4.Dock = DockStyle.Fill;
            groupBox4.Location = new Point(0, 0);
            groupBox4.Margin = new Padding(0);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new Padding(4, 3, 4, 3);
            groupBox4.Size = new Size(352, 141);
            groupBox4.TabIndex = 27;
            groupBox4.TabStop = false;
            groupBox4.Text = "Resolving Duplicates";
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.None;
            label9.AutoSize = true;
            label9.Location = new Point(49, 73);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(163, 15);
            label9.TabIndex = 19;
            label9.Text = "Number of Images to display:";
            // 
            // NumThumbnailViewCount
            // 
            NumThumbnailViewCount.Anchor = AnchorStyles.None;
            NumThumbnailViewCount.Location = new Point(223, 69);
            NumThumbnailViewCount.Margin = new Padding(4, 3, 4, 3);
            NumThumbnailViewCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NumThumbnailViewCount.Name = "NumThumbnailViewCount";
            NumThumbnailViewCount.Size = new Size(74, 23);
            NumThumbnailViewCount.TabIndex = 0;
            NumThumbnailViewCount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // ResolutionSettingsCtrl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox4);
            Name = "ResolutionSettingsCtrl";
            Size = new Size(352, 141);
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NumThumbnailViewCount).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox4;
        private Label label9;
        public NumericUpDown NumThumbnailViewCount;
    }
}
