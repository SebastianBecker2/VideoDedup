namespace DedupEngine
{
    using System;
    using System.IO;
    using VideoDedupShared;

    public class ImageComparedEventArgs : EventArgs
    {
        public VideoFile LeftVideoFile { get; set; }
        public VideoFile RightVideoFile { get; set; }
        public MemoryStream LeftImage { get; set; }
        public MemoryStream RightImage { get; set; }
        public double Difference { get; set; }
        public int ImageLoadLevel { get; set; }
        public int ImageComparisonIndex { get; set; }
        public ComparisonResult ImageComparisonResult { get; set; }
        public ComparisonResult VideoComparisonResult { get; set; }
    }
}
