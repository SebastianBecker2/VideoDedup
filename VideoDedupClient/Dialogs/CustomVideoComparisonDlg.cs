namespace VideoDedupClient.Dialogs
{
    using Controls.ImageComparisonResultView;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using Properties;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.IVideoFileExtensions;
    using static VideoDedupGrpc.VideoDedupGrpcService;
    using ImageComparisonResult =
        Controls.ImageComparisonResultView.ImageComparisonResult;

    public partial class CustomVideoComparisonDlg : Form
    {
        private static VideoDedupGrpcServiceClient GrpcClient =>
            Program.GrpcClient;

        public enum Buttons
        {
            OkCancel,
            Close,
        }

        private static readonly Color DifferenceColor =
            Color.LightCoral;
        private static readonly Color DuplicateColor =
            Color.DarkSeaGreen;
        private static readonly Color LoadedColor =
            Color.FromArgb(0xE5, 0xD4, 0x8B); // Flax
        private static readonly Color NotLoadedColor =
            SystemColors.Control;
        private static readonly Color CancelledColor =
            SystemColors.Control;
        private static readonly Color OddRowBackColor =
            SystemColors.ControlDark;
        private static readonly Color EvenRowBackColor =
            SystemColors.ControlDarkDark;

        public Buttons CloseButtons { get; set; } = Buttons.OkCancel;

        public VideoComparisonSettings? VideoComparisonSettings { get; set; }

        public string? LeftFilePath
        {
            get => TxtLeftFilePath?.Text;
            set => TxtLeftFilePath.Text = value;
        }
        public VideoFile? LeftVideoFile { get; set; }
        public string? RightFilePath
        {
            get => TxtRightFilePath?.Text;
            set => TxtRightFilePath.Text = value;
        }
        public VideoFile? RightVideoFile { get; set; }

        private string? ComparisonToken { get; set; }
        private int ImageComparisonIndex { get; set; }
        private int? FinishedInLoadLevel { get; set; }

        private List<ImageComparisonResult> ImageComparisons { get; } = [];
        private VideoComparisonResult? VideoComparisonResult { get; set; }

        public CustomVideoComparisonDlg()
        {
            InitializeComponent();

            TrbMaxDifferentPercentage.ValueChanged +=
                (_, _) => NumMaxDifferentPercentage.Value =
                    TrbMaxDifferentPercentage.Value;
            TrbMaxDifferentImages.ValueChanged +=
                (_, _) => NumMaxDifferentImages.Value =
                    TrbMaxDifferentImages.Value;
            TrbMaxImageComparison.ValueChanged +=
                (_, _) => NumMaxImageComparison.Value =
                    TrbMaxImageComparison.Value;

            NumMaxDifferentPercentage.ValueChanged +=
                (_, _) => TrbMaxDifferentPercentage.Value =
                    (int)NumMaxDifferentPercentage.Value;
            NumMaxDifferentImages.ValueChanged +=
                (_, _) => TrbMaxDifferentImages.Value =
                    (int)NumMaxDifferentImages.Value;
            NumMaxImageComparison.ValueChanged +=
                (_, _) => TrbMaxImageComparison.Value =
                    (int)NumMaxImageComparison.Value;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (VideoComparisonSettings is not null)
            {
                NumMaxImageComparison.Value =
                    VideoComparisonSettings.CompareCount;
                NumMaxDifferentImages.Value =
                    VideoComparisonSettings.MaxDifferentImages;
                NumMaxDifferentPercentage.Value =
                    VideoComparisonSettings.MaxDifference;
            }

            CleanUpResult();

            if (!string.IsNullOrWhiteSpace(LeftFilePath)
                && !string.IsNullOrWhiteSpace(RightFilePath))
            {
                StartComparison();
            }

            UpdateCloseButtons();

            RdbSortByProcessingOrder.Checked = true;

            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (ComparisonToken is not null)
            {
                StatusTimer.Stop();
                _ = GrpcClient.CancelVideoComparison(
                    new CancelVideoComparisonRequest
                    {
                        ComparisonToken = ComparisonToken,
                    });
            }
            base.OnClosed(e);
        }

        private void UpdateCloseButtons()
        {
            BtnOkay.Visible = CloseButtons == Buttons.OkCancel;
            BtnCancel.Visible = CloseButtons == Buttons.OkCancel;
            btnClose.Visible = CloseButtons == Buttons.Close;
            if (CloseButtons == Buttons.OkCancel)
            {
                CancelButton = BtnCancel;
            }
            else
            {
                CancelButton = btnClose;
            }
        }

        private void BtnSelectLeftFilePath_Click(object sender, EventArgs e)
        {
            using var dlg = new CommonOpenFileDialog();
            if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            LeftFilePath = dlg.FileName;
        }

        private void BtnSelectRightFilePath_Click(object sender, EventArgs e)
        {
            using var dlg = new CommonOpenFileDialog();
            if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            RightFilePath = dlg.FileName;
        }

        private void BtnOkay_Click(object sender, EventArgs e)
        {
            VideoComparisonSettings ??= new VideoComparisonSettings();
            VideoComparisonSettings.CompareCount =
                (int)NumMaxImageComparison.Value;
            VideoComparisonSettings.MaxDifferentImages =
                (int)NumMaxDifferentImages.Value;
            VideoComparisonSettings.MaxDifference =
                (int)NumMaxDifferentPercentage.Value;

            DialogResult = DialogResult.OK;
        }

        private void BtnStartComparison_Click(object sender, EventArgs e) =>
            StartComparison();

        private void StartComparison()
        {
            if (ComparisonToken is not null)
            {
                StatusTimer.Stop();
                _ = GrpcClient.CancelVideoComparison(
                    new CancelVideoComparisonRequest
                    {
                        ComparisonToken = ComparisonToken,
                    });
            }

            CleanUpResult();

            if ((int)NumMaxImageComparison.Value <= 0)
            {
                return;
            }

            PgbComparisonProgress.Visible = true;
            PnlResult.Visible = true;

            var request = new VideoComparisonConfiguration
            {
                ForceLoadingAllImages = true,
                LeftFilePath = LeftFilePath,
                RightFilePath = RightFilePath,
                VideoComparisonSettings = new()
                {
                    CompareCount = (int)NumMaxImageComparison.Value,
                    MaxDifferentImages = (int)NumMaxDifferentImages.Value,
                    MaxDifference = (int)NumMaxDifferentPercentage.Value
                }
            };

            var startData = GrpcClient.StartVideoComparison(request);

            if (string.IsNullOrWhiteSpace(startData.ComparisonToken))
            {
                _ = MessageBox.Show(
                    "Couldn't start comparison." + Environment.NewLine
                    + startData.VideoComparisonResult.Reason,
                    "Error starting comparison",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            ComparisonToken = startData.ComparisonToken;

            StatusTimer.Start();
        }

        private static List<ImageComparisonResult> ToImageComparisonResultEx(
            IEnumerable<VideoDedupGrpc.ImageComparisonResult> icrs)
        {
            var size = ImageComparisonResultViewCtl.ThumbnailSize;
            return icrs
                .Select(icr => new ImageComparisonResult(icr, size))
                .ToList();
        }

        private void HandleStatusTimerTick(object sender, EventArgs e)
        {
            StatusTimer.Stop();
            var status = GrpcClient.GetVideoComparisonStatus(
                new VideoComparisonStatusRequest
                {
                    ComparisonToken = ComparisonToken,
                    ImageComparisonIndex = ImageComparisonIndex,
                });

            LeftVideoFile = status.LeftFile;
            if (string.IsNullOrEmpty(TxtLeftFileInfo.Text))
            {
                TxtLeftFileInfo.Text = LeftVideoFile.GetInfoText();
            }
            RightVideoFile = status.RightFile;
            if (string.IsNullOrEmpty(TxtRightFileInfo.Text))
            {
                TxtRightFileInfo.Text = RightVideoFile.GetInfoText();
            }

            if (VideoComparisonResult?.ComparisonResult
                != status.VideoComparisonResult?.ComparisonResult)
            {
                VideoComparisonResult = status.VideoComparisonResult;
            }
            UpdateVideoComparisonResult(VideoComparisonResult);

            if (status.ImageComparisons.Count == 0)
            {
                StatusTimer.Start();
                return;
            }

            var task = Task.Run(() =>
            {
                var ex = ToImageComparisonResultEx(status.ImageComparisons);
                return ex;
            });
            _ = task.ContinueWith(t =>
            {
                // Try-Catch in case the dialog closed while converting
                // the ImageComparisonResults.
                try
                {
                    Invoke(() =>
                    {
                        // Check if a new video comparison has been started
                        // already.
                        if (ComparisonToken != status.ComparisonToken)
                        {
                            return;
                        }

                        ImageComparisons.AddRange(t.Result);
                        UpdateResultDisplay();

                        ImageComparisonIndex = status.ImageComparisons
                            .Max(kvp => kvp.Index);
                        ImageComparisonIndex += 1;
                        var maxImages = (int)NumMaxImageComparison.Value;
                        if (ImageComparisonIndex >= maxImages)
                        {
                            _ = GrpcClient.CancelVideoComparison(
                                new CancelVideoComparisonRequest
                                {
                                    ComparisonToken = ComparisonToken,
                                });
                            PgbComparisonProgress.Visible = false;
                            return;
                        }

                        StatusTimer.Start();
                    });
                }
                catch { }
            });
        }

        private void UpdateResultDisplay()
        {
            if (ImageComparisons.Count == 0)
            {
                return;
            }

            PnlResult.Visible = true;

            if (RdbSortByProcessingOrder.Checked)
            {
                GrbVideoTimeline.Visible = false;

                foreach (var loadLevel in ImageComparisons
                             .GroupBy(kvp => kvp.LoadLevel))
                {
                    var (tlp, tlpResult) = GetLoadLevelControls(loadLevel.Key);
                    tlp.Visible = true;

                    AddImageComparisonsToTableLayoutPanel(tlpResult, loadLevel);
                }
            }
            else
            {
                GrbVideoTimeline.Visible = true;
                TlpFirstLoadLevel.Visible = false;
                TlpSecondLoadLevel.Visible = false;
                TlpThirdLoadLevel.Visible = false;

                // Get image comparisons in order of timeline
                var images = ImageComparisons.OrderBy(icr =>
                    icr.LeftImages.Index.Quotient);

                AddImageComparisonsToTableLayoutPanel(
                    TlpVideoTimeline,
                    images);
            }
        }

        private void AddImageComparisonsToTableLayoutPanel(
            TableLayoutPanel tableLayoutPanel,
            IEnumerable<ImageComparisonResult> imageComparisonResults)
        {
            var lastComparedIndex = VideoComparisonResult?.LastComparedIndex;
            if (lastComparedIndex != null)
            {
                FinishedInLoadLevel = imageComparisonResults
                    .FirstOrDefault(icr => icr.Index == lastComparedIndex.Value)
                    ?.LoadLevel ?? FinishedInLoadLevel;
            }

            ImageComparisonResultViewCtl ToView(ImageComparisonResult icr) =>
                new()
                {
                    ImageComparisonIndex = icr.Index,
                    ImageComparisonResult = icr,
                    ComparisonAlreadyFinished =
                        lastComparedIndex != null
                        && icr.Index > lastComparedIndex,
                    ImageLoaded = FinishedInLoadLevel == null
                        || icr.LoadLevel <= FinishedInLoadLevel,
                    MaximumDifferencePercentage =
                        (int)NumMaxDifferentPercentage.Value,
                    Dock = DockStyle.Fill,
                    BackColor = GetAlternatingBackColor(),
                    DifferentColor = DifferenceColor,
                    DuplicateColor = DuplicateColor,
                    LoadedColor = LoadedColor,
                    NotLoadedColor = NotLoadedColor,
                    LeftTimestamp = LeftVideoFile!.Duration.ToTimeSpan().Multiply(
                        icr.LeftImages.Index.Quotient),
                    RightTimestamp = RightVideoFile!.Duration.ToTimeSpan().Multiply(
                        icr.RightImages.Index.Quotient),
                };


            var resultViews = tableLayoutPanel.Controls
                .Cast<ImageComparisonResultViewCtl>();

            // Get all icrs that are missing.
            // Add them.
            // Then iterate over all with index to set ALL the new row indexes.
            tableLayoutPanel.SuspendLayout();
            tableLayoutPanel.Controls.AddRange(imageComparisonResults
                .Where(icr => !resultViews.Any(view => view.ImageComparisonResult == icr))
                .Select(ToView)
                .ToArray());
            tableLayoutPanel.ResumeLayout();

            var indexed = Enumerable
                .Range(0, imageComparisonResults.Count())
                .Zip(imageComparisonResults, (rowIndex, icr) => new
                {
                    RowIndex = rowIndex,
                    Result = icr,
                });

            foreach (ImageComparisonResultViewCtl view in tableLayoutPanel.Controls)
            {
                var rowIndex = indexed
                    .First(comparison => comparison.Result == view.ImageComparisonResult)
                    .RowIndex;

                tableLayoutPanel.SetCellPosition(
                    view,
                    new TableLayoutPanelCellPosition
                    {
                        Column = 0,
                        Row = rowIndex,
                    });
            }
        }

        private Dictionary<int, Tuple<TableLayoutPanel, TableLayoutPanel>>?
            loadLevelControls;
        private Tuple<TableLayoutPanel, TableLayoutPanel> GetLoadLevelControls(
            int loadLevel)
        {
            loadLevelControls ??=
                new Dictionary<int, Tuple<TableLayoutPanel, TableLayoutPanel>>
                {
                    {1, Tuple.Create(TlpFirstLoadLevel, TlpFirstLoadLevelResult)},
                    {2, Tuple.Create(TlpSecondLoadLevel, TlpSecondLoadLevelResult)},
                    {3, Tuple.Create(TlpThirdLoadLevel, TlpThirdLoadLevelResult)},
                };
            return loadLevelControls[loadLevel];
        }

        private void UpdateVideoComparisonResult(VideoComparisonResult? result)
        {
            LblResult.Visible = true;

            if (result == null)
            {
                LblResult.Text = "Comparing...";
                LblResult.BackColor = NotLoadedColor;
                return;
            }

            if (result.ComparisonResult == ComparisonResult.Different)
            {
                LblResult.Text = "Videos are considered to be different.";
                LblResult.BackColor = DifferenceColor;
            }
            else if (result.ComparisonResult == ComparisonResult.Duplicate)
            {
                LblResult.Text = "Videos are considered to be duplicates.";
                LblResult.BackColor = DuplicateColor;
            }
            else if (result.ComparisonResult == ComparisonResult.Cancelled)
            {
                LblResult.Text = "Cancelled";
                LblResult.BackColor = CancelledColor;
            }
            else if (result.ComparisonResult == ComparisonResult.Aborted)
            {
                LblResult.Text = "Aborted: " + result.Reason;
                LblResult.BackColor = CancelledColor;
            }
        }

        private Color rowBackColor = EvenRowBackColor;
        private Color GetAlternatingBackColor()
        {
            if (rowBackColor == EvenRowBackColor)
            {
                rowBackColor = OddRowBackColor;
                return rowBackColor;
            }
            rowBackColor = EvenRowBackColor;
            return rowBackColor;
        }

        private void CleanUpResult()
        {
            TxtLeftFileInfo.Text = "";
            TxtRightFileInfo.Text = "";

            ImageComparisons.Clear();

            ImageComparisonIndex = 0;
            FinishedInLoadLevel = null;

            PnlResult.Visible = false;

            UpdateVideoComparisonResult(null);
            PgbComparisonProgress.Visible = false;

            static void ClearTableLayoutPanel(TableLayoutPanel tlp)
            {
                foreach (Control control in tlp.Controls)
                {
                    control.Dispose();
                }
                tlp.Controls.Clear();
                tlp.RowStyles.Clear();
                tlp.RowCount = 0;
            }

            ClearTableLayoutPanel(TlpFirstLoadLevelResult);
            TlpFirstLoadLevel.Visible = false;

            ClearTableLayoutPanel(TlpSecondLoadLevelResult);
            TlpSecondLoadLevel.Visible = false;

            ClearTableLayoutPanel(TlpThirdLoadLevelResult);
            TlpThirdLoadLevel.Visible = false;

            ClearTableLayoutPanel(TlpVideoTimeline);
            GrbVideoTimeline.Visible = false;
        }

        private void FirstLoadLevelHeaderClicked(
            object sender,
            EventArgs e)
        {
            if (TlpFirstLoadLevelResult.Visible)
            {
                TlpFirstLoadLevelResult.Visible = false;
                PibFirstLoadLevel.Image = Resources.ArrowDownBlue;
            }
            else
            {
                TlpFirstLoadLevelResult.Visible = true;
                PibFirstLoadLevel.Image = Resources.ArrowUpGray;
            }
        }

        private void SecondLoadLevelHeaderClicked(
            object sender,
            EventArgs e)
        {
            if (TlpSecondLoadLevelResult.Visible)
            {
                TlpSecondLoadLevelResult.Visible = false;
                PibSecondLoadLevel.Image = Resources.ArrowDownBlue;
            }
            else
            {
                TlpSecondLoadLevelResult.Visible = true;
                PibSecondLoadLevel.Image = Resources.ArrowUpGray;
            }
        }

        private void ThirdLoadLevelHeaderClicked(
            object sender,
            EventArgs e)
        {
            if (TlpThirdLoadLevelResult.Visible)
            {
                TlpThirdLoadLevelResult.Visible = false;
                PibThirdLoadLevel.Image = Resources.ArrowDownBlue;
            }
            else
            {
                TlpThirdLoadLevelResult.Visible = true;
                PibThirdLoadLevel.Image = Resources.ArrowUpGray;
            }
        }

        private void RdbSortByVideoTimeline_CheckedChanged(
            object sender,
            EventArgs e) =>
            UpdateResultDisplay();
        private void RdbSortByProcessingOrder_CheckedChanged(
            object sender,
            EventArgs e) =>
            UpdateResultDisplay();
    }
}
