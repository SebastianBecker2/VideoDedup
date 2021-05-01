namespace VideoDedup.FilePreview
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using VideoDedupShared;

    public partial class FilePreviewDlg : UserControl
    {
        private static readonly Size ThumbnailSize = new Size(256, 256);
        private static readonly ColorDepth ThumbnailColorDepth = ColorDepth.Depth32Bit;

        public Color HighlightColor
        {
            get => TxtInfo.BackColor;
            set => TxtInfo.BackColor = value;
        }

        public VideoFile VideoFile
        {
            get => videoFile;
            set
            {
                videoFile = value;
                if (videoFile != null)
                {
                    UpdateDisplay();
                }
            }
        }
        private VideoFile videoFile = null;

        public FilePreviewDlg()
        {
            InitializeComponent();

            ImlThumbnails.ColorDepth = ThumbnailColorDepth;

        }

        public void UpdateDisplay()
        {
            DisplayInfo();

            SetImageSize(VideoFile.CodecInfo.Size);

            var index = 0;
            foreach (var image in VideoFile.Images)
            {
                ImlThumbnails.Images.Add(image);
                _ = LsvThumbnails.Items.Add(new ListViewItem
                {
                    ImageIndex = index++,
                });
            }
        }

        /// <summary>
        /// Scales the images to max 256x256
        /// but keeping the aspect ratio
        /// </summary>
        /// <param name="originalSize"></param>
        private void SetImageSize(Size originalSize)
        {
            var width = originalSize.Width;
            var height = originalSize.Height;

            if (width > height)
            {
                height = (int)(height / ((double)width / ThumbnailSize.Width));
                ImlThumbnails.ImageSize = new Size(ThumbnailSize.Width, height);
            }
            else
            {
                width = (int)(width / ((double)height / ThumbnailSize.Height));
                ImlThumbnails.ImageSize = new Size(width, ThumbnailSize.Height);
            }
        }

        private void DisplayInfo()
        {
            var fileSize = VideoFile.FileSize;
            var duration = VideoFile.Duration;
            var codecInfo = VideoFile.CodecInfo;

            TxtInfo.Text = VideoFile.FilePath + Environment.NewLine;
            TxtInfo.Text += (fileSize / (1024 * 1024)).ToString() + " MB" + Environment.NewLine;
            var duration_format = duration.Hours >= 1 ? @"hh\:mm\:ss" : @"mm\:ss";
            TxtInfo.Text += duration.ToString(duration_format) + Environment.NewLine;

            if (codecInfo == null)
            {
                return;
            }
            TxtInfo.Text += codecInfo.Size.Width.ToString() +
                " x " + codecInfo.Size.Height.ToString() +
                " @ " + codecInfo.FrameRate + " Frames" + Environment.NewLine +
                codecInfo.Name;
        }
    }
}
