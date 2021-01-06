namespace VideoDedupShared
{
    using System;
    using System.Drawing;

    public interface IVideoFile : IEquatable<IVideoFile>
    {
        string FilePath { get; }
        long FileSize { get; }
        TimeSpan Duration { get; }
        VideoCodecInfo VideoCodec { get; }

        bool IsDurationEqual(IVideoFile other,
            IDurationComparisonSettings settings);
    }
}
