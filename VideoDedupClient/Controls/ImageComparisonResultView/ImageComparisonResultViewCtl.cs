namespace VideoDedupClient.Controls.ImageComparisonResultView
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using Properties;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.TimeSpanExtensions;

    public partial class ImageComparisonResultViewCtl : UserControl
    {
        public static readonly System.Drawing.Size ThumbnailSize = new(256, 256);

        private enum DetailLevel
        {
            Crop,
            Resize,
            Greyscale,
        }

        private Dictionary<DetailLevel, Tuple<string, Image>> DetailInfo { get; } =
            new()
            {
                {
                    DetailLevel.Crop,
                    new Tuple<string, Image>(
                        $"Preparation step 1{Environment.NewLine}" +
                        "Cut off black bars",
                        Resources.PictureCropped)
                },
                {
                    DetailLevel.Resize,
                    new Tuple<string, Image>(
                        $"Preparation step 2{Environment.NewLine}" +
                        "Resize to 16 x 16 pixel",
                        Resources.PictureSizeDown)
                },
                {
                    DetailLevel.Greyscale,
                    new Tuple<string, Image>(
                        $"Preparation step 3{Environment.NewLine}" +
                        "Convert to greyscale",
                        Resources.PictureGreyscale)
                },
            };

        public int ImageComparisonIndex { get; set; }
        public ImageComparisonResult? ImageComparisonResult { get; set; }
        public bool ComparisonAlreadyFinished { get; set; }
        public bool ImageLoaded { get; set; }
        public int MaximumDifferencePercentage { get; set; }
        public TimeSpan LeftTimestamp { get; set; }
        public TimeSpan RightTimestamp { get; set; }

        public Color DifferentColor { get; set; }
        public Color DuplicateColor { get; set; }
        public Color LoadedColor { get; set; }
        public Color NotLoadedColor { get; set; }

        private Label ShowDetailsLabel { get; }
        private PictureBox LeftShowDetailArrow { get; }
        private PictureBox RightShowDetailArrow { get; }
        private TableLayoutPanel TlpDetails { get; }

        public ImageComparisonResultViewCtl()
        {
            InitializeComponent();

            ShowDetailsLabel = new Label
            {
                Text = "Show Details",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Tag = false,
                Margin = new Padding { All = 0 },
            };
            ShowDetailsLabel.Click += HandleShowDetailsClickEvent;
            Controls.Add(ShowDetailsLabel);

            LeftShowDetailArrow = new PictureBox
            {
                Image = Resources.ArrowDownBlue,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Anchor = AnchorStyles.Right,
                Margin = new Padding { All = 0 },
            };
            LeftShowDetailArrow.Click += HandleShowDetailsClickEvent;
            Controls.Add(LeftShowDetailArrow);

            RightShowDetailArrow = new PictureBox
            {
                Image = Resources.ArrowDownBlue,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Anchor = AnchorStyles.Left,
                Margin = new Padding { All = 0 },
            };
            RightShowDetailArrow.Click += HandleShowDetailsClickEvent;
            Controls.Add(RightShowDetailArrow);

            TlpDetails = new TableLayoutPanel
            {
                ColumnCount = 3,
                Visible = false,
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding { All = 0 },
            };
            _ = TlpDetails.ColumnStyles.Add(new ColumnStyle
            {
                SizeType = SizeType.Percent,
                Width = 100,
            });
            _ = TlpDetails.ColumnStyles.Add(new ColumnStyle
            {
                SizeType = SizeType.AutoSize,
            });
            _ = TlpDetails.ColumnStyles.Add(new ColumnStyle
            {
                SizeType = SizeType.Percent,
                Width = 100,
            });
            TlpImageComparison.Controls.Add(TlpDetails, 0, 1);
            TlpImageComparison.SetColumnSpan(TlpDetails, 3);
        }

        protected override void OnLoad(EventArgs e)
        {
            TlpImageComparison.SuspendLayout();
            try
            {
                if (ImageComparisonResult is null)
                {
                    return;
                }

                // Main View
                TlpImageComparison.Controls.Add(
                    GetPictureBox(ImageComparisonResult.LeftImages.Original));
                TlpImageComparison.Controls.Add(GetComparisonResult());
                TlpImageComparison.Controls.Add(
                    GetPictureBox(ImageComparisonResult.RightImages.Original));

                // Detail View
                TlpDetails.Controls.Add(
                    GetPictureBox(ImageComparisonResult.LeftImages.Cropped));
                TlpDetails.Controls.Add(GetDetailInfo(DetailLevel.Crop));
                TlpDetails.Controls.Add(
                    GetPictureBox(ImageComparisonResult.RightImages.Cropped));

                TlpDetails.Controls.Add(
                    GetPictureBox(ImageComparisonResult.LeftImages.Resized));
                TlpDetails.Controls.Add(GetDetailInfo(DetailLevel.Resize));
                TlpDetails.Controls.Add(
                    GetPictureBox(ImageComparisonResult.RightImages.Resized));

                TlpDetails.Controls.Add(
                    GetPictureBox(ImageComparisonResult.LeftImages.Greyscaled));
                TlpDetails.Controls.Add(GetDetailInfo(DetailLevel.Greyscale));
                TlpDetails.Controls.Add(
                    GetPictureBox(ImageComparisonResult.RightImages.Greyscaled));
            }
            finally
            {
                TlpImageComparison.ResumeLayout();

                base.OnLoad(e);
            }
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            TlpImageComparison.BackColor = BackColor;
            base.OnBackColorChanged(e);
        }

        private void ShowDetails()
        {
            ShowDetailsLabel.Text = "Hide Details";
            ShowDetailsLabel.Tag = true;

            LeftShowDetailArrow.Image = Resources.ArrowUpGray;
            RightShowDetailArrow.Image = Resources.ArrowUpGray;

            TlpDetails.Visible = true;
        }

        private void HideDetails()
        {
            ShowDetailsLabel.Text = "Show Details";
            ShowDetailsLabel.Tag = false;

            LeftShowDetailArrow.Image = Resources.ArrowDownBlue;
            RightShowDetailArrow.Image = Resources.ArrowDownBlue;

            TlpDetails.Visible = false;
        }

        private void HandleShowDetailsClickEvent(object? sender, EventArgs e)
        {
            if ((bool)ShowDetailsLabel.Tag)
            {
                HideDetails();
            }
            else
            {
                ShowDetails();
            }
        }

        private Panel GetComparisonResult()
        {
            var tlpResult = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                Anchor = AnchorStyles.None,
                Margin = new Padding { All = 0 },
                BackColor = GetComparisonResultColor(),
            };

            _ = tlpResult.RowStyles.Add(new RowStyle
            {
                SizeType = SizeType.Percent,
                Height = 100,
            });
            _ = tlpResult.RowStyles.Add(new RowStyle
            {
                SizeType = SizeType.AutoSize,
            });

            _ = tlpResult.ColumnStyles.Add(new ColumnStyle
            {
                SizeType = SizeType.Percent,
                Width = 50,
            });
            _ = tlpResult.ColumnStyles.Add(new ColumnStyle
            {
                SizeType = SizeType.AutoSize,
            });
            _ = tlpResult.ColumnStyles.Add(new ColumnStyle
            {
                SizeType = SizeType.Percent,
                Width = 50,
            });

            var info = new Label
            {
                Text = GetResultText(),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
            };
            tlpResult.Controls.Add(info, 0, 0);
            tlpResult.SetColumnSpan(info, 3);

            tlpResult.Controls.Add(LeftShowDetailArrow, 0, 1);
            tlpResult.Controls.Add(ShowDetailsLabel, 1, 1);
            tlpResult.Controls.Add(RightShowDetailArrow, 2, 1);

            var pnlResult = new Panel
            {
                Dock = DockStyle.Fill,
            };
            pnlResult.Controls.Add(tlpResult);

            return pnlResult;
        }

        private string GetResultText()
        {
            var text = $"{ImageComparisonIndex + 1}. Comparison:" +
                $"{Environment.NewLine}{LeftTimestamp.ToPrettyString()}  |  " +
                $"{RightTimestamp.ToPrettyString()}{Environment.NewLine}";
            if (ComparisonAlreadyFinished)
            {
                text += $"Result already determined.{Environment.NewLine}";
                if (ImageLoaded)
                {
                    text += "Images loaded but comparison was skipped.";
                }
                else
                {
                    text += "Images have not been loaded.";
                }
            }
            else if (ImageComparisonResult!.ComparisonResult
                     == ComparisonResult.NoResult)
            {
                text += $"Unable to load image.{Environment.NewLine}" +
                    "Comparison was skipped.";
            }
            else
            {
                text += $"Images loaded and compared.{Environment.NewLine}"
                    + "Difference "
                    + $"{ImageComparisonResult.Difference * 100:0.00} is ";
                if (ImageComparisonResult.ComparisonResult
                    == ComparisonResult.Different)
                {
                    text += "higher";
                }
                else
                {
                    text += "lower";
                }
                text += $" than {MaximumDifferencePercentage}.";
            }
            return text;
        }

        private Color GetComparisonResultColor()
        {
            if (ComparisonAlreadyFinished)
            {
                if (ImageLoaded)
                {
                    return LoadedColor;
                }
                return NotLoadedColor;
            }

            if (ImageComparisonResult!.ComparisonResult
                == ComparisonResult.NoResult)
            {
                return DifferentColor;
            }

            if (ImageComparisonResult!.ComparisonResult
                == ComparisonResult.Different)
            {
                return DifferentColor;
            }

            return DuplicateColor;
        }

        private static PictureBox GetPictureBox(Image image) =>
            new()
            {
                Image = image,
                Size = image.Size,
                Anchor = AnchorStyles.None,
            };

        private TableLayoutPanel GetDetailInfo(DetailLevel detailLevel)
        {
            var tlpDetailInfo = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 2,
                Dock = DockStyle.Fill,
                Margin = new Padding { All = 0 },
            };

            _ = tlpDetailInfo.RowStyles.Add(new RowStyle
            {
                SizeType = SizeType.Percent,
                Height = 50,
            });
            _ = tlpDetailInfo.RowStyles.Add(new RowStyle
            {
                SizeType = SizeType.Percent,
                Height = 50,
            });

            tlpDetailInfo.Controls.Add(new PictureBox
            {
                Image = DetailInfo[detailLevel].Item2,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Anchor = AnchorStyles.Bottom,
            }, 0, 0);

            tlpDetailInfo.Controls.Add(new Label
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                Text = DetailInfo[detailLevel].Item1,
                TextAlign = ContentAlignment.TopCenter,
            }, 0, 1);

            return tlpDetailInfo;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                ImageComparisonResult?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
