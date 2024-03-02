namespace ComparisonManager
{
    using Serilog;
    using SmartTimer;
    using VideoDedupGrpc;

    public class ComparisonManager : IDisposable
    {
        private static readonly TimeSpan TimeoutTimerInterval =
            TimeSpan.FromMinutes(1);
        private static readonly TimeSpan TimeoutTime = TimeSpan.FromMinutes(5);

        private bool disposedValue;

        private ILogger? Logger { get; }

        private Timer TimeoutTimer { get; }

        private IDictionary<Guid, Comparison> Comparisons { get; }
            = new Dictionary<Guid, Comparison>();
        private IDictionary<Guid, DateTime> LastRequests { get; } =
            new Dictionary<Guid, DateTime>();

        private object ComparisonsLock { get; } = new();

        public ComparisonManager() =>
            TimeoutTimer = new Timer(
                HandleTimeoutTimerTick,
                null,
                TimeoutTimerInterval);

        public ComparisonManager(ILogger logger) : this()
        {
            Logger = logger;
            Logger.Information("Started ComparisonManager");
        }

        public VideoComparisonStatus StartComparison(
            VideoComparisonSettings settings,
            string leftFilePath,
            string rightFilePath,
            bool forceLoadingAllImages)
        {
            Logger?.Information($"Starting comparison '{leftFilePath}' - " +
                $"'{rightFilePath}'");

            try
            {
                var comparison = new Comparison(
                    settings,
                    leftFilePath,
                    rightFilePath,
                    forceLoadingAllImages,
                    Logger);

                lock (ComparisonsLock)
                {
                    Comparisons.Add(
                        comparison.Token,
                        comparison);
                    LastRequests[comparison.Token] = DateTime.Now;

                    Logger?.Information($"Started comparison '{leftFilePath}' " +
                        $"- '{rightFilePath}' as {comparison.Token}");
                    return comparison.GetStatus();
                }
            }
            catch (InvalidDataException ex)
            {
                Logger?.Error($"Failed to start comparison '{leftFilePath}' - " +
                    $"'{rightFilePath}'");

                return new VideoComparisonStatus
                {
                    VideoComparisonResult = new VideoComparisonResult
                    {
                        ComparisonResult = ComparisonResult.Aborted,
                        Reason = ex.Message,
                    },
                };
            }
        }

        public VideoComparisonStatus? GetStatus(
            Guid token,
            int imageComparisonIndex = 0)
        {
            lock (ComparisonsLock)
            {
                if (!Comparisons.TryGetValue(token, out var comparison))
                {
                    return null;
                }

                LastRequests[token] = DateTime.Now;
                return comparison.GetStatus(imageComparisonIndex);
            }
        }

        public bool CancelComparison(Guid token)
        {
            Logger?.Information($"Cancelling comparison {token}");
            lock (ComparisonsLock)
            {
                if (!Comparisons.TryGetValue(token, out var comparison))
                {
                    return false;
                }

                comparison.CancelComparison();
                comparison.Dispose();

                _ = LastRequests.Remove(token);
                return Comparisons.Remove(token);
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
                    _ = CancelComparison(token);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Logger?.Information("Stopping ComparisonManager");
                    TimeoutTimer.Dispose();
                    lock (ComparisonsLock)
                    {
                        foreach (var comparison in Comparisons
                            .Select(kvp => kvp.Value))
                        {
                            comparison.CancelComparison();
                            comparison.Dispose();
                        }
                        Comparisons.Clear();
                        LastRequests.Clear();
                    }
                    Logger?.Information("Stopped ComparisonManager");
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
