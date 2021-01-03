namespace VideoDedup
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using VideoDedupShared;

    public partial class FileComparisonDlg : Form
    {
        public IVideoFile LeftFile { get; set; }
        public IVideoFile RightFile { get; set; }

        public IResolverSettings Configuration { get; set; }

        public ResolveOperation ResolveOperation { get; set; }

        public FileComparisonDlg() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            SplitterContainer.SplitterDistance = SplitterContainer.Width / 2;

            FpvLeft.Configuration = Configuration;
            FpvLeft.VideoFile = LeftFile;
            FpvRight.Configuration = Configuration;
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

        private void BtnShowRight_Click(object sender, EventArgs e) =>
            OpenFileInExplorer(RightFile.FilePath);

        private void BtnShowLeft_Click(object sender, EventArgs e) =>
            OpenFileInExplorer(LeftFile.FilePath);

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
