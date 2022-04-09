namespace DedupEngine.EventArgs
{
    using System;
    using VideoDedupGrpc;
    using VideoFile = VideoFile;

    public class ImageComparedEventArgs : EventArgs
    {
        public VideoFile? LeftVideoFile { get; set; }
        public VideoFile? RightVideoFile { get; set; }
        public ImageSet? LeftImages { get; set; }
        public ImageSet? RightImages { get; set; }
        public double Difference { get; set; }
        public int ImageLoadLevelIndex { get; set; }
        public int ImageComparisonIndex { get; set; }
        public ComparisonResult ImageComparisonResult { get; set; }
        public ComparisonResult VideoComparisonResult { get; set; }
    }
}
