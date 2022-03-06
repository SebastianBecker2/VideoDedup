namespace VideoDedup.StatusInfo
{
    // Rename progress to files



    // Calculate speed by recent items per recent time. instead of all items per all time.
    // This way, even with sleep mode / hypernation or when switching to monitoring
    // (as duplicate speed has the issue with that), the speed would normalize itself.
    // Because the time frame from the sleep / hypernation / pre-monitor would move
    // out of scope and would be ignored.


    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Forms;
    using Microsoft.WindowsAPICodePack.Taskbar;
    using VideoDedupShared;
    using VideoDedupShared.TimeSpanExtension;
    using SpeedRingBuffer = CircularBuffer<(int value, System.DateTime stamp)>;

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

        private static readonly int SpeedHistoryLenght = 60;

        private readonly SpeedRingBuffer fileSpeedHistory =
            new SpeedRingBuffer(SpeedHistoryLenght);

        private readonly SpeedRingBuffer duplicateSpeedHistory =
            new SpeedRingBuffer(SpeedHistoryLenght);

        private (double speed, string unit) CalculateSpeed(SpeedRingBuffer buffer)
        {
            var (firstValue, firstStamp) = buffer.First();
            var (lastValue, lastStamp) = buffer.Last();

            return CalculateSpeed(
                lastValue - firstValue,
                lastStamp - firstStamp);
        }

        private (double speed, string unit) CalculateSpeed(
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

        public void UpdateStatusInfo(
            OperationInfo operationInfo,
            int duplicateCount = 0)
        {
            if (operationInfo == null)
            {
                return;
            }

            OperationInfo = operationInfo;
            DuplicateCount = duplicateCount;

            // To clear the speed history, we keep the start time of the
            // operation. If the operationInfo contains a new one, we know
            // we have a different operation. Thus clearing the history.
            if (StartTime != operationInfo.StartTime)
            {
                StartTime = operationInfo.StartTime;
                fileSpeedHistory.Clear();
                fileSpeedHistory.PushBack((0, StartTime));
                duplicateSpeedHistory.Clear();
                duplicateSpeedHistory.PushBack((0, StartTime));
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
            SetCurrentFileCount();
            SetRemainingFileCount();
            SetDuplicateCount();
            SetFileSpeed();
            SetDuplicateSpeed();
            SetElapsedTime();
            SetRemainingTime();
        }

        private OperationInfo OperationInfo { get; set; }
        private int DuplicateCount { get; set; }

        private OperationType Type => OperationInfo.OperationType;
        private int Current => OperationInfo.CurrentProgress;
        private int Maximum => OperationInfo.MaximumProgress;
        private ProgressStyle Style => OperationInfo.ProgressStyle;
        private DateTime StartTime { get; set; }
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

        private void SetCurrentFileCount()
        {
            var visible = Maximum != 0;

            if (visible)
            {
                LblCurrentFileCount.Text = $"{Current} / {Maximum} " +
                    $"({(double)Current / Maximum * 100:0.00}%)";
            }

            LblCurrentFileCount.Visible = visible;
            LblCurrentFileCountTitle.Visible = visible;
        }

        private void SetRemainingFileCount()
        {
            var visible = Maximum != 0;

            if (visible)
            {
                var remaining = Maximum - Current;
                LblRemainingFileCount.Text = $"{remaining} / {Maximum} " +
                    $"({(double)remaining / Maximum * 100:0.00}%)";
            }

            LblRemainingFileCount.Visible = visible;
            LblRemainingFileCountTitle.Visible = visible;
        }

        private void SetDuplicateCount()
        {
            var visible = DuplicateCount != 0;

            if (visible)
            {
                LblDuplicateCount.Text = $"{DuplicateCount}";
            }

            LblDuplicateCount.Visible = visible;
            LblDuplicateCountTitle.Visible = visible;
        }

        private void SetFileSpeed()
        {
            var visible = StartTime != DateTime.MinValue && Current != 0;
            try
            {
                if (!visible)
                {
                    return;
                }

                fileSpeedHistory.PushBack((Current, DateTime.Now));
                var (speed, unit) = CalculateSpeed(fileSpeedHistory);

                visible = !double.IsInfinity(speed) && !double.IsNaN(speed);

                if (!visible)
                {
                    return;
                }

                LblFileCountSpeed.Text = $"{speed:0.00}";
                LblFileCountSpeedUnit.Text = $"Files/{unit}";
            }
            finally
            {
                LblFileCountSpeed.Visible = visible;
                LblFileCountSpeedTitle.Visible = visible;
                LblFileCountSpeedUnit.Visible = visible;
            }
        }

        private void SetDuplicateSpeed()
        {
            var visible = StartTime != DateTime.MinValue && DuplicateCount != 0;
            try
            {
                // For the special case that we are monitoring.
                // Because monitoring resets the elapsed time but the duplicate
                // count is not reset. So we get useless and ridiculous speed.
                visible &= Maximum != 0;

                if (!visible)
                {
                    return;
                }

                duplicateSpeedHistory.PushBack((DuplicateCount, DateTime.Now));
                var (speed, unit) = CalculateSpeed(duplicateSpeedHistory);

                visible = !double.IsInfinity(speed) && !double.IsNaN(speed);

                if (!visible)
                {
                    return;
                }

                LblDuplicateSpeed.Text = $"{speed:0.00}";
                LblDuplicateSpeedUnit.Text = $"Duplicates/{unit}";
            }
            finally
            {
                LblDuplicateSpeed.Visible = visible;
                LblDuplicateSpeedTitle.Visible = visible;
                LblDuplicateSpeedUnit.Visible = visible;
            }
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

            try
            {
                if (!visible)
                {
                    return;
                }

                var (firstValue, firstStamp) = fileSpeedHistory.First();
                var (lastValue, lastStamp) = fileSpeedHistory.Last();
                var files = lastValue - firstValue;
                var duration = lastStamp - firstStamp;

                var timePerFile = duration.TotalSeconds / files;

                visible = !double.IsInfinity(timePerFile)
                    && !double.IsNaN(timePerFile);

                if (!visible)
                {
                    return;
                }

                var remainingTime = timePerFile * Remaining;
                LblRemainingTime.Text =
                    TimeSpan.FromSeconds(remainingTime).ToPrettyString();
            }
            finally
            {
                LblRemainingTime.Visible = visible;
                LblRemainingTimeTitle.Visible = visible;
            }
        }

    }
}
