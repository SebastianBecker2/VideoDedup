namespace VideoDedupShared
{
    public interface IImageComparisonSettings
    {
        int MaxCompares { get; }
        int MaxDifferencePercent { get; }
        int MaxDifferentImages { get; }
    }
}
