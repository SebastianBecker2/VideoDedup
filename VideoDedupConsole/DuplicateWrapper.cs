namespace VideoDedupConsole
{
    using System;
    using VideoDedupShared;
    using Wcf.Contracts.Data;

    internal class DuplicateWrapper
    {
        public DuplicateData DuplicateData { get; }
        public Guid DuplicateId => DuplicateData.DuplicateId;
        public VideoFilePreview File1 => DuplicateData.File1;
        public VideoFilePreview File2 => DuplicateData.File2;
        public DateTime LastRequest { get; set; }

        public DuplicateWrapper(
            VideoFilePreview file1,
            VideoFilePreview file2) => DuplicateData = new DuplicateData
            {
                DuplicateId = Guid.NewGuid(),
                File1 = file1,
                File2 = file2,
            };
    }
}
