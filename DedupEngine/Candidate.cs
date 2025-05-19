namespace DedupEngine
{
    internal class Candidate(VideoFile file1, VideoFile file2)
        : IEquatable<Candidate>
    {
        public VideoFile File1 { get; set; } = file1;
        public VideoFile File2 { get; set; } = file2;

        public bool IsErrorCountExceeded(int maxErrorCount) =>
            File1.ErrorCount > maxErrorCount || File2.ErrorCount > maxErrorCount;

        public bool TryStartProcessing(object processingLock)
        {
            lock (processingLock)
            {
                if (File1.IsProcessing || File2.IsProcessing)
                {
                    return false;
                }
                File1.StartProcessing();
                File2.StartProcessing();
            }
            return true;
        }

        public void StopProcessing()
        {
            File1.StopProcessing();
            File2.StopProcessing();
        }

        public bool Equals(Candidate? other) =>
            other != null
            && (File1.Equals(other.File1) || File1.Equals(other.File2))
            && (File2.Equals(other.File1) || File2.Equals(other.File2));

        public override bool Equals(object? obj) => Equals(obj as Candidate);

        public override int GetHashCode()
        {
            var hash1 = File1.GetHashCode();
            var hash2 = File2.GetHashCode();
            return hash1 < hash2 ? HashCode.Combine(hash1, hash2)
                : HashCode.Combine(hash2, hash1);
        }
    }
}
