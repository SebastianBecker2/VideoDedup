namespace VideoDedup
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class FileComparison : Form
    {
        public VideoFile LeftFile { get; set; }
        public VideoFile RightFile { get; set; }

        private Task leftThumbnailTask;
        private Task rightThumbnailTask;

        public FileComparison() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            SplitterContainer.SplitterDistance = SplitterContainer.Width / 2;

            FpvLeft.VideoFile = LeftFile;
            leftThumbnailTask = FpvLeft.UpdateDisplay();
            FpvRight.VideoFile = RightFile;
            rightThumbnailTask = FpvRight.UpdateDisplay();

            var leftSize = LeftFile.FileSize;
            var rightSize = RightFile.FileSize;
            if (Math.Abs(leftSize - rightSize) > (100 * 1024))
            {
                if (leftSize > rightSize)
                {
                    FpvLeft.HighlightColor = Color.LightGreen;
                }
                else
                {
                    FpvRight.HighlightColor = Color.LightGreen;
                }
            }

            base.OnLoad(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var result = DialogResult;
            if (!leftThumbnailTask.IsCompleted)
            {
                e.Cancel = true;
                FpvLeft.CancelThumbnails();
                _ = leftThumbnailTask.ContinueWith(t => DialogResult = result,
                    TaskScheduler.FromCurrentSynchronizationContext());
            }

            if (!rightThumbnailTask.IsCompleted)
            {
                e.Cancel = true;
                FpvRight.CancelThumbnails();
                _ = rightThumbnailTask.ContinueWith(t => DialogResult = result,
                    TaskScheduler.FromCurrentSynchronizationContext());
            }

            base.OnFormClosing(e);
        }

        private void BtnDeleteLeft_Click(object sender, EventArgs e)
        {
            void delete_and_close()
            {
                try
                {
                    File.Delete(LeftFile.FilePath);
                    DialogResult = DialogResult.Yes;
                }
                catch (Exception exc)
                {
                    _ = MessageBox.Show(exc.Message);
                }
            }

            if (!leftThumbnailTask.IsCompleted)
            {
                FpvLeft.CancelThumbnails();
                _ = leftThumbnailTask.ContinueWith(t => delete_and_close(),
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                delete_and_close();
            }
        }

        private void BtnDeleteRight_Click(object sender, EventArgs e)
        {
            void delete_and_close()
            {
                try
                {
                    File.Delete(RightFile.FilePath);
                    DialogResult = DialogResult.Yes;
                }
                catch (Exception exc)
                {
                    _ = MessageBox.Show(exc.Message);
                }
            }

            if (!rightThumbnailTask.IsCompleted)
            {
                FpvRight.CancelThumbnails();
                _ = rightThumbnailTask.ContinueWith(t => delete_and_close(),
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                delete_and_close();
            }
        }

        private void OpenFileInExplorer(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            // combine the arguments together
            // it doesn't matter if there is a space after ','
            var argument = "/select, \"" + filePath + "\"";

            _ = Process.Start("explorer.exe", argument);
        }

        private void BtnShowRight_Click(object sender, EventArgs e) => OpenFileInExplorer(RightFile.FilePath);

        private void BtnShowLeft_Click(object sender, EventArgs e) => OpenFileInExplorer(LeftFile.FilePath);
    }
}
