namespace VideoDedup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::VideoDedup.Properties;
    using Newtonsoft.Json;

    internal static class ConfigData
    {

        public static string SourcePath
        {
            get => Settings.Default.SourcePath;
            set
            {
                Settings.Default.SourcePath = value;
                Settings.Default.Save();
            }
        }

        public static IList<string> ExcludedDirectories
        {
            get
            {
                if (excludedDirectories == null)
                {
                    excludedDirectories = JsonConvert.DeserializeObject<List<string>>(Settings.Default.ExcludedDirectories);
                }
                return excludedDirectories;
            }
            set
            {
                excludedDirectories = value;
                Settings.Default.ExcludedDirectories = JsonConvert.SerializeObject(excludedDirectories);
                Settings.Default.Save();
            }
        }
        private static IList<string> excludedDirectories = null;

        public static IList<string> FileExtensions
        {
            get
            {
                if (fileExtensions != null)
                {
                    return fileExtensions;
                }

                fileExtensions = JsonConvert.DeserializeObject<List<string>>(Settings.Default.FileExtensions);
                if (fileExtensions != null || fileExtensions.Any())
                {
                    return fileExtensions;
                }

                fileExtensions = new List<string>
                    {
                        ".mp4", ".mpg", ".avi", ".wmv", ".flv", ".m4v", ".mov", ".mpeg", ".rm", ".mts", ".3gp"
                    };
                return fileExtensions;
            }
            set
            {
                fileExtensions = value;
                Settings.Default.FileExtensions = JsonConvert.SerializeObject(fileExtensions);
                Settings.Default.Save();
            }
        }
        private static IList<string> fileExtensions = null;

        public static int ThumbnailViewCount
        {
            get => Settings.Default.ThumbnailViewCount;
            set
            {
                Settings.Default.ThumbnailViewCount = value;
                Settings.Default.Save();
            }
        }

        public static int MaxThumbnailComparison
        {
            get => Settings.Default.MaxThumbnailComparison;
            set
            {
                Settings.Default.MaxThumbnailComparison = value;
                Settings.Default.Save();
            }
        }

        public static int MaxDifferentThumbnails
        {
            get => Settings.Default.MaxDifferentThumbnails;
            set
            {
                Settings.Default.MaxDifferentThumbnails = value;
                Settings.Default.Save();
            }
        }

        public static int MaxDifferencePercentage
        {
            get => Settings.Default.MaxDifferencePercentage;
            set
            {
                Settings.Default.MaxDifferencePercentage = value;
                Settings.Default.Save();
            }
        }

        public static int MaxDurationDifferenceSeconds
        {
            get => Settings.Default.MaxDurationDifferernceSeconds;
            set
            {
                Settings.Default.MaxDurationDifferernceSeconds = value;
                Settings.Default.Save();
            }
        }

        public static int MaxDurationDifferencePercent
        {
            get => Settings.Default.MaxDurationDifferencePercent;
            set
            {
                Settings.Default.MaxDurationDifferencePercent = value;
                Settings.Default.Save();
            }
        }

        public static DurationDifferenceType DurationDifferenceType
        {
            get
            {
                if (durationDifferenceType == null)
                {
                    if (Enum.TryParse(
                        Settings.Default.DurationDifferenceType,
                        true,
                        out DurationDifferenceType value))
                    {
                        durationDifferenceType = value;
                    }
                    else
                    {
                        durationDifferenceType = DurationDifferenceType.Seconds;
                    }
                }
                return durationDifferenceType.Value;
            }
            set
            {
                durationDifferenceType = value;
                Settings.Default.DurationDifferenceType = value.ToString();
                Settings.Default.Save();
            }
        }
        private static DurationDifferenceType? durationDifferenceType = null;
    }
}
