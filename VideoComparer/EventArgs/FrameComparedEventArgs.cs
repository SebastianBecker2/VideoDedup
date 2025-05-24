namespace VideoComparer.EventArgs
{
    using System;
    using VideoDedupGrpc;
    using VideoFile = VideoFile;

    public class FrameComparedEventArgs : EventArgs
    {
        public VideoFile? LeftVideoFile { get; set; }
        public VideoFile? RightVideoFile { get; set; }
        public FrameSet? LeftFrames { get; set; }
        public FrameSet? RightFrames { get; set; }
        public double Difference { get; set; }
        public int FrameLoadLevelIndex { get; set; }
        public int FrameComparisonIndex { get; set; }
        public ComparisonResult FrameComparisonResult { get; set; }
        public ComparisonResult VideoComparisonResult { get; set; }
    }
}
