namespace VideoDedupClient.Controls.FilePreview
{
    using System.ComponentModel;

    partial class FilePreviewCtl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            components = new Container();
            ImlThumbnails = new ImageList(components);
            LsvThumbnails = new ListView();
            TxtInfo = new TextBox();
            SuspendLayout();
            // 
            // ImlThumbnails
            // 
            ImlThumbnails.ColorDepth = ColorDepth.Depth8Bit;
            ImlThumbnails.ImageSize = new Size(16, 16);
            ImlThumbnails.TransparentColor = Color.Transparent;
            // 
            // LsvThumbnails
            // 
            LsvThumbnails.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LsvThumbnails.LargeImageList = ImlThumbnails;
            LsvThumbnails.Location = new Point(4, 137);
            LsvThumbnails.Margin = new Padding(4, 3, 4, 3);
            LsvThumbnails.Name = "LsvThumbnails";
            LsvThumbnails.Size = new Size(486, 358);
            LsvThumbnails.SmallImageList = ImlThumbnails;
            LsvThumbnails.TabIndex = 2;
            LsvThumbnails.UseCompatibleStateImageBehavior = false;
            // 
            // TxtInfo
            // 
            TxtInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TxtInfo.Location = new Point(5, 5);
            TxtInfo.Margin = new Padding(4, 3, 4, 3);
            TxtInfo.Multiline = true;
            TxtInfo.Name = "TxtInfo";
            TxtInfo.ReadOnly = true;
            TxtInfo.ScrollBars = ScrollBars.Both;
            TxtInfo.Size = new Size(485, 126);
            TxtInfo.TabIndex = 3;
            // 
            // FilePreviewCtl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(TxtInfo);
            Controls.Add(LsvThumbnails);
            Margin = new Padding(4, 3, 4, 3);
            Name = "FilePreviewCtl";
            Size = new Size(493, 500);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private ImageList ImlThumbnails;
        private ListView LsvThumbnails;
        private TextBox TxtInfo;
    }
}
