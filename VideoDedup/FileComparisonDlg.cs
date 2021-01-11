namespace VideoDedup
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using VideoDedupShared;

    public partial class FileComparisonDlg : Form
    {
        public VideoFile LeftFile { get; set; }
        public VideoFile RightFile { get; set; }

        public ConfigData Configuration { get; set; }

        public ResolveOperation ResolveOperation { get; set; }

        public FileComparisonDlg() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            SplitterContainer.SplitterDistance = SplitterContainer.Width / 2;

            FpvLeft.VideoFile = LeftFile;
            FpvRight.VideoFile = RightFile;

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

        private void BtnDeleteLeft_Click(object sender, EventArgs e)
        {
            ResolveOperation = ResolveOperation.DeleteFile1;
            DialogResult = DialogResult.OK;
        }

        private void BtnDeleteRight_Click(object sender, EventArgs e)
        {
            ResolveOperation = ResolveOperation.DeleteFile2;
            DialogResult = DialogResult.OK;
        }

        private void OpenFileInExplorer(VideoFile file)
        {
            var filePath = file.FilePath;
            if (!string.IsNullOrWhiteSpace(Configuration.ClientSourcePath))
            {
                filePath = Path.Combine(
                    Configuration.ClientSourcePath,
                    file.RelativeFilePath);
            }
            if (!File.Exists(filePath))
            {
                return;
            }

            // combine the arguments together
            // it doesn't matter if there is a space after ','
            var argument = "/select, \"" + filePath + "\"";

            _ = Process.Start("explorer.exe", argument);
        }

        private void BtnShowRight_Click(object sender, EventArgs e) =>
            OpenFileInExplorer(RightFile);

        private void BtnShowLeft_Click(object sender, EventArgs e) =>
            OpenFileInExplorer(LeftFile);

        private void BtnSkip_Click(object sender, EventArgs e)
        {
            ResolveOperation = ResolveOperation.Skip;
            DialogResult = DialogResult.OK;
        }

        private void BtnDiscard_Click(object sender, EventArgs e)
        {
            ResolveOperation = ResolveOperation.Discard;
            DialogResult = DialogResult.OK;
        }
    }
}
