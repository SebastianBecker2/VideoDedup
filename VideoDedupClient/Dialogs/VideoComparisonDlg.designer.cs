namespace VideoDedupClient.Dialogs
{
    using Controls.FilePreview;

    partial class VideoComparisonDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoComparisonDlg));
            this.SplitterContainer = new System.Windows.Forms.SplitContainer();
            this.FpvLeft = new VideoDedupClient.Controls.FilePreview.FilePreviewCtl();
            this.BtnShowLeft = new System.Windows.Forms.Button();
            this.btnDeleteLeft = new System.Windows.Forms.Button();
            this.FpvRight = new VideoDedupClient.Controls.FilePreview.FilePreviewCtl();
            this.BtnShowRight = new System.Windows.Forms.Button();
            this.btnDeleteRight = new System.Windows.Forms.Button();
            this.BtnClose = new System.Windows.Forms.Button();
            this.BtnSkip = new System.Windows.Forms.Button();
            this.BtnDiscard = new System.Windows.Forms.Button();
            this.BtnReviewComparison = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.SplitterContainer)).BeginInit();
            this.SplitterContainer.Panel1.SuspendLayout();
            this.SplitterContainer.Panel2.SuspendLayout();
            this.SplitterContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // SplitterContainer
            // 
            this.SplitterContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitterContainer.Location = new System.Drawing.Point(14, 14);
            this.SplitterContainer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SplitterContainer.Name = "SplitterContainer";
            // 
            // SplitterContainer.Panel1
            // 
            this.SplitterContainer.Panel1.Controls.Add(this.FpvLeft);
            this.SplitterContainer.Panel1.Controls.Add(this.BtnShowLeft);
            this.SplitterContainer.Panel1.Controls.Add(this.btnDeleteLeft);
            // 
            // SplitterContainer.Panel2
            // 
            this.SplitterContainer.Panel2.Controls.Add(this.FpvRight);
            this.SplitterContainer.Panel2.Controls.Add(this.BtnShowRight);
            this.SplitterContainer.Panel2.Controls.Add(this.btnDeleteRight);
            this.SplitterContainer.Size = new System.Drawing.Size(630, 467);
            this.SplitterContainer.SplitterDistance = 310;
            this.SplitterContainer.SplitterWidth = 7;
            this.SplitterContainer.TabIndex = 0;
            this.SplitterContainer.TabStop = false;
            // 
            // FpvLeft
            // 
            this.FpvLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FpvLeft.HighlightColor = System.Drawing.SystemColors.Control;
            this.FpvLeft.Location = new System.Drawing.Point(4, 3);
            this.FpvLeft.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.FpvLeft.Name = "FpvLeft";
            this.FpvLeft.Size = new System.Drawing.Size(302, 428);
            this.FpvLeft.TabIndex = 3;
            this.FpvLeft.VideoFile = null;
            // 
            // BtnShowLeft
            // 
            this.BtnShowLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnShowLeft.Location = new System.Drawing.Point(4, 437);
            this.BtnShowLeft.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnShowLeft.Name = "BtnShowLeft";
            this.BtnShowLeft.Size = new System.Drawing.Size(113, 27);
            this.BtnShowLeft.TabIndex = 1;
            this.BtnShowLeft.Text = "&Show in Explorer";
            this.BtnShowLeft.UseVisualStyleBackColor = true;
            this.BtnShowLeft.Click += new System.EventHandler(this.BtnShowLeft_Click);
            // 
            // btnDeleteLeft
            // 
            this.btnDeleteLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteLeft.Location = new System.Drawing.Point(125, 437);
            this.btnDeleteLeft.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnDeleteLeft.Name = "btnDeleteLeft";
            this.btnDeleteLeft.Size = new System.Drawing.Size(88, 27);
            this.btnDeleteLeft.TabIndex = 2;
            this.btnDeleteLeft.Text = "Delete &Left";
            this.btnDeleteLeft.UseVisualStyleBackColor = true;
            this.btnDeleteLeft.Click += new System.EventHandler(this.BtnDeleteLeft_Click);
            // 
            // FpvRight
            // 
            this.FpvRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FpvRight.HighlightColor = System.Drawing.SystemColors.Control;
            this.FpvRight.Location = new System.Drawing.Point(4, 3);
            this.FpvRight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.FpvRight.Name = "FpvRight";
            this.FpvRight.Size = new System.Drawing.Size(302, 428);
            this.FpvRight.TabIndex = 3;
            this.FpvRight.VideoFile = null;
            // 
            // BtnShowRight
            // 
            this.BtnShowRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnShowRight.Location = new System.Drawing.Point(193, 437);
            this.BtnShowRight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnShowRight.Name = "BtnShowRight";
            this.BtnShowRight.Size = new System.Drawing.Size(113, 27);
            this.BtnShowRight.TabIndex = 2;
            this.BtnShowRight.Text = "Show in &Explorer";
            this.BtnShowRight.UseVisualStyleBackColor = true;
            this.BtnShowRight.Click += new System.EventHandler(this.BtnShowRight_Click);
            // 
            // btnDeleteRight
            // 
            this.btnDeleteRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteRight.Location = new System.Drawing.Point(97, 437);
            this.btnDeleteRight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnDeleteRight.Name = "btnDeleteRight";
            this.btnDeleteRight.Size = new System.Drawing.Size(88, 27);
            this.btnDeleteRight.TabIndex = 1;
            this.btnDeleteRight.Text = "Delete &Right";
            this.btnDeleteRight.UseVisualStyleBackColor = true;
            this.btnDeleteRight.Click += new System.EventHandler(this.BtnDeleteRight_Click);
            // 
            // BtnClose
            // 
            this.BtnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnClose.Location = new System.Drawing.Point(557, 487);
            this.BtnClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(88, 27);
            this.BtnClose.TabIndex = 4;
            this.BtnClose.Text = "&Close";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // BtnSkip
            // 
            this.BtnSkip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSkip.Location = new System.Drawing.Point(462, 487);
            this.BtnSkip.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnSkip.Name = "BtnSkip";
            this.BtnSkip.Size = new System.Drawing.Size(88, 27);
            this.BtnSkip.TabIndex = 3;
            this.BtnSkip.Text = "&Skip";
            this.BtnSkip.UseVisualStyleBackColor = true;
            this.BtnSkip.Click += new System.EventHandler(this.BtnSkip_Click);
            // 
            // BtnDiscard
            // 
            this.BtnDiscard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnDiscard.Location = new System.Drawing.Point(368, 487);
            this.BtnDiscard.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnDiscard.Name = "BtnDiscard";
            this.BtnDiscard.Size = new System.Drawing.Size(88, 27);
            this.BtnDiscard.TabIndex = 2;
            this.BtnDiscard.Text = "&Discard";
            this.BtnDiscard.UseVisualStyleBackColor = true;
            this.BtnDiscard.Click += new System.EventHandler(this.BtnDiscard_Click);
            // 
            // BtnReviewComparison
            // 
            this.BtnReviewComparison.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnReviewComparison.Location = new System.Drawing.Point(233, 487);
            this.BtnReviewComparison.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnReviewComparison.Name = "BtnReviewComparison";
            this.BtnReviewComparison.Size = new System.Drawing.Size(127, 27);
            this.BtnReviewComparison.TabIndex = 1;
            this.BtnReviewComparison.Text = "Re&view Comparison";
            this.BtnReviewComparison.UseVisualStyleBackColor = true;
            this.BtnReviewComparison.Click += new System.EventHandler(this.BtnReviewComparison_Click);
            // 
            // VideoComparisonDlg
            // 
            this.AcceptButton = this.BtnSkip;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnClose;
            this.ClientSize = new System.Drawing.Size(658, 528);
            this.Controls.Add(this.BtnReviewComparison);
            this.Controls.Add(this.BtnDiscard);
            this.Controls.Add(this.BtnSkip);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.SplitterContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "VideoComparisonDlg";
            this.Text = "File Comparison";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.SplitterContainer.Panel1.ResumeLayout(false);
            this.SplitterContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitterContainer)).EndInit();
            this.SplitterContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer SplitterContainer;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Button btnDeleteLeft;
        private System.Windows.Forms.Button btnDeleteRight;
        private System.Windows.Forms.Button BtnShowRight;
        private System.Windows.Forms.Button BtnShowLeft;
        private System.Windows.Forms.Button BtnSkip;
        private System.Windows.Forms.Button BtnDiscard;
        private System.Windows.Forms.Button BtnReviewComparison;
        private FilePreviewCtl FpvLeft;
        private FilePreviewCtl FpvRight;
    }
}
