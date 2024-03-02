namespace VideoDedupServer
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using VideoDedupGrpc;
    using static VideoDedupGrpc.DurationComparisonSettings.Types;
    using static VideoDedupGrpc.LogSettings.Types;

    internal static class ConfigurationManager
    {
        public static FolderSettings GetFolderSettings()
        {
            var settings = new FolderSettings
            {
                BasePath = Settings.Default.BasePath,
                Recursive = Settings.Default.Recursive,
                MonitorChanges = Settings.Default.MonitorFileChanges,
            };

            settings.ExcludedDirectories.AddRange(GetExcludedDirectories());
            settings.FileExtensions.AddRange(GetFileExtensions());

            return settings;
        }

        public static VideoComparisonSettings GetVideoComparisonSettings() =>
            new()
            {
                CompareCount = Settings.Default.ImageCompareCount,
                MaxDifferentImages = Settings.Default.MaxDifferentImages,
                MaxDifference = Settings.Default.MaxImageDifference,
            };

        public static DurationComparisonSettings GetDurationComparisonSettings() =>
            new()
            {
                MaxDifference = Settings.Default.MaxDurationDifference,
                DifferenceType = ToDurationDifferenceType(
                        Settings.Default.DurationDifferenceType),
            };

        public static ThumbnailSettings GetThumbnailSettings() =>
            new()
            {
                ImageCount = Settings.Default.ThumbnailImageCount,
            };

        public static LogSettings GetLogSettings() =>
            new()
            {
                DedupEngineLogLevel =
                    ToLogLevel(Settings.Default.DedupEngineLogLevel),
                CustomComparisonManagerLogLevel =
                    ToLogLevel(Settings.Default.CustomComparisonManagerLogLevel),
                VideoDedupServiceLogLevel =
                    ToLogLevel(Settings.Default.VideoDedupServiceLogLevel),
            };

        public static void SetFolderSettings(FolderSettings settings)
        {
            Settings.Default.BasePath = settings.BasePath;
            Settings.Default.ExcludedDirectories =
                JsonConvert.SerializeObject(settings.ExcludedDirectories);
            Settings.Default.FileExtensions =
                JsonConvert.SerializeObject(settings.FileExtensions);
            Settings.Default.Recursive = settings.Recursive;
            Settings.Default.MonitorFileChanges = settings.MonitorChanges;
        }

        public static void SetVideoComparisonSettings(
            VideoComparisonSettings settings)
        {
            Settings.Default.ImageCompareCount = settings.CompareCount;
            Settings.Default.MaxDifferentImages = settings.MaxDifferentImages;
            Settings.Default.MaxImageDifference = settings.MaxDifference;
        }

        public static void SetDurationComparisonSettings(
            DurationComparisonSettings settings)
        {
            Settings.Default.MaxDurationDifference = settings.MaxDifference;
            Settings.Default.DurationDifferenceType =
                settings.DifferenceType.ToString();
        }

        public static void SetThumbnailSettings(ThumbnailSettings settings) =>
            Settings.Default.ThumbnailImageCount = settings.ImageCount;

        public static void SetLogSettings(LogSettings settings)
        {
            Settings.Default.VideoDedupServiceLogLevel =
                settings.VideoDedupServiceLogLevel.ToString();
            Settings.Default.CustomComparisonManagerLogLevel =
                settings.CustomComparisonManagerLogLevel.ToString();
            Settings.Default.DedupEngineLogLevel =
                settings.DedupEngineLogLevel.ToString();
        }

        private static List<string> GetExcludedDirectories() =>
            JsonConvert.DeserializeObject<List<string>>(
                Settings.Default.ExcludedDirectories)
            ?? new List<string>();

        private static List<string> GetFileExtensions()
        {
            var fileExtensions = JsonConvert.DeserializeObject<List<string>>(
                Settings.Default.FileExtensions);
            if (fileExtensions is not null && fileExtensions.Count > 0)
            {
                return fileExtensions;
            }
            // Default value here, because it's stored as json.
            return new()
            {
                ".mp4", ".mpg", ".avi", ".wmv", ".flv", ".m4v", ".mov",
                ".mpeg", ".rm", ".3gp"
            };
        }

        private static DurationDifferenceType ToDurationDifferenceType(
            string type)
        {
            if (!Enum.TryParse(
                type,
                true,
                out DurationDifferenceType value))
            {
                return DurationDifferenceType.Seconds;
            }
            return value;
        }

        private static LogLevel ToLogLevel(string logLevel)
        {
            if (!Enum.TryParse(logLevel, true, out LogLevel logEventLevel))
            {
                logEventLevel = LogLevel.Information;
            }
            return logEventLevel;
        }
    }
}
