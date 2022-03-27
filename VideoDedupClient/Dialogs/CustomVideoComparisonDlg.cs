namespace VideoDedupClient.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Controls.ImageComparisonResultView;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.IVideoFileExtensions;

    public partial class CustomVideoComparisonDlg : Form
    {
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

        private List<ImageComparisonResultEx> ImageComparisons { get; } = new();
        private VideoComparisonResult? VideoComparisonResult { get; set; }

        public CustomVideoComparisonDlg()
        {
            InitializeComponent();

            TrbMaxDifferentPercentage.ValueChanged +=
                (s, e) => NumMaxDifferentPercentage.Value =
                    TrbMaxDifferentPercentage.Value;
            TrbMaxDifferentImages.ValueChanged +=
                (s, e) => NumMaxDifferentImages.Value =
                    TrbMaxDifferentImages.Value;
            TrbMaxImageComparison.ValueChanged +=
                (s, e) => NumMaxImageComparison.Value =
                    TrbMaxImageComparison.Value;

            NumMaxDifferentPercentage.ValueChanged +=
                (s, e) => TrbMaxDifferentPercentage.Value =
                    (int)NumMaxDifferentPercentage.Value;
            NumMaxDifferentImages.ValueChanged +=
                (s, e) => TrbMaxDifferentImages.Value =
                    (int)NumMaxDifferentImages.Value;
            NumMaxImageComparison.ValueChanged +=
                (s, e) => TrbMaxImageComparison.Value =
                    (int)NumMaxImageComparison.Value;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (VideoComparisonSettings is not null)
            {
                NumMaxDifferentPercentage.Value =
                    VideoComparisonSettings.CompareCount;
                NumMaxDifferentImages.Value =
                    VideoComparisonSettings.MaxDifferentImages;
                NumMaxImageComparison.Value =
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
                _ = VideoDedupDlg.GrpcClient.CancelCustomVideoComparison(
                    new CancelCustomVideoComparisonRequest
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
                _ = VideoDedupDlg.GrpcClient.CancelCustomVideoComparison(
                    new CancelCustomVideoComparisonRequest
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

            var request = new CustomVideoComparisonConfiguration
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

            var startData = VideoDedupDlg.GrpcClient.StartCustomVideoComparison(
                request);

            if (startData.ComparisonToken == null)
            {
                _ = MessageBox.Show(
                    "Couldn't start comparison." + Environment.NewLine
                    + startData.VideoComparisonResult.Reason,
                    "Erro starting comparison",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            ComparisonToken = startData.ComparisonToken;

            StatusTimer.Start();
        }

        private static IEnumerable<ImageComparisonResultEx> ToImageComparisonResultEx(
            IEnumerable<ImageComparisonResult> icrs)
        {
            var size = ImageComparisonResultViewCtl.ThumbnailSize;
            return icrs
                .Select(icr => new ImageComparisonResultEx(icr, size))
                .ToList();
        }

        private void HandleStatusTimerTick(object sender, EventArgs e)
        {
            StatusTimer.Stop();
            var status = VideoDedupDlg.GrpcClient.GetVideoComparisonStatus(
                new CustomVideoComparisonStatusRequest
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

            if (!status.ImageComparisons.Any())
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
                            _ = VideoDedupDlg.GrpcClient.CancelCustomVideoComparison(
                                new CancelCustomVideoComparisonRequest
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
            if (!ImageComparisons.Any())
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
            IEnumerable<ImageComparisonResultEx> imageComparisonResults)
        {
            var lastComparedIndex = VideoComparisonResult?.LastComparedIndex;
            if (lastComparedIndex != null)
            {
                FinishedInLoadLevel = imageComparisonResults
                    .FirstOrDefault(icr => icr.Index == lastComparedIndex.Value)
                    ?.LoadLevel ?? FinishedInLoadLevel;
            }

            ImageComparisonResultViewCtl toView(ImageComparisonResultEx icr) =>
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
                .Select(icr => toView(icr))
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
            if (loadLevelControls == null)
            {
                loadLevelControls =
                    new Dictionary<int, Tuple<TableLayoutPanel, TableLayoutPanel>>
                    {
                    { 1, Tuple.Create(TlpFirstLoadLevel, TlpFirstLoadLevelResult) },
                    { 2, Tuple.Create(TlpSecondLoadLevel, TlpSecondLoadLevelResult) },
                    { 3, Tuple.Create(TlpThirdLoadLevel, TlpThirdLoadLevelResult) },
                    };
            }
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

            static void clearTableLayoutPanel(TableLayoutPanel tlp)
            {
                var localRef = tlp.Controls;
                tlp.Controls.Clear();
                foreach (Control control in localRef)
                {
                    control.Dispose();
                }
                tlp.RowStyles.Clear();
                tlp.RowCount = 0;
            }

            clearTableLayoutPanel(TlpFirstLoadLevelResult);
            TlpFirstLoadLevel.Visible = false;

            clearTableLayoutPanel(TlpSecondLoadLevelResult);
            TlpSecondLoadLevel.Visible = false;

            clearTableLayoutPanel(TlpThirdLoadLevelResult);
            TlpThirdLoadLevel.Visible = false;

            clearTableLayoutPanel(TlpVideoTimeline);
            GrbVideoTimeline.Visible = false;
        }

        private void FirstLoadLevelHeaderClicked(
            object sender,
            EventArgs e)
        {
            if (TlpFirstLoadLevelResult.Visible)
            {
                TlpFirstLoadLevelResult.Visible = false;
                PibFirstLoadLevel.Image = Properties.Resources.ArrowDownBlue;
            }
            else
            {
                TlpFirstLoadLevelResult.Visible = true;
                PibFirstLoadLevel.Image = Properties.Resources.ArrowUpGray;
            }
        }

        private void SecondLoadLevelHeaderClicked(
            object sender,
            EventArgs e)
        {
            if (TlpSecondLoadLevelResult.Visible)
            {
                TlpSecondLoadLevelResult.Visible = false;
                PibSecondLoadLevel.Image = Properties.Resources.ArrowDownBlue;
            }
            else
            {
                TlpSecondLoadLevelResult.Visible = true;
                PibSecondLoadLevel.Image = Properties.Resources.ArrowUpGray;
            }
        }

        private void ThirdLoadLevelHeaderClicked(
            object sender,
            EventArgs e)
        {
            if (TlpThirdLoadLevelResult.Visible)
            {
                TlpThirdLoadLevelResult.Visible = false;
                pictureBox1.Image = Properties.Resources.ArrowDownBlue;
            }
            else
            {
                TlpThirdLoadLevelResult.Visible = true;
                pictureBox1.Image = Properties.Resources.ArrowUpGray;
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
