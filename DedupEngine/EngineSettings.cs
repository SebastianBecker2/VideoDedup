namespace DedupEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using VideoDedupShared;

    internal class EngineSettings :
        IDedupEngineSettings,
        IEquatable<EngineSettings>
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
            MaxDurationDifferencePercent = settings.MaxDifferencePercent;
            MaxImageCompares = settings.MaxImageCompares;
            MaxDifferentImages = settings.MaxDifferentImages;
            MaxImageDifferencePercent = settings.MaxImageDifferencePercent;
            SaveStateIntervalMinutes = settings.SaveStateIntervalMinutes;
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
        int IImageComparisonSettings.MaxImageCompares => MaxImageCompares;

        public int MaxDifferentImages { get; set; }

        public int MaxImageDifferencePercent { get; set; }
        int IImageComparisonSettings.MaxImageDifferencePercent =>
            MaxImageDifferencePercent;

        public int SaveStateIntervalMinutes
        {
            get => (int)SaveStateInterval.TotalMinutes;
            set => SaveStateInterval = TimeSpan.FromMinutes(value);
        }
        public TimeSpan SaveStateInterval { get; set; }

        public override bool Equals(object obj) => Equals(obj as EngineSettings);
        public bool Equals(EngineSettings other) =>
            other != null
            && BasePath == other.BasePath
            && ExcludedDirectories.OrderBy(e => e).SequenceEqual(
                other.ExcludedDirectories.OrderBy(e => e))
            && FileExtensions.OrderBy(e => e).SequenceEqual(
                other.FileExtensions.OrderBy(e => e))
            && Recursive == other.Recursive
            && DifferenceType == other.DifferenceType
            && MaxDurationDifferenceSeconds
                == other.MaxDurationDifferenceSeconds
            && MaxDurationDifferencePercent
                == other.MaxDurationDifferencePercent
            && MaxImageCompares == other.MaxImageCompares
            && MaxDifferentImages == other.MaxDifferentImages
            && MaxImageDifferencePercent == other.MaxImageDifferencePercent;

        public override int GetHashCode()
        {
            var hashCode = 862207841;
            hashCode = (hashCode * -1521134295)
                + EqualityComparer<string>.Default.GetHashCode(BasePath);
            if (ExcludedDirectories != null)
            {
                foreach (var excludedDirectory in ExcludedDirectories)
                {
                    hashCode = (hashCode * -1521134295)
                        + EqualityComparer<string>.Default.GetHashCode(
                            excludedDirectory);
                }
            }
            if (FileExtensions != null)
            {
                foreach (var fileExtension in FileExtensions)
                {
                    hashCode = (hashCode * -1521134295)
                        + EqualityComparer<string>.Default.GetHashCode(
                            fileExtension);
                }
            }
            hashCode = (hashCode * -1521134295)
                + Recursive.GetHashCode();
            hashCode = (hashCode * -1521134295)
                + DifferenceType.GetHashCode();
            hashCode = (hashCode * -1521134295)
                + MaxDurationDifferenceSeconds.GetHashCode();
            hashCode = (hashCode * -1521134295)
                + MaxDurationDifferencePercent.GetHashCode();
            hashCode = (hashCode * -1521134295)
                + MaxImageCompares.GetHashCode();
            hashCode = (hashCode * -1521134295)
                + MaxDifferentImages.GetHashCode();
            hashCode = (hashCode * -1521134295)
                + MaxImageDifferencePercent.GetHashCode();
            return hashCode;
        }

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
