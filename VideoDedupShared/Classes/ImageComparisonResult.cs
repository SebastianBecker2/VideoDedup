namespace VideoDedupShared
{
    using System.IO;

    public class ImageComparisonResult
    {
        public MemoryStream LeftImage { get; set; }
        public MemoryStream RightImage { get; set; }
        public double Difference { get; set; }
        public int ImageLoadLevel { get; set; }
        public ComparisonResult ComparisonResult { get; set; }
    }
}
