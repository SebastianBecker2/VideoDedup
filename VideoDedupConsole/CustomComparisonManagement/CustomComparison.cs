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

    internal class CustomComparison : IDisposable
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
            Status.ImageComparisons = ImageComparisons;

            try
            {
                LeftVideoFile = new VideoFile(data.LeftFilePath);
            }
            catch (ArgumentException)
            {
                Status.VideoCompareResult = new VideoComparisonResult
                {
                    Reason = "Left video file path invalid.",
                    ComparisonResult = ComparisonResult.Aborted,
                    LastComparisonIndex = 0,
                };
            }

            try
            {
                RightVideoFile = new VideoFile(data.RightFilePath);
            }
            catch (ArgumentException)
            {
                Status.VideoCompareResult = new VideoComparisonResult
                {
                    Reason = "Right video file path invalid.",
                    ComparisonResult = ComparisonResult.Aborted,
                    LastComparisonIndex = 0,
                };
            }

            Comparer = new VideoComparer
            {
                LeftVideoFile = LeftVideoFile,
                RightVideoFile = RightVideoFile,
                Settings = Data,
                AlwaysLoadAllImages = true,
            };

            Comparer.ImageCompared += HandleImageCompared;
            Comparer.ComparisonFinished += HandleComparisonFinished;

            ComparerTask = Task.Factory.StartNew(Compare);
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

        private void Compare()
        {
            try
            {
                _ = Comparer.Compare(CancelTokenSource.Token);
            }
            catch (Exception exc)
            {
                lock (StatusLock)
                {
                    var last = ImageComparisons.LastOrDefault();
                    Status.VideoCompareResult = new VideoComparisonResult
                    {
                        Reason = exc.Message,
                        ComparisonResult = ComparisonResult.Aborted,
                        LastComparisonIndex = last != null ? last.Item1 : 0,
                    };
                }
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
                    Reason = "Comparison cancelled",
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
                        Reason = "Comparison ran to completion",
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

}
