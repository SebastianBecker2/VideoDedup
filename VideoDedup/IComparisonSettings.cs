namespace VideoDedup
{
    public interface IComparisonSettings
    {
        int MaxThumbnailComparison { get; }
        int MaxDifferentThumbnails { get; }
        int MaxDifferencePercentage { get; }
        int MaxDurationDifferenceSeconds { get; }
        int MaxDurationDifferencePercent { get; }
        DurationDifferenceType DurationDifferenceType { get; }
    }
}
