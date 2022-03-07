namespace VideoDedup.FilePreview
{
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using VideoDedup.Properties;
    using VideoDedupShared;
    using VideoDedupShared.ExtensionMethods;

    public partial class FilePreviewCtl : UserControl
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

        public FilePreviewCtl()
        {
            InitializeComponent();

            ImlThumbnails.ColorDepth = ThumbnailColorDepth;

        }

        public void UpdateDisplay()
        {
            DisplayInfo();

            var resolution = GetVideoResolution(videoFile);
            SetThumbnailImageSize(resolution);

            var index = 0;
            foreach (var image in VideoFile.Images)
            {
                if (image == null)
                {
                    ImlThumbnails.Images.Add(Resources.BrokenImageIcon);
                }
                else
                {
                    ImlThumbnails.Images.Add(image);
                }
                _ = LsvThumbnails.Items.Add(new ListViewItem
                {
                    ImageIndex = index++,
                });
            }
        }

        /// <summary>
        /// We can't rely on having the codec information of the video.
        /// So if we are missing those, we use the resolution of the
        /// first available image from the video. If we don't have any
        /// we use the resolution of the "BrokenImageIcon", since that
        /// is what we will display anyways.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static Size GetVideoResolution(VideoFile file)
        {
            if (file.CodecInfo != null)
            {
                return file.CodecInfo.Size;
            }

            var img = file.Images.FirstOrDefault(i => i != null);
            if (img != null)
            {
                return img.Size;
            }

            return Resources.BrokenImageIcon.Size;
        }

        /// <summary>
        /// Scales the images to max 256x256
        /// but keeping the aspect ratio
        /// </summary>
        /// <param name="originalSize"></param>
        private void SetThumbnailImageSize(Size originalSize)
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

        private void DisplayInfo() => TxtInfo.Text = VideoFile.GetInfoText();
    }
}