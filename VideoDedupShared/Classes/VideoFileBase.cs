using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("System.Runtime.Serialization")]
[assembly: InternalsVisibleTo("Newtonsoft.Json")]
namespace VideoDedupShared
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using NReco.VideoConverter;
    using NReco.VideoInfo;

    [DataContract]
    public class VideoFileBase : IVideoFile
    {
        internal VideoFileBase() { }
        public VideoFileBase(string filePath)
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
        [DataMember]
        [JsonProperty]
        protected internal string filePath;

        [JsonIgnore]
        public string FileName => Path.GetFileName(FilePath);

        [JsonIgnore]
        public long FileSize
        {
            get
            {
                if (fileSize == 0)
                {
                    fileSize = new FileInfo(FilePath).Length;
                }
                return fileSize;
            }
        }
        [JsonProperty]
        [DataMember]
        protected internal long fileSize = 0;

        [JsonIgnore]
        public TimeSpan Duration
        {
            get
            {
                if (duration == TimeSpan.Zero)
                {
                    try
                    {
                        duration = MediaInfo.Duration;
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
        [DataMember]
        protected internal TimeSpan duration = TimeSpan.Zero;

        [JsonIgnore]
        public VideoCodecInfo VideoCodec
        {
            get
            {
                if (MediaInfo == null)
                {
                    return null;
                }

                var stream = MediaInfo.Streams.FirstOrDefault(s => s.CodecType == "video");
                if (stream == null)
                {
                    return null;
                }

                return new VideoCodecInfo(stream);
            }
        }
        [JsonIgnore]
        [DataMember]
        protected internal VideoCodecInfo videoCodec = null;

        [JsonIgnore]
        protected MediaInfo MediaInfo
        {
            get
            {
                if (mediaInfo == null)
                {
                    var probe = new FFProbe();
                    mediaInfo = probe.GetMediaInfo(FilePath);
                }
                return mediaInfo;
            }
        }
        [JsonIgnore]
        protected internal MediaInfo mediaInfo = null;

        protected virtual MemoryStream GetImage(int index,
            int imageCount)
        {
            if (index >= imageCount || index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                    "Index out of range.");
            }

            var ffMpeg = new FFMpegConverter();
            var imageStream = new MemoryStream();
            try
            {
                var stepping = Duration.TotalSeconds /
                    (imageCount + 1);

                ffMpeg.GetVideoThumbnail(FilePath,
                    imageStream,
                    (float)stepping * (index + 1));

                return imageStream;
            }
            catch (Exception)
            {
                Debug.Print($"Unable to load image sample index {index} " +
                    $"for {FilePath}");
                return new MemoryStream();
            }
        }

        public bool IsDurationEqual(IVideoFile other,
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

        public static bool operator ==(VideoFileBase left, IVideoFile right) =>
            EqualityComparer<IVideoFile>.Default.Equals(left, right);

        public static bool operator !=(VideoFileBase left, IVideoFile right) =>
            !(left == right);
    }
}
