namespace DedupEngine
{
    using System;
    using VideoDedupShared;

    public class ComparisonFinishedEventArgs : EventArgs
    {
        public ComparisonResult ComparisonResult { get; set; }
        public VideoFile LeftVideoFile { get; set; }
        public VideoFile RightVideoFile { get; set; }
    }
}
