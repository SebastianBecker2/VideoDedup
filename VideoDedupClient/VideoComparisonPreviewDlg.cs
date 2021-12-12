namespace VideoDedup
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using ImageGroupBox;
    using VideoDedupShared;
    using Wcf.Contracts.Data;

    public partial class VideoComparisonPreviewDlg : Form
    {
        public enum Buttons
        {
            OkCancel,
            Close,
        }

        private struct ImageComparison
        {
            public Image LeftImage { get; set; }
            public double Difference { get; set; }
            public Image RightImage { get; set; }
            public bool ComparisonFinished { get; set; }
            public bool ImagesLoaded { get; set; }
            public ComparisonResult ComparisonResult { get; set; }
        }

        private static readonly Color DifferenceColor =
            Color.LightCoral;
        private static readonly Color DuplicateColor =
            Color.DarkSeaGreen;
        private static readonly Color NeutralColor =
            SystemColors.Control;
        private static readonly Color OddRowBackColor =
            SystemColors.ControlDark;
        private static readonly Color EvenRowBackColor =
            SystemColors.ControlDarkDark;

        public Buttons CloseButtons { get; set; } = Buttons.OkCancel;

        public Wcf.Contracts.Data.ConfigData ServerConfig { get; set; }

        public string LeftFilePath
        {
            get => TxtLeftFilePath.Text;
            set => TxtLeftFilePath.Text = value;
        }
        public string RightFilePath
        {
            get => TxtRightFilePath.Text;
            set => TxtRightFilePath.Text = value;
        }

        private Guid? ComparisonToken { get; set; } = null;
        private int ImageComparisonIndex { get; set; } = 0;
        private int? FinishedInLoadLevel { get; set; }

        private List<ImageComparisonResult> ImageComparisons { get; set; }
            = new List<ImageComparisonResult>();
        private VideoComparisonResult VideoComparisonResult { get; set; }

        public VideoComparisonPreviewDlg() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            NumMaxDifferentPercentage.Value =
                ServerConfig.MaxImageDifferencePercent;
            NumMaxDifferentImages.Value = ServerConfig.MaxDifferentImages;
            NumMaxImageComparison.Value = ServerConfig.MaxImageCompares;

            // Set minimum size for ImageGroupBoxs
            // so that we at least see the header when collapsed.
            GrbFirstLevelLoad.MinimumSize =
                GrbFirstLevelLoad.HeaderRectangle.Size;
            GrbSecondLevelLoad.MinimumSize =
                GrbSecondLevelLoad.HeaderRectangle.Size;
            GrbThirdLevelLoad.MinimumSize =
                GrbThirdLevelLoad.HeaderRectangle.Size;

            CleanUpResult();

            if (!string.IsNullOrWhiteSpace(LeftFilePath)
                && !string.IsNullOrWhiteSpace(RightFilePath))
            {
                BtnStartComparison.PerformClick();
            }

            UpdateCloseButtons();

            RdbSortByLoadLevel.Checked = true;

            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (ComparisonToken.HasValue)
            {
                StatusTimer.Stop();
                VideoDedupDlg.WcfProxy.CancelCustomVideoComparison(
                    ComparisonToken.Value);
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
            using (var dlg = new CommonOpenFileDialog())
            {
                if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                LeftFilePath = dlg.FileName;
            }
        }

        private void BtnSelectRightFilePath_Click(object sender, EventArgs e)
        {
            using (var dlg = new CommonOpenFileDialog())
            {
                if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                RightFilePath = dlg.FileName;
            }
        }

        private void BtnOkay_Click(object sender, EventArgs e)
        {
            ServerConfig.MaxImageCompares = (int)NumMaxImageComparison.Value;
            ServerConfig.MaxImageDifferencePercent =
                (int)NumMaxDifferentPercentage.Value;
            ServerConfig.MaxDifferentImages = (int)NumMaxDifferentImages.Value;

            DialogResult = DialogResult.OK;
        }

        private void BtnStartComparison_Click(object sender, EventArgs e)
        {
            if (ComparisonToken.HasValue)
            {
                StatusTimer.Stop();
                VideoDedupDlg.WcfProxy.CancelCustomVideoComparison(
                    ComparisonToken.Value);
            }

            CleanUpResult();

            if ((int)NumMaxImageComparison.Value <= 0)
            {
                return;
            }

            PgbComparisonProgress.Visible = true;
            PnlResult.Visible = true;

            var startData = VideoDedupDlg.WcfProxy.StartCustomVideoComparison(
                 new CustomVideoComparisonData
                 {
                     AlwaysLoadAllImages = true,
                     LeftFilePath = LeftFilePath,
                     RightFilePath = RightFilePath,
                     MaxImageCompares = (int)NumMaxImageComparison.Value,
                     MaxImageDifferencePercent =
                         (int)NumMaxDifferentPercentage.Value,
                     MaxDifferentImages = (int)NumMaxDifferentImages.Value,
                 });

            if (startData.ComparisonToken == null)
            {
                _ = MessageBox.Show(
                    "Couldn't start comparison." + Environment.NewLine
                    + startData.ErrorMessage,
                    "Erro starting comparison",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            ComparisonToken = startData.ComparisonToken;

            StatusTimer.Start();
        }

        private void HandleStatusTimerTick(object sender, EventArgs e)
        {
            StatusTimer.Stop();
            var status = VideoDedupDlg.WcfProxy.GetVideoComparisonStatus(
                    ComparisonToken.Value,
                    ImageComparisonIndex);

            if (VideoComparisonResult != status.VideoCompareResult)
            {
                VideoComparisonResult =
                    status.VideoCompareResult ?? VideoComparisonResult;
                UpdateVideoComparisonResult(VideoComparisonResult);
            }

            if (!status.ImageComparisons.Any())
            {
                StatusTimer.Start();
                return;
            }
            else
            {
                ImageComparisons.AddRange(status.ImageComparisons);
                UpdateResultDisplay();
            }

            ImageComparisonIndex = status.ImageComparisons
                .Max(kvp => kvp.Index);
            ImageComparisonIndex += 1;
            var maxImages = (int)NumMaxImageComparison.Value;
            if (ImageComparisonIndex >= maxImages)
            {
                VideoDedupDlg.WcfProxy.CancelCustomVideoComparison(
                    ComparisonToken.Value);
                PgbComparisonProgress.Visible = false;
                return;
            }

            StatusTimer.Start();
        }

        private void UpdateResultDisplay()
        {
            if (!ImageComparisons.Any())
            {
                return;
            }

            PnlResult.Visible = true;

            if (RdbSortByLoadLevel.Checked)
            {
                foreach (var loadLevel in ImageComparisons
                    .GroupBy(kvp => kvp.LoadLevel))
                {
                    var (grb, tlp) = GetLoadLevelControls(loadLevel.Key);
                    grb.Visible = true;

                    AddImageComparisonsToTableLayoutPanel(tlp, loadLevel);
                }
            }
            else
            {
                GrbVideoTimeline.Visible = true;

                // Get image comparisons in order of timeline
                var images = ImageComparisons.OrderBy(icr =>
                        icr.LeftImages.Index.Numerator
                        / (double)icr.LeftImages.Index.Denominator);

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
                    ?.LoadLevel;
            }

            ImageComparisonResultView toView(ImageComparisonResult icr) =>
                new ImageComparisonResultView
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
                    DifferenceColor = DifferenceColor,
                    DuplicateColor = DuplicateColor,
                    NeutralColor = NeutralColor,
                };


            var resultViews = tableLayoutPanel.Controls
                .Cast<ImageComparisonResultView>();

            // Get all icrs that are missing.
            // Add them.
            // Then iterate over all with index to set ALL the new row indexes.
            tableLayoutPanel.Controls.AddRange(imageComparisonResults
                .Where(icr => !resultViews.Any(view => view.ImageComparisonResult == icr))
                .Select(icr => toView(icr))
                .ToArray());

            var indexed = Enumerable
                .Range(0, imageComparisonResults.Count())
                .Zip(imageComparisonResults, (rowIndex, icr) => new
                {
                    RowIndex = rowIndex,
                    Result = icr,
                });

            foreach (ImageComparisonResultView view in tableLayoutPanel.Controls)
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

        private Dictionary<int, Tuple<ImageGroupBox, TableLayoutPanel>>
            loadLevelControls;
        private Tuple<ImageGroupBox, TableLayoutPanel> GetLoadLevelControls(
            int loadLevel)
        {
            if (loadLevelControls == null)
            {
                loadLevelControls =
                    new Dictionary<int, Tuple<ImageGroupBox, TableLayoutPanel>>
                    {
                        { 1, Tuple.Create(GrbFirstLevelLoad, TlpFirstLevelLoad) },
                        { 2, Tuple.Create(GrbSecondLevelLoad, TlpSecondLevelLoad) },
                        { 3, Tuple.Create(GrbThirdLevelLoad, TlpThirdLevelLoad) },
                    };
            }
            return loadLevelControls[loadLevel];
        }

        private void UpdateVideoComparisonResult(VideoComparisonResult result)
        {
            if (result != null && !LblResult.Visible)
            {
                PnlResult.Visible = true;
                LblResult.Visible = true;
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
                    LblResult.BackColor = NeutralColor;
                }
                else if (result.ComparisonResult == ComparisonResult.Aborted)
                {
                    LblResult.Text = "Aborted: " + result.Reason;
                    LblResult.BackColor = NeutralColor;
                }
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
            ImageComparisons.Clear();

            ImageComparisonIndex = 0;
            FinishedInLoadLevel = null;

            PnlResult.Visible = false;

            LblResult.Visible = false;
            PgbComparisonProgress.Visible = false;

            void clearTableLayoutPanel(TableLayoutPanel tlp)
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

            clearTableLayoutPanel(TlpFirstLevelLoad);
            GrbFirstLevelLoad.Visible = false;

            clearTableLayoutPanel(TlpSecondLevelLoad);
            GrbSecondLevelLoad.Visible = false;

            clearTableLayoutPanel(TlpThirdLevelLoad);
            GrbThirdLevelLoad.Visible = false;

            clearTableLayoutPanel(TlpVideoTimeline);
            GrbVideoTimeline.Visible = false;
        }

        private void GrbFirstLevelLoad_HeaderClicked(
            object sender,
            MouseEventArgs e)
        {
            if (TlpFirstLevelLoad.Visible)
            {
                TlpFirstLevelLoad.Visible = false;
                GrbFirstLevelLoad.Icon = Properties.Resources.ArrowDownBlue;
            }
            else
            {
                TlpFirstLevelLoad.Visible = true;
                GrbFirstLevelLoad.Icon = Properties.Resources.ArrowUpGray;
            }
        }

        private void GrbSecondLevelLoad_HeaderClicked(
            object sender,
            MouseEventArgs e)
        {
            if (TlpSecondLevelLoad.Visible)
            {
                TlpSecondLevelLoad.Visible = false;
                GrbSecondLevelLoad.Icon = Properties.Resources.ArrowDownBlue;
            }
            else
            {
                TlpSecondLevelLoad.Visible = true;
                GrbSecondLevelLoad.Icon = Properties.Resources.ArrowUpGray;
            }
        }

        private void GrbThirdLevelLoad_HeaderClicked(
            object sender,
            MouseEventArgs e)
        {
            if (TlpThirdLevelLoad.Visible)
            {
                TlpThirdLevelLoad.Visible = false;
                GrbThirdLevelLoad.Icon = Properties.Resources.ArrowDownBlue;
            }
            else
            {
                TlpThirdLevelLoad.Visible = true;
                GrbThirdLevelLoad.Icon = Properties.Resources.ArrowUpGray;
            }
        }
    }
}
