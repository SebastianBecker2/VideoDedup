namespace CustomComparisonManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.IO;
    using VideoComparer = DedupEngine.VideoComparer;
    using ImageComparedEventArgs = DedupEngine.ImageComparedEventArgs;
    using ComparisonFinishedEventArgs = DedupEngine.ComparisonFinishedEventArgs;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.IVideoFileExtensions;

    internal class CustomComparison : IDisposable
    {
        public Guid Token { get; }
        private DedupEngine.VideoFile LeftVideoFile => comparer.LeftVideoFile;
        private DedupEngine.VideoFile RightVideoFile => comparer.RightVideoFile;

        private bool disposedValue;

        private readonly CancellationTokenSource cancelTokenSource = new();
        private readonly Task comparerTask;
        private readonly VideoComparer comparer;
        private readonly object statusLock = new();

        private VideoComparisonResult? comparisonResult;
        private readonly IList<ImageComparisonResult> imageComparisons =
            new List<ImageComparisonResult>();

        public CustomComparison(
            VideoComparisonSettings settings,
            string leftFilePath,
            string rightFilePath,
            bool forceLoadingAllImages)
        {
            Token = Guid.NewGuid();

            if (!File.Exists(leftFilePath))
            {
                throw new InvalidDataException("Left video file path invalid.");
            }
            if (!File.Exists(rightFilePath))
            {
                throw new InvalidDataException("Right video file path invalid.");
            }

            comparer = new VideoComparer(
                settings,
                new DedupEngine.VideoFile(leftFilePath),
                new DedupEngine.VideoFile(rightFilePath))
            {
                ForceLoadingAllImages = forceLoadingAllImages,
            };

            comparer.ImageCompared += HandleImageCompared;
            comparer.ComparisonFinished += HandleComparisonFinished;

            comparerTask = Task.Run(Compare);
        }

        public void CancelComparison() => cancelTokenSource.Cancel();

        public CustomVideoComparisonStatus GetStatus(
            int imageComparisonIndex = 0)
        {
            lock (statusLock)
            {
                var status = new CustomVideoComparisonStatus
                {
                    ComparisonToken = Token.ToString(),
                    LeftFile = LeftVideoFile.ToVideoFile(),
                    RightFile = RightVideoFile.ToVideoFile(),
                    VideoComparisonResult = comparisonResult,
                };

                status.ImageComparisons.AddRange(imageComparisons
                    .Skip(imageComparisonIndex)
                    .ToList());

                return status;
            }
        }

        private void Compare()
        {
            try
            {
                _ = comparer.Compare(cancelTokenSource.Token);
            }
            catch (Exception exc)
            {
                lock (statusLock)
                {
                    var last = imageComparisons.LastOrDefault();
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

                var last = imageComparisons.LastOrDefault();
                comparisonResult = new VideoComparisonResult
                {
                    Reason = "Comparison cancelled",
                    ComparisonResult = ComparisonResult.Cancelled,
                    LastComparedIndex = last?.LoadLevel ?? 0,
                };
            }
        }

        private void HandleImageCompared(
            object? sender,
            ImageComparedEventArgs e)
        {
            lock (statusLock)
            {
                if (e.VideoComparisonResult != ComparisonResult.NoResult
                    && comparisonResult == null)
                {
                    comparisonResult = new VideoComparisonResult
                    {
                        Reason = "Comparison ran to completion",
                        ComparisonResult = e.VideoComparisonResult,
                        LastComparedIndex = e.ImageComparisonIndex,
                    };
                }

                imageComparisons.Add(new ImageComparisonResult
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
                    cancelTokenSource?.Cancel();
                    comparerTask?.Wait();
                    cancelTokenSource?.Dispose();
                    comparerTask?.Dispose();
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
