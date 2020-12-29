namespace Wcf.Contracts.Data
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using VideoDedupShared;

    [DataContract]
    public class ConfigData : IDedupperSettings
    {
        private static readonly string CacheFolderName = "VideoDedupCache";
        private static readonly string CacheFileName = "video_files.cache";

        [DataMember]
        public string SourcePath { get; set; }

        // Make IReadOnlyList?
        [DataMember]
        public IList<string> ExcludedDirectories { get; set; }

        // Make IReadOnlyList?
        [DataMember]
        public IList<string> FileExtensions { get; set; }

        [DataMember]
        public int MaxThumbnailComparison { get; set; }

        [DataMember]
        public int MaxDifferentThumbnails { get; set; }

        [DataMember]
        public int MaxDifferencePercentage { get; set; }

        [DataMember]
        public int MaxDurationDifferenceSeconds { get; set; }

        [DataMember]
        public int MaxDurationDifferencePercent { get; set; }

        [DataMember]
        public DurationDifferenceType DurationDifferenceType { get; set; }

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
