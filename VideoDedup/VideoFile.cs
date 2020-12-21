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
        public string FileName => Path.GetFileName(FilePath);
        [JsonIgnore]
        public MediaInfo MediaInfo
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
        public long FileSize
        {
            get
            {
                if (fileSize == null)
                {
                    fileSize = new FileInfo(FilePath).Length;
                }
                return fileSize.Value;
            }
            private set => fileSize = value;
        }
        [JsonIgnore]
        public TimeSpan Duration
        {
            get
            {
                if (duration == null)
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
                return duration.Value;
            }
            private set => duration = value;
        }

        [JsonProperty]
        private TimeSpan? duration = null;
        [JsonProperty]
        private long? fileSize = null;
        private readonly IDictionary<int, Image> thumbnails =
            new Dictionary<int, Image>();
        private MediaInfo mediaInfo = null;

        public VideoFile(string path) => FilePath = path;

        /// <summary>
        /// Special case for FileSystemWatcher change events.
        /// When a large file has been written, the file might
        /// not be accessable yet when the event comes in.
        /// If we would try to read the duration right away
        /// it would fail.
        /// So we wait for read access to the file.
        /// For a maximum of 1 second.
        /// </summary>
        /// <param name="timeout">For how long should we try
        /// to access the file?</param>
        /// <param name="interval">How long should we wait
        /// between each try?</param>
        /// <returns>
        /// true: File could be access.
        /// false: File could not be access in time.
        /// </returns>
        public bool WaitForFileAccess(int timeout = 1000, int interval = 50) =>
            WaitForFileAccess(timeout, interval, null);

        /// <summary>
        /// Special case for FileSystemWatcher change events.
        /// When a large file has been written, the file might
        /// not be accessable yet when the event comes in.
        /// If we would try to read the duration right away
        /// it would fail.
        /// So we wait for read access to the file.
        /// For a maximum of 1 second.
        /// </summary>
        /// <param name="cancelToken">A token to be able to
        /// cancel the wait.</param>
        /// <param name="timeout">For how long should we try
        /// to access the file?</param>
        /// <param name="interval">How long should we wait
        /// between each try?</param>
        /// <returns>
        /// true: File could be access.
        /// false: File could not be access in time.
        /// </returns>
        public bool WaitForFileAccess(CancellationToken cancelToken,
            int timeout = 1000,
            int interval = 50) =>
            WaitForFileAccess(timeout, interval, cancelToken);

        private bool WaitForFileAccess(int timeout,
            int interval,
            CancellationToken? cancelToken = null)
        {
            for (var i = 0; i < (timeout / interval); i++)
            {
                try
                {
                    using (var stream = File.Open(FilePath,
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

                if (cancelToken.HasValue &&
                    cancelToken.Value.IsCancellationRequested)
                {
                    return false;
                }
                Thread.Sleep(interval);
            }
            return false;
        }

        public bool IsDurationEqual(VideoFile other, IDurationComparisonSettings settings)
        {
            switch (settings.DifferenceType)
            {
                case DurationDifferenceType.Seconds:
                    return Math.Abs((Duration - other.Duration).TotalSeconds) < settings.MaxDifferenceSeconds;
                case DurationDifferenceType.Percent:
                    var difference = Math.Abs((Duration - other.Duration).TotalSeconds);
                    var max_diff = Duration.TotalSeconds / 100 * settings.MaxDifferencePercent;
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

            if (!thumbnails.ContainsKey(index))
            {
                var ffMpeg = new FFMpegConverter();
                var image_stream = new MemoryStream();
                try
                {
                    var stepping = Duration.TotalSeconds / (thumbnailCount + 1);
                    ffMpeg.GetVideoThumbnail(FilePath, image_stream, (float)stepping * (index + 1));
                    thumbnails[index] = Image.FromStream(image_stream);
                }
                catch (Exception)
                {
                    Debug.Print($"Unable to load thumbnail index {index} for {FilePath}");
                    thumbnails[index] = new Bitmap(1, 1);
                }
            }

            return thumbnails[index];
        }

        public bool AreThumbnailsEqual(VideoFile other,
            IThumbnailComparisonSettings settings,
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
            foreach (var i in Enumerable.Range(0, settings.MaxCompares))
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return false;
                }

                var this_thumbnail = GetThumbnail(i, settings.MaxCompares);
                var other_thumbnail = other.GetThumbnail(i, settings.MaxCompares);
                var diff = this_thumbnail.PercentageDifference(other_thumbnail);
                Debug.Print($"{i} Difference: {diff}");
                if (diff > (double)settings.MaxDifferencePercent / 100)
                {
                    ++differernceCount;
                }

                if (differernceCount > settings.MaxDifferentThumbnails)
                {
                    return false;
                }
            }
            return true;
        }

        public void DisposeThumbnails() => thumbnails.Clear();

        public override bool Equals(object obj) => Equals(obj as VideoFile);

        public bool Equals(VideoFile other) => other != null &&
                   FilePath == other.FilePath;

        public override int GetHashCode() =>
            1230029444 + EqualityComparer<string>.Default.GetHashCode(FilePath);

        public static bool operator ==(VideoFile left, VideoFile right) =>
            EqualityComparer<VideoFile>.Default.Equals(left, right);

        public static bool operator !=(VideoFile left, VideoFile right) =>
            !(left == right);
    }
}
