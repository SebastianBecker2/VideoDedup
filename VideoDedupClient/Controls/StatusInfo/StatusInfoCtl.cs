namespace VideoDedupClient.Controls.StatusInfo
{
    using Google.Protobuf.WellKnownTypes;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.TimeSpanExtensions;
    using static VideoDedupGrpc.OperationInfo.Types;

    public partial class StatusInfoCtl : UserControl
    {
        private static readonly int ProgressInfoBatchSize = 500;

        private static readonly Dictionary<OperationType, string>
            OperationTypeTexts = new()
            {
                { OperationType.Comparing, "Comparing files" },
                { OperationType.LoadingMedia, "Loading media info" },
                { OperationType.Searching, "Searching for files" },
                { OperationType.Monitoring, "Monitoring for file changes" },
                { OperationType.Completed, "Finished comparison" },
                { OperationType.Initializing, "Initializing" },
                { OperationType.Error, "Critical error occurred!" },
                { OperationType.Connecting, "Connecting..." },
                { OperationType.Preparing, "Preparing..." },
            };

        private OperationInfo OperationInfo { get; set; }
        private int TotalDuplicatesCount { get; set; }
        private int PreparedDuplicatesCount { get; set; }
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

            LblCurrentFileCount.Text = "";
            LblDuplicateCount.Text = "";
            LblFileCountSpeed.Text = "0";
            LblDuplicateSpeed.Text = "0";
        }

        public void Clear()
        {
            OperationInfo = new OperationInfo
            {
                OperationType = OperationType.Connecting,
                MaximumFiles = 0,
                ProgressStyle = ProgressStyle.NoProgress,
                ProgressCount = 0,
                ProgressToken = "",
                StartTime = Timestamp.FromDateTime(DateTime.UtcNow),
            };
            LatestProgressInfo = null;
            progressCount = 0;
            progressToken = "";
            PrgProgress.Clear();
            PrgProgress.MaxProgress = 0;
            PrgProgress.Refresh();
            LblStatusInfo.Text = OperationTypeTexts[OperationType.Connecting];
            // Workaround! Empty LblCurrentFileCount causes the table layout
            // panel to resize incorrectly, so we set it to a space character.
            LblCurrentFileCount.Text = " ";
            LblDuplicateCount.Text = "";
            LblFileCountSpeed.Text = "0";
            LblDuplicateSpeed.Text = "0";
            LblElapsedTime.Text = TimeSpan.Zero.ToPrettyString();
            LblRemainingTime.Text = TimeSpan.Zero.ToPrettyString();
        }

        public void UpdateStatusInfo(
            OperationInfo operationInfo,
            int totalDuplicatesCount = 0,
            int preparedDuplicatesCount = 0)
        {
            OperationInfo = operationInfo;
            TotalDuplicatesCount = totalDuplicatesCount;
            PreparedDuplicatesCount = preparedDuplicatesCount;

            SetStatusInfo();
            SetDuplicateCount();
            SetElapsedTime();

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

            if (Style is ProgressStyle.Marquee)
            {
                PrgProgress.DisplayMarquee();
            }

            PrgProgress.Refresh();

            SetCurrentFileCount();
            SetFileSpeed();
            SetDuplicatesSpeed();
            SetRemainingTime();
        }

        private IEnumerable<ProgressInfo> GetNextProgressInfo()
        {
            // If we don't have a progress token and neither had one before,
            // we don't want to clear anymore. It's already cleared.
            // Otherwise we stop the marquee each time, which looks weird.
            if (string.IsNullOrWhiteSpace(OperationInfo.ProgressToken)
                && OperationInfo.ProgressToken == progressToken)
            {
                yield break;
            }
            if (string.IsNullOrWhiteSpace(OperationInfo.ProgressToken)
                || OperationInfo.ProgressToken != progressToken)
            {
                LatestProgressInfo = null;
                PrgProgress.Clear();
                PrgProgress.MaxProgress = MaximumFiles;
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
            if (LatestProgressInfo is null)
            {
                return;
            }

            LblCurrentFileCount.Text = $"{FileCount} / {MaximumFiles} " +
                    $"({(double)FileCount / MaximumFiles * 100:0.00}%)";
        }

        private void SetDuplicateCount() =>
            LblDuplicateCount.Text =
                $"{PreparedDuplicatesCount}/" +
                $"{TotalDuplicatesCount}";

        private void SetFileSpeed()
        {
            if (LatestProgressInfo is null)
            {
                return;
            }

            LblFileCountSpeed.Text = $"{FileSpeed:0.00}";
        }

        private void SetDuplicatesSpeed()
        {
            if (LatestProgressInfo is null)
            {
                return;
            }

            LblDuplicateSpeed.Text = $"{DuplicatesSpeed:0.00}";
        }

        private void SetElapsedTime()
        {
            if (OperationInfo.OperationType is OperationType.Monitoring
                or OperationType.Completed)
            {
                return;
            }

            LblElapsedTime.Text = Duration.ToPrettyString();
        }

        private void SetRemainingTime()
        {
            if (LatestProgressInfo is null
                || FileCount == 0
                || FileCount == MaximumFiles
                || FileSpeed == 0)
            {
                LblRemainingTime.Text = TimeSpan.Zero.ToPrettyString();
                return;
            }

            LblRemainingTime.Text =
                TimeSpan.FromSeconds(Remaining / FileSpeed).ToPrettyString();
        }
    }
}
