namespace VideoComparer
{
    using VideoDedupGrpc;
    using VideoDedupSharedLib.Interfaces;
    using static VideoDedupGrpc.DurationComparisonSettings.Types;
    using System.Diagnostics;
    using FfmpegLib;
    using FfmpegLib.Exceptions;
    using FrameIndex = FfmpegLib.FrameIndex;

    [DebuggerDisplay("{FilePath}")]
    public class VideoFile : IVideoFile
    {
        public VideoFile(VideoFile other)
        {
            ArgumentNullException.ThrowIfNull(other);

            // We have to do a bit more work here since we can't use
            // the other ctors because we don't want to eagerly load
            // the frames when we can copy them from the other file.
            FilePath = other.FilePath;
            fileSize = other.fileSize;
            lastWriteTime = other.lastWriteTime;
            duration = other.duration;
            codecInfo = other.codecInfo;


            if (other.FrameCount == 0)
            {
                return;
            }

            FrameCount = other.FrameCount;
            FrameBytes = new Dictionary<FrameIndex, byte[]?>(other.FrameBytes);
        }

        public VideoFile(IVideoFile other)
            : this(other.FilePath)
        {
            ArgumentNullException.ThrowIfNull(other);

            fileSize = other.FileSize;
            lastWriteTime = other.LastWriteTime;
            duration = other.Duration;
            codecInfo = other.CodecInfo;
        }

        public VideoFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be" +
                    " null or whitespace", nameof(filePath));
            }

            FilePath = filePath;
        }

        public string FilePath { get; }

        public string FileName => Path.GetFileName(FilePath);

        public long FileSize
        {
            get
            {
                if (!fileSize.HasValue)
                {
                    try
                    {
                        var fi = new FileInfo(FilePath);
                        fileSize = fi.Length;
                        lastWriteTime = fi.LastWriteTime;
                    }
                    catch (Exception)
                    {
                        fileSize = 0;
                        lastWriteTime = null;
                    }
                }
                return fileSize.Value;
            }
        }
        private long? fileSize;

        public DateTime LastWriteTime
        {
            get
            {
                if (!lastWriteTime.HasValue)
                {
                    try
                    {
                        lastWriteTime = File.GetLastWriteTime(FilePath);
                    }
                    catch (Exception)
                    {
                        lastWriteTime = DateTime.MinValue;
                    }
                }
                return lastWriteTime.Value;
            }
        }
        private DateTime? lastWriteTime;

        public DateTime CreationTime
        {
            get
            {
                if (!creationTime.HasValue)
                {
                    try
                    {
                        creationTime = File.GetCreationTime(FilePath);
                    }
                    catch (Exception)
                    {
                        creationTime = DateTime.MinValue;
                    }
                }
                return creationTime.Value;
            }
        }
        private DateTime? creationTime;

        public DateTime LastAccessTime
        {
            get
            {
                if (!lastAccessTime.HasValue)
                {
                    try
                    {
                        lastAccessTime = File.GetLastAccessTime(FilePath);
                    }
                    catch (Exception)
                    {
                        lastAccessTime = DateTime.MinValue;
                    }
                }
                return lastAccessTime.Value;
            }
        }
        private DateTime? lastAccessTime;

        public TimeSpan Duration
        {
            get
            {
                if (!duration.HasValue)
                {
                    try
                    {
                        duration = FfmpegWrapper.GetDuration(FilePath);
                    }
                    catch (FfmpegOperationException)
                    {
                        duration = TimeSpan.Zero;
                    }
                }
                return duration.Value;
            }
            set => duration = value;
        }
        private TimeSpan? duration;

        public CodecInfo? CodecInfo
        {
            get
            {
                if (codecInfo == null)
                {
                    try
                    {
                        codecInfo = FfmpegWrapper.GetCodecInfo(FilePath);
                    }
                    catch (FfmpegOperationException) { }
                }
                return codecInfo;
            }
        }
        private CodecInfo? codecInfo;

        public IDictionary<FrameIndex, byte[]?> FrameBytes { get; } =
            new Dictionary<FrameIndex, byte[]?>();

        public int FrameCount { get; set; }

        public bool IsDurationEqual(
            IVideoFile other,
            DurationComparisonSettings settings)
        {
            var diff = Math.Abs((Duration - other.Duration).TotalSeconds);
            switch (settings.DifferenceType)
            {
                case DurationDifferenceType.Seconds:
                    return diff < settings.MaxDifference;
                case DurationDifferenceType.Percent:
                    var max_diff = Duration.TotalSeconds / 100
                        * settings.MaxDifference;
                    return diff < max_diff;
                default:
                    throw new InvalidOperationException(
                        "DurationDifferenceType has not valid value");
            }
        }

        public int ErrorCount { get => errorCount; set => errorCount = value; }
        private int errorCount;
        public void IncrementErrorCount() => Interlocked.Increment(ref errorCount);

        public override bool Equals(object? obj) => Equals(obj as IVideoFile);

        public bool Equals(IVideoFile? other) =>
            other != null && FilePath == other.FilePath;

        public override int GetHashCode() => FilePath.GetHashCode();

        public static bool operator ==(VideoFile left, IVideoFile right) =>
            EqualityComparer<IVideoFile>.Default.Equals(left, right);

        public static bool operator !=(VideoFile left, IVideoFile right) =>
            !(left == right);
    }
}
