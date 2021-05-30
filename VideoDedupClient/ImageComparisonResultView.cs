namespace VideoDedup
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using VideoDedup.Properties;
    using VideoDedupShared;
    using VideoDedupShared.ImageExtension;

    public partial class ImageComparisonResultView : UserControl
    {
        private static readonly Size ThumbnailSize = new Size(256, 256);

        private enum DetailLevel
        {
            Crop,
            Resize,
            Greyscale,
        }

        private Dictionary<DetailLevel, Tuple<string, Image>> DetailInfo { get; } =
            new Dictionary<DetailLevel, Tuple<string, Image>>
        {
            { DetailLevel.Crop, new Tuple<string, Image>(
                $"Preparation step 1{Environment.NewLine}Cut off black bars",
                Resources.PictureCropped) },
            { DetailLevel.Resize, new Tuple<string, Image>(
                $"Preparation step 2{Environment.NewLine}Resize to 16 x 16 pixel",
                Resources.PictureSizeDown) },
            { DetailLevel.Greyscale, new Tuple<string, Image>(
                $"Preparation step 3{Environment.NewLine}Convert to greyscale",
                Resources.PictureGreyscale) },
        };

        public int ImageComparisonIndex { get; set; }
        public ImageComparisonResult ImageComparisonResult { get; set; }
        public bool ComparisonFinished { get; set; }
        public bool ImageLoaded { get; set; }
        public int MaximumDifferencePercentage { get; set; }

        public Color DifferenceColor { get; set; }
        public Color DuplicateColor { get; set; }
        public Color NeutralColor { get; set; }

        private Label ShowDetailsLabel { get; set; }
        private PictureBox LeftShowDetailArrow { get; set; }
        private PictureBox RightShowDetailArrow { get; set; }
        private TableLayoutPanel TlpDetails { get; set; }

        public ImageComparisonResultView()
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

            LeftShowDetailArrow = new PictureBox
            {
                Image = Resources.ArrowDownBlue,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Anchor = AnchorStyles.Right,
                Margin = new Padding { All = 0 },
            };
            LeftShowDetailArrow.Click += HandleShowDetailsClickEvent;

            RightShowDetailArrow = new PictureBox
            {
                Image = Resources.ArrowDownBlue,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Anchor = AnchorStyles.Left,
                Margin = new Padding { All = 0 },
            };
            RightShowDetailArrow.Click += HandleShowDetailsClickEvent;

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
            // Main View
            TlpImageComparison.Controls.Add(
                GetPictureBox(ImageComparisonResult.LeftImages?.Orignal));
            TlpImageComparison.Controls.Add(GetComparisonResult());
            TlpImageComparison.Controls.Add(
                GetPictureBox(ImageComparisonResult.RightImages?.Orignal));

            // Detail View
            TlpDetails.Controls.Add(
                GetPictureBox(ImageComparisonResult.LeftImages?.Cropped));
            TlpDetails.Controls.Add(GetDetailInfo(DetailLevel.Crop));
            TlpDetails.Controls.Add(
                GetPictureBox(ImageComparisonResult.RightImages?.Cropped));

            TlpDetails.Controls.Add(
                GetPictureBox(ImageComparisonResult.LeftImages?.Resized));
            TlpDetails.Controls.Add(GetDetailInfo(DetailLevel.Resize));
            TlpDetails.Controls.Add(
                GetPictureBox(ImageComparisonResult.RightImages?.Resized));

            TlpDetails.Controls.Add(
                GetPictureBox(ImageComparisonResult.LeftImages?.Greyscaled));
            TlpDetails.Controls.Add(GetDetailInfo(DetailLevel.Greyscale));
            TlpDetails.Controls.Add(
                GetPictureBox(ImageComparisonResult.RightImages?.Greyscaled));

            base.OnLoad(e);
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

        private void HandleShowDetailsClickEvent(object sender, EventArgs e)
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
            var text = $"{ImageComparisonIndex + 1}. Comparison:{Environment.NewLine}";
            if (!ComparisonFinished)
            {
                text += $"Images loaded and compared.{Environment.NewLine}"
                    + $"Difference {ImageComparisonResult.Difference * 100} is ";
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
            else
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
            return text;
        }

        private Color GetComparisonResultColor()
        {
            if (!ComparisonFinished)
            {
                if (ImageComparisonResult.ComparisonResult
                    == ComparisonResult.Different)
                {
                    return DifferenceColor;
                }
                else
                {
                    return DuplicateColor;
                }
            }
            return NeutralColor;
        }

        private static Image StreamToImage(MemoryStream stream)
        {
            if (stream != null)
            {
                return Image.FromStream(stream);
            }
            return new Bitmap(1, 1);
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

        private static Image GetThumbnail(Image image)
        {
            if (image.Size.Height > ThumbnailSize.Height
                || image.Size.Width > ThumbnailSize.Width)
            {
                return image.Resize(GetThumbnailSize(image.Size));
            }
            return image.Resize(
                GetThumbnailSize(image.Size),
                System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor);
        }

        private static PictureBox GetPictureBox(MemoryStream imageStream) =>
            new PictureBox
            {
                Image = GetThumbnail(StreamToImage(imageStream)),
                SizeMode = PictureBoxSizeMode.AutoSize,
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
    }
}
