namespace DedupEngine
{
    using VideoDedupGrpc;
    using static VideoDedupGrpc.DurationComparisonSettings.Types;

    internal sealed class EngineSettings(
        FolderSettings folderSettings,
        DurationComparisonSettings durationComparisonSettings,
        VideoComparisonSettings videoComparisonSettings) :
        IEquatable<EngineSettings>
    {
        public string BasePath => folderSettings.BasePath;

        public IEnumerable<string>? ExcludedDirectories => folderSettings.ExcludedDirectories;

        public IEnumerable<string>? FileExtensions => folderSettings.FileExtensions;

        public bool Recursive => folderSettings.Recursive;

        public bool MonitorChanges => folderSettings.MonitorChanges;

        public DurationDifferenceType DurationDifferenceType => durationComparisonSettings.DifferenceType;

        public int MaxDurationDifferenceSeconds => durationComparisonSettings.MaxDifference;

        public int MaxImageCompares => videoComparisonSettings.CompareCount;

        public int MaxDifferentImages => videoComparisonSettings.MaxDifferentImages;

        public int MaxImageDifferencePercent => videoComparisonSettings.MaxDifference;

        public override bool Equals(object? obj) => Equals(obj as EngineSettings);
        public bool Equals(EngineSettings? other) =>
            other is not null
            && BasePath == other.BasePath
            && MaxImageCompares == other.MaxImageCompares;

        public override int GetHashCode() =>
            HashCode.Combine(BasePath, MaxImageCompares);

        public static bool operator ==(
            EngineSettings left,
            EngineSettings right) =>
            EqualityComparer<EngineSettings>.Default.Equals(left, right);
        public static bool operator !=(
            EngineSettings left,
            EngineSettings right) =>
            !(left == right);
    }
}
