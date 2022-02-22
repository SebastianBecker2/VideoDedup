namespace VideoDedup.FilePreview
{
    partial class FilePreviewDlg
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
            this.components = new System.ComponentModel.Container();
            this.ImlThumbnails = new System.Windows.Forms.ImageList(this.components);
            this.LsvThumbnails = new System.Windows.Forms.ListView();
            this.TxtInfo = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ImlThumbnails
            // 
            this.ImlThumbnails.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.ImlThumbnails.ImageSize = new System.Drawing.Size(16, 16);
            this.ImlThumbnails.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // LsvThumbnails
            // 
            this.LsvThumbnails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LsvThumbnails.HideSelection = false;
            this.LsvThumbnails.LargeImageList = this.ImlThumbnails;
            this.LsvThumbnails.Location = new System.Drawing.Point(3, 80);
            this.LsvThumbnails.Name = "LsvThumbnails";
            this.LsvThumbnails.Size = new System.Drawing.Size(417, 350);
            this.LsvThumbnails.SmallImageList = this.ImlThumbnails;
            this.LsvThumbnails.TabIndex = 2;
            this.LsvThumbnails.UseCompatibleStateImageBehavior = false;
            // 
            // TxtInfo
            // 
            this.TxtInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtInfo.Location = new System.Drawing.Point(4, 4);
            this.TxtInfo.Multiline = true;
            this.TxtInfo.Name = "TxtInfo";
            this.TxtInfo.ReadOnly = true;
            this.TxtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtInfo.Size = new System.Drawing.Size(416, 71);
            this.TxtInfo.TabIndex = 3;
            // 
            // FilePreviewDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TxtInfo);
            this.Controls.Add(this.LsvThumbnails);
            this.Name = "FilePreviewDlg";
            this.Size = new System.Drawing.Size(423, 433);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ImageList ImlThumbnails;
        private System.Windows.Forms.ListView LsvThumbnails;
        private System.Windows.Forms.TextBox TxtInfo;
    }
}
