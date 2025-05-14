namespace DedupEngine
{
    internal class Candidate(VideoFile file1, VideoFile file2)
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
    }
}
