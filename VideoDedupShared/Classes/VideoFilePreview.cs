using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("System.Runtime.Serialization")]
[assembly: InternalsVisibleTo("Newtonsoft.Json")]
namespace VideoDedupShared
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using Newtonsoft.Json;
    using XnaFan.ImageComparison;

    [DataContract]
    public class VideoFilePreview : VideoFileBase, IDisposable
    {
        internal VideoFilePreview() { }

        public VideoFilePreview(
            VideoFilePreview other,
            int imageCount = 0)
            : base(other.FilePath)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

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
                    kvp.Value.CopyTo(ms);
                    imageStreams.Add(kvp.Key, ms);
                }
            }

            this.imageCount = imageCount;

            foreach (var index in Enumerable.Range(0, ImageCount))
            {
                _ = GetImage(index, this.imageCount);
            }
        }

        public VideoFilePreview(
            VideoFileBase other,
            int imageCount = 0)
            : this(other.FilePath, imageCount)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            // We can't use the ctor that takes IVideoFile since
            // we would screw with the caching in VideoFileBase.
            fileSize = other.fileSize;
            duration = other.duration;
            videoCodec = other.videoCodec;
            mediaInfo = other.mediaInfo;
        }

        public VideoFilePreview(
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

        public VideoFilePreview(
            string filePath,
            int imageCount = 0)
            : base(filePath)
        {
            this.imageCount = imageCount;

            foreach (var index in Enumerable.Range(0, ImageCount))
            {
                _ = GetImage(index, this.imageCount);
            }
        }

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

        protected override MemoryStream GetImage(int index,
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
                ms = base.GetImage(index, imageCount);
                imageStreams.Add(index, ms);
            }

            Debug.Print($"{FileName} GetImage({index})");
            //ms.Position = 0;
            return ms;
        }

        public bool AreImagesEqual(VideoFilePreview other,
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

                var thisImageSample =
                    Image.FromStream(GetImage(index, settings.MaxCompares));
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
