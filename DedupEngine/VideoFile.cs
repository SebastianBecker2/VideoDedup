using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Newtonsoft.Json")]
namespace DedupEngine
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using global::DedupEngine.MpvLib;
    using Newtonsoft.Json;
    using VideoDedupShared;

    public class VideoFile : IVideoFile
    {
        internal VideoFile() { }

        public VideoFile(VideoFile other)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            // We have to do a bit more work here since we can't use
            // the other ctors because we don't want to eagerly load
            // the images when we can copy them from the other file.
            filePath = other.filePath;
            fileSize = other.fileSize;
            duration = other.duration;
            codecInfo = other.codecInfo;

            if (other.ImageCount == 0)
            {
                return;
            }

            ImageCount = other.ImageCount;
            ImageBytes = other.ImageBytes.ToList();
        }

        public VideoFile(IVideoFile other)
            : this(other.FilePath)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            fileSize = other.FileSize;
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

            this.filePath = filePath;
        }

        [JsonIgnore]
        public string FilePath => filePath;
        [JsonProperty]
        private readonly string filePath;

        [JsonIgnore]
        public string FileName => Path.GetFileName(FilePath);

        [JsonIgnore]
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
        [JsonProperty]
        private long? fileSize = null;

        [JsonIgnore]
        public TimeSpan Duration
        {
            get
            {
                if (duration == TimeSpan.Zero)
                {
                    try
                    {
                        duration = MpvWrapper.GetDuration(filePath);
                    }
                    catch (Exception)
                    {
                        duration = TimeSpan.Zero;
                    }
                }
                return duration;
            }
        }
        [JsonProperty]
        private TimeSpan duration = TimeSpan.Zero;

        [JsonIgnore]
        public CodecInfo CodecInfo
        {
            get
            {
                if (codecInfo == null)
                {
                    codecInfo = MpvWrapper.GetCodecInfo(filePath);
                }
                return codecInfo;
            }
        }
        [JsonIgnore]
        private CodecInfo codecInfo = null;

        [JsonProperty]
        public List<byte[]> ImageBytes { get; } = new List<byte[]>();

        [JsonIgnore]
        public int ImageCount { get; set; } = 0;

        public bool IsDurationEqual(
            IVideoFile other,
            IDurationComparisonSettings settings)
        {
            var diff = Math.Abs((Duration - other.Duration).TotalSeconds);
            switch (settings.DifferenceType)
            {
                case DurationDifferenceType.Seconds:
                    return diff < settings.MaxDifferenceSeconds;
                case DurationDifferenceType.Percent:
                    var max_diff = Duration.TotalSeconds / 100
                        * settings.MaxDifferencePercent;
                    return diff < max_diff;
                default:
                    throw new ConfigurationErrorsException(
                        "DurationDifferenceType has not valid value");
            }
        }

        public override bool Equals(object obj) => Equals(obj as IVideoFile);

        public bool Equals(IVideoFile other) => other != null &&
                   FilePath == other.FilePath;

        public override int GetHashCode() =>
            1230029444 + EqualityComparer<string>.Default.GetHashCode(FilePath);

        public static bool operator ==(VideoFile left, IVideoFile right) =>
            EqualityComparer<IVideoFile>.Default.Equals(left, right);

        public static bool operator !=(VideoFile left, IVideoFile right) =>
            !(left == right);
    }
}
