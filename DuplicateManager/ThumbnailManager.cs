namespace DuplicateManager
{
    using Google.Protobuf;
    using MpvLib;
    using MpvLib.Exceptions;
    using VideoDedupGrpc;

    internal sealed class ThumbnailManager(ResolutionSettings settings)
    {
        private sealed class VideoFileRefCounter(VideoFile videoFile)
            : IEqualityComparer<VideoFileRefCounter>
        {
            public VideoFile VideoFile { get; } = videoFile;
            public int RefCount { get; set; } = 1;


            public bool Equals(VideoFileRefCounter? x, VideoFileRefCounter? y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                return x.VideoFile.FilePath == y.VideoFile.FilePath;
            }

            public int GetHashCode(VideoFileRefCounter obj) =>
                HashCode.Combine(VideoFile.FilePath);
        }

        private readonly HashSet<VideoFileRefCounter> uniqueVideoFiles = [];

        public ResolutionSettings Settings { get; set; } = settings
                ?? throw new ArgumentNullException(nameof(settings));

        public VideoFile AddVideoFileReference(VideoFile videoFile)
        {
            if (uniqueVideoFiles.TryGetValue(
                    new VideoFileRefCounter(videoFile),
                    out var refCounter))
            {
                refCounter.RefCount++;
                return refCounter.VideoFile;
            }

            IEnumerable<byte[]?> GetThumbnails()
            {
                try
                {
                    using var mpv = new MpvWrapper(
                        videoFile.FilePath,
                        videoFile.Duration?.ToTimeSpan());
                    return mpv
                        .GetImages(0, Settings.ImageCount, Settings.ImageCount)
                        .ToList();
                }
                catch (MpvOperationException)
                {
                    return new byte[Settings.ImageCount][];
                }
            }

            videoFile.Images.AddRange(
                GetThumbnails().Select(ByteString.CopyFrom));

            _ = uniqueVideoFiles.Add(new VideoFileRefCounter(videoFile));
            return videoFile;
        }

        public void RemoveVideoFileReference(VideoFile videoFile)
        {
            if (uniqueVideoFiles.TryGetValue(
                    new VideoFileRefCounter(videoFile),
                    out var refCounter))
            {
                if (refCounter.RefCount == 1)
                {
                    _ = uniqueVideoFiles.Remove(refCounter);
                    return;
                }

                refCounter.RefCount--;
            }
        }
    }
}
