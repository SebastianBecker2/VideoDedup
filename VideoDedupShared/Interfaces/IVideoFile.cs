namespace VideoDedupShared
{
    using System;

    public interface IVideoFile : IEquatable<IVideoFile>
    {
        string FilePath { get; }
        long FileSize { get; }
        DateTime LastWriteTime { get; }
        TimeSpan Duration { get; }
        CodecInfo CodecInfo { get; }
    }
}
