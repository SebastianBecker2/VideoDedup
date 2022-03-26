namespace VideoDedupSharedLib.Interfaces
{
    using System;
    using VideoDedupGrpc;

    public interface IVideoFile : IEquatable<IVideoFile>
    {
        string FilePath { get; }
        long FileSize { get; }
        TimeSpan Duration { get; }
        CodecInfo? CodecInfo { get; }
        DateTime LastWriteTime { get; }
    }
}