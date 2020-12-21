namespace VideoDedup
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class ConfigData :
        IDedupperSettings,
        IResolverSettings
    {
        private static readonly string CacheFolderName = "VideoDedupCache";
        private static readonly string CacheFileName = "video_files.cache";

        public string SourcePath { get; set; }

        // Make IReadOnlyList?
        public IList<string> ExcludedDirectories { get; set; }

        // Make IReadOnlyList?
        public IList<string> FileExtensions { get; set; }

        public int MaxThumbnailComparison { get; set; }

        public int MaxDifferentThumbnails { get; set; }

        public int MaxDifferencePercentage { get; set; }

        public int MaxDurationDifferenceSeconds { get; set; }

        public int MaxDurationDifferencePercent { get; set; }

        public DurationDifferenceType DurationDifferenceType { get; set; }

        public int ThumbnailViewCount { get; set; }

        public ConfigData Copy() => new ConfigData
        {
            SourcePath = SourcePath,
            ExcludedDirectories = ExcludedDirectories.ToList(),
            FileExtensions = FileExtensions.ToList(),
            MaxThumbnailComparison = MaxThumbnailComparison,
            MaxDifferentThumbnails = MaxDifferentThumbnails,
            MaxDifferencePercentage = MaxDifferencePercentage,
            MaxDurationDifferenceSeconds = MaxDurationDifferenceSeconds,
            MaxDurationDifferencePercent = MaxDurationDifferencePercent,
            DurationDifferenceType = DurationDifferenceType,
            ThumbnailViewCount = ThumbnailViewCount,
        };

        DurationDifferenceType IDurationComparisonSettings.DifferenceType =>
            DurationDifferenceType;

        int IDurationComparisonSettings.MaxDifferenceSeconds =>
            MaxDurationDifferenceSeconds;

        int IDurationComparisonSettings.MaxDifferencePercent =>
            MaxDurationDifferencePercent;

        int IThumbnailComparisonSettings.MaxDifferencePercent =>
            MaxDifferencePercentage;

        int IThumbnailComparisonSettings.MaxCompares => MaxThumbnailComparison;

        int IThumbnailComparisonSettings.MaxDifferentThumbnails =>
            MaxDifferentThumbnails;

        int IResolverSettings.ThumbnailViewCount => ThumbnailViewCount;

        string IFolderSettings.BasePath => SourcePath;

        string IFolderSettings.CachePath
        {
            get
            {
                var cache_folder = Path.Combine(SourcePath, CacheFolderName);
                _ = Directory.CreateDirectory(cache_folder);
                return Path.Combine(cache_folder, CacheFileName);
            }
        }

        IEnumerable<string> IFolderSettings.ExcludedDirectories =>
            ExcludedDirectories;

        IEnumerable<string> IFolderSettings.FileExtensions =>
            FileExtensions;

        IDedupperSettings IDedupperSettings.Copy() => Copy();
    }
}
