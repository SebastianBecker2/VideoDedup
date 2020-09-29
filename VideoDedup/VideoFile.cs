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

namespace VideoDedup
{
    public class VideoFile
    {
        private readonly static int ThumbnailCount = 20;
        private readonly static int DurationEqualityLimit = 10; // as seconds

        public string FilePath { get; }
        public string FileName
        {
            get
            {
                return Path.GetFileName(FilePath);
            }
        }
        public MediaInfo FileInfo
        {
            get
            {
                if (_FileInfo == null)
                {
                    var probe = new FFProbe();
                    _FileInfo = probe.GetMediaInfo(FilePath);
                }
                return _FileInfo;
            }
        }
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
        }
        public TimeSpan Duration { get { return FileInfo.Duration; } }
        public IList<Image> Thumbnails
        {
            get
            {
                if (_Thumbnails == null)
                {
                    _Thumbnails = new List<Image>();
                    var step = (int)(Duration.TotalSeconds / (ThumbnailCount + 1));
                    foreach (var i in Enumerable.Range(0, ThumbnailCount))
                    {
                        var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                        var image_stream = new MemoryStream();
                        ffMpeg.GetVideoThumbnail(FilePath, image_stream, step * i);
                        _Thumbnails.Add(Image.FromStream(image_stream));
                    }
                }
                return _Thumbnails;
            }
        }

        private MediaInfo _FileInfo = null;
        private long? _FileSize = null;
        private IList<Image> _Thumbnails = null;

        public VideoFile(string path)
        {
            FilePath = path;
        }

        public bool IsDurationEqual(VideoFile other)
        {
            return Math.Abs((Duration - other.Duration).TotalSeconds) < DurationEqualityLimit;
        }

        public bool AreThumbnailsEqual(VideoFile other)
        {
            IList<bool> ThumbnailDifferences = new List<bool>();
            foreach (var i in Enumerable.Range(0, ThumbnailCount))
            {
                var diff = Thumbnails[i].PercentageDifference(other.Thumbnails[i]);
                ThumbnailDifferences.Add(diff > 0.5d);

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
            foreach (var thumbnail in Thumbnails)
            {
                thumbnail.Dispose();
            }
            Thumbnails.Clear();
            _Thumbnails = null;
        }
    }
}
