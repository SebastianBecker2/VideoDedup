namespace DuplicateManager
{
    using System.Collections.Generic;
    using System.Linq;
    using DedupEngine.MpvLib;
    using Google.Protobuf;
    using VideoDedupGrpc;

    internal class ThumbnailManager
    {
        private class VideoFileRefCounter
        {
            public VideoFileRefCounter(VideoFile videoFile) =>
                VideoFile = videoFile;

            public VideoFile VideoFile { get; }
            public int RefCount { get; set; } = 1;
        }

        private readonly IDictionary<VideoFile, VideoFileRefCounter> uniqueVideoFiles =
            new Dictionary<VideoFile, VideoFileRefCounter>();

        public ThumbnailSettings Settings { get; set; }

        public ThumbnailManager(ThumbnailSettings settings) =>
            Settings = settings
                ?? throw new ArgumentNullException(nameof(settings));

        public VideoFile AddVideoFileReference(VideoFile videoFile)
        {
            if (uniqueVideoFiles.TryGetValue(videoFile, out var refCounter))
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

            uniqueVideoFiles.Add(
                videoFile,
                new VideoFileRefCounter(videoFile));
            return videoFile;
        }

        public void RemoveVideoFileReference(VideoFile videoFile)
        {
            var refCounter = uniqueVideoFiles[videoFile];

            if (refCounter.RefCount == 1)
            {
                _ = uniqueVideoFiles.Remove(videoFile);
                return;
            }

            refCounter.RefCount--;
        }
    }
}
