namespace DedupEngine
{
    using VideoDedupSharedLib.Interfaces;

    internal class VideoFile : VideoComparer.VideoFile
    {
        public VideoFile(VideoComparer.VideoFile other) : base(other)
        {
        }

        public VideoFile(IVideoFile other) : base(other)
        {
        }

        public VideoFile(string filePath) : base(filePath)
        {
        }

        private int processing;
        public bool IsProcessing => processing == 1;
        public void StartProcessing() => Interlocked.Exchange(ref processing, 1);
        public void StopProcessing() => Interlocked.Exchange(ref processing, 0);
    }
}
