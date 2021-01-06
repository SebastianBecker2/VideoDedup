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
            VideoFile file2) => DuplicateData = new DuplicateData
            {
                DuplicateId = Guid.NewGuid(),
                File1 = file1,
                File2 = file2,
            };
    }
}
