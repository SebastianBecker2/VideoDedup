namespace VideoDedupShared
{
    using System;
    using System.Drawing;

    public interface IVideoFile
    {
        string FilePath { get; }
        long FileSize { get; }
        TimeSpan Duration { get; }
        VideoCodecInfo VideoCodec { get; }

        Bitmap GetThumbnail(int index, int thumbnailCount);
    }
}
