namespace VideoComparer
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Numerics;
    using System.Runtime.Intrinsics.X86;
    using System.Runtime.Intrinsics;
    using EventArgs;
    using Google.Protobuf;
    using KGySoft.Drawing;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.ImageExtensions;
    using Size = System.Drawing.Size;
    using FfmpegLib;
    using FfmpegLib.Exceptions;
    using FrameIndex = FfmpegLib.FrameIndex;

    public class VideoComparer(
        VideoComparisonSettings settings,
        VideoFile leftVideoFile,
        VideoFile rightVideoFile)
    {
        private static readonly Dictionary<string, ComparerDatastore>
            DatastoreCache = [];
        private static readonly object DatastoreCacheMutex = new();

        public VideoComparer(
            VideoComparisonSettings settings,
            string datastorePath,
            VideoFile leftVideoFile,
            VideoFile rightVideoFile)
            : this(settings, leftVideoFile, rightVideoFile)
        {
            lock (DatastoreCacheMutex)
            {
                if (DatastoreCache.TryGetValue(datastorePath, out var datastore))
                {
                    comparerDatastore = datastore;
                    return;
                }
                comparerDatastore = new ComparerDatastore(datastorePath);
                DatastoreCache.Add(datastorePath, comparerDatastore);
            }
        }

        private sealed class CacheableFrameSet
        {
            private CacheableFrameSet(FrameIndex index) =>
                Index = index;

            public static CacheableFrameSet FromPreprocessedFrame(
                FrameIndex index,
                byte[]? preprocessedFrame,
                bool loaded = false) =>
                new(index)
                {
                    Bytes = preprocessedFrame,
                    Loaded = loaded
                };

            public static CacheableFrameSet FromOriginalFrame(
                FrameIndex index,
                byte[]? originalFrame,
                bool provideIntermediateFrames = false)
            {
                if (originalFrame is null)
                {
                    return new(index);
                }

                try
                {
                    var stream = new MemoryStream(originalFrame);
                    using var frame = (Bitmap)Image.FromStream(stream);
                    using var cropped = frame.CropBlackBars();
                    using var small = cropped?.Resize(
                        DownscaleSize,
                        ScalingMode.NearestNeighbor,
                        false);
                    using var greyscaled = small?.MakeGrayScale();

                    var frameSet = new CacheableFrameSet(index)
                    {
                        Original = stream,
                        Bytes = GetFrameBytes(greyscaled),
                        Loaded = true,
                    };

                    if (provideIntermediateFrames)
                    {
                        frameSet.Cropped = cropped?.ToMemoryStream();
                        frameSet.Resized = small?.ToMemoryStream();
                        frameSet.Greyscaled = greyscaled?.ToMemoryStream();
                    }

                    frameSet.Original.Position = 0;
                    return frameSet;
                }
                catch (ArgumentNullException)
                {
                    return new CacheableFrameSet(index);
                }
            }

            public FrameSet ToFrameSet()
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

            public FrameIndex Index { get; }
            private MemoryStream? Original { get; init; }
            private MemoryStream? Cropped { get; set; }
            private MemoryStream? Resized { get; set; }
            private MemoryStream? Greyscaled { get; set; }
            public byte[]? Bytes { get; set; }
            public bool Loaded { get; set; }
        }

        private static IList<FrameIndex>? frameIndices;
        private static IEnumerable<FrameIndex> GetOrderedFrameIndices(
            int frameCount)
        {
            // Make local copy of the reference
            var indices = frameIndices;
            if (indices is null || indices.Count != frameCount)
            {
                indices = [.. FrameIndex
                    .CreateFrameIndices(frameCount)
                    .OrderBy(i => i.Denominator)
                    .ThenBy(i => i.Numerator)];
                frameIndices = indices;
            }
            return indices;
        }

        private static IEnumerable<FrameIndex> GetOrderedFrameIndices(
            int frameCount,
            LoadLevel loadLevel) =>
            GetOrderedFrameIndices(frameCount)
                .Skip(loadLevel.FrameStartIndex)
                .Take(loadLevel.FrameCount);

        private sealed class LoadLevel
        {
            public int FrameCount { get; init; }
            public int FrameStartIndex { get; init; }
        }

        private const int LoadLevelCount = 3;

        private static readonly Size DownscaleSize = new(16, 16);
        private const int ByteDifferenceThreshold = 3;

        private static byte[]? GetFrameBytes(Bitmap? frame)
        {
            if (frame is null)
            {
                return null;
            }
            unsafe
            {
                var bitmapData = frame.LockBits(
                    new Rectangle(0, 0, frame.Width, frame.Height),
                    ImageLockMode.ReadOnly,
                    frame.PixelFormat);

                var bytesPerPixel =
                    Image.GetPixelFormatSize(frame.PixelFormat) / 8;
                return [.. Enumerable
                    .Range(0, frame.Height)
                    .SelectMany(y => Enumerable
                        .Range(0, frame.Width)
                        .Select(x =>
                            ((byte*)bitmapData.Scan0
                                + (y * bitmapData.Stride)
                                + (x * bytesPerPixel))[0]))];
            }
        }

        private static float GetDifferenceOfBytes(
            byte[] left,
            byte[] right,
            byte threshold = ByteDifferenceThreshold)
        {
            if (Avx2.IsSupported)
            {
                return GetDifferenceOfBytesAvx2(left, right, threshold);
            }
            return GetDifferenceOfBytesSimd(left, right, threshold);
        }

        private static unsafe float GetDifferenceOfBytesSimd(
            byte[] left,
            byte[] right,
            byte threshold = ByteDifferenceThreshold)
        {
            // Typically 16 or 32 bytes depending on hardware
            var vectorSize = Vector<byte>.Count;
            var diffBytes = 0;
            var i = 0;

            // Process in chunks of Vector<byte>.Count (16 or 32 bytes)
            for (; i <= left.Length - vectorSize; i += vectorSize)
            {
                var leftVector = new Vector<byte>(left, i);
                var rightVector = new Vector<byte>(right, i);

                // Calculate absolute difference: |left - right|
                var diffVector = Vector.Abs(
                    Vector.Subtract(leftVector, rightVector));

                // Compare with threshold and count elements that exceed it
                for (var j = 0; j < vectorSize; j++)
                {
                    if (diffVector[j] > threshold)
                    {
                        diffBytes++;
                    }
                }
            }

            // Process remaining elements (if any)
            for (; i < left.Length; i++)
            {
                var diff = Math.Abs(left[i] - right[i]);
                if (diff > threshold)
                {
                    diffBytes++;
                }
            }

            return diffBytes / 256f;
        }

        private static unsafe float GetDifferenceOfBytesAvx2(
            byte[] left,
            byte[] right,
            byte threshold = ByteDifferenceThreshold)
        {
            if (left.Length != 256 || right.Length != 256)
            {
                throw new ArgumentException("Arrays must have a length of 256.");
            }

            var diffBytes = 0;
            var vectorSize = 32; // AVX2 processes 32 bytes at a time

            // Create threshold vectors for byte and short
            var thresholdVectorByte = Vector256.Create(threshold);
            var thresholdVectorShort = Vector256.Create((short)threshold);

            fixed (byte* leftPtr = left, rightPtr = right)
            {
                for (var i = 0; i <= left.Length - vectorSize; i += vectorSize)
                {
                    // Load 32 bytes from each array into AVX2 registers
                    var leftVector = Avx.LoadVector256(leftPtr + i);
                    var rightVector = Avx.LoadVector256(rightPtr + i);

                    // Perform saturated subtraction in both directions
                    // to calculate absolute difference
                    var diff1 = Avx2.SubtractSaturate(leftVector, rightVector);
                    var diff2 = Avx2.SubtractSaturate(rightVector, leftVector);
                    // Combine results to get |left - right|
                    var diffVector = Avx2.Or(diff1, diff2);

                    // Widen diffVector from byte to short to allow
                    // CompareGreaterThan
                    var diffVectorLow =
                        Avx2.ConvertToVector256Int16(diffVector.GetLower());
                    var diffVectorHigh =
                        Avx2.ConvertToVector256Int16(diffVector.GetUpper());

                    // Compare each short element with the threshold and count
                    var comparisonLow = Avx2.CompareGreaterThan(
                        diffVectorLow,
                        thresholdVectorShort);
                    var comparisonHigh = Avx2.CompareGreaterThan(
                        diffVectorHigh,
                        thresholdVectorShort);

                    // Use Popcnt to count bits set in comparison results
                    diffBytes += (int)Popcnt.PopCount(
                        (uint)Avx2.MoveMask(comparisonLow.AsByte()));
                    diffBytes += (int)Popcnt.PopCount(
                        (uint)Avx2.MoveMask(comparisonHigh.AsByte()));
                }
            }

            return diffBytes / 256f;
        }

        public VideoComparisonSettings Settings { get; } = settings;

        public VideoFile LeftVideoFile { get; } = leftVideoFile;

        public VideoFile RightVideoFile { get; } = rightVideoFile;

        public bool ForceLoadingAllFrames { get; set; }

        public event EventHandler<FrameComparedEventArgs>? FrameCompared;
        protected virtual void OnFrameCompared(
            Func<FrameComparedEventArgs> eventArgsCreator) =>
            FrameCompared?.Invoke(this, eventArgsCreator.Invoke());
        private FrameComparedEventArgs CreateFrameComparedEventArgs(
            int frameComparisonIndex,
            CacheableFrameSet leftFrames,
            CacheableFrameSet rightFrames,
            ComparisonResult frameComparisonResult,
            ComparisonResult videoComparisonResult,
            double difference,
            int loadLevel) =>
            new()
            {
                LeftVideoFile = LeftVideoFile,
                RightVideoFile = RightVideoFile,
                LeftFrames = leftFrames.ToFrameSet(),
                RightFrames = rightFrames.ToFrameSet(),
                FrameComparisonResult = frameComparisonResult,
                VideoComparisonResult = videoComparisonResult,
                Difference = difference,
                FrameLoadLevelIndex = loadLevel,
                FrameComparisonIndex = frameComparisonIndex,
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
                count = settings.MaxDifferentFrames + 1;
            }
            else if (loadLevelIndex == 2)
            {
                startIndex = settings.MaxDifferentFrames + 1;
                count = settings.CompareCount
                    - (settings.MaxDifferentFrames + 1)
                    - settings.MaxDifferentFrames;
            }
            else
            {
                startIndex = Math.Max(
                    settings.CompareCount - settings.MaxDifferentFrames,
                    settings.MaxDifferentFrames + 1);
                count = Math.Min(
                    settings.MaxDifferentFrames,
                    settings.CompareCount - (settings.MaxDifferentFrames + 1));
            }

            // Make sure we are between 0 and MaxFrameCompares at all time
            count = Math.Max(count, 0);
            count = Math.Min(count, settings.CompareCount);
            startIndex = Math.Max(startIndex, 0);
            startIndex = Math.Min(startIndex, settings.CompareCount - 1);
            return new LoadLevel
            {
                FrameCount = count,
                FrameStartIndex = startIndex
            };
        }

        private IEnumerable<CacheableFrameSet> LoadFramesFromFile(
            VideoFile videoFile,
            IEnumerable<FrameIndex> indices,
            CancellationToken cancelToken)
        {
            try
            {
                var ffmpeg = new FfmpegWrapper(videoFile.FilePath);
                var frames = ffmpeg.GetFrames(indices, cancelToken)
                    .ToList()
                    .Zip(indices, (stream, index) => (index, stream))
                    // Converting to frame set including the intermediate
                    // frames if necessary.
                    .Select(kvp => CacheableFrameSet.FromOriginalFrame(
                        kvp.index,
                        kvp.stream,
                        FrameCompared != null));

                foreach (var frame in frames)
                {
                    // Cache in memory
                    videoFile.FrameBytes[frame.Index] = frame.Bytes;

                    // Cache in DB if we have a DB cache
                    comparerDatastore?.InsertFrame(
                        frame.Index,
                        frame.Bytes,
                        videoFile);
                }

                return frames;
            }
            catch (FfmpegException exc)
            {
                throw new ComparisonException(exc.Message, videoFile, exc);
            }
        }

        private IEnumerable<CacheableFrameSet> GetFramesFromFile(
            VideoFile videoFile,
            LoadLevel loadLevel,
            CancellationToken cancelToken)
        {
            if (videoFile.FrameCount != Settings.CompareCount)
            {
                videoFile.FrameBytes.Clear();
                videoFile.FrameCount = Settings.CompareCount;
            }

            var indices = GetOrderedFrameIndices(
                Settings.CompareCount,
                loadLevel);

            // If we need to call the FrameCompared event, we
            // cannot use the cached frames since they are
            // already prepared and scaled down versions of the
            // frames. For the FrameCompared event, we need
            // a copy of the frame in every state.
            if (FrameCompared != null)
            {
                return LoadFramesFromFile(videoFile, indices, cancelToken);
            }

            var frames = indices.Select(index =>
            {
                // Try to get frame from memory cache
                _ = videoFile.FrameBytes.TryGetValue(
                    index,
                    out var bytes);
                return CacheableFrameSet.FromPreprocessedFrame(index, bytes);
            }).ToList();

            // Try to get the frame from DB cache
            if (comparerDatastore is not null && frames.Any(i => !i.Loaded))
            {
                foreach (var (index, bytes) in comparerDatastore.GetFrames(
                    frames.Where(i => !i.Loaded).Select(i => i.Index),
                    videoFile))
                {
                    var frame = frames.First(i => i.Index == index);
                    frame.Bytes = bytes;
                    frame.Loaded = true;
                    // And advance the frame into memory cache
                    videoFile.FrameBytes[index] = bytes;
                }
            }

            // Try to load the frame from file
            if (frames.Any(i => !i.Loaded))
            {
                foreach (var frame in LoadFramesFromFile(
                    videoFile,
                    frames.Where(i => !i.Loaded).Select(i => i.Index),
                    cancelToken))
                {
                    frames.First(i => i.Index == frame.Index).Bytes = frame.Bytes;
                }
            }

            return frames;
        }

        private ComparisonResult CompareLoadLevel(
            int loadLevelIndex,
            ref int differenceCount,
            CancellationToken cancelToken)
        {
            var loadLevel = CalculateLoadLevel(loadLevelIndex, Settings);

            var leftFrames = GetFramesFromFile(
                LeftVideoFile,
                loadLevel,
                cancelToken).ToList();

            var rightFrames = GetFramesFromFile(
                RightVideoFile,
                loadLevel,
                cancelToken).ToList();

            var videoComparisonResult = ComparisonResult.NoResult;

            foreach (var index in Enumerable
                    .Range(0, loadLevel.FrameCount))
            {
                if (cancelToken.IsCancellationRequested)
                {
                    videoComparisonResult = ComparisonResult.Cancelled;
                    break;
                }

                ComparisonResult frameComparisonResult;
                var diff = 0.0f;

                var leftFrameBytes = leftFrames[index].Bytes;
                var rightFrameBytes = rightFrames[index].Bytes;
                // If we don't have either one of the frames, we don't have
                // a result, but consider them different.
                if (leftFrameBytes == null || rightFrameBytes == null)
                {
                    frameComparisonResult = ComparisonResult.NoResult;
                    ++differenceCount;

                    // Early return when we already exceeded the number of
                    // MaxDifferentFrames
                    if (differenceCount > Settings.MaxDifferentFrames)
                    {
                        videoComparisonResult = ComparisonResult.Different;
                    }
                }
                else
                {
                    diff = GetDifferenceOfBytes(leftFrameBytes, rightFrameBytes);

                    if (diff <= (double)Settings.MaxDifference / 100)
                    {
                        frameComparisonResult = ComparisonResult.Duplicate;

                        // Early return when there are not enough frames left to
                        // compare to exceed the MaxDifferentFrames
                        if ((Settings.CompareCount - (index + loadLevel.FrameStartIndex + 1))
                            <= (Settings.MaxDifferentFrames - differenceCount))
                        {
                            videoComparisonResult = ComparisonResult.Duplicate;
                        }
                    }
                    else
                    {
                        frameComparisonResult = ComparisonResult.Different;
                        ++differenceCount;

                        // Early return when we already exceeded the number of
                        // MaxDifferentFrames
                        if (differenceCount > Settings.MaxDifferentFrames)
                        {
                            videoComparisonResult = ComparisonResult.Different;
                        }
                    }
                }

                OnFrameCompared(() => CreateFrameComparedEventArgs(
                    index + loadLevel.FrameStartIndex,
                    leftFrames[index],
                    rightFrames[index],
                    frameComparisonResult,
                    videoComparisonResult,
                    diff,
                    loadLevelIndex));

                if (videoComparisonResult != ComparisonResult.NoResult
                    && !ForceLoadingAllFrames)
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
                    && !ForceLoadingAllFrames)
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
