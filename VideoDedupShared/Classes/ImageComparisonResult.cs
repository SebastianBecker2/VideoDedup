namespace VideoDedupShared
{
    public class ImageComparisonResult
    {
        public int Index { get; set; }
        public ImageSet LeftImages { get; set; }
        public ImageSet RightImages { get; set; }
        public double Difference { get; set; }
        public int LoadLevel { get; set; }
        public ComparisonResult ComparisonResult { get; set; }
    }
}
