namespace VideoDedupConsole
{
    using System;
    using VideoDedupShared;

    internal class DuplicateWrapper
    {
        public Guid DuplicateId { get; set; }
        public VideoFile File1 { get; set; }
        public VideoFile File2 { get; set; }
        public Tuple<VideoFile, VideoFile> InnerDuplicate { get; set; }
        public DateTime LastRequest { get; set; }

        public DuplicateWrapper(VideoFile file1, VideoFile file2)
        {
            DuplicateId = Guid.NewGuid();
            File1 = file1;
            File2 = file2;
        }
    }
}
