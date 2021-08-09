namespace DuplicateManager
{
    using System.Collections.Generic;
    using DedupEngine.MpvLib;
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

        public IThumbnailSettings Settings { get; set; }

        public VideoFile AddVideoFileReference(
            DedupEngine.VideoFile videoFile)
        {
            if (UniqueVideoFiles.TryGetValue(videoFile, out var refCounter))
            {
                refCounter.RefCount++;
                return refCounter.VideoFile;
            }

            using (var mpv = new MpvWrapper(
                videoFile.FilePath,
                videoFile.Duration))
            {
                var videoFilePreview = new VideoFile(
                    videoFile,
                    mpv.GetImages(0, Settings.Count, Settings.Count));
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
