namespace VideoDedupClient.Dialogs
{
    using System.Diagnostics;
    using System.IO;
    using Google.Protobuf.WellKnownTypes;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.StringExtensions;
    using static VideoDedupGrpc.VideoDedupGrpcService;

    public partial class ResolveDuplicateDlg : Form
    {
        private static VideoDedupGrpcServiceClient GrpcClient =>
            Program.GrpcClient;

        private const int MinimumSizeDifference = 100 * 1024; // 100 kB

        private DuplicateData? duplicate;

        public ResolveDuplicateDlg() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            LoadNextDuplicate();

            base.OnLoad(e);
        }

        private void LoadNextDuplicate()
        {
            try
            {
                duplicate = GrpcClient.GetDuplicate(new Empty());
            }
            catch (Exception)
            {
                _ = MessageBox.Show(
                    $"Unable to process duplicate.{Environment.NewLine}" +
                    "Connection to server failed.",
                    "Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                DialogResult = DialogResult.Abort;
                return;
            }
            if (string.IsNullOrWhiteSpace(duplicate?.DuplicateId))
            {
                DialogResult = DialogResult.OK;
                return;
            }

            SplitterContainer.SplitterDistance = SplitterContainer.Width / 2;

            var sizeDifference =
                duplicate.File1.FileSize - duplicate.File2.FileSize;

            // If files are the same size
            if (Math.Abs(sizeDifference) <= MinimumSizeDifference)
            {
                FpvLeft.VideoFile = duplicate.File1;
                FpvRight.VideoFile = duplicate.File2;

                FpvLeft.HighlightColor = FpvRight.HighlightColor;
            }
            // If the left file is larger than the right
            else if (sizeDifference > 0)
            {
                FpvLeft.VideoFile = duplicate.File1;
                FpvRight.VideoFile = duplicate.File2;

                FpvLeft.HighlightColor = Color.LightGreen;
            }
            // If the right file is larger than the left
            else
            {
                // Switch left and right
                // Since we want the larger file on the left side
                FpvLeft.VideoFile = duplicate.File2;
                FpvRight.VideoFile = duplicate.File1;

                FpvLeft.HighlightColor = Color.LightGreen;
            }
        }

        private void ResolveDuplicate(
            ResolveOperation resolveOperation,
            VideoFile? fileToDelete = null) =>
            _ = GrpcClient.ResolveDuplicate(new ResolveDuplicateRequest
            {
                DuplicateId = duplicate!.DuplicateId,
                ResolveOperation = resolveOperation,
                File = fileToDelete,
            });

        private void BtnDeleteLeft_Click(object sender, EventArgs e)
        {
            ResolveDuplicate(ResolveOperation.DeleteFile, FpvLeft.VideoFile!);
            LoadNextDuplicate();
        }

        private void BtnDeleteRight_Click(object sender, EventArgs e)
        {
            ResolveDuplicate(ResolveOperation.DeleteFile, FpvRight.VideoFile!);
            LoadNextDuplicate();
        }

        private void OpenFileInExplorer(VideoFile file)
        {
            var filePath = file.FilePath;

            if (!string.IsNullOrWhiteSpace(Program.Configuration.ClientSourcePath)
                && !string.IsNullOrWhiteSpace(duplicate!.BasePath))
            {
                var relFilePath = filePath.MakeRelativePath(duplicate.BasePath);

                filePath = Path.Combine(
                    Program.Configuration.ClientSourcePath,
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

        private void BtnShowLeft_Click(object sender, EventArgs e) =>
            OpenFileInExplorer(FpvLeft.VideoFile!);

        private void BtnShowRight_Click(object sender, EventArgs e) =>
            OpenFileInExplorer(FpvRight.VideoFile!);

        private void BtnSkip_Click(object sender, EventArgs e)
        {
            ResolveDuplicate(ResolveOperation.Skip);
            LoadNextDuplicate();
        }

        private void BtnDiscard_Click(object sender, EventArgs e)
        {
            ResolveDuplicate(ResolveOperation.Discard);
            LoadNextDuplicate();
        }

        private void BtnReviewComparison_Click(object sender, EventArgs e)
        {
            using var dlg = new CustomVideoComparisonDlg
            {
                VideoComparisonSettings =
                    Program.GrpcClient.GetConfiguration(new Empty()).
                        VideoComparisonSettings,
                LeftFilePath = FpvLeft.VideoFile!.FilePath,
                RightFilePath = FpvRight.VideoFile!.FilePath,
                CloseButtons = CustomVideoComparisonDlg.Buttons.Close,
            };
            _ = dlg.ShowDialog();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            ResolveDuplicate(ResolveOperation.Cancel);
            DialogResult = DialogResult.Cancel;
        }
    }
}
