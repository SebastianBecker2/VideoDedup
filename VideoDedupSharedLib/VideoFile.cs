// ReSharper disable once CheckNamespace
namespace VideoDedupGrpc
{
    using VideoDedupSharedLib.Interfaces;

    public sealed partial class VideoFile : IVideoFile
    {
        TimeSpan IVideoFile.Duration => Duration.ToTimeSpan();

        DateTime IVideoFile.LastWriteTime => LastWriteTime.ToDateTime();
    }
}
