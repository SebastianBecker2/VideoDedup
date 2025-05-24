namespace VideoDedupClient.Dialogs
{
    using Controls.FrameComparisonResultView;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using Properties;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.IVideoFileExtensions;
    using static VideoDedupGrpc.VideoDedupGrpcService;
    using FrameComparisonResult =
        Controls.FrameComparisonResultView.FrameComparisonResult;

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
        private int FrameComparisonIndex { get; set; }
        private int? FinishedInLoadLevel { get; set; }

        private List<FrameComparisonResult> FrameComparisons { get; } = [];
        private VideoComparisonResult? VideoComparisonResult { get; set; }

        public CustomVideoComparisonDlg()
        {
            InitializeComponent();

            TrbMaxDifferentPercentage.ValueChanged +=
                (_, _) => NumMaxDifferentPercentage.Value =
                    TrbMaxDifferentPercentage.Value;
            TrbMaxDifferentFrames.ValueChanged +=
                (_, _) => NumMaxDifferentFrames.Value =
                    TrbMaxDifferentFrames.Value;
            TrbMaxFrameComparison.ValueChanged +=
                (_, _) => NumMaxFrameComparison.Value =
                    TrbMaxFrameComparison.Value;

            NumMaxDifferentPercentage.ValueChanged +=
                (_, _) => TrbMaxDifferentPercentage.Value =
                    (int)NumMaxDifferentPercentage.Value;
            NumMaxDifferentFrames.ValueChanged +=
                (_, _) => TrbMaxDifferentFrames.Value =
                    (int)NumMaxDifferentFrames.Value;
            NumMaxFrameComparison.ValueChanged +=
                (_, _) => TrbMaxFrameComparison.Value =
                    (int)NumMaxFrameComparison.Value;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (VideoComparisonSettings is not null)
            {
                NumMaxFrameComparison.Value =
                    VideoComparisonSettings.CompareCount;
                NumMaxDifferentFrames.Value =
                    VideoComparisonSettings.MaxDifferentFrames;
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
                (int)NumMaxFrameComparison.Value;
            VideoComparisonSettings.MaxDifferentFrames =
                (int)NumMaxDifferentFrames.Value;
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

            if ((int)NumMaxFrameComparison.Value <= 0)
            {
                return;
            }

            PgbComparisonProgress.Visible = true;
            PnlResult.Visible = true;

            var request = new VideoComparisonConfiguration
            {
                ForceLoadingAllFrames = true,
                LeftFilePath = LeftFilePath,
                RightFilePath = RightFilePath,
                VideoComparisonSettings = new()
                {
                    CompareCount = (int)NumMaxFrameComparison.Value,
                    MaxDifferentFrames = (int)NumMaxDifferentFrames.Value,
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

        private static List<FrameComparisonResult> ToFrameComparisonResultEx(
            IEnumerable<VideoDedupGrpc.FrameComparisonResult> icrs)
        {
            var size = FrameComparisonResultViewCtl.ThumbnailSize;
            return [.. icrs.Select(icr => new FrameComparisonResult(icr, size))];
        }

        private void HandleStatusTimerTick(object sender, EventArgs e)
        {
            StatusTimer.Stop();
            var status = GrpcClient.GetVideoComparisonStatus(
                new VideoComparisonStatusRequest
                {
                    ComparisonToken = ComparisonToken,
                    FrameComparisonIndex = FrameComparisonIndex,
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

            if (status.FrameComparisons.Count == 0)
            {
                StatusTimer.Start();
                return;
            }

            var task = Task.Run(() =>
            {
                var ex = ToFrameComparisonResultEx(status.FrameComparisons);
                return ex;
            });
            _ = task.ContinueWith(t =>
            {
                // Try-Catch in case the dialog closed while converting
                // the FrameComparisonResults.
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

                        FrameComparisons.AddRange(t.Result);
                        UpdateResultDisplay();

                        static int selector(VideoDedupGrpc.FrameComparisonResult kvp) =>
                            kvp.Index;
                        FrameComparisonIndex = status.FrameComparisons
                            .Max(selector);
                        FrameComparisonIndex += 1;
                        var maxFrames = (int)NumMaxFrameComparison.Value;
                        if (FrameComparisonIndex >= maxFrames)
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
            if (FrameComparisons.Count == 0)
            {
                return;
            }

            PnlResult.Visible = true;

            if (RdbSortByProcessingOrder.Checked)
            {
                GrbVideoTimeline.Visible = false;

                foreach (var loadLevel in FrameComparisons
                             .GroupBy(kvp => kvp.LoadLevel))
                {
                    var (tlp, tlpResult) = GetLoadLevelControls(loadLevel.Key);
                    tlp.Visible = true;

                    AddFrameComparisonsToTableLayoutPanel(tlpResult, loadLevel);
                }
            }
            else
            {
                GrbVideoTimeline.Visible = true;
                TlpFirstLoadLevel.Visible = false;
                TlpSecondLoadLevel.Visible = false;
                TlpThirdLoadLevel.Visible = false;

                // Get frame comparisons in order of timeline
                var frames = FrameComparisons.OrderBy(icr =>
                    icr.LeftFrames.Index.Quotient);

                AddFrameComparisonsToTableLayoutPanel(
                    TlpVideoTimeline,
                    frames);
            }
        }

        private void AddFrameComparisonsToTableLayoutPanel(
            TableLayoutPanel tableLayoutPanel,
            IEnumerable<FrameComparisonResult> frameComparisonResults)
        {
            var lastComparedIndex = VideoComparisonResult?.LastComparedIndex;
            if (lastComparedIndex != null)
            {
                FinishedInLoadLevel = frameComparisonResults
                    .FirstOrDefault(icr => icr.Index == lastComparedIndex.Value)
                    ?.LoadLevel ?? FinishedInLoadLevel;
            }

            FrameComparisonResultViewCtl ToView(FrameComparisonResult icr) =>
                new()
                {
                    FrameComparisonIndex = icr.Index,
                    FrameComparisonResult = icr,
                    ComparisonAlreadyFinished =
                        lastComparedIndex != null
                        && icr.Index > lastComparedIndex,
                    FrameLoaded = FinishedInLoadLevel == null
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
                        icr.LeftFrames.Index.Quotient),
                    RightTimestamp = RightVideoFile!.Duration.ToTimeSpan().Multiply(
                        icr.RightFrames.Index.Quotient),
                };


            var resultViews = tableLayoutPanel.Controls
                .Cast<FrameComparisonResultViewCtl>();

            // Get all icrs that are missing.
            // Add them.
            // Then iterate over all with index to set ALL the new row indexes.
            tableLayoutPanel.SuspendLayout();
            tableLayoutPanel.Controls.AddRange([.. frameComparisonResults
                .Where(icr => !resultViews.Any(view => view.FrameComparisonResult == icr))
                .Select(ToView)]);
            tableLayoutPanel.ResumeLayout();

            var indexed = Enumerable
                .Range(0, frameComparisonResults.Count())
                .Zip(frameComparisonResults, (rowIndex, icr) => new
                {
                    RowIndex = rowIndex,
                    Result = icr,
                });

            foreach (FrameComparisonResultViewCtl view in tableLayoutPanel.Controls)
            {
                var rowIndex = indexed
                    .First(comparison => comparison.Result == view.FrameComparisonResult)
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

            FrameComparisons.Clear();

            FrameComparisonIndex = 0;
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
