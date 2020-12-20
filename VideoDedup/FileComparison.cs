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

        public FileComparison() => this.InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            this.SplitterContainer.SplitterDistance = this.SplitterContainer.Width / 2;

            this.FpvLeft.VideoFile = this.LeftFile;
            this.leftThumbnailTask = this.FpvLeft.UpdateDisplay();
            this.FpvRight.VideoFile = this.RightFile;
            this.rightThumbnailTask = this.FpvRight.UpdateDisplay();

            var leftSize = this.LeftFile.FileSize;
            var rightSize = this.RightFile.FileSize;
            if (Math.Abs(leftSize - rightSize) > (100 * 1024))
            {
                if (leftSize > rightSize)
                {
                    this.FpvLeft.HighlightColor = Color.LightGreen;
                }
                else
                {
                    this.FpvRight.HighlightColor = Color.LightGreen;
                }
            }

            base.OnLoad(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var result = this.DialogResult;
            if (!this.leftThumbnailTask.IsCompleted)
            {
                e.Cancel = true;
                this.FpvLeft.CancelThumbnails();
                _ = this.leftThumbnailTask.ContinueWith(t => this.DialogResult = result,
                    TaskScheduler.FromCurrentSynchronizationContext());
            }

            if (!this.rightThumbnailTask.IsCompleted)
            {
                e.Cancel = true;
                this.FpvRight.CancelThumbnails();
                _ = this.rightThumbnailTask.ContinueWith(t => this.DialogResult = result,
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
                    File.Delete(this.LeftFile.FilePath);
                    this.DialogResult = DialogResult.Yes;
                }
                catch (Exception exc)
                {
                    _ = MessageBox.Show(exc.Message);
                }
            }

            if (!this.leftThumbnailTask.IsCompleted)
            {
                this.FpvLeft.CancelThumbnails();
                _ = this.leftThumbnailTask.ContinueWith(t => delete_and_close(),
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
                    File.Delete(this.RightFile.FilePath);
                    this.DialogResult = DialogResult.Yes;
                }
                catch (Exception exc)
                {
                    _ = MessageBox.Show(exc.Message);
                }
            }

            if (!this.rightThumbnailTask.IsCompleted)
            {
                this.FpvRight.CancelThumbnails();
                _ = this.rightThumbnailTask.ContinueWith(t => delete_and_close(),
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

        private void BtnShowRight_Click(object sender, EventArgs e) => this.OpenFileInExplorer(this.RightFile.FilePath);

        private void BtnShowLeft_Click(object sender, EventArgs e) => this.OpenFileInExplorer(this.LeftFile.FilePath);
    }
}
