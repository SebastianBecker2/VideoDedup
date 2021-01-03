namespace VideoDedup.FilePreview
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using VideoDedupShared;

    public partial class FilePreviewDlg : UserControl
    {
        private static readonly Size ThumbnailSize = new Size(256, 256);
        private static readonly ColorDepth ThumbnailColorDepth = ColorDepth.Depth32Bit;

        public IResolverSettings Configuration { get; set; }

        public Color HighlightColor
        {
            get => TxtInfo.BackColor;
            set => TxtInfo.BackColor = value;
        }

        public IVideoFile VideoFile
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
        private IVideoFile videoFile = null;

        public FilePreviewDlg()
        {
            InitializeComponent();

            ImlThumbnails.ColorDepth = ThumbnailColorDepth;

        }

        public void UpdateDisplay()
        {
            DisplayInfo();

            var width = VideoFile.VideoCodec.Width;
            var height = VideoFile.VideoCodec.Height;
            SetImageSize(new Size(width, height));

            foreach (var index in Enumerable.Range(0, Configuration.ThumbnailViewCount))
            {
                var image = VideoFile.GetThumbnail(index, Configuration.ThumbnailViewCount);

                ImlThumbnails.Images.Add(image);
                _ = LsvThumbnails.Items.Add(new ListViewItem { ImageIndex = index });
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
            var videoCodec = VideoFile.VideoCodec;

            TxtInfo.Text = VideoFile.FilePath + Environment.NewLine;
            TxtInfo.Text += (fileSize / (1024 * 1024)).ToString() + " MB" + Environment.NewLine;
            var duration_format = duration.Hours >= 1 ? @"hh\:mm\:ss" : @"mm\:ss";
            TxtInfo.Text += duration.ToString(duration_format) + Environment.NewLine;

            if (videoCodec == null)
            {
                return;
            }
            TxtInfo.Text += videoCodec.Width.ToString() +
                " x " + videoCodec.Height.ToString() +
                " @ " + videoCodec.FrameRate + " Frames" + Environment.NewLine +
                videoCodec.CodecLongName;
        }
    }
}
