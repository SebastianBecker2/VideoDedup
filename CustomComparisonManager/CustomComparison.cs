namespace CustomComparisonManager
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
    using System.IO;

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
        private IList<ImageComparisonResult> ImageComparisons { get; set; } =
            new List<ImageComparisonResult>();
        private object StatusLock { get; set; } = new object();

        public CustomComparison(CustomVideoComparisonData data)
        {
            Token = Guid.NewGuid();
            Data = data;
            Status.ImageComparisons = ImageComparisons;

            if (!File.Exists(data.LeftFilePath))
            {
                throw new InvalidDataException("Left video file path invalid.");
            }
            LeftVideoFile = new VideoFile(data.LeftFilePath);

            if (!File.Exists(data.RightFilePath))
            {
                throw new InvalidDataException("Right video file path invalid.");
            }
            RightVideoFile = new VideoFile(data.RightFilePath);

            Comparer = new VideoComparer
            {
                LeftVideoFile = LeftVideoFile,
                RightVideoFile = RightVideoFile,
                Settings = Data,
                AlwaysLoadAllImages = Data.AlwaysLoadAllImages,
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
                var statusData = new CustomVideoComparisonStatusData
                {
                    ImageComparisons =
                        Status.ImageComparisons
                            .Skip(imageComparisonIndex)
                            .ToList(),
                    ComparisonToken = Token,
                    VideoComparisonResult = Status.VideoComparisonResult,
                };
                if (LeftVideoFile != null)
                {
                    statusData.LeftVideoFile =
                        new VideoDedupShared.VideoFile(LeftVideoFile);
                }
                if (RightVideoFile != null)
                {
                    statusData.RightVideoFile =
                        new VideoDedupShared.VideoFile(RightVideoFile);
                }
                return statusData;
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
                    Status.VideoComparisonResult = new VideoComparisonResult
                    {
                        Reason = exc.Message,
                        ComparisonResult = ComparisonResult.Aborted,
                        LastComparedIndex = last?.Index ?? 0,
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
                if (Status.VideoComparisonResult != null)
                {
                    return;
                }

                var last = ImageComparisons.LastOrDefault();
                Status.VideoComparisonResult = new VideoComparisonResult
                {
                    Reason = "Comparison cancelled",
                    ComparisonResult = ComparisonResult.Cancelled,
                    LastComparedIndex = last?.LoadLevel ?? 0,
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
                    && Status.VideoComparisonResult == null)
                {
                    Status.VideoComparisonResult = new VideoComparisonResult
                    {
                        Reason = "Comparison ran to completion",
                        ComparisonResult = e.VideoComparisonResult,
                        LastComparedIndex = e.ImageComparisonIndex,
                    };
                }

                Status.ComparisonToken = Token;
                ImageComparisons.Add(
                    new ImageComparisonResult
                    {
                        Index = e.ImageComparisonIndex,
                        ComparisonResult = e.ImageComparisonResult,
                        Difference = e.Difference,
                        LoadLevel = e.ImageLoadLevelIndex,
                        LeftImages = e.LeftImages,
                        RightImages = e.RightImages,
                    });
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
