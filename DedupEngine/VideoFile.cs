using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Newtonsoft.Json")]
namespace DedupEngine
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using global::DedupEngine.MpvLib;
    using Newtonsoft.Json;
    using VideoDedupShared;

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

            ImageCount = imageCount;

            // We have to do a bit more work here since we can't use
            // the other ctors because we don't want to eagerly load
            // the images when we can copy them from the other file.
            filePath = other.filePath;
            fileSize = other.fileSize;
            duration = other.duration;
            codecInfo = other.codecInfo;

            if (ImageCount == 0)
            {
                return;
            }

            if (other.ImageCount == ImageCount)
            {
                imageStreams = other.imageStreams
                    .Select(image =>
                    {
                        var ms = new MemoryStream();
                        image.Position = 0;
                        image.CopyTo(ms);
                        return ms;
                    })
                    .ToList();
                return;
            }

            using (var mpv = new MpvWrapper(FilePath, imageCount, Duration))
            {
                imageStreams = mpv.GetImages(0, imageCount).ToList();
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
            codecInfo = other.CodecInfo;
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
            ImageCount = imageCount;

            if (ImageCount == 0)
            {
                return;
            }

            using (var mpv = new MpvWrapper(FilePath, ImageCount, Duration))
            {
                imageStreams = mpv.GetImages(0, ImageCount).ToList();
            }
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

        [JsonIgnore]
        public IEnumerable<Image> Images
        {
            get
            {
                if (imageStreams == null)
                {
                    return Enumerable.Empty<Image>();
                }
                return imageStreams.Select(image => Image.FromStream(image));
            }
        }
        [JsonIgnore]
        public IList<MemoryStream> ImageStreams => imageStreams;
        [JsonIgnore]
        private readonly IList<MemoryStream> imageStreams =
            new List<MemoryStream>();

        [JsonIgnore]
        public int ImageCount { get; set; }

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

        // For the IDisposable pattern
        private bool disposedValue;

        public void DisposeImages()
        {
            foreach (var image in imageStreams)
            {
                image.Dispose();
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
