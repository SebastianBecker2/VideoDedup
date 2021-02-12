namespace DedupEngine
{
    using System.Collections.Generic;
    using VideoDedupShared;

    internal class EngineSettings : IDedupEngineSettings
    {
        public EngineSettings()
        {
        }

        public EngineSettings(IDedupEngineSettings settings)
        {
            BasePath = settings.BasePath;
            ExcludedDirectories = settings.ExcludedDirectories;
            FileExtensions = settings.FileExtensions;
            Recursive = settings.Recursive;
            MonitorChanges = settings.MonitorChanges;
            DifferenceType = settings.DifferenceType;
            MaxDurationDifferenceSeconds = settings.MaxDifferenceSeconds;
            MaxDurationDifferencePercent =
                (settings as IDurationComparisonSettings).MaxDifferencePercent;
            MaxImageCompares = settings.MaxCompares;
            MaxDifferentImages = settings.MaxDifferentImages;
            MaxImageDifferencePercent =
                (settings as IImageComparisonSettings).MaxDifferencePercent;
        }

        public string BasePath { get; set; }

        public IEnumerable<string> ExcludedDirectories { get; set; }

        public IEnumerable<string> FileExtensions { get; set; }

        public bool Recursive { get; set; }

        public bool MonitorChanges { get; set; }

        public DurationDifferenceType DifferenceType { get; set; }

        public int MaxDurationDifferenceSeconds { get; set; }
        int IDurationComparisonSettings.MaxDifferenceSeconds =>
            MaxDurationDifferenceSeconds;

        public int MaxDurationDifferencePercent { get; set; }
        int IDurationComparisonSettings.MaxDifferencePercent =>
            MaxDurationDifferencePercent;

        public int MaxImageCompares { get; set; }
        int IImageComparisonSettings.MaxCompares => MaxImageCompares;

        public int MaxDifferentImages { get; set; }

        public int MaxImageDifferencePercent { get; set; }
        int IImageComparisonSettings.MaxDifferencePercent =>
            MaxImageDifferencePercent;
    }
}
