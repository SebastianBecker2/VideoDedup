using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Diagnostics;
using NReco.VideoInfo;

namespace VideoDedup.FilePreview
{
    public partial class FilePreview : UserControl
    {
        private static readonly Size ThumbnailSize = new Size(256, 256);
        private static readonly ColorDepth ThumbnailColorDepth = ColorDepth.Depth32Bit;

        public Color HighlightColor
        {
            get { return TxtInfo.BackColor; }
            set { TxtInfo.BackColor = value; }
        }

        public VideoFile VideoFile { get => _VideoFile;
        set
            {
                _VideoFile = value;
            }
        }
        private VideoFile _VideoFile;

        private CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        public event EventHandler<ThumbnailLoadedEventArgs> ThumbnailLoaded;

        protected virtual void OnThumbnailLoaded(Image thumbnail, int index)
        {
            ThumbnailLoaded?.Invoke(this, new ThumbnailLoadedEventArgs
            {
                Thumbnail = thumbnail,
                Index = index
            });
        }

        public FilePreview()
        {
            InitializeComponent();

            ImlThumbnails.ColorDepth = ThumbnailColorDepth;
        }

        public Task UpdateDisplay()
        {
            if (VideoFile == null)
            {
                return null;
            }

            DisplayInfo();
            return LoadThumbnails();
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
            var file_size = VideoFile.FileSize;
            var media_info = VideoFile.MediaInfo;

            TxtInfo.Text = VideoFile.FilePath + Environment.NewLine;
            TxtInfo.Text += (file_size / (1024 * 1024)).ToString() + " MB" + Environment.NewLine;
            var duration_format = media_info.Duration.Hours >= 1 ? @"hh\:mm\:ss" : @"mm\:ss";
            TxtInfo.Text += media_info.Duration.ToString(duration_format) + Environment.NewLine;

            var stream = media_info.Streams.FirstOrDefault(s => s.CodecType == "video");
            if (stream == null)
            {
                return;
            }
            TxtInfo.Text += stream.Width.ToString() +
                " x " + stream.Height.ToString() +
                " @ " + stream.FrameRate + " Frames" + Environment.NewLine +
                stream.CodecLongName;
        }

        private Task LoadThumbnails() {
            var width = VideoFile.MediaInfo.Streams.First().Width;
            var height = VideoFile.MediaInfo.Streams.First().Height;
            SetImageSize(new Size(width, height));

            var cancelToken = CancellationTokenSource.Token;
            return Task.Run(() =>
            {
                foreach (var index in Enumerable.Range(0, ConfigData.ThumbnailViewCount))
                {
                    var image = VideoFile.GetThumbnail(index, ConfigData.ThumbnailViewCount);

                    if (cancelToken.IsCancellationRequested)
                    {
                        Debug.Print("Task cancelled");
                        return;
                    }

                    Invoke((Action)delegate
                    {
                        ImlThumbnails.Images.Add(image);
                        OnThumbnailLoaded(image, index);
                        LsvThumbnails.Items.Add(new ListViewItem { ImageIndex = index });
                    });
                }
            }, CancellationTokenSource.Token).ContinueWith(t => CancellationTokenSource.Dispose());
        }

        public void CancelThumbnails()
        {
            Debug.Print("Cancellation requested");
            CancellationTokenSource.Cancel();
        }
    }
}
