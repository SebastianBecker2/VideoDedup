namespace VideoComparer
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using EventArgs;
    using Google.Protobuf;
    using KGySoft.Drawing;
    using MpvLib;
    using MpvLib.Exceptions;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.ImageExtensions;
    using ImageIndex = MpvLib.ImageIndex;
    using Size = System.Drawing.Size;

    public class VideoComparer(
        VideoComparisonSettings settings,
        VideoFile leftVideoFile,
        VideoFile rightVideoFile)
    {
        public VideoComparer(
            VideoComparisonSettings settings,
            string datastorePath,
            VideoFile leftVideoFile,
            VideoFile rightVideoFile)
            : this(settings, leftVideoFile, rightVideoFile) =>
            comparerDatastore = new ComparerDatastore(datastorePath);

        private sealed class CacheableImageSet
        {
            private CacheableImageSet(ImageIndex index) =>
                Index = index;

            public static CacheableImageSet FromPreprocessedImage(
                ImageIndex index,
                byte[]? preprocessedImage,
                bool loaded = false) =>
                new(index)
                {
                    Bytes = preprocessedImage,
                    Loaded = loaded
                };

            public static CacheableImageSet FromOriginalImage(
                ImageIndex index,
                byte[]? originalImage,
                bool provideIntermediateImages = false)
            {
                if (originalImage is null)
                {
                    return new(index);
                }

                try
                {
                    var stream = new MemoryStream(originalImage);
                    using var image = (Bitmap)Image.FromStream(stream);
                    using var cropped = image.CropBlackBars();
                    using var small = cropped?.Resize(
                        DownscaleSize,
                        ScalingMode.NearestNeighbor,
                        false);
                    using var greyscaled = small?.MakeGrayScale();

                    var imageSet = new CacheableImageSet(index)
                    {
                        Original = stream,
                        Bytes = GetImageBytes(greyscaled),
                        Loaded = true,
                    };

                    if (provideIntermediateImages)
                    {
                        imageSet.Cropped = cropped?.ToMemoryStream();
                        imageSet.Resized = small?.ToMemoryStream();
                        imageSet.Greyscaled = greyscaled?.ToMemoryStream();
                    }

                    imageSet.Original.Position = 0;
                    return imageSet;
                }
                catch (ArgumentNullException)
                {
                    return new CacheableImageSet(index);
                }
            }

            public ImageSet ToImageSet()
            {
                static ByteString ToByteString(Stream? stream)
                {
                    if (stream is null)
                    {
                        return ByteString.Empty;
                    }
                    return ByteString.FromStream(stream);
                }
                return new()
                {
                    Index = Index,
                    Original = ToByteString(Original),
                    Cropped = ToByteString(Cropped),
                    Resized = ToByteString(Resized),
                    Greyscaled = ToByteString(Greyscaled),
                    Bytes = ByteString.CopyFrom(Bytes ?? []),
                };
            }

            public ImageIndex Index { get; }
            private MemoryStream? Original { get; init; }
            private MemoryStream? Cropped { get; set; }
            private MemoryStream? Resized { get; set; }
            private MemoryStream? Greyscaled { get; set; }
            public byte[]? Bytes { get; set; }
            public bool Loaded { get; set; }
        }

        private static IList<ImageIndex>? imageIndices;
        private static IEnumerable<ImageIndex> GetOrderedImageIndices(
            int imageCount)
        {
            // Make local copy of the reference
            var indices = imageIndices;
            if (indices is null || indices.Count != imageCount)
            {
                indices = [.. ImageIndex
                    .CreateImageIndices(imageCount)
                    .OrderBy(i => i.Denominator)
                    .ThenBy(i => i.Numerator)];
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

        private sealed class LoadLevel
        {
            public int ImageCount { get; init; }
            public int ImageStartIndex { get; init; }
        }

        private const int LoadLevelCount = 3;

        private static readonly Size DownscaleSize = new(16, 16);
        private const int ByteDifferenceThreshold = 3;

        private static byte[]? GetImageBytes(Bitmap? image)
        {
            if (image is null)
            {
                return null;
            }
            unsafe
            {
                var bitmapData = image.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadOnly,
                    image.PixelFormat);

                var bytesPerPixel =
                    Image.GetPixelFormatSize(image.PixelFormat) / 8;
                return Enumerable
                    .Range(0, image.Height)
                    .SelectMany(y => Enumerable
                        .Range(0, image.Width)
                        .Select(x =>
                            ((byte*)bitmapData.Scan0
                                + (y * bitmapData.Stride)
                                + (x * bytesPerPixel))[0]))
                    .ToArray();
            }
        }

        private static float GetDifferenceOfBytes(
            IReadOnlyList<byte> left,
            IReadOnlyList<byte> right,
            byte threshold = ByteDifferenceThreshold)
        {
            var diffBytes = Enumerable
                .Range(0, left.Count)
                .Aggregate((d, i) =>
                    Math.Abs(left[i] - right[i]) > threshold ? d + 1 : d);

            return diffBytes / 256f;
        }

        public VideoComparisonSettings Settings { get; } = settings;

        public VideoFile LeftVideoFile { get; } = leftVideoFile;

        public VideoFile RightVideoFile { get; } = rightVideoFile;

        public bool ForceLoadingAllImages { get; set; }

        public event EventHandler<ImageComparedEventArgs>? ImageCompared;
        protected virtual void OnImageCompared(
            Func<ImageComparedEventArgs> eventArgsCreator) =>
            ImageCompared?.Invoke(this, eventArgsCreator.Invoke());
        private ImageComparedEventArgs CreateImageComparedEventArgs(
            int imageComparisonIndex,
            CacheableImageSet leftImages,
            CacheableImageSet rightImages,
            ComparisonResult imageComparisonResult,
            ComparisonResult videoComparisonResult,
            double difference,
            int loadLevel) =>
            new()
            {
                LeftVideoFile = LeftVideoFile,
                RightVideoFile = RightVideoFile,
                LeftImages = leftImages.ToImageSet(),
                RightImages = rightImages.ToImageSet(),
                ImageComparisonResult = imageComparisonResult,
                VideoComparisonResult = videoComparisonResult,
                Difference = difference,
                ImageLoadLevelIndex = loadLevel,
                ImageComparisonIndex = imageComparisonIndex,
            };

        public event EventHandler<ComparisonFinishedEventArgs>? ComparisonFinished;
        protected virtual void OnComparisonFinished(
            Func<ComparisonFinishedEventArgs> eventArgsCreator) =>
            ComparisonFinished?.Invoke(this, eventArgsCreator.Invoke());

        private readonly ComparerDatastore? comparerDatastore;

        private static LoadLevel CalculateLoadLevel(
            int loadLevelIndex,
            VideoComparisonSettings settings)
        {
            if (loadLevelIndex is <= 0 or >= 4)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(loadLevelIndex),
                    $"LoadLevel has to be between 1 and {LoadLevelCount}");
            }

            int startIndex;
            int count;
            if (loadLevelIndex == 1)
            {
                startIndex = 0;
                count = settings.MaxDifferentImages + 1;
            }
            else if (loadLevelIndex == 2)
            {
                startIndex = settings.MaxDifferentImages + 1;
                count = settings.CompareCount
                    - (settings.MaxDifferentImages + 1)
                    - settings.MaxDifferentImages;
            }
            else
            {
                startIndex = Math.Max(
                    settings.CompareCount - settings.MaxDifferentImages,
                    settings.MaxDifferentImages + 1);
                count = Math.Min(
                    settings.MaxDifferentImages,
                    settings.CompareCount - (settings.MaxDifferentImages + 1));
            }

            // Make sure we are between 0 and MaxImageCompares at all time
            count = Math.Max(count, 0);
            count = Math.Min(count, settings.CompareCount);
            startIndex = Math.Max(startIndex, 0);
            startIndex = Math.Min(startIndex, settings.CompareCount - 1);
            return new LoadLevel
            {
                ImageCount = count,
                ImageStartIndex = startIndex
            };
        }

        private IEnumerable<CacheableImageSet> LoadImagesFromFile(
            VideoFile videoFile,
            IEnumerable<ImageIndex> indices,
            CancellationToken cancelToken)
        {
            try
            {
                using var mpv = new MpvWrapper(
                   videoFile.FilePath,
                   videoFile.Duration);
                var images = mpv.GetImages(indices, cancelToken)
                    .ToList()
                    .Zip(indices, (stream, index) => (index, stream))
                    // Converting to image set including the intermediate
                    // images if necessary.
                    .Select(kvp => CacheableImageSet.FromOriginalImage(
                        kvp.index,
                        kvp.stream,
                        ImageCompared != null));

                foreach (var image in images)
                {
                    // Cache in memory
                    videoFile.ImageBytes[image.Index] = image.Bytes;

                    // Cache in DB if we have a DB cache
                    comparerDatastore?.InsertImage(
                        image.Index,
                        image.Bytes,
                        videoFile);
                }

                return images;
            }
            catch (MpvException exc)
            {
                throw new ComparisonException(exc.Message, videoFile, exc);
            }
        }

        private IEnumerable<CacheableImageSet> GetImagesFromFile(
            VideoFile videoFile,
            LoadLevel loadLevel,
            CancellationToken cancelToken)
        {
            if (videoFile.ImageCount != Settings.CompareCount)
            {
                videoFile.ImageBytes.Clear();
                videoFile.ImageCount = Settings.CompareCount;
            }

            var indices = GetOrderedImageIndices(
                Settings.CompareCount,
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

            var images = indices.Select(index =>
            {
                // Try to get image from memory cache
                _ = videoFile.ImageBytes.TryGetValue(
                    index,
                    out var bytes);
                return CacheableImageSet.FromPreprocessedImage(index, bytes);
            }).ToList();

            // Try to get the image from DB cache
            if (comparerDatastore is not null && images.Any(i => !i.Loaded))
            {
                foreach (var (index, bytes) in comparerDatastore.GetImages(
                    images.Where(i => !i.Loaded).Select(i => i.Index),
                    videoFile))
                {
                    var image = images.First(i => i.Index == index);
                    image.Bytes = bytes;
                    image.Loaded = true;
                    // And advance the image into memory cache
                    videoFile.ImageBytes[index] = bytes;
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

                ComparisonResult imageComparisonResult;
                var diff = 0.0f;

                var leftImageBytes = leftImages[index].Bytes;
                var rightImageBytes = rightImages[index].Bytes;
                // If we don't have either one of the images, we don't have
                // a result, but consider them different.
                if (leftImageBytes == null || rightImageBytes == null)
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
                    diff = GetDifferenceOfBytes(leftImageBytes, rightImageBytes);

                    if (diff <= (double)Settings.MaxDifference / 100)
                    {
                        imageComparisonResult = ComparisonResult.Duplicate;

                        // Early return when there are not enough images left to
                        // compare to exceed the MaxDifferentImages
                        if ((Settings.CompareCount - (index + loadLevel.ImageStartIndex + 1))
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
                    && !ForceLoadingAllImages)
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
                throw new InvalidOperationException("Object reference of " +
                    $"{nameof(LeftVideoFile)} not set to an instance of an " +
                    "object.");
            }
            if (RightVideoFile is null)
            {
                throw new InvalidOperationException("Object reference of " +
                    $"{nameof(RightVideoFile)} not set to an instance of an " +
                    "object.");
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
                    && !ForceLoadingAllImages)
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

            OnComparisonFinished(() => new ComparisonFinishedEventArgs(
                LeftVideoFile,
                RightVideoFile)
            {
                ComparisonResult = comparisonResult,
            });
            return comparisonResult;
        }
    }
}
