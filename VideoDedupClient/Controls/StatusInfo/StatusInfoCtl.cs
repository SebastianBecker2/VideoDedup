namespace VideoDedupClient.Controls.StatusInfo
{
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.TimeSpanExtensions;
    using static VideoDedupGrpc.OperationInfo.Types;

    public partial class StatusInfoCtl : UserControl
    {
        private static readonly int ProgressInfoBatchSize = 500;

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

        private OperationInfo OperationInfo { get; set; }
        private int CurrentDuplicateCount { get; set; }
        private ProgressInfo? LatestProgressInfo { get; set; }

        private int progressCount;
        private string progressToken = "";

        private int MaximumFiles => OperationInfo.MaximumFiles;
        private int FileCount => LatestProgressInfo!.FileCount;
        private int Remaining => MaximumFiles - FileCount;
        private ProgressStyle Style => OperationInfo.ProgressStyle;
        private DateTime StartTime => OperationInfo.StartTime.ToDateTime();
        private TimeSpan Duration => DateTime.UtcNow - StartTime;
        private double FileSpeed => LatestProgressInfo!.FileCountSpeed;
        private double DuplicatesSpeed =>
            LatestProgressInfo!.DuplicatesFoundSpeed;

        public StatusInfoCtl()
        {
            OperationInfo = new OperationInfo
            {
                OperationType = OperationType.Initializing,
                MaximumFiles = 0,
                ProgressStyle = ProgressStyle.NoProgress,
            };

            InitializeComponent();
        }

        public void UpdateStatusInfo(
            OperationInfo operationInfo,
            int currentDuplicateCount = 0)
        {
            OperationInfo = operationInfo;
            CurrentDuplicateCount = currentDuplicateCount;

            SetStatusInfo();
            SetDuplicateCount();
            SetElapsedTime();

            if (Style is ProgressStyle.NoProgress)
            {
                //PrgProgress.Clear();
                LatestProgressInfo = null;
                progressCount = 0;
                progressToken = "";
            }
            else if (Style is ProgressStyle.Marquee)
            {
                PrgProgress.DisplayMarquee();
                PrgProgress.Refresh();
                LatestProgressInfo = null;
                progressCount = 0;
                progressToken = "";
            }
            else if (Style == ProgressStyle.Continuous)
            {
                PrgProgress.MaxProgress = MaximumFiles;
                foreach (var pi in GetNextProgressInfo())
                {
                    var progressText =
                        $"{(double)pi.FileCount / MaximumFiles * 100:0.00}%";
                    PrgProgress.AddProgress(
                        pi.FileCount,
                        progressText,
                        pi.FileCountSpeed,
                        $"{pi.FileCountSpeed:0.00} Files/s",
                        pi.DuplicatesFound,
                        $"{pi.DuplicatesFound} Duplicates");
                }
                PrgProgress.Refresh();
            }

            SetCurrentFileCount();
            SetFileSpeed();
            SetDuplicatesSpeed();
            SetRemainingTime();
        }

        private IEnumerable<ProgressInfo> GetNextProgressInfo()
        {
            if (string.IsNullOrWhiteSpace(OperationInfo.ProgressToken)
                || OperationInfo.ProgressToken != progressToken)
            {
                LatestProgressInfo = null;
                PrgProgress.Clear();
                progressCount = 0;
                progressToken = OperationInfo.ProgressToken;
            }

            while (OperationInfo.ProgressCount > progressCount)
            {
                var response = Program.GrpcClient.GetProgressInfo(
                    new GetProgressInfoRequest()
                    {
                        ProgressToken = progressToken,
                        Start = progressCount,
                        Count = ProgressInfoBatchSize,
                    });
                if (response.ProgressInfos.Count <= 0)
                {
                    yield break;
                }

                LatestProgressInfo = response.ProgressInfos[^1];
                progressCount += response.ProgressInfos.Count;

                foreach (var progressInfo in response.ProgressInfos)
                {
                    yield return progressInfo;
                }
            }
        }

        private void SetStatusInfo() =>
            LblStatusInfo.Text = OperationTypeTexts[OperationInfo.OperationType];

        private void SetCurrentFileCount()
        {
            var visible = LatestProgressInfo is not null;

            if (visible)
            {
                LblCurrentFileCount.Text = $"{FileCount} / {MaximumFiles} " +
                    $"({(double)FileCount / MaximumFiles * 100:0.00}%)";
            }

            LblCurrentFileCount.Visible = visible;
            LblCurrentFileCountTitle.Visible = visible;
        }

        private void SetDuplicateCount()
        {
            var visible = CurrentDuplicateCount != 0;

            if (visible)
            {
                LblDuplicateCount.Text = $"{CurrentDuplicateCount}";
            }

            LblDuplicateCount.Visible = visible;
            LblDuplicateCountTitle.Visible = visible;
        }

        private void SetFileSpeed()
        {
            var visible = LatestProgressInfo is not null;

            if (!visible)
            {
                return;
            }

            LblFileCountSpeed.Text = $"{FileSpeed:0.00}";

            LblFileCountSpeed.Visible = visible;
            LblFileCountSpeedTitle.Visible = visible;
            LblFileCountSpeedUnit.Visible = visible;
        }

        private void SetDuplicatesSpeed()
        {
            var visible = LatestProgressInfo is not null;

            if (!visible)
            {
                return;
            }

            LblDuplicateSpeed.Text = $"{DuplicatesSpeed:0.00}";

            LblDuplicateSpeed.Visible = visible;
            LblDuplicateSpeedTitle.Visible = visible;
            LblDuplicateSpeedUnit.Visible = visible;
        }

        private void SetElapsedTime() =>
            LblElapsedTime.Text = Duration.ToPrettyString();

        private void SetRemainingTime()
        {
            var visible = LatestProgressInfo is not null
                && FileCount > 0
                && Remaining > 0
                && FileSpeed > 0;

            if (!visible)
            {
                return;
            }
            LblRemainingTime.Text =
                TimeSpan.FromSeconds(Remaining / FileSpeed).ToPrettyString();

            LblRemainingTime.Visible = visible;
            LblRemainingTimeTitle.Visible = visible;
        }
    }
}
