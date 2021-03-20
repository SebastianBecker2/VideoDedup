namespace VideoDedup
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using VideoDedupShared;
    using VideoDedupShared.ImageExtension;
    using Wcf.Contracts.Data;

    public partial class VideoComparisonPreviewDlg : Form
    {
        private struct ImageComparison
        {
            public Image LeftImage { get; set; }
            public double Difference { get; set; }
            public Image RightImage { get; set; }
            public bool ComparisonFinished { get; set; }
            public bool ImagesLoaded { get; set; }
            public ComparisonResult ComparisonResult { get; set; }
        }

        private static readonly Size ThumbnailSize = new Size(256, 256);
        private static readonly Color DifferenceColor =
            Color.FromKnownColor(KnownColor.LightCoral);
        private static readonly Color DuplicateColor =
            Color.FromKnownColor(KnownColor.DarkSeaGreen);
        private static readonly Color DefaultColor =
            Color.FromKnownColor(KnownColor.Control);

        public Wcf.Contracts.Data.ConfigData ServerConfig { get; set; }

        private Guid? ComparisonToken { get; set; } = null;
        private int ImageComparisonIndex { get; set; } = 0;
        private int? FinishedInLoadLevel { get; set; }

        public VideoComparisonPreviewDlg() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            NumMaxDifferentPercentage.Value = ServerConfig.MaxImageDifferencePercent;
            NumMaxDifferentImages.Value = ServerConfig.MaxDifferentImages;
            NumMaxImageComparison.Value = ServerConfig.MaxImageCompares;

            CleanUpResult();

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

            var startData = VideoDedupDlg.WcfProxy.StartCustomVideoComparison(
                 new CustomVideoComparisonData
                 {
                     AlwaysLoadAllImages = true,
                     LeftFilePath = TxtLeftFilePath.Text,
                     RightFilePath = TxtRightFilePath.Text,
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

        private void BtnOkay_Click(object sender, EventArgs e)
        {
            ServerConfig.MaxImageCompares = (int)NumMaxImageComparison.Value;
            ServerConfig.MaxImageDifferencePercent =
                (int)NumMaxDifferentPercentage.Value;
            ServerConfig.MaxDifferentImages = (int)NumMaxDifferentImages.Value;

            DialogResult = DialogResult.OK;
        }

        private void BtnSelectLeftFilePath_Click(object sender, EventArgs e)
        {
            using (var dlg = new CommonOpenFileDialog())
            {
                if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                TxtLeftFilePath.Text = dlg.FileName;
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

                TxtRightFilePath.Text = dlg.FileName;
            }
        }

        private void HandleStatusTimerTick(object sender, EventArgs e)
        {
            StatusTimer.Stop();
            var status = VideoDedupDlg.WcfProxy.GetVideoComparisonStatus(
                    ComparisonToken.Value,
                    ImageComparisonIndex);

            UpdateVideoComparisonResult(status.VideoCompareResult);
            UpdateImageComparisonResult(status);

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
                return;
            }

            StatusTimer.Start();
        }

        private void CleanUpResult()
        {
            ImageComparisonIndex = 0;
            FinishedInLoadLevel = null;

            PnlResult.Visible = false;

            LblResult.Visible = false;

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

        private static Size GetThumbnailSize(CodecInfo codecInfo)
        {
            if (codecInfo == null)
            {
                return ThumbnailSize;
            }
            return GetThumbnailSize(codecInfo.Size);
        }

        private static Size GetThumbnailSize(Size originalSize)
        {
            var width = originalSize.Width;
            var height = originalSize.Height;

            if (width > height)
            {
                height = (int)(height / ((double)width / ThumbnailSize.Width));
                return new Size(ThumbnailSize.Width, height);
            }
            else
            {
                width = (int)(width / ((double)height / ThumbnailSize.Height));
                return new Size(width, ThumbnailSize.Height);
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

        private string GetTextFromImageComparison(
            int index,
            ImageComparisonResult imageComparison,
            bool finished,
            bool loaded)
        {
            var text = $"{index + 1}. Comparison:{Environment.NewLine}";
            if (!finished)
            {
                text += $"Images loaded and compared{Environment.NewLine}"
                    + $"Difference {imageComparison.Difference * 100} is ";
                if (imageComparison.ComparisonResult
                    == ComparisonResult.Different)
                {
                    text += "higher";
                }
                else
                {
                    text += "lower";
                }
                text += $" than {NumMaxDifferentPercentage.Value}";
            }
            else
            {
                text += $"Result already determined.{Environment.NewLine}";
                if (loaded)
                {
                    text += "Images loaded but comparison was skipped.";
                }
                else
                {
                    text += "Images have not been loaded.";
                }
            }
            return text;
        }

        private static Color GetColorFromImageComparison(
            ImageComparisonResult imageComparison,
            bool finished)
        {
            if (!finished)
            {
                if (imageComparison.ComparisonResult
                    == ComparisonResult.Different)
                {
                    return DifferenceColor;
                }
                else
                {
                    return DuplicateColor;
                }
            }
            return DefaultBackColor;
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
                    LblResult.BackColor = DefaultColor;
                }
                else if (result.ComparisonResult == ComparisonResult.Aborted)
                {
                    LblResult.Text = "Aborted: " + result.Reason;
                    LblResult.BackColor = DefaultColor;
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

            var leftThumbnailSize = GetThumbnailSize(
                status.LeftVideoFile.CodecInfo);
            var rightThumbnailSize = GetThumbnailSize(
                status.RightVideoFile.CodecInfo);

            PnlResult.Visible = true;

            Image StreamToImage(MemoryStream stream)
            {
                if (stream != null)
                {
                    return Image.FromStream(stream);
                }
                return new Bitmap(1, 1);
            }

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

                tlp.Controls.AddRange(loadLevel.SelectMany(kvp =>
                {
                    var leftPib = new PictureBox
                    {
                        Image = StreamToImage(kvp.Item2.LeftImage)
                            .Resize(leftThumbnailSize),
                        SizeMode = PictureBoxSizeMode.AutoSize,
                        Anchor = AnchorStyles.None,
                    };

                    var finished = lastCompared != null
                        && kvp.Item1 > lastCompared;
                    var text = GetTextFromImageComparison(
                        kvp.Item1,
                        kvp.Item2,
                        finished,
                        loaded);
                    var backColor = GetColorFromImageComparison(
                        kvp.Item2,
                        finished);
                    var result = new Label
                    {
                        Text = text,
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = backColor,
                    };

                    var rightPib = new PictureBox
                    {
                        Image = StreamToImage(kvp.Item2.RightImage)
                            .Resize(rightThumbnailSize),
                        SizeMode = PictureBoxSizeMode.AutoSize,
                        Anchor = AnchorStyles.None,
                    };
                    return new Control[] { leftPib, result, rightPib };
                }).ToArray());
            }
        }
    }
}
