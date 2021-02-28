namespace VideoDedupConsole.CustomComparisonManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using DedupEngine;
    using Wcf.Contracts.Data;
    using VideoComparisonResult = VideoDedupShared.VideoComparisonResult;
    using ImageComparisonResult = VideoDedupShared.ImageComparisonResult;
    using ComparisonResult = VideoDedupShared.ComparisonResult;

    internal class CustomComparisonManager
    {
        private class CustomComparison : IDisposable
        {
            private bool disposedValue;

            public Guid Token { get; set; }
            private CustomVideoComparisonData Data { get; set; }
            private VideoFile LeftVideoFile { get; set; }
            private VideoFile RightVideoFile { get; set; }
            private CancellationTokenSource CancelTokenSource { get; set; } =
                new CancellationTokenSource();
            private VideoComparer Comparer { get; set; }
            private Task ComparerTask { get; set; }

            private CustomVideoComparisonStatusData Status { get; set; } =
                new CustomVideoComparisonStatusData();
            private IList<Tuple<int, ImageComparisonResult>> ImageComparisons
            { get; set; } = new List<Tuple<int, ImageComparisonResult>>();
            private object StatusLock { get; set; } = new object();

            public CustomComparison(CustomVideoComparisonData data)
            {
                Token = Guid.NewGuid();
                Data = data;
                LeftVideoFile = new VideoFile(data.LeftFilePath);
                RightVideoFile = new VideoFile(data.RightFilePath);
                Status.ImageComparisons = ImageComparisons;

                Comparer = new VideoComparer
                {
                    LeftVideoFile = LeftVideoFile,
                    RightVideoFile = RightVideoFile,
                    Settings = Data,
                    AlwaysLoadAllImages = true,
                };

                Comparer.ImageCompared += HandleImageCompared;
                Comparer.ComparisonFinished += HandleComparisonFinished;

                ComparerTask = Task.Factory.StartNew(
                    () => Comparer.Compare(CancelTokenSource.Token));
            }

            public void CancelComparison() => CancelTokenSource.Cancel();

            public CustomVideoComparisonStatusData GetStatus(
                int imageComparisonIndex = 0)
            {
                lock (StatusLock)
                {
                    return new CustomVideoComparisonStatusData
                    {
                        ImageComparisons =
                            Status.ImageComparisons
                                .Skip(imageComparisonIndex)
                                .ToList(),
                        Token = Token,
                        VideoCompareResult = Status.VideoCompareResult,
                        LeftVideoFile =
                            new VideoDedupShared.VideoFile(LeftVideoFile),
                        RightVideoFile =
                            new VideoDedupShared.VideoFile(RightVideoFile),
                    };
                }
            }

            private void HandleComparisonFinished(
                object sender,
                ComparisonFinishedEventArgs e)
            {
                if (e.ComparisonResult
                    != ComparisonResult.Cancelled)
                {
                    return;
                }

                lock (StatusLock)
                {
                    if (Status.VideoCompareResult != null)
                    {
                        return;
                    }

                    var last = ImageComparisons.LastOrDefault()?.Item2;
                    Status.VideoCompareResult = new VideoComparisonResult
                    {
                        ComparisonResult = ComparisonResult.Cancelled,
                        LastComparisonIndex =
                            last != null ? last.ImageLoadLevel : 0,
                    };
                }
            }

            private void HandleImageCompared(
                object sender,
                ImageComparedEventArgs e)
            {
                lock (StatusLock)
                {
                    if (e.VideoComparisonResult != ComparisonResult.NoResult
                        && Status.VideoCompareResult == null)
                    {
                        Status.VideoCompareResult = new VideoComparisonResult
                        {
                            ComparisonResult = e.VideoComparisonResult,
                            LastComparisonIndex = e.ImageComparisonIndex - 1,
                        };
                    }

                    Status.Token = Token;
                    ImageComparisons.Add(Tuple.Create(
                        e.ImageComparisonIndex,
                        new ImageComparisonResult
                        {
                            ComparisonResult = e.ImageComparisonResult,
                            Difference = e.Difference,
                            ImageLoadLevel = e.ImageLoadLevel,
                            LeftImage = e.LeftImage,
                            RightImage = e.RightImage,
                        }));
                }
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        CancelTokenSource?.Cancel();
                        ComparerTask?.Wait();
                        CancelTokenSource?.Dispose();
                        ComparerTask?.Dispose();
                        LeftVideoFile?.Dispose();
                        RightVideoFile?.Dispose();
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

        private IDictionary<Guid, CustomComparison> CustomComparisons { get; set; }
            = new Dictionary<Guid, CustomComparison>();

        public CustomVideoComparisonStartData StartCustomComparison(
            CustomVideoComparisonData customVideoComparisonData)
        {
            try
            {
                var customComparison = new CustomComparison(
                    customVideoComparisonData);

                CustomComparisons.Add(customComparison.Token, customComparison);

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
            if (!CustomComparisons.TryGetValue(token, out var customComparison))
            {
                return null;
            }

            return customComparison.GetStatus(imageComparisonIndex);
        }

        public bool CancelCustomComparison(Guid token)
        {
            if (!CustomComparisons.TryGetValue(token, out var customComparison))
            {
                return false;
            }

            customComparison.CancelComparison();
            return true;
        }
    }
}
