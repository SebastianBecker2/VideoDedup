namespace VideoDedupShared
{
    public class ImageComparisonResult
    {
        public ImageSet LeftImages { get; set; }
        public ImageSet RightImages { get; set; }
        public double Difference { get; set; }
        public int ImageLoadLevel { get; set; }
        public ComparisonResult ComparisonResult { get; set; }
    }
}
