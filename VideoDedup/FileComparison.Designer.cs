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
            this.SplitterContainer = new System.Windows.Forms.SplitContainer();
            this.FpvSource = new VideoDedup.FilePreview.FilePreview();
            this.FpvDestination = new VideoDedup.FilePreview.FilePreview();
            this.LblFilePath = new System.Windows.Forms.Label();
            this.BtnSkip = new System.Windows.Forms.Button();
            this.btnKeepBoth = new System.Windows.Forms.Button();
            this.btnKeepNew = new System.Windows.Forms.Button();
            this.btnKeepOld = new System.Windows.Forms.Button();
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
            this.SplitterContainer.Location = new System.Drawing.Point(12, 29);
            this.SplitterContainer.Name = "SplitterContainer";
            // 
            // SplitterContainer.Panel1
            // 
            this.SplitterContainer.Panel1.Controls.Add(this.FpvSource);
            // 
            // SplitterContainer.Panel2
            // 
            this.SplitterContainer.Panel2.Controls.Add(this.FpvDestination);
            this.SplitterContainer.Size = new System.Drawing.Size(1321, 559);
            this.SplitterContainer.SplitterDistance = 661;
            this.SplitterContainer.SplitterWidth = 6;
            this.SplitterContainer.TabIndex = 2;
            // 
            // FpvSource
            // 
            this.FpvSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FpvSource.FileInfo = null;
            this.FpvSource.FileSize = ((long)(0));
            this.FpvSource.HighlightColor = System.Drawing.SystemColors.Control;
            this.FpvSource.Location = new System.Drawing.Point(0, 0);
            this.FpvSource.Name = "FpvSource";
            this.FpvSource.Size = new System.Drawing.Size(661, 559);
            this.FpvSource.TabIndex = 0;
            this.FpvSource.FileInfoLoaded += new System.EventHandler<VideoDedup.FilePreview.FileInfoLoadedEventArgs>(this.FpvSource_FileInfoLoaded);
            this.FpvSource.ThumbnailLoaded += new System.EventHandler<VideoDedup.FilePreview.ThumbnailLoadedEventArgs>(this.FpvSource_ThumbnailLoaded);
            // 
            // FpvDestination
            // 
            this.FpvDestination.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FpvDestination.FileInfo = null;
            this.FpvDestination.FileSize = ((long)(0));
            this.FpvDestination.HighlightColor = System.Drawing.SystemColors.Control;
            this.FpvDestination.Location = new System.Drawing.Point(0, 0);
            this.FpvDestination.Name = "FpvDestination";
            this.FpvDestination.Size = new System.Drawing.Size(654, 559);
            this.FpvDestination.TabIndex = 0;
            this.FpvDestination.FileInfoLoaded += new System.EventHandler<VideoDedup.FilePreview.FileInfoLoadedEventArgs>(this.FpvDestination_FileInfoLoaded);
            this.FpvDestination.ThumbnailLoaded += new System.EventHandler<VideoDedup.FilePreview.ThumbnailLoadedEventArgs>(this.FpvDestination_ThumbnailLoaded);
            // 
            // LblFilePath
            // 
            this.LblFilePath.AutoSize = true;
            this.LblFilePath.Location = new System.Drawing.Point(13, 13);
            this.LblFilePath.Name = "LblFilePath";
            this.LblFilePath.Size = new System.Drawing.Size(35, 13);
            this.LblFilePath.TabIndex = 3;
            this.LblFilePath.Text = "label1";
            // 
            // BtnSkip
            // 
            this.BtnSkip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSkip.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnSkip.Location = new System.Drawing.Point(1258, 594);
            this.BtnSkip.Name = "BtnSkip";
            this.BtnSkip.Size = new System.Drawing.Size(75, 23);
            this.BtnSkip.TabIndex = 17;
            this.BtnSkip.Text = "Skip";
            this.BtnSkip.UseVisualStyleBackColor = true;
            // 
            // btnKeepBoth
            // 
            this.btnKeepBoth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnKeepBoth.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnKeepBoth.Location = new System.Drawing.Point(1015, 594);
            this.btnKeepBoth.Name = "btnKeepBoth";
            this.btnKeepBoth.Size = new System.Drawing.Size(75, 23);
            this.btnKeepBoth.TabIndex = 16;
            this.btnKeepBoth.Text = "Keep both";
            this.btnKeepBoth.UseVisualStyleBackColor = true;
            // 
            // btnKeepNew
            // 
            this.btnKeepNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnKeepNew.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.btnKeepNew.Location = new System.Drawing.Point(1096, 594);
            this.btnKeepNew.Name = "btnKeepNew";
            this.btnKeepNew.Size = new System.Drawing.Size(75, 23);
            this.btnKeepNew.TabIndex = 18;
            this.btnKeepNew.Text = "Keep new";
            this.btnKeepNew.UseVisualStyleBackColor = true;
            // 
            // btnKeepOld
            // 
            this.btnKeepOld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnKeepOld.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnKeepOld.Location = new System.Drawing.Point(1177, 594);
            this.btnKeepOld.Name = "btnKeepOld";
            this.btnKeepOld.Size = new System.Drawing.Size(75, 23);
            this.btnKeepOld.TabIndex = 19;
            this.btnKeepOld.Text = "Keep old";
            this.btnKeepOld.UseVisualStyleBackColor = true;
            // 
            // FileComparison
            // 
            this.AcceptButton = this.btnKeepBoth;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnSkip;
            this.ClientSize = new System.Drawing.Size(1345, 629);
            this.Controls.Add(this.btnKeepOld);
            this.Controls.Add(this.btnKeepNew);
            this.Controls.Add(this.BtnSkip);
            this.Controls.Add(this.btnKeepBoth);
            this.Controls.Add(this.LblFilePath);
            this.Controls.Add(this.SplitterContainer);
            this.Name = "FileComparison";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FileComparison";
            this.SplitterContainer.Panel1.ResumeLayout(false);
            this.SplitterContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitterContainer)).EndInit();
            this.SplitterContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.SplitContainer SplitterContainer;
        private System.Windows.Forms.Label LblFilePath;
        private FilePreview.FilePreview FpvSource;
        private FilePreview.FilePreview FpvDestination;
        private System.Windows.Forms.Button BtnSkip;
        private System.Windows.Forms.Button btnKeepBoth;
        private System.Windows.Forms.Button btnKeepNew;
        private System.Windows.Forms.Button btnKeepOld;
    }
}