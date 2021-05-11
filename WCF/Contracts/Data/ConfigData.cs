namespace Wcf.Contracts.Data
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using VideoDedupShared;

    [DataContract]
    public class ConfigData : IDedupEngineSettings, IThumbnailSettings
    {
        [DataMember]
        public string BasePath { get; set; }

        // Make IReadOnlyList?
        [DataMember]
        public IEnumerable<string> ExcludedDirectories { get; set; }

        // Make IReadOnlyList?
        [DataMember]
        public IEnumerable<string> FileExtensions { get; set; }

        [DataMember]
        public bool Recursive { get; set; }

        [DataMember]
        public bool MonitorChanges { get; set; }

        [DataMember]
        public int MaxImageCompares { get; set; }
        int IImageComparisonSettings.MaxImageCompares => MaxImageCompares;

        [DataMember]
        public int MaxDifferentImages { get; set; }

        [DataMember]
        public int MaxImageDifferencePercent { get; set; }
        int IImageComparisonSettings.MaxImageDifferencePercent =>
            MaxImageDifferencePercent;

        [DataMember]
        public int MaxDurationDifferenceSeconds { get; set; }
        int IDurationComparisonSettings.MaxDifferenceSeconds =>
            MaxDurationDifferenceSeconds;

        [DataMember]
        public int MaxDurationDifferencePercent { get; set; }
        int IDurationComparisonSettings.MaxDifferencePercent =>
            MaxDurationDifferencePercent;

        [DataMember]
        public DurationDifferenceType DifferenceType { get; set; }

        [DataMember]
        public int ThumbnailCount { get; set; }
        int IThumbnailSettings.Count => ThumbnailCount;

        [DataMember]
        public int SaveStateIntervalMinutes { get; set; }
    }
}
