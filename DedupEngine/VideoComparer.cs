namespace DedupEngine
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using global::DedupEngine.MpvLib;
    using VideoDedupShared;
    using VideoDedupShared.ImageExtension;

    public class VideoComparer
    {
        private class CacheableImageSet : ImageSet
        {
            public bool Loaded { get; set; } = false;
        }

        private static IOrderedEnumerable<ImageIndex> imageIndices;
        private static IOrderedEnumerable<ImageIndex> GetOrderedImageIndices(
            int imageCount)
        {
            // Make local copy of the reference
            var indices = imageIndices;
            if (indices is null || indices.Count() != imageCount)
            {
                indices = ImageIndex
                    .CreateImageIndices(imageCount)
                    .OrderBy(i => i);
                imageIndices = indices;
            }
            return indices;
        }

        private static IEnumerable<ImageIndex> GetOrderedImageIndices(
            int imageCount,
            LoadLevel loadLevel) =>
            GetOrderedImageIndices(imageCount)
                .Skip(loadLevel.ImageStartIndex)
                .Take(loadLevel.ImageCount);

        private static ImageSet ToImageSet(
            ImageIndex index,
            MemoryStream stream)
        {
            if (stream == null)
            {
                return new ImageSet { Index = index };
            }

            try
            {
                using (var image = (Bitmap)Image.FromStream(stream))
                using (var cropped = image?.CropBlackBars())
                using (var small = cropped?.Resize(DownscaleSize))
                using (var greysaled = small?.MakeGrayScale())
                {
                    return new ImageSet
                    {
                        Index = index,
                        Orignal = stream,
                        Cropped = cropped?.ToMemoryStream(),
                        Resized = small?.ToMemoryStream(),
                        Greyscaled = greysaled?.ToMemoryStream(),
                        Bytes = GetImageBytes(greysaled),
                    };
                }
            }
            catch (ArgumentNullException)
            {
                return new ImageSet { Index = index };
            }
        }

        private class LoadLevel
        {
            public int ImageCount { get; set; }
            public int ImageStartIndex { get; set; }
        }

        private static readonly int LoadLevelCount = 3;

        private static readonly Size DownscaleSize = new Size(16, 16);
        private const int ByteDifferenceThreshold = 3;

        private static byte[] GetImageBytes(Bitmap image)
        {
            unsafe
            {
                if (image is null)
                {
                    return null;
                }

                var bitmapData = image.LockBits(
                    new Rectangle(new Point(0, 0), DownscaleSize),
                    ImageLockMode.ReadOnly,
                    image.PixelFormat);
                var bytesPerPixel =
                    Image.GetPixelFormatSize(image.PixelFormat) / 8;
                var widthInBytes = bitmapData.Width * bytesPerPixel;

                return Enumerable
                    .Range(0, DownscaleSize.Height)
                    .SelectMany(y => Enumerable
                        .Range(0, DownscaleSize.Width)
                        .Select(x =>
                            ((byte*)bitmapData.Scan0
                                + (y * bitmapData.Stride)
                                + (x * bytesPerPixel))[0]))
                    .ToArray();
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
        private ImageComparedEventArgs CreateImageComparedEventArgs(
            int imageComparisonIndex,
            ImageSet leftImages,
            ImageSet rightImages,
            ComparisonResult imageComparisonResult,
            ComparisonResult videoComparisonResult,
            double difference,
            int loadLevel) =>
            new ImageComparedEventArgs
            {
                LeftVideoFile = LeftVideoFile,
                RightVideoFile = RightVideoFile,
                LeftImages = leftImages,
                RightImages = rightImages,
                ImageComparisonResult = imageComparisonResult,
                VideoComparisonResult = videoComparisonResult,
                Difference = difference,
                ImageLoadLevelIndex = loadLevel,
                ImageComparisonIndex = imageComparisonIndex,
            };

        public event EventHandler<ComparisonFinishedEventArgs> ComparisonFinished;
        protected virtual void OnComparisonFinished(
            Func<ComparisonFinishedEventArgs> eventArgsCreator) =>
            ComparisonFinished?.Invoke(this, eventArgsCreator.Invoke());

        private readonly EngineDatastore engineDatastore;

        public VideoComparer() { }

        public VideoComparer(string datastoreFilePath)
            : this(new EngineDatastore(datastoreFilePath))
        { }

        internal VideoComparer(EngineDatastore engineDatastore) : this() =>
            this.engineDatastore = engineDatastore;

        private static LoadLevel CalculateLoadLevel(
            int loadLevelIndex,
            IImageComparisonSettings settings)
        {
            if (loadLevelIndex <= 0 || loadLevelIndex >= 4)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(loadLevelIndex),
                    $"LoadLevel has to be between 1 and {LoadLevelCount}");
            }

            var startIndex = 0;
            var count = 0;
            if (loadLevelIndex == 1)
            {
                startIndex = 0;
                count = settings.MaxDifferentImages + 1;
            }
            else if (loadLevelIndex == 2)
            {
                startIndex = settings.MaxDifferentImages + 1;
                count = settings.MaxImageCompares
                    - (settings.MaxDifferentImages + 1)
                    - settings.MaxDifferentImages;
            }
            else if (loadLevelIndex == 3)
            {
                startIndex = Math.Max(
                    settings.MaxImageCompares - settings.MaxDifferentImages,
                    settings.MaxDifferentImages + 1);
                count = Math.Min(
                    settings.MaxDifferentImages,
                    settings.MaxImageCompares - (settings.MaxDifferentImages + 1));
            }

            // Make sure we are between 0 and MaxImageCompares at all time
            count = Math.Max(count, 0);
            count = Math.Min(count, settings.MaxImageCompares);
            startIndex = Math.Max(startIndex, 0);
            startIndex = Math.Min(startIndex, settings.MaxImageCompares - 1);
            return new LoadLevel
            {
                ImageCount = count,
                ImageStartIndex = startIndex
            };
        }

        private IEnumerable<ImageSet> LoadImagesFromFile(
            VideoFile videoFile,
            IEnumerable<ImageIndex> indices,
            CancellationToken cancelToken)
        {
            using (var mpv = new MpvWrapper(
                videoFile.FilePath,
                videoFile.Duration))
            {
                var images = mpv.GetImages(indices, cancelToken)
                    .ToList()
                    .Zip(indices, (stream, index) => (index, stream))
                    .Select(kvp => ToImageSet(kvp.index, kvp.stream));

                foreach (var image in images)
                {
                    // Cache in memory
                    videoFile.ImageBytes[image.Index] = image.Bytes;

                    // Cache in DB if we have a DB cache
                    if (engineDatastore is null)
                    {
                        continue;
                    }
                    engineDatastore.InsertImage(
                        image.Index,
                        image.Bytes,
                        videoFile);
                }

                return images;
            }
        }

        private IEnumerable<ImageSet> GetImagesFromFile(
            VideoFile videoFile,
            LoadLevel loadLevel,
            CancellationToken cancelToken)
        {
            if (videoFile.ImageCount != Settings.MaxImageCompares)
            {
                videoFile.ImageBytes.Clear();
                videoFile.ImageCount = Settings.MaxImageCompares;
            }

            var indices = GetOrderedImageIndices(
                Settings.MaxImageCompares,
                loadLevel);

            // If we need to call the ImageCompared event, we
            // cannot use the cached images since they are
            // already prepared and scaled down versions of the
            // images. For the ImageCompared event, we need
            // a copy of the image in every state.
            if (ImageCompared != null)
            {
                return LoadImagesFromFile(videoFile, indices, cancelToken);
            }

            var images = indices
                .Select(i => new CacheableImageSet { Index = i })
                .ToList();
            foreach (var image in images)
            {
                // Try to get image from memory cache
                image.Loaded = videoFile.ImageBytes.TryGetValue(
                    image.Index,
                    out var bytes);
                image.Bytes = bytes;
            }

            // Try to get the image from DB cache
            if (images.Any(i => !i.Loaded))
            {
                foreach (var dbImage in engineDatastore.GetImages(
                    images.Where(i => !i.Loaded).Select(i => i.Index),
                    videoFile))
                {
                    var image = images.First(i => i.Index == dbImage.Index);
                    image.Bytes = dbImage.Bytes;
                    image.Loaded = true;
                    // And advance the image into memory cache
                    videoFile.ImageBytes[dbImage.Index] = dbImage.Bytes;
                }
            }

            // Try to load the image from file
            if (images.Any(i => !i.Loaded))
            {
                foreach (var image in LoadImagesFromFile(
                    videoFile,
                    images.Where(i => !i.Loaded).Select(i => i.Index),
                    cancelToken))
                {
                    images.First(i => i.Index == image.Index).Bytes = image.Bytes;
                }
            }

            return images;
        }

        private ComparisonResult CompareLoadLevel(
            int loadLevelIndex,
            ref int differenceCount,
            CancellationToken cancelToken)
        {
            var loadLevel = CalculateLoadLevel(loadLevelIndex, Settings);

            var leftImages = GetImagesFromFile(
                LeftVideoFile,
                loadLevel,
                cancelToken).ToList();

            var rightImages = GetImagesFromFile(
                RightVideoFile,
                loadLevel,
                cancelToken).ToList();

            var videoComparisonResult = ComparisonResult.NoResult;

            foreach (var index in Enumerable
                    .Range(0, loadLevel.ImageCount))
            {
                if (cancelToken.IsCancellationRequested)
                {
                    videoComparisonResult = ComparisonResult.Cancelled;
                    break;
                }

                var imageComparisonResult = ComparisonResult.NoResult;
                var diff = 0.0f;

                // If we don't have either one of the images, we don't have
                // a result, but consider them different.
                if (leftImages[index].Bytes == null
                    || rightImages[index].Bytes == null)
                {
                    imageComparisonResult = ComparisonResult.NoResult;
                    ++differenceCount;

                    // Early return when we already exceeded the number of
                    // MaxDifferentImages
                    if (differenceCount > Settings.MaxDifferentImages)
                    {
                        videoComparisonResult = ComparisonResult.Different;
                    }
                }
                else
                {
                    diff = GetDifferenceOfBytes(
                        leftImages[index].Bytes,
                        rightImages[index].Bytes);

                    if (diff <= (double)Settings.MaxImageDifferencePercent / 100)
                    {
                        imageComparisonResult = ComparisonResult.Duplicate;

                        // Early return when there are not enough images left to
                        // compare to exceed the MaxDifferentImages
                        if ((Settings.MaxImageCompares - (index + loadLevel.ImageStartIndex + 1))
                            <= (Settings.MaxDifferentImages - differenceCount))
                        {
                            videoComparisonResult = ComparisonResult.Duplicate;
                        }
                    }
                    else
                    {
                        imageComparisonResult = ComparisonResult.Different;
                        ++differenceCount;

                        // Early return when we already exceeded the number of
                        // MaxDifferentImages
                        if (differenceCount > Settings.MaxDifferentImages)
                        {
                            videoComparisonResult = ComparisonResult.Different;
                        }
                    }
                }

                OnImageCompared(() => CreateImageComparedEventArgs(
                    index + loadLevel.ImageStartIndex,
                    leftImages[index],
                    rightImages[index],
                    imageComparisonResult,
                    videoComparisonResult,
                    diff,
                    loadLevelIndex));

                if (videoComparisonResult != ComparisonResult.NoResult
                    && !AlwaysLoadAllImages)
                {
                    break;
                }
            }

            return videoComparisonResult;
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
            foreach (var loadLevelIndex in Enumerable.Range(1, LoadLevelCount))
            {
                if (cancelToken.IsCancellationRequested)
                {
                    comparisonResult = ComparisonResult.Cancelled;
                    break;
                }

                comparisonResult = CompareLoadLevel(
                    loadLevelIndex,
                    ref differenceCount,
                    cancelToken);

                if (comparisonResult != ComparisonResult.NoResult
                    && !AlwaysLoadAllImages)
                {
                    break;
                }
            }

            // If we didn't stop comparison early because of a
            // precondition, then we end up here with no result.
            // Which means, the videos are considered to be duplicates.
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
