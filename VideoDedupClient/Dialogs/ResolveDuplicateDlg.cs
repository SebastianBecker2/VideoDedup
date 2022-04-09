namespace VideoDedupClient.Dialogs
{
    using System.Diagnostics;
    using System.IO;
    using Google.Protobuf.WellKnownTypes;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.StringExtensions;
    using VideoDedupSharedLib.Interfaces;

    public partial class ResolveDuplicateDlg : Form
    {
        private const int MinimumSizeDifference = 100 * 1024; // 100 kB

        public VideoFile? LeftFile { get; set; }
        public VideoFile? RightFile { get; set; }

        public string? ServerSourcePath { get; set; }
        public string? ClientSourcePath { get; set; }

        public ResolveOperation ResolveOperation { get; set; }
        public VideoFile? FileToDelete { get; set; }

        public ResolveDuplicateDlg() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            if (LeftFile is not null && RightFile is not null)
            {
                DisplayVideoFiles();
            }

            base.OnLoad(e);
        }

        private void DisplayVideoFiles()
        {
            if (LeftFile is null || RightFile is null)
            {
                return;
            }

            SplitterContainer.SplitterDistance = SplitterContainer.Width / 2;

            var sizeDifference = LeftFile.FileSize - RightFile.FileSize;

            // If files are the same size
            if (Math.Abs(sizeDifference) <= MinimumSizeDifference)
            {
                FpvLeft.VideoFile = LeftFile;
                FpvRight.VideoFile = RightFile;
            }
            // If the left file is larger than the right
            else if (sizeDifference > 0)
            {
                FpvLeft.VideoFile = LeftFile;
                FpvRight.VideoFile = RightFile;

                FpvLeft.HighlightColor = Color.LightGreen;
            }
            // If the right file is larger than the left
            else
            {
                // Switch left and right
                // Since we want the larger file on the left side
                FpvLeft.VideoFile = RightFile;
                FpvRight.VideoFile = LeftFile;

                LeftFile = FpvLeft.VideoFile;
                RightFile = FpvRight.VideoFile;

                FpvLeft.HighlightColor = Color.LightGreen;
            }
        }

        private void BtnDeleteLeft_Click(object sender, EventArgs e)
        {
            ResolveOperation = ResolveOperation.DeleteFile;
            FileToDelete = LeftFile;
            DialogResult = DialogResult.OK;
        }

        private void BtnDeleteRight_Click(object sender, EventArgs e)
        {
            ResolveOperation = ResolveOperation.DeleteFile;
            FileToDelete = RightFile;
            DialogResult = DialogResult.OK;
        }

        private void OpenFileInExplorer(IVideoFile file)
        {
            var filePath = file.FilePath;

            if (!string.IsNullOrWhiteSpace(ClientSourcePath)
                && !string.IsNullOrWhiteSpace(ServerSourcePath))
            {
                var relFilePath = filePath.MakeRelativePath(ServerSourcePath);

                filePath = Path.Combine(
                    ClientSourcePath,
                    relFilePath);
            }

            if (!File.Exists(filePath))
            {
                _ = MessageBox.Show($"Can't find the file.{Environment.NewLine}" +
                    "Did you set the Client-Side Source Directory properly?",
                    "File not found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // combine the arguments together
            // it doesn't matter if there is a space after ','
            var argument = "/select, \"" + filePath + "\"";

            _ = Process.Start("explorer.exe", argument);
        }

        private void BtnShowRight_Click(object sender, EventArgs e)
        {
            if (RightFile is null)
            {
                return;
            }

            OpenFileInExplorer(RightFile);
        }

        private void BtnShowLeft_Click(object sender, EventArgs e)
        {
            if (LeftFile is null)
            {
                return;
            }

            OpenFileInExplorer(LeftFile);
        }

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

        private void BtnReviewComparison_Click(object sender, EventArgs e)
        {
            if (LeftFile is null || RightFile is null)
            {
                return;
            }

            using var dlg = new CustomVideoComparisonDlg
            {
                VideoComparisonSettings =
                    Program.GrpcClient.GetConfiguration(new Empty()).
                        VideoComparisonSettings,
                LeftFilePath = LeftFile.FilePath,
                RightFilePath = RightFile.FilePath,
                CloseButtons = CustomVideoComparisonDlg.Buttons.Close,
            };
            _ = dlg.ShowDialog();
        }
    }
}
