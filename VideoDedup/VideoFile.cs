using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NReco.VideoInfo;
using System.Diagnostics;
using System.Drawing;
using XnaFan.ImageComparison;
using Newtonsoft.Json;
using NReco.VideoConverter;
using System.Threading;
using System.Configuration;

namespace VideoDedup
{
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
                if (_MediaInfo == null)
                {
                    var probe = new FFProbe();
                    _MediaInfo = probe.GetMediaInfo(FilePath);
                }
                return _MediaInfo;
            }
        }
        [JsonIgnore]
        public long FileSize
        {
            get
            {
                if (_FileSize == null)
                {
                    _FileSize = new FileInfo(FilePath).Length;
                }
                return _FileSize.Value;
            }
            private set => _FileSize = value;
        }
        [JsonIgnore]
        public TimeSpan Duration
        {
            get
            {
                if (_Duration == null)
                {
                    try
                    {
                        _Duration = MediaInfo.Duration;

                    }
                    catch (Exception)
                    {
                        _Duration = new TimeSpan();
                    }
                }
                return _Duration.Value;
            }
            private set => _Duration = value;
        }

        [JsonProperty]
        private TimeSpan? _Duration = null;
        [JsonProperty]
        private long? _FileSize = null;
        private IDictionary<int, Image> _Thumbnails = new Dictionary<int, Image>();
        private MediaInfo _MediaInfo = null;

        public VideoFile(string path)
        {
            FilePath = path;
        }

        public bool IsDurationEqual(VideoFile other)
        {
            switch (ConfigData.DurationDifferenceType)
            {
            case DurationDifferenceType.Seconds:
                return Math.Abs((Duration - other.Duration).TotalSeconds) < ConfigData.MaxDurationDifference;
            case DurationDifferenceType.Percent:
                var difference = Math.Abs((Duration - other.Duration).TotalSeconds);
                var max_diff = (Duration.TotalSeconds / 100 * ConfigData.MaxDurationDifference);
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

            if (!_Thumbnails.ContainsKey(index))
            {
                var ffMpeg = new FFMpegConverter();
                var image_stream = new MemoryStream();
                try
                {
                    var stepping = Duration.TotalSeconds / (thumbnailCount + 1);
                    ffMpeg.GetVideoThumbnail(FilePath, image_stream, (float)stepping * (index + 1));
                    _Thumbnails[index] = Image.FromStream(image_stream);
                }
                catch (Exception)
                {
                    Debug.Print($"Unable to load thumbnail index {index} for {FilePath}");
                    _Thumbnails[index] = new Bitmap(1, 1);
                }
            }

            return _Thumbnails[index];
        }

        public bool AreThumbnailsEqual(VideoFile other)
        {
            int differernceCount = 0;
            foreach (var i in Enumerable.Range(0, ConfigData.MaxThumbnailComparison))
            {
                var this_thumbnail = GetThumbnail(i, ConfigData.MaxThumbnailComparison);
                var other_thumbnail = other.GetThumbnail(i, ConfigData.MaxThumbnailComparison);
                var diff = this_thumbnail.PercentageDifference(other_thumbnail);

                if (diff > ConfigData.MaxDifferencePercentage / 100)
                {
                    ++differernceCount;
                }

                if (differernceCount > ConfigData.MaxDifferentThumbnails)
                {
                    return false;
                }
            }
            return true;
        }

        public void DisposeThumbnails()
        {
            _Thumbnails.Clear();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as VideoFile);
        }

        public bool Equals(VideoFile other)
        {
            return other != null &&
                   FilePath == other.FilePath;
        }

        public override int GetHashCode()
        {
            return 1230029444 + EqualityComparer<string>.Default.GetHashCode(FilePath);
        }

        public static bool operator ==(VideoFile left, VideoFile right)
        {
            return EqualityComparer<VideoFile>.Default.Equals(left, right);
        }

        public static bool operator !=(VideoFile left, VideoFile right)
        {
            return !(left == right);
        }
    }
}
