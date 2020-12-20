namespace VideoDedup.FilePreview
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class FilePreview : UserControl
    {
        private static readonly Size ThumbnailSize = new Size(256, 256);
        private static readonly ColorDepth ThumbnailColorDepth = ColorDepth.Depth32Bit;

        public Color HighlightColor
        {
            get => this.TxtInfo.BackColor;
            set => this.TxtInfo.BackColor = value;
        }

        public VideoFile VideoFile { get; set; }

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public event EventHandler<ThumbnailLoadedEventArgs> ThumbnailLoaded;

        protected virtual void OnThumbnailLoaded(Image thumbnail, int index) => ThumbnailLoaded?.Invoke(this, new ThumbnailLoadedEventArgs
        {
            Thumbnail = thumbnail,
            Index = index
        });

        public FilePreview()
        {
            this.InitializeComponent();

            this.ImlThumbnails.ColorDepth = ThumbnailColorDepth;
        }

        public Task UpdateDisplay()
        {
            if (this.VideoFile == null)
            {
                return null;
            }

            this.DisplayInfo();
            return this.LoadThumbnails();
        }

        /// <summary>
        /// Scales the images to max 256x256
        /// but keeping the aspect ratio
        /// </summary>
        /// <param name="original_size"></param>
        private void SetImageSize(Size original_size)
        {
            var width = original_size.Width;
            var height = original_size.Height;

            if (width > height)
            {
                height = (int)(height / ((double)width / ThumbnailSize.Width));
                this.ImlThumbnails.ImageSize = new Size(ThumbnailSize.Width, height);
            }
            else
            {
                width = (int)(width / ((double)height / ThumbnailSize.Height));
                this.ImlThumbnails.ImageSize = new Size(width, ThumbnailSize.Height);
            }
        }

        private void DisplayInfo()
        {
            var file_size = this.VideoFile.FileSize;
            var media_info = this.VideoFile.MediaInfo;

            this.TxtInfo.Text = this.VideoFile.FilePath + Environment.NewLine;
            this.TxtInfo.Text += (file_size / (1024 * 1024)).ToString() + " MB" + Environment.NewLine;
            var duration_format = media_info.Duration.Hours >= 1 ? @"hh\:mm\:ss" : @"mm\:ss";
            this.TxtInfo.Text += media_info.Duration.ToString(duration_format) + Environment.NewLine;

            var stream = media_info.Streams.FirstOrDefault(s => s.CodecType == "video");
            if (stream == null)
            {
                return;
            }
            this.TxtInfo.Text += stream.Width.ToString() +
                " x " + stream.Height.ToString() +
                " @ " + stream.FrameRate + " Frames" + Environment.NewLine +
                stream.CodecLongName;
        }

        private Task LoadThumbnails()
        {
            var width = this.VideoFile.MediaInfo.Streams.First().Width;
            var height = this.VideoFile.MediaInfo.Streams.First().Height;
            this.SetImageSize(new Size(width, height));

            var cancelToken = this.cancellationTokenSource.Token;
            return Task.Run(() =>
            {
                foreach (var index in Enumerable.Range(0, ConfigData.ThumbnailViewCount))
                {
                    var image = this.VideoFile.GetThumbnail(index, ConfigData.ThumbnailViewCount);

                    if (cancelToken.IsCancellationRequested)
                    {
                        Debug.Print("Task cancelled");
                        return;
                    }

                    _ = this.Invoke((Action)delegate
                      {
                          this.ImlThumbnails.Images.Add(image);
                          this.OnThumbnailLoaded(image, index);
                          _ = this.LsvThumbnails.Items.Add(new ListViewItem { ImageIndex = index });
                      });
                }
            }, this.cancellationTokenSource.Token).ContinueWith(t => this.cancellationTokenSource.Dispose());
        }

        public void CancelThumbnails()
        {
            Debug.Print("Cancellation requested");
            this.cancellationTokenSource.Cancel();
        }
    }
}
