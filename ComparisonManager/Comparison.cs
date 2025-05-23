namespace ComparisonManager
{
    using Serilog;
    using VideoComparer;
    using VideoComparer.EventArgs;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.IVideoFileExtensions;
    using VideoFile = VideoComparer.VideoFile;

    internal sealed class Comparison : IDisposable
    {
        private ILogger? Logger { get; }

        public Guid Token { get; }

        private VideoFile LeftVideoFile => comparer.LeftVideoFile;
        private VideoFile RightVideoFile => comparer.RightVideoFile;

        private readonly CancellationTokenSource cancelTokenSource = new();
        private readonly VideoComparer comparer;
        private readonly Task comparerTask;
        private readonly List<FrameComparisonResult> frameComparisons = [];
        private readonly object statusLock = new();

        private VideoComparisonResult? comparisonResult;
        private bool disposedValue;

        public Comparison(
            VideoComparisonSettings settings,
            string leftFilePath,
            string rightFilePath,
            bool forceLoadingAllFrames,
            ILogger? logger = null)
        {
            Logger = logger;

            Token = Guid.NewGuid();

            if (!File.Exists(leftFilePath))
            {
                Logger?.Error($"Comparison {Token} can't find file " +
                    $"{leftFilePath}");
                throw new InvalidDataException("Left video file path invalid.");
            }

            if (!File.Exists(rightFilePath))
            {
                Logger?.Error($"Comparison {Token} can't find file " +
                    $"{rightFilePath}");
                throw new InvalidDataException("Right video file path invalid.");
            }

            comparer = new VideoComparer(
                settings,
                new VideoFile(leftFilePath),
                new VideoFile(rightFilePath))
            {
                ForceLoadingAllFrames = forceLoadingAllFrames,
            };

            comparer.FrameCompared += HandleFrameCompared;
            comparer.ComparisonFinished += HandleComparisonFinished;

            comparerTask = Task.Run(Compare);
        }

        public void CancelComparison() => cancelTokenSource.Cancel();

        public VideoComparisonStatus GetStatus(
            int frameComparisonIndex = 0)
        {
            lock (statusLock)
            {
                var status = new VideoComparisonStatus
                {
                    ComparisonToken = Token.ToString(),
                    LeftFile = LeftVideoFile.ToVideoFile(),
                    RightFile = RightVideoFile.ToVideoFile(),
                    VideoComparisonResult = comparisonResult,
                };

                status.FrameComparisons.AddRange(
                    [.. frameComparisons.Skip(frameComparisonIndex)]);

                return status;
            }
        }

        private void Compare()
        {
            try
            {
                Logger?.Information($"Comparison {Token} is comparing");
                _ = comparer.Compare(cancelTokenSource.Token);
            }
            catch (Exception exc)
            {
                lock (statusLock)
                {
                    var last = frameComparisons.LastOrDefault();
                    comparisonResult = new VideoComparisonResult
                    {
                        Reason = exc.Message,
                        ComparisonResult = ComparisonResult.Aborted,
                        LastComparedIndex = last?.Index ?? 0,
                    };
                }
            }
        }

        private void HandleComparisonFinished(
            object? sender,
            ComparisonFinishedEventArgs e)
        {
            Logger?.Information($"Comparison {Token} finished with result " +
                $"{e.ComparisonResult}");

            if (e.ComparisonResult != ComparisonResult.Cancelled)
            {
                return;
            }

            lock (statusLock)
            {
                if (comparisonResult != null)
                {
                    return;
                }

                var last = frameComparisons.LastOrDefault();
                comparisonResult = new VideoComparisonResult
                {
                    Reason = "Comparison cancelled",
                    ComparisonResult = ComparisonResult.Cancelled,
                    LastComparedIndex = last?.LoadLevel ?? 0,
                };
            }

            Logger?.Information($"Comparison {Token} was cancelled");
        }

        private void HandleFrameCompared(
            object? sender,
            FrameComparedEventArgs e)
        {
            Logger?.Debug($"Comparison {Token} compared frame " +
                $"{e.FrameComparisonIndex}");
            lock (statusLock)
            {
                if (e.VideoComparisonResult != ComparisonResult.NoResult
                    && comparisonResult == null)
                {
                    comparisonResult = new VideoComparisonResult
                    {
                        Reason = "Comparison ran to completion",
                        ComparisonResult = e.VideoComparisonResult,
                        LastComparedIndex = e.FrameComparisonIndex,
                    };
                }

                frameComparisons.Add(new FrameComparisonResult
                {
                    Index = e.FrameComparisonIndex,
                    ComparisonResult = e.FrameComparisonResult,
                    Difference = e.Difference,
                    LoadLevel = e.FrameLoadLevelIndex,
                    LeftFrames = e.LeftFrames,
                    RightFrames = e.RightFrames,
                });
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Logger?.Information($"Disposing comparison {Token}");
                    cancelTokenSource?.Cancel();
                    comparerTask?.Wait();
                    cancelTokenSource?.Dispose();
                    comparerTask?.Dispose();

                    Logger?.Information($"Disposed comparison {Token}");
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
