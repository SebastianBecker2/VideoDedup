namespace DedupEngine
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using global::DedupEngine.MpvLib;
    using VideoDedupShared;
    using XnaFan.ImageComparison;

    public class VideoComparer
    {
        public IImageComparisonSettings Settings { get; set; }

        public VideoFile LeftVideoFile { get; set; }

        public VideoFile RightVideoFile { get; set; }

        public bool AlwaysLoadAllImages { get; set; } = false;

        public event EventHandler<ImageComparedEventArgs> ImageCompared;
        protected virtual void OnImageCompared(
            Func<ImageComparedEventArgs> eventArgsCreator) =>
            ImageCompared?.Invoke(this, eventArgsCreator.Invoke());

        public event EventHandler<ComparisonFinishedEventArgs> ComparisonFinished;
        protected virtual void OnComparisonFinished(
            Func<ComparisonFinishedEventArgs> eventArgsCreator) =>
            ComparisonFinished?.Invoke(this, eventArgsCreator.Invoke());

        private (MemoryStream ImageStream, int LoadLevel) LoadImage(
            VideoFile videoFile,
            int index,
            IImageComparisonSettings settings)
        {
            if (index >= settings.MaxImageCompares || index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                    "Index out of range.");
            }

            if (videoFile.ImageCount != 0
                && videoFile.ImageCount != settings.MaxImageCompares)
            {
                videoFile.DisposeImages();
            }
            videoFile.ImageCount = settings.MaxImageCompares;

            if (index < videoFile.ImageStreams.Count())
            {
                return (videoFile.ImageStreams[index], 0);
            }

            using (var mpv = new MpvWrapper(
                videoFile.FilePath,
                settings.MaxImageCompares,
                videoFile.Duration))
            {
                // First loading step, only the minimum (when index == 0)
                var imagesToLoad = settings.MaxDifferentImages + 1;
                var loadLevel = 1;
                // Second loading step
                if (index == settings.MaxDifferentImages + 1)
                {
                    imagesToLoad = settings.MaxImageCompares
                        - index
                        - settings.MaxDifferentImages;
                    loadLevel = 2;
                }
                // Third loading step
                else if (index
                    == settings.MaxImageCompares - settings.MaxDifferentImages)
                {
                    imagesToLoad = settings.MaxDifferentImages;
                    loadLevel = 3;
                }
                // To make sure we never load more than we initially wanted.
                imagesToLoad = Math.Min(
                    imagesToLoad,
                    settings.MaxImageCompares - index);
                foreach (var image in mpv.GetImages(index, imagesToLoad))
                {
                    videoFile.ImageStreams.Add(image);
                }
                return (videoFile.ImageStreams[index], loadLevel);
            }
        }

        public ComparisonResult Compare(CancellationToken cancelToken)
        {
            if (LeftVideoFile is null)
            {
                throw new ArgumentNullException(nameof(LeftVideoFile));
            }
            if (RightVideoFile is null)
            {
                throw new ArgumentNullException(nameof(RightVideoFile));
            }

            var differenceCount = 0;
            var comparisonResult = ComparisonResult.NoResult;
            var leftLoadLevel = 0;
            var rightLoadLevel = 0;
            foreach (var index in Enumerable.Range(0, Settings.MaxImageCompares))
            {
                if (cancelToken.IsCancellationRequested)
                {
                    comparisonResult = ComparisonResult.Cancelled;
                    break;
                }

                var (leftImageStream, lll) = LoadImage(
                    LeftVideoFile,
                    index,
                    Settings);
                leftLoadLevel = Math.Max(lll, leftLoadLevel);
                var leftImage = Image.FromStream(leftImageStream);

                var (rightImageStream, rll) = LoadImage(
                    RightVideoFile,
                    index,
                    Settings);
                rightLoadLevel = Math.Max(rll, rightLoadLevel);
                var rightImage = Image.FromStream(rightImageStream);

                var diff = leftImage.PercentageDifference(rightImage);

                ImageComparedEventArgs eventArgsCreator(ComparisonResult result) =>
                    new ImageComparedEventArgs
                    {
                        LeftVideoFile = LeftVideoFile,
                        RightVideoFile = RightVideoFile,
                        LeftImage = leftImageStream,
                        RightImage = rightImageStream,
                        ImageComparisonResult = result,
                        VideoComparisonResult = comparisonResult,
                        Difference = diff,
                        ImageLoadLevel = Math.Max(leftLoadLevel, rightLoadLevel),
                        ImageComparisonIndex = index,
                    };

                if (diff > (double)Settings.MaxImageDifferencePercent / 100)
                {
                    ++differenceCount;
                    OnImageCompared(
                        () => eventArgsCreator(ComparisonResult.Different));
                }
                else
                {
                    OnImageCompared(
                        () => eventArgsCreator(ComparisonResult.Duplicate));
                }

                // Early return when we already exceeded the number of
                // MaxDifferentImages
                if (differenceCount > Settings.MaxDifferentImages)
                {
                    comparisonResult = ComparisonResult.Different;
                    if (!AlwaysLoadAllImages)
                    {
                        break;
                    }
                }

                // Early return when there are not enough to compare left
                // to exceed the MaxDifferentImages
                if ((Settings.MaxImageCompares - (index + 1))
                    <= (Settings.MaxDifferentImages - differenceCount))
                {
                    comparisonResult = ComparisonResult.Duplicate;
                    if (!AlwaysLoadAllImages)
                    {
                        break;
                    }
                }
            }

            if (comparisonResult == ComparisonResult.NoResult)
            {
                comparisonResult = ComparisonResult.Duplicate;
            }

            OnComparisonFinished(() => new ComparisonFinishedEventArgs
            {
                LeftVideoFile = LeftVideoFile,
                RightVideoFile = RightVideoFile,
                ComparisonResult = comparisonResult,
            });
            return comparisonResult;
        }
    }
}
