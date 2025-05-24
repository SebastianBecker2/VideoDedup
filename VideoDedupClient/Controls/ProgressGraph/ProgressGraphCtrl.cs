namespace VideoDedupClient.Controls.ProgressGraph
{
    using OxyPlot;
    using OxyPlot.Annotations;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using System;
    using System.Windows.Forms;
    using HorizontalAlignment = OxyPlot.HorizontalAlignment;

    public partial class ProgressGraphCtrl : UserControl
    {
        public double MaxProgress
        {
            get => ProgressAxis.Maximum;
            set => ProgressAxis.Maximum = value;
        }

        private PlotModel Model { get; } = new();

        private AreaSeries FilesSeries { get; } = new()
        {
            Color = OxyColors.Green,
            XAxisKey = "Progress",
            YAxisKey = "FileSpeed",
        };
        private AreaSeries DuplicatesSeries { get; } = new()
        {
            Color = OxyColors.Orange,
            XAxisKey = "Progress",
            YAxisKey = "Duplicates",
        };

        private LinearAxis FileSpeedAxis { get; } = new()
        {
            Minimum = 0,
            Maximum = 1,
            Key = "FileSpeed",
            Position = AxisPosition.Right,
            IsZoomEnabled = false,
            IsPanEnabled = false,
            IsAxisVisible = false,
        };
        private LinearAxis DuplicatesAxis { get; } = new()
        {
            Minimum = 0,
            Maximum = 1,
            Key = "Duplicates",
            Position = AxisPosition.Left,
            IsZoomEnabled = false,
            IsPanEnabled = false,
            IsAxisVisible = false,
        };
        private LinearAxis MarqueeVerticalAxis { get; } = new()
        {
            Minimum = 0,
            Maximum = 1,
            Key = "MarqueeVerticalAxis",
            Position = AxisPosition.Left,
            IsZoomEnabled = false,
            IsPanEnabled = false,
            IsAxisVisible = false,
        };
        private LinearAxis ProgressAxis { get; } = new()
        {
            Minimum = 0,
            Maximum = 100,
            Key = "Progress",
            Position = AxisPosition.Bottom,
            IsZoomEnabled = false,
            IsPanEnabled = false,
            IsAxisVisible = false,
        };
        private LinearAxis MarqueeHorizontalAxis { get; } = new()
        {
            Minimum = 0,
            Maximum = 200,
            Key = "MarqueeHorizontalAxis",
            Position = AxisPosition.Bottom,
            IsZoomEnabled = false,
            IsPanEnabled = false,
            IsAxisVisible = false,
        };

        private TextAnnotation FileSpeedText { get; } = new()
        {
            Text = "",
            FontSize = 10,
            StrokeThickness = 0,
            Background = OxyColors.Undefined,
            XAxisKey = "Progress",
            YAxisKey = "FileSpeed",
        };
        private TextAnnotation DuplicatesText { get; } = new()
        {
            Text = "",
            FontSize = 10,
            StrokeThickness = 0,
            Background = OxyColors.Undefined,
            XAxisKey = "Progress",
            YAxisKey = "Duplicates",
        };
        private TextAnnotation ProgressText { get; } = new()
        {
            Text = "",
            FontSize = 10,
            StrokeThickness = 0,
            Background = OxyColors.Undefined,
            XAxisKey = "Progress",
            YAxisKey = "FileSpeed",
        };

        private LineAnnotation ProgressLine { get; } = new()
        {
            Type = LineAnnotationType.Vertical,
            Color = OxyColors.Green,
            LineStyle = LineStyle.Solid,
            StrokeThickness = 2,
            XAxisKey = "Progress",
            YAxisKey = "FileSpeed",
        };

        private RectangleAnnotation Marquee { get; } = new()
        {
            Fill = OxyColors.Green,
            Stroke = OxyColors.Green,
            XAxisKey = "MarqueeHorizontalAxis",
            YAxisKey = "MarqueeVerticalAxis",
            MaximumY = 100,
            MinimumY = 0,
            MaximumX = 0,
            MinimumX = 0,
        };
        private int MarqueePosition { get; set; }
        private FlowDirection MarqueeDiration { get; set; } =
            FlowDirection.LeftToRight;
        private int MarqueeWidth { get; } = 10;
        private Timer MarqueeTimer { get; } = new()
        {
            Interval = 10,
        };

        public ProgressGraphCtrl()
        {
            InitializeComponent();

            Model.Axes.Add(FileSpeedAxis);
            Model.Axes.Add(DuplicatesAxis);
            Model.Axes.Add(MarqueeVerticalAxis);
            Model.Axes.Add(ProgressAxis);
            Model.Axes.Add(MarqueeHorizontalAxis);
            Model.Series.Add(FilesSeries);
            Model.Series.Add(DuplicatesSeries);
            Model.Annotations.Add(FileSpeedText);
            Model.Annotations.Add(DuplicatesText);
            Model.Annotations.Add(ProgressText);
            Model.Annotations.Add(ProgressLine);
            Model.Annotations.Add(Marquee);

            MarqueeTimer.Tick += MarqueeTimer_Tick;

            Pv.Model = Model;
        }

        public void AddProgress(
            double progress,
            string progressText,
            double fileSpeed,
            string fileSpeedText,
            double duplicates,
            string duplicatesText)
        {
            var filesDataPoint = new DataPoint(progress, fileSpeed);
            var duplicatesDataPoint = new DataPoint(progress, duplicates);

            FilesSeries.Points.Add(filesDataPoint);
            DuplicatesSeries.Points.Add(duplicatesDataPoint);
            if (duplicates != 0)
            {
                DuplicatesSeries.IsVisible = true;
            }

            if (FileSpeedAxis.Maximum <= fileSpeed * 1.1)
            {
                FileSpeedAxis.Maximum = fileSpeed * 1.3;
            }

            if (DuplicatesAxis.Maximum <= duplicates * 1.1)
            {
                DuplicatesAxis.Maximum = duplicates * 1.3;
            }

            Pv.InvalidatePlot(true);

            UpdateTextAnnotation(
                FileSpeedText,
                FileSpeedAxis.Key,
                fileSpeedText,
                filesDataPoint);

            if (DuplicatesSeries.IsVisible)
            {
                UpdateTextAnnotation(
                    DuplicatesText,
                    DuplicatesAxis.Key,
                    duplicatesText,
                    duplicatesDataPoint);
            }

            UpdateTextAnnotation(
                ProgressText,
                FileSpeedAxis.Key,
                progressText,
                new DataPoint(progress, FileSpeedAxis.Maximum));

            ProgressLine.X = progress;
        }

        private void UpdateTextAnnotation(
            TextAnnotation annotation,
            string yAxisKey,
            string text,
            DataPoint position)
        {
            if (MarqueeTimer.Enabled)
            {
                Clear();
            }

            annotation.Text = text;
            annotation.TextPosition = position;
            annotation.YAxisKey = yAxisKey;

            var screenPosition = annotation.Transform(position);

            if (screenPosition.X > (Model.PlotArea.Width / 2))
            {
                annotation.TextHorizontalAlignment = HorizontalAlignment.Right;
                annotation.Offset = new ScreenVector(-5, 0);
            }
            else
            {
                annotation.TextHorizontalAlignment = HorizontalAlignment.Left;
                annotation.Offset = new ScreenVector(5, 0);
            }

            if (screenPosition.Y > (Model.PlotArea.Height / 2))
            {
                annotation.TextVerticalAlignment = VerticalAlignment.Bottom;
            }
            else
            {
                annotation.TextVerticalAlignment = VerticalAlignment.Top;
            }
        }

        public override void Refresh()
        {
            Pv.InvalidatePlot(true);
            base.Refresh();
        }

        public void Clear()
        {
            FilesSeries.Points.Clear();
            DuplicatesSeries.Points.Clear();
            DuplicatesSeries.IsVisible = false;

            FileSpeedAxis.Maximum = 1;
            DuplicatesAxis.Maximum = 1;
            ProgressAxis.Maximum = 200;

            FileSpeedText.Text = "";
            DuplicatesText.Text = "";
            ProgressText.Text = "";
            ProgressLine.X = 0;

            MarqueeTimer.Stop();
            MarqueePosition = 0;
            MarqueeDiration = FlowDirection.LeftToRight;
            Marquee.MaximumX = 0;
            Marquee.MinimumX = 0;
        }

        public void DisplayMarquee()
        {
            if (MarqueeTimer.Enabled)
            {
                return;
            }
            MarqueeTimer.Start();
        }

        private void MarqueeTimer_Tick(object? sender, EventArgs e)
        {
            if (MarqueeDiration == FlowDirection.LeftToRight)
            {
                MarqueePosition++;
                if (MarqueePosition == MarqueeHorizontalAxis.Maximum)
                {
                    MarqueeDiration = FlowDirection.RightToLeft;
                }

                Marquee.MaximumX = MarqueePosition;
                Marquee.MinimumX = MarqueePosition - MarqueeWidth;

                Pv.InvalidatePlot(true);
                return;
            }

            MarqueePosition--;
            if (MarqueePosition == MarqueeHorizontalAxis.Minimum + MarqueeWidth)
            {
                MarqueeDiration = FlowDirection.LeftToRight;
            }

            Marquee.MinimumX = MarqueePosition - MarqueeWidth;
            Marquee.MaximumX = MarqueePosition;

            Pv.InvalidatePlot(true);
        }
    }
}
