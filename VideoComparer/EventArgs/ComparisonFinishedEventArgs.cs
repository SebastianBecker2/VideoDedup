namespace VideoComparer.EventArgs
{
    using System;
    using VideoDedupGrpc;
    using VideoFile = VideoFile;

    public class ComparisonFinishedEventArgs : EventArgs
    {
        public ComparisonFinishedEventArgs(
            VideoFile leftVideoFile,
            VideoFile rightVideoFile)
        {
            LeftVideoFile = leftVideoFile;
            RightVideoFile = rightVideoFile;
        }

        public ComparisonResult ComparisonResult { get; set; }
        public VideoFile LeftVideoFile { get; set; }
        public VideoFile RightVideoFile { get; set; }
    }
}
