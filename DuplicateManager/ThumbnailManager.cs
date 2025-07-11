namespace DuplicateManager
{
    using Google.Protobuf;
    using FfmpegLib;
    using VideoDedupGrpc;
    using FfmpegLib.Exceptions;

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

        public VideoFile AddVideoFileReference(
            VideoFile videoFile,
            CancellationToken cancelToken)
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
                    var ffmpeg = new FfmpegWrapper(videoFile.FilePath);
                    return [.. ffmpeg
                        .GetFrames(
                            0,
                            Settings.ThumbnailCount,
                            Settings.ThumbnailCount,
                            cancelToken)
                        .Select(i => i is null ? [] : i)];
                }
                catch (FfmpegOperationException)
                {
                    return new byte[Settings.ThumbnailCount][];
                }
            }

            var p = GetThumbnails();

            videoFile.Frames.AddRange(
                p.Select(ByteString.CopyFrom));

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
