namespace VideoDedup
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Newtonsoft.Json;
    using NReco.VideoConverter;
    using NReco.VideoInfo;
    using XnaFan.ImageComparison;

    public class VideoFile : object, IEquatable<VideoFile>
    {
        [JsonProperty]
        public string FilePath { get; private set; }
        [JsonIgnore]
        public string FileName => Path.GetFileName(this.FilePath);
        [JsonIgnore]
        public MediaInfo MediaInfo
        {
            get
            {
                if (this.mediaInfo == null)
                {
                    var probe = new FFProbe();
                    this.mediaInfo = probe.GetMediaInfo(this.FilePath);
                }
                return this.mediaInfo;
            }
        }
        [JsonIgnore]
        public long FileSize
        {
            get
            {
                if (this.fileSize == null)
                {
                    this.fileSize = new FileInfo(this.FilePath).Length;
                }
                return this.fileSize.Value;
            }
            private set => this.fileSize = value;
        }
        [JsonIgnore]
        public TimeSpan Duration
        {
            get
            {
                if (this.duration == null)
                {
                    try
                    {
                        this.duration = this.MediaInfo.Duration;

                    }
                    catch (Exception)
                    {
                        this.duration = TimeSpan.Zero;
                    }
                }
                return this.duration.Value;
            }
            private set => this.duration = value;
        }

        [JsonProperty]
        private TimeSpan? duration = null;
        [JsonProperty]
        private long? fileSize = null;
        private readonly IDictionary<int, Image> thumbnails =
            new Dictionary<int, Image>();
        private MediaInfo mediaInfo = null;
        private readonly IComparisonSettings settings;

        public VideoFile(string path, IComparisonSettings comparisonSettings)
        {
            this.FilePath = path;
            this.settings = comparisonSettings;
        }

        /// <summary>
        /// Special case for FileSystemWatcher change events.
        /// When a large file has been written, the file might
        /// not be accessable yet when the event comes in.
        /// If we would try to read the duration right away
        /// it would fail.
        /// So we wait for read access to the file.
        /// For a maximum of 1 second.
        /// </summary>
        /// <returns></returns>
        public bool WaitForFileAccess()
        {
            for (var i = 0; i < 20; i++)
            {
                try
                {
                    using (var stream = File.Open(this.FilePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite))
                    {
                        if (stream != null)
                        {
                            return true;
                        }
                    }
                }
                catch (ArgumentOutOfRangeException) { throw; }
                catch (ArgumentNullException) { return false; }
                catch (ArgumentException) { return false; }
                catch (PathTooLongException) { return false; }
                catch (DirectoryNotFoundException) { return false; }
                catch (NotSupportedException) { return false; }
                catch (Exception) { }

                Thread.Sleep(50);
            }
            return false;
        }

        public bool IsDurationEqual(VideoFile other)
        {
            switch (this.settings.DurationDifferenceType)
            {
                case DurationDifferenceType.Seconds:
                    return Math.Abs((this.Duration - other.Duration).TotalSeconds) < this.settings.MaxDurationDifferenceSeconds;
                case DurationDifferenceType.Percent:
                    var difference = Math.Abs((this.Duration - other.Duration).TotalSeconds);
                    var max_diff = this.Duration.TotalSeconds / 100 * this.settings.MaxDurationDifferencePercent;
                    return difference < max_diff;
                default:
                    throw new ConfigurationErrorsException("DurationDifferenceType has not valid value");
            }
        }

        public Image GetThumbnail(int index, int thumbnailCount)
        {
            if (index >= thumbnailCount || index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range.");
            }

            if (!this.thumbnails.ContainsKey(index))
            {
                var ffMpeg = new FFMpegConverter();
                var image_stream = new MemoryStream();
                try
                {
                    var stepping = this.Duration.TotalSeconds / (thumbnailCount + 1);
                    ffMpeg.GetVideoThumbnail(this.FilePath, image_stream, (float)stepping * (index + 1));
                    this.thumbnails[index] = Image.FromStream(image_stream);
                }
                catch (Exception)
                {
                    Debug.Print($"Unable to load thumbnail index {index} for {this.FilePath}");
                    this.thumbnails[index] = new Bitmap(1, 1);
                }
            }

            return this.thumbnails[index];
        }

        public bool AreThumbnailsEqual(VideoFile other)
        {
            var differernceCount = 0;
            foreach (var i in Enumerable.Range(0, this.settings.MaxThumbnailComparison))
            {
                var this_thumbnail = this.GetThumbnail(i, this.settings.MaxThumbnailComparison);
                var other_thumbnail = other.GetThumbnail(i, this.settings.MaxThumbnailComparison);
                var diff = this_thumbnail.PercentageDifference(other_thumbnail);
                Debug.Print($"{i} Difference: {diff}");
                if (diff > (double)this.settings.MaxDifferencePercentage / 100)
                {
                    ++differernceCount;
                }

                if (differernceCount > this.settings.MaxDifferentThumbnails)
                {
                    return false;
                }
            }
            return true;
        }

        public void DisposeThumbnails() => this.thumbnails.Clear();

        public override bool Equals(object obj) => this.Equals(obj as VideoFile);

        public bool Equals(VideoFile other) => other != null &&
                   this.FilePath == other.FilePath;

        public override int GetHashCode() =>
            1230029444 + EqualityComparer<string>.Default.GetHashCode(this.FilePath);

        public static bool operator ==(VideoFile left, VideoFile right) =>
            EqualityComparer<VideoFile>.Default.Equals(left, right);

        public static bool operator !=(VideoFile left, VideoFile right) =>
            !(left == right);
    }
}
