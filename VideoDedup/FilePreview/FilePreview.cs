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

namespace VideoDedup.FilePreview
{
    public partial class FilePreview : UserControl
    {
        private static readonly int ThumbnailCount = 20;
        private static readonly Size ThumbnailSize = new Size(256, 256);
        private static readonly ColorDepth ThumbnailColorDepth = ColorDepth.Depth32Bit;

        private CancellationTokenSource CancelTokenSource = new CancellationTokenSource();

        public Color HighlightColor
        {
            get { return TxtInfo.BackColor; }
            set { TxtInfo.BackColor = value; }
        }

        public NReco.VideoInfo.MediaInfo FileInfo { get; set; }
        public long FileSize { get; set; }

        public event EventHandler<FileInfoLoadedEventArgs> FileInfoLoaded;

        protected virtual void OnFileInfoLoaded(NReco.VideoInfo.MediaInfo file_info, long file_size)
        {
            FileInfoLoaded?.Invoke(this, new FileInfoLoadedEventArgs
            {
                FileInfo = file_info,
                FileSize = file_size
            });
        }

        public Image GetThumbnail(int index)
        {
            return ImlThumbnails.Images[index];
        }

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

        private void DisplayInfo(NReco.VideoInfo.MediaInfo info)
        {
            var duration_format = info.Duration.Hours >= 1 ? @"hh\:mm\:ss" : @"mm\:ss";
            TxtInfo.Text += info.Duration.ToString(duration_format) + Environment.NewLine;

            var stream = info.Streams.FirstOrDefault(s => s.CodecType == "video");
            if (stream == null)
            {
                return;
            }
            TxtInfo.Text += stream.Width.ToString() +
                " x " + stream.Height.ToString() +
                " @ " + stream.FrameRate + " Frames" + Environment.NewLine +
                stream.CodecLongName;
        }

        public Task LoadFile(string file_path)
        {
            var probe = new NReco.VideoInfo.FFProbe();
            FileInfo = probe.GetMediaInfo(file_path);
            FileSize = new System.IO.FileInfo(file_path).Length;
            OnFileInfoLoaded(FileInfo, FileSize);

            TxtInfo.Text = (FileSize / (1024 * 1024)).ToString() + " MB" + Environment.NewLine;
            DisplayInfo(FileInfo);

            var width = FileInfo.Streams.First().Width;
            var height = FileInfo.Streams.First().Height;
            SetImageSize(new Size(width, height));

            var duration = FileInfo.Duration;

            var step = (int)(duration.TotalSeconds / (ThumbnailCount + 1));

            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
            var cancel_token = CancelTokenSource.Token;
            return Task.Run(() =>
            {
                for (int i = 1; i <= ThumbnailCount; i++)
                {
                    var image_stream = new MemoryStream();
                    ffMpeg.GetVideoThumbnail(file_path, image_stream, step * i);

                    if (cancel_token.IsCancellationRequested)
                    {
                        Debug.Print("Task cancelled");
                        return;
                    }
                    Invoke((Action)delegate
                    {
                        var image = Image.FromStream(image_stream);
                        ImlThumbnails.Images.Add(image);
                        OnThumbnailLoaded(image, i - 1);
                        LsvThumbnails.Items.Add(new ListViewItem { ImageIndex = i -1 });
                    });
                }
            }, CancelTokenSource.Token).ContinueWith(t => CancelTokenSource.Dispose());
        }

        public void CancelThumbnails()
        {
            Debug.Print("Cancellation requested");
            CancelTokenSource.Cancel();
        }
    }
}
