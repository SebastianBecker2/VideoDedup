namespace VideoDedupServer
{
    using System.IO;
    using Newtonsoft.Json;

    /// <summary>
    /// Persists <see cref="Settings"/> to the same app data directory used for logs.
    /// <see cref="System.Configuration.LocalFileSettingsProvider"/> is unreliable on
    /// non-Windows (invalid path layout / Save failures); this store is used instead.
    /// </summary>
    internal static class PortableServerSettingsStore
    {
        private const string FileName = "user.settings.json";

        public static void TryLoadFromAppDataDirectory(string appDataFolderPath)
        {
            if (OperatingSystem.IsWindows())
            {
                return;
            }

            var path = Path.Combine(appDataFolderPath, FileName);
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                var snap = JsonConvert.DeserializeObject<Snapshot>(
                    File.ReadAllText(path));
                snap?.Apply();
            }
            catch (JsonException)
            {
            }
            catch (IOException)
            {
            }
        }

        public static void PersistToAppDataDirectory(string appDataFolderPath)
        {
            if (OperatingSystem.IsWindows())
            {
                return;
            }

            _ = Directory.CreateDirectory(appDataFolderPath);
            var path = Path.Combine(appDataFolderPath, FileName);
            var tmp = path + ".tmp";
            var json = JsonConvert.SerializeObject(
                Snapshot.FromCurrentSettings(),
                Formatting.Indented);
            File.WriteAllText(tmp, json);
            File.Move(tmp, path, overwrite: true);
        }

        private sealed class Snapshot
        {
            public string BasePath { get; set; } = "";
            public string ExcludedDirectories { get; set; } = "";
            public string FileExtensions { get; set; } = "";
            public int FrameCompareCount { get; set; }
            public int MaxDifferentFrames { get; set; }
            public int MaxFrameDifference { get; set; }
            public int MaxDurationDifference { get; set; }
            public string DurationDifferenceType { get; set; } = "";
            public int ThumbnailCount { get; set; }
            public bool Recursive { get; set; }
            public bool MonitorFileChanges { get; set; }
            public int SaveStateIntervalMinutes { get; set; }
            public bool UpgradeRequired { get; set; }
            public string VideoDedupServiceLogLevel { get; set; } = "";
            public string ComparisonManagerLogLevel { get; set; } = "";
            public string DedupEngineLogLevel { get; set; } = "";
            public bool MoveToTrash { get; set; }
            public string TrashPath { get; set; } = "";
            public int DedupEngineConcurrencyLevel { get; set; }

            public static Snapshot FromCurrentSettings() =>
                new()
                {
                    BasePath = Settings.Default.BasePath,
                    ExcludedDirectories = Settings.Default.ExcludedDirectories,
                    FileExtensions = Settings.Default.FileExtensions,
                    FrameCompareCount = Settings.Default.FrameCompareCount,
                    MaxDifferentFrames = Settings.Default.MaxDifferentFrames,
                    MaxFrameDifference = Settings.Default.MaxFrameDifference,
                    MaxDurationDifference = Settings.Default.MaxDurationDifference,
                    DurationDifferenceType = Settings.Default.DurationDifferenceType,
                    ThumbnailCount = Settings.Default.ThumbnailCount,
                    Recursive = Settings.Default.Recursive,
                    MonitorFileChanges = Settings.Default.MonitorFileChanges,
                    SaveStateIntervalMinutes =
                        Settings.Default.SaveStateIntervalMinutes,
                    UpgradeRequired = Settings.Default.UpgradeRequired,
                    VideoDedupServiceLogLevel =
                        Settings.Default.VideoDedupServiceLogLevel,
                    ComparisonManagerLogLevel =
                        Settings.Default.ComparisonManagerLogLevel,
                    DedupEngineLogLevel = Settings.Default.DedupEngineLogLevel,
                    MoveToTrash = Settings.Default.MoveToTrash,
                    TrashPath = Settings.Default.TrashPath,
                    DedupEngineConcurrencyLevel =
                        Settings.Default.DedupEngineConcurrencyLevel,
                };

            public void Apply()
            {
                Settings.Default.BasePath = BasePath ?? "";
                Settings.Default.ExcludedDirectories = ExcludedDirectories ?? "";
                Settings.Default.FileExtensions = FileExtensions ?? "";
                Settings.Default.FrameCompareCount = FrameCompareCount;
                Settings.Default.MaxDifferentFrames = MaxDifferentFrames;
                Settings.Default.MaxFrameDifference = MaxFrameDifference;
                Settings.Default.MaxDurationDifference = MaxDurationDifference;
                Settings.Default.DurationDifferenceType =
                    DurationDifferenceType ?? "";
                Settings.Default.ThumbnailCount = ThumbnailCount;
                Settings.Default.Recursive = Recursive;
                Settings.Default.MonitorFileChanges = MonitorFileChanges;
                Settings.Default.SaveStateIntervalMinutes =
                    SaveStateIntervalMinutes;
                Settings.Default.UpgradeRequired = UpgradeRequired;
                Settings.Default.VideoDedupServiceLogLevel =
                    VideoDedupServiceLogLevel ?? "";
                Settings.Default.ComparisonManagerLogLevel =
                    ComparisonManagerLogLevel ?? "";
                Settings.Default.DedupEngineLogLevel = DedupEngineLogLevel ?? "";
                Settings.Default.MoveToTrash = MoveToTrash;
                Settings.Default.TrashPath = TrashPath ?? "";
                Settings.Default.DedupEngineConcurrencyLevel =
                    DedupEngineConcurrencyLevel;
            }
        }
    }
}
