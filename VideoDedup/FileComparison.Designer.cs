namespace VideoDedup
{
    partial class FileComparison
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileComparison));
            this.SplitterContainer = new System.Windows.Forms.SplitContainer();
            this.BtnShowLeft = new System.Windows.Forms.Button();
            this.FpvLeft = new global::VideoDedup.FilePreview.FilePreview();
            this.btnDeleteLeft = new System.Windows.Forms.Button();
            this.BtnShowRight = new System.Windows.Forms.Button();
            this.btnDeleteRight = new System.Windows.Forms.Button();
            this.FpvRight = new global::VideoDedup.FilePreview.FilePreview();
            this.btnClose = new System.Windows.Forms.Button();
            this.BtnSkip = new System.Windows.Forms.Button();
            this.BtnDiscard = new System.Windows.Forms.Button();
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
            this.SplitterContainer.Location = new System.Drawing.Point(12, 12);
            this.SplitterContainer.Name = "SplitterContainer";
            // 
            // SplitterContainer.Panel1
            // 
            this.SplitterContainer.Panel1.Controls.Add(this.BtnShowLeft);
            this.SplitterContainer.Panel1.Controls.Add(this.FpvLeft);
            this.SplitterContainer.Panel1.Controls.Add(this.btnDeleteLeft);
            // 
            // SplitterContainer.Panel2
            // 
            this.SplitterContainer.Panel2.Controls.Add(this.BtnShowRight);
            this.SplitterContainer.Panel2.Controls.Add(this.btnDeleteRight);
            this.SplitterContainer.Panel2.Controls.Add(this.FpvRight);
            this.SplitterContainer.Size = new System.Drawing.Size(611, 372);
            this.SplitterContainer.SplitterDistance = 305;
            this.SplitterContainer.SplitterWidth = 6;
            this.SplitterContainer.TabIndex = 2;
            // 
            // BtnShowLeft
            // 
            this.BtnShowLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnShowLeft.Location = new System.Drawing.Point(3, 346);
            this.BtnShowLeft.Name = "BtnShowLeft";
            this.BtnShowLeft.Size = new System.Drawing.Size(97, 23);
            this.BtnShowLeft.TabIndex = 21;
            this.BtnShowLeft.Text = "&Show in Explorer";
            this.BtnShowLeft.UseVisualStyleBackColor = true;
            this.BtnShowLeft.Click += new System.EventHandler(this.BtnShowLeft_Click);
            // 
            // FpvLeft
            // 
            this.FpvLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FpvLeft.HighlightColor = System.Drawing.SystemColors.Control;
            this.FpvLeft.Location = new System.Drawing.Point(0, 0);
            this.FpvLeft.Name = "FpvLeft";
            this.FpvLeft.Size = new System.Drawing.Size(308, 340);
            this.FpvLeft.TabIndex = 0;
            this.FpvLeft.VideoFile = null;
            // 
            // btnDeleteLeft
            // 
            this.btnDeleteLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteLeft.Location = new System.Drawing.Point(106, 346);
            this.btnDeleteLeft.Name = "btnDeleteLeft";
            this.btnDeleteLeft.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteLeft.TabIndex = 18;
            this.btnDeleteLeft.Text = "Delete &Left";
            this.btnDeleteLeft.UseVisualStyleBackColor = true;
            this.btnDeleteLeft.Click += new System.EventHandler(this.BtnDeleteLeft_Click);
            // 
            // BtnShowRight
            // 
            this.BtnShowRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnShowRight.Location = new System.Drawing.Point(184, 346);
            this.BtnShowRight.Name = "BtnShowRight";
            this.BtnShowRight.Size = new System.Drawing.Size(97, 23);
            this.BtnShowRight.TabIndex = 20;
            this.BtnShowRight.Text = "Show in &Explorer";
            this.BtnShowRight.UseVisualStyleBackColor = true;
            this.BtnShowRight.Click += new System.EventHandler(this.BtnShowRight_Click);
            // 
            // btnDeleteRight
            // 
            this.btnDeleteRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteRight.Location = new System.Drawing.Point(103, 346);
            this.btnDeleteRight.Name = "btnDeleteRight";
            this.btnDeleteRight.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteRight.TabIndex = 19;
            this.btnDeleteRight.Text = "Delete &Right";
            this.btnDeleteRight.UseVisualStyleBackColor = true;
            this.btnDeleteRight.Click += new System.EventHandler(this.BtnDeleteRight_Click);
            // 
            // FpvRight
            // 
            this.FpvRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FpvRight.HighlightColor = System.Drawing.SystemColors.Control;
            this.FpvRight.Location = new System.Drawing.Point(0, 0);
            this.FpvRight.Name = "FpvRight";
            this.FpvRight.Size = new System.Drawing.Size(291, 340);
            this.FpvRight.TabIndex = 0;
            this.FpvRight.VideoFile = null;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(548, 390);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 16;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // BtnSkip
            // 
            this.BtnSkip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSkip.DialogResult = System.Windows.Forms.DialogResult.No;
            this.BtnSkip.Location = new System.Drawing.Point(467, 390);
            this.BtnSkip.Name = "BtnSkip";
            this.BtnSkip.Size = new System.Drawing.Size(75, 23);
            this.BtnSkip.TabIndex = 17;
            this.BtnSkip.Text = "&Skip";
            this.BtnSkip.UseVisualStyleBackColor = true;
            // 
            // BtnDiscard
            // 
            this.BtnDiscard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnDiscard.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.BtnDiscard.Location = new System.Drawing.Point(386, 390);
            this.BtnDiscard.Name = "BtnDiscard";
            this.BtnDiscard.Size = new System.Drawing.Size(75, 23);
            this.BtnDiscard.TabIndex = 18;
            this.BtnDiscard.Text = "&Discard";
            this.BtnDiscard.UseVisualStyleBackColor = true;
            // 
            // FileComparison
            // 
            this.AcceptButton = this.BtnSkip;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(635, 425);
            this.Controls.Add(this.BtnDiscard);
            this.Controls.Add(this.BtnSkip);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.SplitterContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FileComparison";
            this.Text = "FileComparison";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.SplitterContainer.Panel1.ResumeLayout(false);
            this.SplitterContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitterContainer)).EndInit();
            this.SplitterContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer SplitterContainer;
        private FilePreview.FilePreview FpvLeft;
        private FilePreview.FilePreview FpvRight;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnDeleteLeft;
        private System.Windows.Forms.Button btnDeleteRight;
        private System.Windows.Forms.Button BtnShowRight;
        private System.Windows.Forms.Button BtnShowLeft;
        private System.Windows.Forms.Button BtnSkip;
        private System.Windows.Forms.Button BtnDiscard;
    }
}