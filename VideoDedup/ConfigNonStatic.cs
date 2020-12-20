namespace VideoDedup
{
    using System.Collections.Generic;

    internal class ConfigNonStatic : IDurationComparisonSettings, IThumbnailComparisonSettings
    {
        public string SourcePath { get; set; }

        public IList<string> ExcludedDirectories { get; set; }

        public IList<string> FileExtensions { get; set; }

        public int MaxThumbnailComparison { get; set; }

        public int MaxDifferentThumbnails { get; set; }

        public int MaxDifferencePercentage { get; set; }

        public int MaxDurationDifferenceSeconds { get; set; }

        public int MaxDurationDifferencePercent { get; set; }

        public DurationDifferenceType DurationDifferenceType { get; set; }

        DurationDifferenceType IDurationComparisonSettings.DifferenceType => DurationDifferenceType;

        int IDurationComparisonSettings.MaxDifferenceSeconds => MaxDurationDifferenceSeconds;

        int IDurationComparisonSettings.MaxDifferencePercent => MaxDurationDifferencePercent;

        int IThumbnailComparisonSettings.MaxDifferencePercent => MaxDifferencePercentage;

        int IThumbnailComparisonSettings.MaxCompares => MaxThumbnailComparison;

        int IThumbnailComparisonSettings.MaxDifferentThumbnails => MaxDifferentThumbnails;
    }
}
