namespace VideoDedupShared
{
    public class VideoComparisonResult
    {
        public int LastComparedIndex { get; set; }
        public ComparisonResult ComparisonResult { get; set; }
        public string Reason { get; set; }
    }
}
