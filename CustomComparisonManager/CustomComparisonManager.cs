namespace CustomComparisonManager
{
    using Serilog;
    using SmartTimer;
    using VideoDedupGrpc;

    public class CustomComparisonManager : IDisposable
    {
        private static readonly TimeSpan TimeoutTimerInterval =
            TimeSpan.FromMinutes(1);
        private static readonly TimeSpan TimeoutTime = TimeSpan.FromMinutes(5);

        private bool disposedValue;

        private ILogger? Logger { get; }

        private Timer TimeoutTimer { get; }

        private IDictionary<Guid, CustomComparison> CustomComparisons { get; }
            = new Dictionary<Guid, CustomComparison>();
        private IDictionary<Guid, DateTime> LastRequests { get; } =
            new Dictionary<Guid, DateTime>();

        private object ComparisonsLock { get; } = new();

        public CustomComparisonManager() =>
            TimeoutTimer = new Timer(
                HandleTimeoutTimerTick,
                null,
                TimeoutTimerInterval);

        public CustomComparisonManager(ILogger logger) : this()
        {
            Logger = logger;
            Logger.Information("Started CustomComparisonManager");
        }

        public CustomVideoComparisonStatus StartCustomComparison(
            VideoComparisonSettings settings,
            string leftFilePath,
            string rightFilePath,
            bool forceLoadingAllImages)
        {
            Logger?.Information($"Starting comparison '{leftFilePath}' - " +
                $"'{rightFilePath}'");

            try
            {
                var customComparison = new CustomComparison(
                    settings,
                    leftFilePath,
                    rightFilePath,
                    forceLoadingAllImages,
                    Logger);

                lock (ComparisonsLock)
                {
                    CustomComparisons.Add(
                        customComparison.Token,
                        customComparison);
                    LastRequests[customComparison.Token] = DateTime.Now;

                    Logger?.Information($"Started comparison '{leftFilePath}' " +
                        $"- '{rightFilePath}' as {customComparison.Token}");
                    return customComparison.GetStatus();
                }
            }
            catch (InvalidDataException ex)
            {
                Logger?.Error($"Failed to start comparison '{leftFilePath}' - " +
                    $"'{rightFilePath}'");

                return new CustomVideoComparisonStatus
                {
                    VideoComparisonResult = new VideoComparisonResult
                    {
                        ComparisonResult = ComparisonResult.Aborted,
                        Reason = ex.Message,
                    },
                };
            }
        }

        public CustomVideoComparisonStatus? GetStatus(
            Guid token,
            int imageComparisonIndex = 0)
        {
            lock (ComparisonsLock)
            {
                if (!CustomComparisons.TryGetValue(
                    token,
                    out var customComparison))
                {
                    return null;
                }

                LastRequests[token] = DateTime.Now;
                return customComparison.GetStatus(imageComparisonIndex);
            }
        }

        public bool CancelCustomComparison(Guid token)
        {
            Logger?.Information($"Cancelling comparison {token}");
            lock (ComparisonsLock)
            {
                if (!CustomComparisons.TryGetValue(
                    token,
                    out var customComparison))
                {
                    return false;
                }

                customComparison.CancelComparison();
                customComparison.Dispose();

                _ = LastRequests.Remove(token);
                return CustomComparisons.Remove(token);
            }
        }

        private void HandleTimeoutTimerTick(object? sender)
        {
            var now = DateTime.Now;
            lock (ComparisonsLock)
            {
                foreach (var token in LastRequests
                    .Where(kvp => (now - kvp.Value) > TimeoutTime)
                    .Select(kvp => kvp.Key)
                    .ToList())
                {
                    Logger?.Information($"Removing obsolete comparison {token}");
                    _ = CancelCustomComparison(token);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Logger?.Information("Stopping CustomComparisonManager");
                    TimeoutTimer.Dispose();
                    lock (ComparisonsLock)
                    {
                        foreach (var comparison in CustomComparisons
                            .Select(kvp => kvp.Value))
                        {
                            comparison.CancelComparison();
                            comparison.Dispose();
                        }
                        CustomComparisons.Clear();
                        LastRequests.Clear();
                    }
                    Logger?.Information("Stopped CustomComparisonManager");
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
