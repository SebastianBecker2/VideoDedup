namespace VideoDedupShared
{
    public interface IImageComparisonSettings
    {
        int MaxImageCompares { get; }
        int MaxImageDifferencePercent { get; }
        int MaxDifferentImages { get; }
    }
}
