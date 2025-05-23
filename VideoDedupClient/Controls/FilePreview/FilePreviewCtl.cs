namespace VideoDedupClient.Controls.FilePreview
{
    using System.Drawing;
    using System.Windows.Forms;
    using Properties;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.ByteStringExtensions;
    using VideoDedupSharedLib.ExtensionMethods.IVideoFileExtensions;

    public partial class FilePreviewCtl : UserControl
    {
        private static readonly System.Drawing.Size ThumbnailSize = new(256, 256);
        private static readonly ColorDepth ThumbnailColorDepth =
            ColorDepth.Depth32Bit;

        public Color HighlightColor
        {
            get => TxtInfo.BackColor;
            set => TxtInfo.BackColor = value;
        }

        public VideoFile? VideoFile
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
        private VideoFile? videoFile;

        public FilePreviewCtl()
        {
            InitializeComponent();

            ImlThumbnails.ColorDepth = ThumbnailColorDepth;

        }

        public void UpdateDisplay()
        {
            if (VideoFile is null)
            {
                return;
            }

            ImlThumbnails.Images.Clear();
            LsvThumbnails.Items.Clear();

            DisplayInfo(VideoFile);

            var resolution = GetVideoResolution(VideoFile);
            SetThumbnailSize(resolution);

            var index = 0;
            foreach (var frame in VideoFile.Frames)
            {
                if (frame.Length == 0)
                {
                    ImlThumbnails.Images.Add(Resources.BrokenImageIcon);
                }
                else
                {
                    ImlThumbnails.Images.Add(frame.ToImage());
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
        /// first available frame from the video. If we don't have any
        /// we use the resolution of the "BrokenImageIcon", since that
        /// is what we will display anyways.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static System.Drawing.Size GetVideoResolution(VideoFile file)
        {
            if (file.CodecInfo != null)
            {
                return file.CodecInfo.Size;
            }

            var img = file.Frames.FirstOrDefault(i => i.Length != 0);
            if (img != null)
            {
                return img.ToImage().Size;
            }

            return Resources.BrokenImageIcon.Size;
        }

        /// <summary>
        /// Scales the images to max 256x256
        /// but keeping the aspect ratio
        /// </summary>
        /// <param name="originalSize"></param>
        private void SetThumbnailSize(System.Drawing.Size originalSize)
        {
            var width = originalSize.Width;
            var height = originalSize.Height;

            if (width > height)
            {
                height = (int)(height / ((double)width / ThumbnailSize.Width));
                ImlThumbnails.ImageSize =
                    new System.Drawing.Size(ThumbnailSize.Width, height);
            }
            else
            {
                width = (int)(width / ((double)height / ThumbnailSize.Height));
                ImlThumbnails.ImageSize =
                    new System.Drawing.Size(width, ThumbnailSize.Height);
            }
        }

        private void DisplayInfo(VideoFile videoFile) =>
            TxtInfo.Text = videoFile.GetInfoText();
    }
}
