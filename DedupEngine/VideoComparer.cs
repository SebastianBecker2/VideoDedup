namespace DedupEngine
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using global::DedupEngine.MpvLib;
    using VideoDedupShared;
    using VideoDedupShared.ImageExtension;

    public class VideoComparer
    {
        private static readonly Size ImageDownscaleSize = new Size(16, 16);
        private static int ImageDownscalePixelCount =>
            ImageDownscaleSize.Width * ImageDownscaleSize.Height;
        private const int ByteDifferenceThreshold = 3;

        private static IEnumerable<byte> GetImageBytes(Bitmap image)
        {
            foreach (var y in Enumerable.Range(0, 16))
            {
                foreach (var x in Enumerable.Range(0, 16))
                {
                    yield return (byte)Math.Abs(image.GetPixel(x, y).R);
                }
            }
        }

        private static float GetDifferenceOfBytes(
            byte[] left,
            byte[] right,
            byte threshold = ByteDifferenceThreshold)
        {
            var diffBytes = Enumerable
                .Range(0, left.Count())
                .Aggregate((d, i) =>
                    Math.Abs(left[i] - right[i]) > threshold ? d + 1 : d);

            return diffBytes / 256f;
        }

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

        private (byte[] ImageBytes, int LoadLevel) LoadImage(
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
                videoFile.ImageBytes.Clear();
            }
            videoFile.ImageCount = settings.MaxImageCompares;

            if (index < videoFile.ImageBytes.Count())
            {
                return (videoFile.ImageBytes[index], 0);
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
                foreach (var imageStream in mpv.GetImages(index, imagesToLoad))
                {
                    using (var image = Image.FromStream(imageStream))
                    using (var smallImage = image.Resize(ImageDownscaleSize))
                    using (var greyScaleImage = smallImage.MakeGrayScale())
                    {
                        videoFile.ImageBytes.Add(
                            GetImageBytes(greyScaleImage)
                                .ToArray());
                    }
                    imageStream.Dispose();
                }

                if (index >= videoFile.ImageBytes.Count())
                {
                    return (Enumerable
                            .Repeat<byte>(0, ImageDownscalePixelCount)
                            .ToArray(),
                        loadLevel);
                }

                return (videoFile.ImageBytes[index], loadLevel);
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

                var (leftImageBytes, lll) = LoadImage(
                    LeftVideoFile,
                    index,
                    Settings);
                leftLoadLevel = Math.Max(lll, leftLoadLevel);

                var (rightImageBytes, rll) = LoadImage(
                    RightVideoFile,
                    index,
                    Settings);
                rightLoadLevel = Math.Max(rll, rightLoadLevel);

                var diff = GetDifferenceOfBytes(leftImageBytes, rightImageBytes);

                ImageComparedEventArgs eventArgsCreator(ComparisonResult result)
                {
                    using (var leftFileMpv = new MpvWrapper(
                        LeftVideoFile.FilePath,
                        Settings.MaxImageCompares,
                        LeftVideoFile.Duration))
                    using (var rightFileMpv = new MpvWrapper(
                        RightVideoFile.FilePath,
                        Settings.MaxImageCompares,
                        RightVideoFile.Duration))
                    {
                        return new ImageComparedEventArgs
                        {
                            LeftVideoFile = LeftVideoFile,
                            RightVideoFile = RightVideoFile,
                            LeftImage = leftFileMpv
                                .GetImages(index, 1)
                                .FirstOrDefault(),
                            RightImage = rightFileMpv
                                .GetImages(index, 1)
                                .FirstOrDefault(),
                            ImageComparisonResult = result,
                            VideoComparisonResult = comparisonResult,
                            Difference = diff,
                            ImageLoadLevel = Math.Max(leftLoadLevel, rightLoadLevel),
                            ImageComparisonIndex = index,
                        };
                    }
                }

                if (diff <= (double)Settings.MaxImageDifferencePercent / 100)
                {
                    // Early return when there are not enough images left to
                    // compare to exceed the MaxDifferentImages
                    if ((Settings.MaxImageCompares - (index + 1))
                        <= (Settings.MaxDifferentImages - differenceCount))
                    {
                        comparisonResult = ComparisonResult.Duplicate;
                    }

                    OnImageCompared(
                        () => eventArgsCreator(ComparisonResult.Duplicate));
                }
                else
                {
                    ++differenceCount;

                    // Early return when we already exceeded the number of
                    // MaxDifferentImages
                    if (differenceCount > Settings.MaxDifferentImages)
                    {
                        comparisonResult = ComparisonResult.Different;
                    }

                    OnImageCompared(
                        () => eventArgsCreator(ComparisonResult.Different));
                }

                if (comparisonResult != ComparisonResult.NoResult
                    && !AlwaysLoadAllImages)
                {
                    break;
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
