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

        DurationDifferenceType IDurationComparisonSettings.DifferenceType => this.DurationDifferenceType;

        int IDurationComparisonSettings.MaxDifferenceSeconds => this.MaxDurationDifferenceSeconds;

        int IDurationComparisonSettings.MaxDifferencePercent => this.MaxDurationDifferencePercent;

        int IThumbnailComparisonSettings.MaxDifferencePercent => this.MaxDifferencePercentage;

        int IThumbnailComparisonSettings.MaxCompares => this.MaxThumbnailComparison;

        int IThumbnailComparisonSettings.MaxDifferentThumbnails => this.MaxDifferentThumbnails;
    }
}
