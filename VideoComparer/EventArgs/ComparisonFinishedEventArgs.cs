namespace VideoComparer.EventArgs
{
    using System;
    using VideoDedupGrpc;
    using VideoFile = VideoFile;

    public class ComparisonFinishedEventArgs(
        VideoFile leftVideoFile,
        VideoFile rightVideoFile)
        : EventArgs
    {
        public ComparisonResult ComparisonResult { get; set; }
        public VideoFile LeftVideoFile { get; set; } = leftVideoFile;
        public VideoFile RightVideoFile { get; set; } = rightVideoFile;
    }
}
