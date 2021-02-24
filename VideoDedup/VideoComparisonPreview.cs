namespace VideoDedup
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using DedupEngine;
    using DedupEngine.MpvLib;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using VideoDedupShared.ImageExtension;
    using XnaFan.ImageComparison;

    public partial class VideoComparisonPreview : Form
    {
        private struct ImageComparison
        {
            public Image LeftImage { get; set; }
            public double Difference { get; set; }
            public Image RightImage { get; set; }
            public bool ComparisonFinished { get; set; }
            public bool ImagesLoaded { get; set; }
            public bool ImagesDifferent { get; set; }
        }

        private static readonly Size ThumbnailSize = new Size(256, 256);
        private static readonly Color DifferenceColor =
            Color.FromKnownColor(KnownColor.LightCoral);
        private static readonly Color DuplicateColor =
            Color.FromKnownColor(KnownColor.DarkSeaGreen);

        public Wcf.Contracts.Data.ConfigData ServerConfig { get; set; }

        public VideoComparisonPreview() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            NumMaxDifferentPercentage.Value = ServerConfig.MaxImageDifferencePercent;
            NumMaxDifferentImages.Value = ServerConfig.MaxDifferentImages;
            NumMaxImageComparison.Value = ServerConfig.MaxImageCompares;

            base.OnLoad(e);
        }

        private Wcf.Contracts.Data.ConfigData GetConfigDataFromDialog() =>
            new Wcf.Contracts.Data.ConfigData
            {
                MaxImageCompares = (int)NumMaxImageComparison.Value,
                MaxImageDifferencePercent = (int)NumMaxDifferentPercentage.Value,
                MaxDifferentImages = (int)NumMaxDifferentImages.Value,
            };

        private Size GetThumbnailSize(Size originalSize)
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
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var leftPictureBox = new PictureBox
            {
                Image = imageComparison.LeftImage,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Anchor = AnchorStyles.None,
            };
            tableLayoutPanel.Controls.Add(leftPictureBox, 0, index);

            var text = $"{index + 1}. Comparison:{Environment.NewLine}";
            if (imageComparison.ComparisonFinished)
            {
                text += $"Images loaded and compares{Environment.NewLine}"
                    + $"Difference {imageComparison.Difference * 100} is ";
                if (imageComparison.ImagesDifferent)
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
            if (imageComparison.ComparisonFinished)
            {
                if (imageComparison.ImagesDifferent)
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

        private void HandleSettingChange(object sender, EventArgs e)
        {
            CleanUpPreview();

            var leftFilePath = TxtLeftFilePath.Text;
            if (!File.Exists(leftFilePath))
            {
                // Display file path not valid
                return;
            }
            var rightFilePath = TxtRightFilePath.Text;
            if (!File.Exists(rightFilePath))
            {
                // Display file path not valid;
                return;
            }

            var config = GetConfigDataFromDialog();

            if (config.MaxImageCompares <= 0)
            {
                return;
            }

            GrbResult.Visible = true;

            var leftFile = new VideoFile(leftFilePath);
            var leftThumbnailSize = GetThumbnailSize(leftFile.CodecInfo.Size);
            var rightFile = new VideoFile(rightFilePath);
            var rightThumbnailSize = GetThumbnailSize(rightFile.CodecInfo.Size);

            var differenceCount = 0;
            var currentTlp = TlpFirstLevelLoad;
            var currentGrb = GrbFirstLevelLoad;
            var comparisonFinished = false;
            var imagesLoaded = true;
            foreach (var index in Enumerable.Range(0, config.MaxImageCompares))
            {
                var leftImage = Image.FromStream(
                    leftFile.GetImage(index, config.MaxImageCompares, config));
                var rightImage = Image.FromStream(
                    rightFile.GetImage(index, config.MaxImageCompares, config));

                var diff = leftImage.PercentageDifference(rightImage);
                leftImage = leftImage.Resize(leftThumbnailSize);
                rightImage = rightImage.Resize(rightThumbnailSize);

                var areDifferent =
                    diff > (double)config.MaxImageDifferencePercent / 100;

                if (index == config.MaxDifferentImages + 1)
                {
                    currentTlp = TlpSecondLevelLoad;
                    currentGrb = GrbSecondLevelLoad;
                }
                if (index == config.MaxImageCompares - config.MaxDifferentImages)
                {
                    currentTlp = TlpThirdLevelLoad;
                    currentGrb = GrbThirdLevelLoad;
                }

                if (index == config.MaxDifferentImages + 1)
                {
                    imagesLoaded = !comparisonFinished;
                }
                else if (index ==
                    config.MaxImageCompares - config.MaxDifferentImages)
                {
                    imagesLoaded = !comparisonFinished;
                }

                AddImageComparison(
                    currentTlp,
                    index,
                    new ImageComparison
                    {
                        LeftImage = leftImage,
                        Difference = diff,
                        RightImage = rightImage,
                        ComparisonFinished = !comparisonFinished,
                        ImagesDifferent = areDifferent,
                        ImagesLoaded = imagesLoaded,
                    },
                    config.MaxImageDifferencePercent);

                currentGrb.Visible = true;

                if (areDifferent)
                {
                    ++differenceCount;
                }

                // Early finish when we already exceeded the number of
                // MaxDifferentImages
                comparisonFinished =
                    comparisonFinished
                    || differenceCount > config.MaxDifferentImages;

                // Early return when there are not enough to compare left
                // to exceed the MaxDifferentImages
                comparisonFinished =
                    comparisonFinished
                    || (config.MaxImageCompares - (index + 1))
                    <= (config.MaxDifferentImages - differenceCount);
            }

            if (differenceCount > config.MaxDifferentImages)
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
