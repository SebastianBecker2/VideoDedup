namespace CustomComparisonManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Wcf.Contracts.Data;

    public class CustomComparisonManager : IDisposable
    {
        private static readonly TimeSpan TimeoutTimerInterval =
            TimeSpan.FromMinutes(1);
        private static readonly TimeSpan TimeoutTime = TimeSpan.FromMinutes(5);

        private bool disposedValue;

        private SmartTimer.Timer TimeoutTimer { get; set; }

        private IDictionary<Guid, CustomComparison> CustomComparisons { get; set; }
            = new Dictionary<Guid, CustomComparison>();
        private IDictionary<Guid, DateTime> LastRequests { get; set; } =
            new Dictionary<Guid, DateTime>();

        private object ComparisonsLock { get; } = new object();

        public CustomComparisonManager() =>
            TimeoutTimer = new SmartTimer.Timer(
                HandleTimeoutTimerTick,
                null,
                TimeoutTimerInterval);

        public CustomVideoComparisonStartData StartCustomComparison(
            CustomVideoComparisonData customVideoComparisonData)
        {
            try
            {
                var customComparison = new CustomComparison(
                    customVideoComparisonData);

                lock (ComparisonsLock)
                {
                    CustomComparisons.Add(
                        customComparison.Token,
                        customComparison);
                    LastRequests[customComparison.Token] = DateTime.Now;
                }

                return new CustomVideoComparisonStartData
                {
                    ComparisonToken = customComparison.Token,
                };
            }
            catch (Exception exc)
            {
                return new CustomVideoComparisonStartData
                {
                    ErrorMessage = exc.Message,
                };
            }
        }

        public CustomVideoComparisonStatusData GetStatus(
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

        private void HandleTimeoutTimerTick(object _)
        {
            var now = DateTime.Now;
            lock (ComparisonsLock)
            {
                foreach (var token in LastRequests
                    .Where(kvp => (now - kvp.Value) > TimeoutTime)
                    .Select(kvp => kvp.Key)
                    .ToList())
                {
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
                    TimeoutTimer?.Dispose();
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
