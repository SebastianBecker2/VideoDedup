namespace VideoDedupShared
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;

    public interface IVideoFile : IEquatable<IVideoFile>
    {
        string FilePath { get; }
        long FileSize { get; }
        TimeSpan Duration { get; }
        CodecInfo CodecInfo { get; }
        IEnumerable<Image> Images { get; }
        IList<MemoryStream> ImageStreams { get; }
    }
}
