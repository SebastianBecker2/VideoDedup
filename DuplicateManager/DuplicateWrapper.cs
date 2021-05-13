namespace DuplicateManager
{
    using System;
    using System.Collections.Generic;
    using VideoDedupShared;
    using Wcf.Contracts.Data;

    internal class DuplicateWrapper : IEquatable<DuplicateWrapper>
    {
        public DuplicateData DuplicateData { get; }
        public Guid DuplicateId => DuplicateData.DuplicateId;
        public VideoFile File1 => DuplicateData.File1;
        public VideoFile File2 => DuplicateData.File2;
        public DateTime LastRequest { get; set; }

        public DuplicateWrapper(
            VideoFile file1,
            VideoFile file2,
            string basePath)
        {
            _ = file1.FileSize;
            _ = file1.Duration;
            _ = file1.CodecInfo;
            _ = file2.FileSize;
            _ = file2.Duration;
            _ = file2.CodecInfo;
            DuplicateData = new DuplicateData
            {
                DuplicateId = Guid.NewGuid(),
                File1 = file1,
                File2 = file2,
                BasePath = basePath,
            };
        }

        public override bool Equals(object obj) =>
            Equals(obj as DuplicateWrapper);

        public bool Equals(DuplicateWrapper other) =>
            other != null
            && ((EqualityComparer<VideoFile>.Default.Equals(File1, other.File1)
            && EqualityComparer<VideoFile>.Default.Equals(File2, other.File2))
            || (EqualityComparer<VideoFile>.Default.Equals(File1, other.File2)
            && EqualityComparer<VideoFile>.Default.Equals(File2, other.File1)));

        public override int GetHashCode()
        {
            var hashCode = 1051446793;
            hashCode = (hashCode * -1521134295)
                + EqualityComparer<VideoFile>.Default.GetHashCode(File1);
            hashCode = (hashCode * -1521134295)
                + EqualityComparer<VideoFile>.Default.GetHashCode(File2);
            return hashCode;
        }

        public static bool operator ==(
            DuplicateWrapper left,
            DuplicateWrapper right) =>
            EqualityComparer<DuplicateWrapper>.Default.Equals(left, right);

        public static bool operator !=(
            DuplicateWrapper left,
            DuplicateWrapper right) =>
            !(left == right);
    }
}
