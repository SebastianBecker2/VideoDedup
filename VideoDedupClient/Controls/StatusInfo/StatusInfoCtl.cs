namespace VideoDedup.StatusInfo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Forms;
    using Microsoft.WindowsAPICodePack.Taskbar;
    using VideoDedupShared;
    using VideoDedupShared.TimeSpanExtension;

    public partial class StatusInfoCtl : UserControl
    {
        private static readonly IReadOnlyDictionary<OperationType, string>
            OperationTypeTexts = new Dictionary<OperationType, string>
            {
                { OperationType.Comparing, "Comparing files" },
                { OperationType.LoadingMedia, "Loading media info" },
                { OperationType.Searching, "Searching for files" },
                { OperationType.Monitoring, "Monitoring for file changes" },
                { OperationType.Completed, "Finished comparison" },
                { OperationType.Initializing, "Initializing" },
                { OperationType.Error, "Critical error occurred!" },
                { OperationType.Connecting, "Connecting..." },
            };

        private class SpeedScale
        {
            public string Unit { get; }
            public double Max { get; }
            public Func<TimeSpan, double> Divisor { get; }

            public SpeedScale(
                string unit,
                double max,
                Func<TimeSpan, double> divisor)
            {
                Unit = unit;
                Max = max;
                Divisor = divisor;
            }
        }

        private static readonly IReadOnlyCollection<SpeedScale>
            SpeedScales = new List<SpeedScale>
        {
            new SpeedScale("d", 240, timespan => timespan.TotalDays),
            new SpeedScale("h", 300, timespan => timespan.TotalHours),
            new SpeedScale("m", 300, timespan => timespan.TotalMinutes),
            new SpeedScale("s", 5000, timespan => timespan.TotalSeconds),
            new SpeedScale(
                "ms",
                double.MaxValue,
                timespan => timespan.TotalMilliseconds),
        };

        private (double, string) CalculateSpeed(
            int value,
            TimeSpan timeSpan)
        {
            foreach (var scale in SpeedScales)
            {
                var speed = value / scale.Divisor(timeSpan);
                if (speed < scale.Max)
                {
                    return (speed, scale.Unit);
                }
            }
            var highestScale = SpeedScales.Last();
            return (value / highestScale.Divisor(timeSpan), highestScale.Unit);
        }


        public OperationInfo OperationInfo
        {
            get => operationInfo;
            set
            {
                operationInfo = value;

                if (operationInfo == null)
                {
                    return;
                }

                if (StartTime != DateTime.MinValue)
                {
                    Duration = DateTime.Now - StartTime;
                }

                if (Maximum != 0)
                {
                    Remaining = Maximum - Current;
                }

                LblStatusInfo.Text = OperationTypeTexts[Type];
                SetProgressBar();
                SetElapsedTime();
                SetRemainingTime();
                SetCurrentProgress();
                SetRemainingProgress();
                SetProgressSpeed();
                SetDuplicateCount();
                SetDuplicateSpeed();
            }
        }
        public int DuplicateCount
        {
            get => duplicateCount;
            set
            {
                duplicateCount = value;

                SetDuplicateCount();
                if (operationInfo == null)
                {
                    return;
                }
                SetDuplicateSpeed();
            }
        }

        private OperationInfo operationInfo;
        private int duplicateCount;

        private OperationType Type => OperationInfo.OperationType;
        private int Current => OperationInfo.CurrentProgress;
        private int Maximum => OperationInfo.MaximumProgress;
        private ProgressStyle Style => OperationInfo.ProgressStyle;
        private DateTime StartTime => OperationInfo.StartTime;
        private TimeSpan Duration { get; set; }
        private int Remaining { get; set; }

        public StatusInfoCtl() => InitializeComponent();

        private void SetProgressBar()
        {
            // Off (invalid configuration) [value = 0, max = 0]
            // Continuous (searching duplicates)
            // Marquee (conecting, monitoring) [style = marquee, max = 1]
            if (Style == ProgressStyle.NoProgress)
            {
                TaskbarManager.Instance.SetProgressState(
                    TaskbarProgressBarState.NoProgress,
                    Handle);
                ProgressBar.Style = ProgressBarStyle.Continuous;
                // Assignment order of max and value for ProgressBar is important!
                ProgressBar.Maximum = 0;
                ProgressBar.Value = 0;
            }
            else if (Style == ProgressStyle.Continuous)
            {
                TaskbarManager.Instance.SetProgressState(
                    TaskbarProgressBarState.Normal,
                    Handle);
                TaskbarManager.Instance.SetProgressValue(Current, Maximum, Handle);
                ProgressBar.Style = ProgressBarStyle.Continuous;
                // Assignment order of max and value for ProgressBar is important!
                ProgressBar.Maximum = Maximum;
                ProgressBar.Value = Current;
            }
            else if (Style == ProgressStyle.Marquee)
            {
                TaskbarManager.Instance.SetProgressState(
                    TaskbarProgressBarState.Indeterminate,
                    Handle);
                ProgressBar.Style = ProgressBarStyle.Marquee;
                // Assignment order of max and value for ProgressBar is important!
                ProgressBar.Maximum = 1;
                ProgressBar.Value = 0;
            }
            else
            {
                Debug.Assert(false);
            }
        }

        private void SetCurrentProgress()
        {
            var visible = Maximum != 0;

            if (visible)
            {
                LblCurrentProgress.Text = $"{Current} / {Maximum} " +
                    $"({(double)Current / Maximum * 100:0.00}%)";
            }

            LblCurrentProgress.Visible = visible;
            LblCurrentProgressTitle.Visible = visible;
        }

        private void SetRemainingProgress()
        {
            var visible = Maximum != 0;

            if (visible)
            {
                var remaining = Maximum - Current;
                LblRemainingProgress.Text = $"{remaining} / {Maximum} " +
                    $"({(double)remaining / Maximum * 100:0.00}%)";
            }

            LblRemainingProgress.Visible = visible;
            LblRemainingProgressTitle.Visible = visible;
        }

        private void SetDuplicateCount()
        {
            var visible = duplicateCount != 0;

            if (visible)
            {
                LblDuplicateCount.Text = $"{duplicateCount}";
            }

            LblDuplicateCount.Visible = visible;
            LblDuplicateCountTitle.Visible = visible;
        }


        private void SetProgressSpeed()
        {
            var visible = StartTime != DateTime.MinValue && Current != 0;

            if (visible)
            {
                var (speed, unit) = CalculateSpeed(Current, Duration);
                LblProgressSpeed.Text = $"{speed:0.00}";
                LblProgressSpeedUnit.Text = $"Files/{unit}";
            }

            LblProgressSpeed.Visible = visible;
            LblProgressSpeedTitle.Visible = visible;
            LblProgressSpeedUnit.Visible = visible;
        }

        private void SetDuplicateSpeed()
        {
            var visible = StartTime != DateTime.MinValue && duplicateCount != 0;
            // For the special case that we are monitoring.
            // Because monitoring resets the elapsed time but the duplicate
            // count is not reset. So we get useless and ridiculous speed.
            visible &= Maximum != 0;

            if (visible)
            {
                var (speed, unit) = CalculateSpeed(DuplicateCount, Duration);
                LblDuplicateSpeed.Text = $"{speed:0.00}";
                LblDuplicateSpeedUnit.Text = $"Duplicates/{unit}";
            }

            LblDuplicateSpeed.Visible = visible;
            LblDuplicateSpeedTitle.Visible = visible;
            LblDuplicateSpeedUnit.Visible = visible;
        }

        private void SetElapsedTime()
        {
            var visible = StartTime != DateTime.MinValue;

            if (visible)
            {
                LblElapsedTime.Text = Duration.ToPrettyString();
            }

            LblElapsedTime.Visible = visible;
            LblElapsedTimeTitle.Visible = visible;
        }

        private void SetRemainingTime()
        {
            var visible =
                StartTime != DateTime.MinValue && Current != 0 && Maximum != 0;

            if (visible)
            {
                var remainingTime =
               Duration.TotalSeconds
               / Current
               * Remaining;

                LblRemainingTime.Text =
                    TimeSpan.FromSeconds(remainingTime).ToPrettyString();
            }

            LblRemainingTime.Visible = visible;
            LblRemainingTimeTitle.Visible = visible;
        }

    }
}
