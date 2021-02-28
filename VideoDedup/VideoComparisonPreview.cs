namespace VideoDedup
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using VideoDedupShared;
    using VideoDedupShared.ImageExtension;

    public partial class VideoComparisonPreview : Form
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

        public Wcf.Contracts.Data.ConfigData ServerConfig { get; set; }

        private Guid? ComparisonToken { get; set; } = null;
        private int ImageComparisonIndex { get; set; } = 0;
        private TableLayoutPanel CurrentTableLayoutPanel { get; set; }

        public VideoComparisonPreview() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            NumMaxDifferentPercentage.Value = ServerConfig.MaxImageDifferencePercent;
            NumMaxDifferentImages.Value = ServerConfig.MaxDifferentImages;
            NumMaxImageComparison.Value = ServerConfig.MaxImageCompares;

            base.OnLoad(e);
        }

        private void HandleStatusTimerTick(object sender, EventArgs e)
        {
            var status = VideoDedupDlg.WcfProxy.GetVideoComparisonStatus(
                ComparisonToken.Value,
                ImageComparisonIndex);

            GrbResult.Visible = true;

            if (status.VideoCompareResult != null)
            {
                if (status.VideoCompareResult.ComparisonResult
                    == ComparisonResult.Different)
                {
                    LblResult.Text = "Videos are considered to be different.";
                    LblResult.BackColor = DifferenceColor;
                }
                else
                {
                    LblResult.Text = "Videos are considered to be duplicates.";
                    LblResult.BackColor = DuplicateColor;
                }
            }

            var leftThumbnailSize = GetThumbnailSize(
                status.LeftVideoFile.CodecInfo.Size);
            var rightThumbnailSize = GetThumbnailSize(
                status.RightVideoFile.CodecInfo.Size);

            var imagesLoaded = true;
            foreach (var (index, imageComparison) in status.ImageComparisons)
            {
                var comparisonFinished = status.VideoCompareResult != null
                    && index > status.VideoCompareResult.LastComparisonIndex;

                if (imageComparison.ImageLoadLevel == 2)
                {
                    if (CurrentTableLayoutPanel != TlpSecondLevelLoad
                        && comparisonFinished)
                    {
                        imagesLoaded = false;
                    }
                    CurrentTableLayoutPanel = TlpSecondLevelLoad;
                }
                if (imageComparison.ImageLoadLevel == 3)
                {
                    if (CurrentTableLayoutPanel != TlpThirdLevelLoad
                        && comparisonFinished)
                    {
                        imagesLoaded = false;
                    }
                    CurrentTableLayoutPanel = TlpThirdLevelLoad;
                }

                var comparison = new ImageComparison
                {
                    ComparisonFinished = comparisonFinished,
                    Difference = imageComparison.Difference,
                    ComparisonResult = imageComparison.ComparisonResult,
                    ImagesLoaded = imagesLoaded,
                    LeftImage = Image.FromStream(imageComparison.LeftImage)
                        .Resize(leftThumbnailSize),
                    RightImage = Image.FromStream(imageComparison.RightImage)
                        .Resize(rightThumbnailSize),
                };
                var maxDifference = (int)NumMaxDifferentPercentage.Value;
                AddImageComparison(
                    CurrentTableLayoutPanel,
                    index,
                    comparison,
                    maxDifference);

                var maxImages = (int)NumMaxImageComparison.Value;
                if (index == maxImages - 1)
                {
                    StatusTimer.Stop();
                }
                ImageComparisonIndex = index + 1;
            }
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

        private void CleanUpPreview()
        {
            ImageComparisonIndex = 0;
            CurrentTableLayoutPanel = TlpFirstLevelLoad;

            GrbResult.Visible = false;

            var localRef = TlpFirstLevelLoad.Controls;
            TlpFirstLevelLoad.Controls.Clear();
            foreach (Control control in localRef)
            {
                control.Dispose();
            }

            TlpFirstLevelLoad.RowStyles.Clear();
            TlpFirstLevelLoad.RowCount = 0;

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

        private static void AddImageComparison(
            TableLayoutPanel tableLayoutPanel,
            int index,
            ImageComparison imageComparison,
            int maxDifference)
        {
            tableLayoutPanel.Parent.Visible = true;
            _ = tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var leftPictureBox = new PictureBox
            {
                Image = imageComparison.LeftImage,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Anchor = AnchorStyles.None,
            };
            tableLayoutPanel.Controls.Add(leftPictureBox, 0, index);

            var text = $"{index + 1}. Comparison:{Environment.NewLine}";
            if (!imageComparison.ComparisonFinished)
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
                text += $" than {maxDifference}";
            }
            else
            {
                text += $"Result already determined.{Environment.NewLine}";
                if (imageComparison.ImagesLoaded)
                {
                    text += "Images loaded but comparison was skipped.";
                }
                else
                {
                    text += "Images have not been loaded.";
                }
            }
            var diffLabel = new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
            };
            if (!imageComparison.ComparisonFinished)
            {
                if (imageComparison.ComparisonResult
                    == ComparisonResult.Different)
                {
                    diffLabel.BackColor = DifferenceColor;
                }
                else
                {
                    diffLabel.BackColor = DuplicateColor;
                }
            }
            tableLayoutPanel.Controls.Add(diffLabel, 1, index);

            var rightPictureBox = new PictureBox
            {
                Image = imageComparison.RightImage,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Anchor = AnchorStyles.None,
            };
            tableLayoutPanel.Controls.Add(rightPictureBox, 2, index);
        }

        private void BtnStartComparison_Click(object sender, EventArgs e)
        {
            if (ComparisonToken.HasValue)
            {
                StatusTimer.Stop();
                VideoDedupDlg.WcfProxy.CancelCustomVideoComparison(
                    ComparisonToken.Value);

            }

            CleanUpPreview();

            if ((int)NumMaxImageComparison.Value <= 0)
            {
                return;
            }

            var startData = VideoDedupDlg.WcfProxy.StartCustomVideoComparison(
                 new Wcf.Contracts.Data.CustomVideoComparisonData
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
    }
}
