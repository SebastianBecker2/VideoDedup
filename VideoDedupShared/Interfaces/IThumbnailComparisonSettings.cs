namespace VideoDedupShared
{
    public interface IThumbnailComparisonSettings
    {
        int MaxCompares { get; }
        int MaxDifferencePercent { get; }
        int MaxDifferentThumbnails { get; }
    }
}