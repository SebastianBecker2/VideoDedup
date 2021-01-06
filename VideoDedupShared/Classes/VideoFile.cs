using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("System.Runtime.Serialization")]
[assembly: InternalsVisibleTo("Newtonsoft.Json")]
namespace VideoDedupShared
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using Newtonsoft.Json;
    using NReco.VideoConverter;
    using NReco.VideoInfo;
    using XnaFan.ImageComparison;

    [DataContract]
    public class VideoFile : IVideoFile, IDisposable
    {
        internal VideoFile() { }

        public VideoFile(
            VideoFile other,
            int imageCount = 0)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            filePath = other.filePath;
            this.imageCount = imageCount;

            // We have to do a bit more work here since we can't use
            // the other ctors because we don't want to eagerly load
            // the images when we can copy them from the other file.
            fileSize = other.fileSize;
            duration = other.duration;
            videoCodec = other.videoCodec;
            mediaInfo = other.mediaInfo;

            if (imageCount == other.imageCount)
            {
                foreach (var kvp in other.imageStreams)
                {
                    var ms = new MemoryStream();
                    kvp.Value.Position = 0;
                    kvp.Value.CopyTo(ms);
                    imageStreams.Add(kvp.Key, ms);
                }
            }

            foreach (var index in Enumerable.Range(0, ImageCount))
            {
                _ = GetImage(index, this.imageCount);
            }
        }

        public VideoFile(
            IVideoFile other,
            int imageCount = 0)
            : this(other.FilePath, imageCount)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            fileSize = other.FileSize;
            duration = other.Duration;
            videoCodec = other.VideoCodec;
        }

        public VideoFile(
            string filePath,
            int imageCount = 0)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be" +
                    $" null or whitespace", nameof(filePath));
            }

            this.filePath = filePath;
            this.imageCount = imageCount;

            foreach (var index in Enumerable.Range(0, ImageCount))
            {
                _ = GetImage(index, this.imageCount);
            }
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

        [JsonIgnore]
        [DataMember]
        protected internal readonly IDictionary<int, MemoryStream> imageStreams =
            new Dictionary<int, MemoryStream>();

        [JsonIgnore]
        public int ImageCount => imageCount;
        [DataMember]
        [JsonIgnore]
        protected internal int imageCount = 0;

        [JsonIgnore]
        public IEnumerable<KeyValuePair<int, Image>> Images =>
            imageStreams.Select(kvp => new KeyValuePair<int, Image>(
                kvp.Key,
                Image.FromStream(kvp.Value)));

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

        protected MemoryStream GetImage(int index,
            int imageCount)
        {
            if (index >= imageCount || index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                    "Index out of range.");
            }

            if (imageCount != ImageCount)
            {
                DisposeImages();
                this.imageCount = imageCount;
            }

            if (!imageStreams.TryGetValue(index, out var ms))
            {
                var ffMpeg = new FFMpegConverter();
                ms = new MemoryStream();
                try
                {
                    var stepping = Duration.TotalSeconds /
                        (imageCount + 1);

                    ffMpeg.GetVideoThumbnail(FilePath,
                        ms,
                        (float)stepping * (index + 1));
                }
                catch (Exception)
                {
                    Debug.Print($"Unable to load image sample index {index} " +
                        $"for {FilePath}");
                    return new MemoryStream();
                }
                imageStreams.Add(index, ms);
            }

            return ms;
        }

        public bool AreImagesEqual(VideoFile other,
            IImageComparisonSettings settings,
            CancellationToken cancelToken)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var differernceCount = 0;
            foreach (var index in Enumerable.Range(0, settings.MaxCompares))
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return false;
                }

                var ms = GetImage(index, settings.MaxCompares);
                var thisImageSample =
                    Image.FromStream(ms);
                var otherImageSample =
                    Image.FromStream(other.GetImage(index, settings.MaxCompares));

                var diff = thisImageSample.PercentageDifference(otherImageSample);

                if (diff > (double)settings.MaxDifferencePercent / 100)
                {
                    ++differernceCount;
                }

                if (differernceCount > settings.MaxDifferentImages)
                {
                    return false;
                }
            }
            return true;
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

        // For the IDisposable pattern
        private bool disposedValue;

        public void DisposeImages()
        {
            foreach (var kvp in imageStreams)
            {
                kvp.Value.Dispose();
            }
            imageStreams.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DisposeImages();
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
