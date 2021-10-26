namespace VideoDedup
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using Microsoft.WindowsAPICodePack.Dialogs;
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

        private List<Tuple<int, ImageComparisonResult>> ImageComparisons { get; set; }
            = new List<Tuple<int, ImageComparisonResult>>();
        private VideoComparisonResult VideoComparisonResult { get; set; }

        public VideoComparisonPreviewDlg() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            NumMaxDifferentPercentage.Value =
                ServerConfig.MaxImageDifferencePercent;
            NumMaxDifferentImages.Value = ServerConfig.MaxDifferentImages;
            NumMaxImageComparison.Value = ServerConfig.MaxImageCompares;

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

            // We store the status data
            ImageComparisons.AddRange(status.ImageComparisons);
            VideoComparisonResult = status.VideoCompareResult ?? VideoComparisonResult;

            // Instead of updateting with the new status data, we need to
            // update the view with the stored status data.
            // But can we do it properly and fast?
            //UpdateVideoComparisonResult(status.VideoCompareResult);
            //UpdateImageComparisonResult(status);

            if (!status.ImageComparisons.Any())
            {
                StatusTimer.Start();
                return;
            }

            ImageComparisonIndex = status.ImageComparisons
                .Max(kvp => kvp.Item1);
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
                    .GroupBy(kvp => kvp.Item2.ImageLoadLevel))
                {
                    var (grb, tlp) = GetLoadLevelControls(loadLevel.Key);
                    grb.Visible = true;

                    var lastCompared = VideoComparisonResult?.LastComparisonIndex;
                    if (lastCompared != null
                        && loadLevel.Any(k => k.Item1 == lastCompared.Value))
                    {
                        FinishedInLoadLevel = loadLevel.Key;
                    }
                    var loaded = FinishedInLoadLevel == null
                        || loadLevel.Key <= FinishedInLoadLevel;

                    tlp.Controls.AddRange(loadLevel.Select(kvp =>
                        new ImageComparisonResultView
                        {
                            ImageComparisonIndex = kvp.Item1,
                            ImageComparisonResult = kvp.Item2,
                            ComparisonFinished = lastCompared != null
                                    && kvp.Item1 > lastCompared,
                            ImageLoaded = loaded,
                            MaximumDifferencePercentage =
                                    (int)NumMaxDifferentPercentage.Value,
                            Dock = DockStyle.Fill,
                            BackColor = GetAlternatingBackColor(),
                            DifferenceColor = DifferenceColor,
                            DuplicateColor = DuplicateColor,
                            NeutralColor = NeutralColor,
                        }).ToArray());
                }
            }
            else
            {
                var grp = GrbVideoTimeline;
                var tlp = TlpVideoTimeline;


                foreach (var loadLevel in ImageComparisons
                                    .GroupBy(kvp => kvp.Item2.ImageLoadLevel))
                {
                    var (grb, tlp) = GetLoadLevelControls(loadLevel.Key);
                    grb.Visible = true;

                    var lastCompared = VideoComparisonResult?.LastComparisonIndex;
                    if (lastCompared != null
                        && loadLevel.Any(k => k.Item1 == lastCompared.Value))
                    {
                        FinishedInLoadLevel = loadLevel.Key;
                    }
                    var loaded = FinishedInLoadLevel == null
                        || loadLevel.Key <= FinishedInLoadLevel;

                    tlp.Controls.AddRange(loadLevel.Select(kvp =>
                        new ImageComparisonResultView
                        {
                            ImageComparisonIndex = kvp.Item1,
                            ImageComparisonResult = kvp.Item2,
                            ComparisonFinished = lastCompared != null
                                    && kvp.Item1 > lastCompared,
                            ImageLoaded = loaded,
                            MaximumDifferencePercentage =
                                    (int)NumMaxDifferentPercentage.Value,
                            Dock = DockStyle.Fill,
                            BackColor = GetAlternatingBackColor(),
                            DifferenceColor = DifferenceColor,
                            DuplicateColor = DuplicateColor,
                            NeutralColor = NeutralColor,
                        }).ToArray());
                }
            }
        }

        private Dictionary<int, Tuple<GroupBox, TableLayoutPanel>> loadLevelControls;
        private Tuple<GroupBox, TableLayoutPanel> GetLoadLevelControls(
            int loadLevel)
        {
            if (loadLevelControls == null)
            {
                loadLevelControls =
                    new Dictionary<int, Tuple<GroupBox, TableLayoutPanel>>
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

        private void UpdateImageComparisonResult(
            CustomVideoComparisonStatusData status)
        {
            if (!status.ImageComparisons.Any())
            {
                return;
            }

            PnlResult.Visible = true;

            foreach (var loadLevel in status.ImageComparisons
                .GroupBy(kvp => kvp.Item2.ImageLoadLevel))
            {
                var (grb, tlp) = GetLoadLevelControls(loadLevel.Key);
                grb.Visible = true;

                var lastCompared = status.VideoCompareResult?.LastComparisonIndex;
                if (lastCompared != null
                    && loadLevel.Any(k => k.Item1 == lastCompared.Value))
                {
                    FinishedInLoadLevel = loadLevel.Key;
                }
                var loaded = FinishedInLoadLevel == null
                    || loadLevel.Key <= FinishedInLoadLevel;

                tlp.Controls.AddRange(loadLevel.Select(kvp =>
                    new ImageComparisonResultView
                    {
                        ImageComparisonIndex = kvp.Item1,
                        ImageComparisonResult = kvp.Item2,
                        ComparisonFinished = lastCompared != null
                                && kvp.Item1 > lastCompared,
                        ImageLoaded = loaded,
                        MaximumDifferencePercentage =
                                (int)NumMaxDifferentPercentage.Value,
                        Dock = DockStyle.Fill,
                        BackColor = GetAlternatingBackColor(),
                        DifferenceColor = DifferenceColor,
                        DuplicateColor = DuplicateColor,
                        NeutralColor = NeutralColor,
                    }).ToArray());
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
            ImageComparisonIndex = 0;
            FinishedInLoadLevel = null;

            PnlResult.Visible = false;

            LblResult.Visible = false;
            PgbComparisonProgress.Visible = false;

            var localRef = TlpFirstLevelLoad.Controls;
            TlpFirstLevelLoad.Controls.Clear();
            foreach (Control control in localRef)
            {
                control.Dispose();
            }

            TlpFirstLevelLoad.RowStyles.Clear();
            TlpFirstLevelLoad.RowCount = 0;

            GrbFirstLevelLoad.Visible = false;

            localRef = TlpSecondLevelLoad.Controls;
            TlpSecondLevelLoad.Controls.Clear();
            foreach (Control control in localRef)
            {
                control.Dispose();
            }

            TlpSecondLevelLoad.RowStyles.Clear();
            TlpSecondLevelLoad.RowCount = 0;

            GrbSecondLevelLoad.Visible = false;

            localRef = TlpThirdLevelLoad.Controls;
            TlpThirdLevelLoad.Controls.Clear();
            foreach (Control control in localRef)
            {
                control.Dispose();
            }

            TlpThirdLevelLoad.RowStyles.Clear();
            TlpThirdLevelLoad.RowCount = 0;

            GrbThirdLevelLoad.Visible = false;
        }

    }
}
