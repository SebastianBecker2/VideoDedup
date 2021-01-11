namespace VideoDedupConsole
{
    using System;
    using VideoDedupShared;
    using Wcf.Contracts.Data;

    internal class DuplicateWrapper
    {
        public DuplicateData DuplicateData { get; }
        public Guid DuplicateId => DuplicateData.DuplicateId;
        public VideoFile File1 => DuplicateData.File1;
        public VideoFile File2 => DuplicateData.File2;
        public DateTime LastRequest { get; set; }

        public DuplicateWrapper(
            VideoFile file1,
            VideoFile file2)
        {
            _ = file1.FileSize;
            _ = file1.Duration;
            _ = file1.VideoCodec;
            _ = file2.FileSize;
            _ = file2.Duration;
            _ = file2.VideoCodec;
            DuplicateData = new DuplicateData
            {
                DuplicateId = Guid.NewGuid(),
                File1 = file1,
                File2 = file2,
            };
        }
    }
}
