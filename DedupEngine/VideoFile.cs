namespace DedupEngine
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using MpvLib;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.Interfaces;
    using static VideoDedupGrpc.DurationComparisonSettings.Types;

    public class VideoFile : IVideoFile
    {
        public VideoFile(VideoFile other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            // We have to do a bit more work here since we can't use
            // the other ctors because we don't want to eagerly load
            // the images when we can copy them from the other file.
            FilePath = other.FilePath;
            fileSize = other.fileSize;
            lastWriteTime = other.lastWriteTime;
            duration = other.duration;
            codecInfo = other.codecInfo;


            if (other.ImageCount == 0)
            {
                return;
            }

            ImageCount = other.ImageCount;
            ImageBytes = new Dictionary<ImageIndex, byte[]?>(other.ImageBytes);
        }

        public VideoFile(IVideoFile other)
            : this(other.FilePath)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

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
                    $" null or whitespace", nameof(filePath));
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
                        fileSize = new FileInfo(FilePath).Length;
                    }
                    catch (Exception)
                    {
                        fileSize = 0;
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

        public TimeSpan Duration
        {
            get
            {
                if (!duration.HasValue)
                {
                    try
                    {
                        duration = MpvWrapper.GetDuration(FilePath);
                    }
                    catch (MpvOperationException)
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
                        codecInfo = MpvWrapper.GetCodecInfo(FilePath);
                    }
                    catch (MpvOperationException) { }
                }
                return codecInfo;
            }
        }
        private CodecInfo? codecInfo;

        public IDictionary<ImageIndex, byte[]?> ImageBytes { get; } =
            new Dictionary<ImageIndex, byte[]?>();

        public int ImageCount { get; set; }

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
                    throw new ConfigurationErrorsException(
                        "DurationDifferenceType has not valid value");
            }
        }

        public int ErrorCount { get; set; }

        public override bool Equals(object? obj) => Equals(obj as IVideoFile);

        public bool Equals(IVideoFile? other) =>
            other != null && FilePath == other.FilePath;

        public override int GetHashCode() => HashCode.Combine(FilePath);

        public static bool operator ==(VideoFile left, IVideoFile right) =>
            EqualityComparer<IVideoFile>.Default.Equals(left, right);

        public static bool operator !=(VideoFile left, IVideoFile right) =>
            !(left == right);
    }
}
