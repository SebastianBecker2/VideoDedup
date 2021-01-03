namespace VideoDedupShared
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;

    public class VideoFileWithThumbnails : IVideoFile
    {
        public VideoFileWithThumbnails() { }
        public VideoFileWithThumbnails(VideoFile videoFile, int thumbnailCount)
        {
            VideoFile = videoFile;
            this.thumbnailCount = thumbnailCount;
        }

        private VideoFile VideoFile { get; }

        public string FilePath
        {
            get
            {
                if (filePath != null)
                {
                    return filePath;
                }
                if (VideoFile != null)
                {
                    return VideoFile.FilePath;
                }
                return null;
            }
            set => filePath = value;
        }
        private string filePath = null;

        public TimeSpan Duration
        {
            get
            {
                if (duration != TimeSpan.Zero)
                {
                    return duration;
                }
                if (VideoFile != null)
                {
                    return VideoFile.Duration;
                }
                return TimeSpan.Zero;
            }
            set => duration = value;
        }
        private TimeSpan duration = TimeSpan.Zero;

        public long FileSize
        {
            get
            {
                if (fileSize != 0)
                {
                    return fileSize;
                }
                if (VideoFile != null)
                {
                    return VideoFile.FileSize;
                }
                return 0;
            }
            set => fileSize = value;
        }
        private long fileSize = 0;

        public VideoCodecInfo VideoCodec
        {
            get
            {
                if (videoCodec != null)
                {
                    return videoCodec;
                }
                if (VideoFile != null)
                {
                    return VideoFile.VideoCodec;
                }
                return null;
            }
            set => videoCodec = value;
        }
        private VideoCodecInfo videoCodec = null;

        // Type Dictionary<int,string> works.
        // Interface  doesn't though.
        // Bitmap instead of Image doesn't either.
        // To bytestream
        //public Bitmap Thumbnail
        //{
        //    get
        //    {
        //        if (thumbnail != null)
        //        {
        //            return thumbnail;
        //        }
        //        if (VideoFile != null)
        //        {
        //            //return null;
        //            //using (var ms = new MemoryStream())
        //            //{
        //            //    VideoFile
        //            //    .GetThumbnail(1, thumbnailCount)
        //            //        .Save(ms, ImageFormat.Jpeg);
        //            //    return ms.ToArray();
        //            //}
        //            //return new Dictionary<int, string>
        //            //{
        //            //    {1, "1" },
        //            //    {2,"2" },
        //            //    {3,"3" },
        //            //    {4,"4" },
        //            //    {5,"5" },
        //            //};
        //            return (Bitmap)VideoFile.GetThumbnail(1, thumbnailCount);
        //        }
        //        return null;
        //    }
        //    set => thumbnail = value;
        //}
        //private Bitmap thumbnail = null;

        public Dictionary<int, Bitmap> Thumbnails
        {
            get
            {
                if (thumbnails != null)
                {
                    return thumbnails;
                }
                if (VideoFile != null)
                {
                    //return null; // Issue: Can't serialize Image!
                    return Enumerable.Range(0, thumbnailCount)
                        .ToDictionary(
                            i => i,
                            i => VideoFile.GetThumbnail(i, thumbnailCount));
                    //{
                    //    var ms = new MemoryStream();
                    //    VideoFile
                    //        .GetThumbnail(i, thumbnailCount)
                    //        .Save(ms, ImageFormat.Jpeg);
                    //    return ms;
                    //});
                }
                return null;
            }
            set => thumbnails = value;
        }
        private Dictionary<int, Bitmap> thumbnails = null;
        private readonly int thumbnailCount = 0;

        public Bitmap GetThumbnail(int index, int _) => Thumbnails[index];
        //Image.FromStream(Thumbnails[index]);
    }
}
