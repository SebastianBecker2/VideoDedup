namespace VideoDedupShared
{
    using System;

    public interface IVideoFile : IEquatable<IVideoFile>
    {
        string FilePath { get; }
        long FileSize { get; }
        TimeSpan Duration { get; }
        CodecInfo CodecInfo{ get; }
    }
}
