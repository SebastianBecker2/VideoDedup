namespace VideoDedupShared
{
    public class VideoComparisonResult
    {
        public int LastComparisonIndex { get; set; }
        public ComparisonResult ComparisonResult { get; set; }
        public string Reason { get; set; }
    }
}
