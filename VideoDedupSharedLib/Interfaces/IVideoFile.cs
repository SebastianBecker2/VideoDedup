namespace VideoDedupSharedLib.Interfaces
{
    using VideoDedupGrpc;

    public interface IVideoFile
    {
        string FilePath { get; }
        long FileSize { get; }
        TimeSpan Duration { get; }
        CodecInfo? CodecInfo { get; }
        DateTime LastWriteTime { get; }
        DateTime CreationTime { get; }
        DateTime LastAccessTime { get; }
    }
}
