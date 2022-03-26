namespace VideoDedupGrpc
{
    using System;
    using VideoDedupSharedLib.Interfaces;

    public sealed partial class VideoFile : IVideoFile
    {
        TimeSpan IVideoFile.Duration => Duration.ToTimeSpan();

        DateTime IVideoFile.LastWriteTime => LastWriteTime.ToDateTime();

        public bool Equals(IVideoFile? other) =>
            other != null && FilePath == other.FilePath;
    }
}
