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

namespace VideoDedup
{
    public class VideoFile : IEquatable<VideoFile>
    {
        public readonly static int ThumbnailCount = 20;
        private readonly static int DurationEqualityLimit = 10; // as seconds

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

        [JsonIgnore]
        private double ThumbnailStepping
        {
            get => (Duration.TotalSeconds / (ThumbnailCount + 1));
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
            return Math.Abs((Duration - other.Duration).TotalSeconds) < DurationEqualityLimit;
        }

        public Image GetThumbnail(int index)
        {
            if (index >= ThumbnailCount || index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range.");
            }

            if (!_Thumbnails.ContainsKey(index))
            {
                var ffMpeg = new FFMpegConverter();
                var image_stream = new MemoryStream();
                try
                {
                    ffMpeg.GetVideoThumbnail(FilePath, image_stream, (float)ThumbnailStepping * (index + 1));
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
            IList<bool> ThumbnailDifferences = new List<bool>();
            foreach (var i in Enumerable.Range(0, ThumbnailCount))
            {
                var diff = GetThumbnail(i).PercentageDifference(other.GetThumbnail(i));
                ThumbnailDifferences.Add(diff > 0.2d);

                var diff_count = ThumbnailDifferences.Count(d => d);

                if (diff_count >= 2)
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
