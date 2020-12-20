namespace VideoDedup
{
    public interface IDurationComparisonSettings
    {
        DurationDifferenceType DifferenceType { get; }
        int MaxDifferenceSeconds { get; }
        int MaxDifferencePercent { get; }
    }
}
