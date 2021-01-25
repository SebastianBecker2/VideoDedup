namespace VideoDedupConsole
{
    using System.Collections.Generic;
    using VideoDedupShared;

    internal class ThumbnailManager
    {
        private class VideoFileRefCounter
        {
            public VideoFile VideoFile { get; set; }
            public int RefCount { get; set; }

        }

        private Dictionary<IVideoFile, VideoFileRefCounter> UniqueVideoFiles { get; } =
            new Dictionary<IVideoFile, VideoFileRefCounter>();

        public IThumbnailSettings Configuration { get; set; }

        public VideoFile AddVideoFileReference(
            DedupEngine.VideoFile videoFile)
        {
            if (UniqueVideoFiles.TryGetValue(videoFile, out var refCounter))
            {
                refCounter.RefCount++;
                return refCounter.VideoFile;
            }

            if (videoFile.ImageCount == Configuration.Count)
            {
                var videoFilePreview = new VideoFile(videoFile);
                UniqueVideoFiles.Add(videoFile, new VideoFileRefCounter
                {
                    VideoFile = videoFilePreview,
                    RefCount = 1,
                });
                return videoFilePreview;
            }

            using (var videoFileWithImages = new DedupEngine.VideoFile(
                    videoFile, Configuration.Count))
            {
                var videoFilePreview = new VideoFile(videoFileWithImages);
                UniqueVideoFiles.Add(videoFile, new VideoFileRefCounter
                {
                    VideoFile = videoFilePreview,
                    RefCount = 1,
                });
                return videoFilePreview;
            }
        }

        public void RemoveVideoFileReference(IVideoFile videoFile)
        {
            var refCounter = UniqueVideoFiles[videoFile];

            if (refCounter.RefCount == 1)
            {
                refCounter.VideoFile.Dispose();
                _ = UniqueVideoFiles.Remove(videoFile);
                return;
            }

            refCounter.RefCount--;
        }

    }
}
