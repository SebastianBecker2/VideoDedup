using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XnaFan.ImageComparison;

namespace VideoDedup
{
    public partial class FileComparison : Form
    {
        public string SourceFilePath { get; set; }
        public string DestinationFilePath { get; set; }

        private Task SourceThumbnailTask;
        private Task DestinationThumbnailTask;

        public FileComparison()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            SplitterContainer.SplitterDistance = SplitterContainer.Width / 2;

            LblFilePath.Text = DestinationFilePath;

            SourceThumbnailTask = FpvSource.LoadFile(SourceFilePath);
            DestinationThumbnailTask = FpvDestination.LoadFile(DestinationFilePath);

            base.OnLoad(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var result = DialogResult;
            if (!SourceThumbnailTask.IsCompleted)
            {

                e.Cancel = true;
                FpvSource.CancelThumbnails();
                SourceThumbnailTask.ContinueWith(t => DialogResult = result,
                    TaskScheduler.FromCurrentSynchronizationContext());
            }

            if (!DestinationThumbnailTask.IsCompleted)
            {
                e.Cancel = true;
                FpvDestination.CancelThumbnails();
                DestinationThumbnailTask.ContinueWith(t => DialogResult = result,
                    TaskScheduler.FromCurrentSynchronizationContext());
            }

            base.OnFormClosing(e);
        }

        private void FpvSource_FileInfoLoaded(object sender, FilePreview.FileInfoLoadedEventArgs e)
        {
            if (FpvDestination.FileInfo == null)
            {
                return;
            }

            TryAutoCompare();
        }

        private void FpvDestination_FileInfoLoaded(object sender, FilePreview.FileInfoLoadedEventArgs e)
        {
            if (FpvSource.FileInfo == null)
            {
                return;
            }

            TryAutoCompare();
        }

        private void TryAutoCompare()
        {
            var dest_dur = FpvDestination.FileInfo.Duration;
            var src_dur = FpvSource.FileInfo.Duration;
            var dur_diff = Math.Abs((dest_dur - src_dur).TotalSeconds);
            if (dur_diff > 10)
            {
                // Keep both?
                btnKeepBoth.Focus();

                // Files are too different
                // User has to decide
                return;
            }

            var dest_size = FpvDestination.FileSize;
            var src_size = FpvSource.FileSize;
            var size_diff = Math.Abs(dest_size - src_size);
            if (size_diff < 100 * 1024)
            {
                // Keep old
                btnKeepOld.Focus();
                FpvDestination.HighlightColor = Color.LightGreen;
            }
            else
            {
                // Thumbnail compare is done
                // for each event of the thumbnails
                // loaded
                DoThumbnailCompare = true;
                return;
            }
        }

        bool DoThumbnailCompare = false;
        int SourceThumbnailIndex = -1;
        int DestinationThumbnailIndex = -1;
        IList<bool> FrameDifferences = new List<bool>();

        private void FpvSource_ThumbnailLoaded(object sender, FilePreview.ThumbnailLoadedEventArgs e)
        {
            SourceThumbnailIndex = e.Index;

            // Do we even want to compare?
            if (!DoThumbnailCompare)
            {
                return;
            }

            // Do we even have the corresponding image?
            if (e.Index > DestinationThumbnailIndex)
            {
                return;
            }

            var corresponding_thumbnail = FpvDestination.GetThumbnail(e.Index);
            CompareThumbnails(e.Thumbnail, corresponding_thumbnail);
        }

        private void FpvDestination_ThumbnailLoaded(object sender, FilePreview.ThumbnailLoadedEventArgs e)
        {
            DestinationThumbnailIndex = e.Index;

            // Do we even want to compare?
            if (!DoThumbnailCompare)
            {
                return;
            }

            // Do we even have the corresponding image?
            if (e.Index > SourceThumbnailIndex)
            {
                return;
            }

            var corresponding_thumbnail = FpvSource.GetThumbnail(e.Index);
            CompareThumbnails(e.Thumbnail, corresponding_thumbnail);
        }

        private void CompareThumbnails(Image source, Image destination)
        {
            var diff = source.PercentageDifference(destination);

            FrameDifferences.Add(diff > 0.5d);

            var diff_count = FrameDifferences.Count(d => d);

            if (diff_count == 1)
            {
                return;
            }

            if (diff_count >= 2)
            {
                DoThumbnailCompare = false;
                btnKeepBoth.Focus();
                return;
            }

            var dest_size = FpvDestination.FileSize;
            var src_size = FpvSource.FileSize;
            if (dest_size > src_size)
            {
                // Keep old
                btnKeepOld.Focus();
                FpvDestination.HighlightColor = Color.LightGreen;
            }
            else
            {
                // Take new
                btnKeepNew.Focus();
                FpvSource.HighlightColor = Color.LightGreen;
            }
        }
    }
}
