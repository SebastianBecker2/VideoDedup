namespace DuplicateManager
{
    using System;
    using System.Collections.Generic;
    using VideoDedupGrpc;

    internal class DuplicateWrapper : IEquatable<DuplicateWrapper>
    {
        public DuplicateData DuplicateData { get; }
        public string DuplicateId => DuplicateData.DuplicateId;
        public VideoFile File1 => DuplicateData.File1;
        public VideoFile File2 => DuplicateData.File2;
        public DateTime LastRequest { get; set; }

        public DuplicateWrapper(
            VideoFile file1,
            VideoFile file2,
            string basePath) =>
            DuplicateData = new()
            {
                DuplicateId = Guid.NewGuid().ToString(),
                File1 = file1,
                File2 = file2,
                BasePath = basePath,
            };

        public override bool Equals(object? obj) =>
            Equals(obj as DuplicateWrapper);

        public bool Equals(DuplicateWrapper? other) =>
            other is not null
            && ((File1.FilePath == other.File1.FilePath
                && File2.FilePath == other.File2.FilePath)
            || (File1.FilePath == other.File1.FilePath
                && File2.FilePath == other.File2.FilePath));

        public override int GetHashCode() =>
            HashCode.Combine(File1.FilePath, File2.FilePath);

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
