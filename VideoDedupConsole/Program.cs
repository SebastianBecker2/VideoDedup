namespace VideoDedupConsole
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using VideoDedupConsole.Properties;
    using VideoDedupShared;
    using Wcf.Contracts.Data;

    internal class Program
    {
        private static readonly IReadOnlyDictionary<OperationType, string>
            OperationTypeTexts = new Dictionary<OperationType, string>
        {
            { OperationType.Comparing, "Comparing: {0}/{1}" },
            { OperationType.Loading, "Loading media info: {0}/{1}" },
            { OperationType.Searching, "Searching for files..." },
            { OperationType.Monitoring, "Monitoring for file changes..." },
            { OperationType.Completed, "Finished comparison" },
        };

        private static IList<DuplicateWrapper> Duplicates { get; } =
            new List<DuplicateWrapper>();
        private static ThumbnailManager ThumbnailManager { get; } =
            new ThumbnailManager();
        private static readonly object DuplicatesLock = new object();

        public static int GetDuplicateCount()
        {
            lock (DuplicatesLock)
            {
                return Duplicates.Count();
            }
        }

        internal static DuplicateData GetDuplicate()
        {
            lock (DuplicatesLock)
            {
                while (true)
                {
                    if (!Duplicates.Any())
                    {
                        return null;
                    }

                    // OrderBy is stable, so we get the first if multiple
                    // don't have a real LastRequest time-stamp
                    var duplicate = Duplicates
                        .OrderBy(d => d.LastRequest)
                        .First();

                    if (!File.Exists(duplicate.File1.FilePath)
                        || !File.Exists(duplicate.File2.FilePath))
                    {
                        _ = Duplicates.Remove(duplicate);
                        continue;
                    }

                    // To preserve specific order
                    // even when using multiple clients.
                    // The most recently requested will be last
                    // next time (when skipped). Or first when canceled.
                    duplicate.LastRequest = DateTime.Now;
                    return duplicate.DuplicateData;
                }
            }
        }

        internal static void DiscardDuplicates()
        {
            lock (DuplicatesLock)
            {
                foreach (var duplicate in Duplicates)
                {
                    ThumbnailManager.RemoveVideoFileReference(duplicate.File1);
                    ThumbnailManager.RemoveVideoFileReference(duplicate.File2);
                }
                Duplicates.Clear();
            }
        }

        private static void DuplicateFoundCallback(object sender,
            DuplicateFoundEventArgs e)
        {
            lock (DuplicatesLock)
            {
                var file1 = ThumbnailManager.AddVideoFileReference(e.File1);
                var file2 = ThumbnailManager.AddVideoFileReference(e.File2);

                try
                {
                    Duplicates.Add(new DuplicateWrapper(file1, file2, e.BasePath));
                }
                catch (AggregateException exc)
                when (exc.InnerException is VideoDedupShared.MpvLib.MpvException)
                {
                    lock (LogEntriesLock)
                    {
                        AddLogEntry(exc.Message);
                        AddLogEntry(exc.InnerException.Message);
                    }
                }
            }
        }

        internal static void ResolveDuplicate(Guid duplicateId,
            ResolveOperation resolveOperation)
        {
            lock (DuplicatesLock)
            {
                var duplicate = Duplicates
                    .FirstOrDefault(d =>
                        d.DuplicateId == duplicateId);

                if (duplicate == null)
                {
                    return;
                }

                void DeleteFile(string path)
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch (Exception exc)
                    {
                        AddLogEntry(exc.Message);
                    }
                }

                switch (resolveOperation)
                {
                    case ResolveOperation.DeleteFile1:
                        DeleteFile(duplicate.File1.FilePath);
                        ThumbnailManager.RemoveVideoFileReference(duplicate.File1);
                        ThumbnailManager.RemoveVideoFileReference(duplicate.File2);
                        _ = Duplicates.Remove(duplicate);
                        break;
                    case ResolveOperation.DeleteFile2:
                        DeleteFile(duplicate.File2.FilePath);
                        ThumbnailManager.RemoveVideoFileReference(duplicate.File1);
                        ThumbnailManager.RemoveVideoFileReference(duplicate.File2);
                        _ = Duplicates.Remove(duplicate);
                        break;
                    case ResolveOperation.Skip:
                        // Do nothing.
                        // The duplicate is kept in the list for later.
                        break;
                    case ResolveOperation.Discard:
                        ThumbnailManager.RemoveVideoFileReference(duplicate.File1);
                        ThumbnailManager.RemoveVideoFileReference(duplicate.File2);
                        _ = Duplicates.Remove(duplicate);
                        break;
                    case ResolveOperation.Cancel:
                        duplicate.LastRequest = DateTime.MinValue;
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"\"{resolveOperation}\" is invalid"
                            + $"for enum {nameof(ResolveOperation)}");
                }
            }
        }

        private static List<string> LogEntries { get; } = new List<string>();
        private static readonly object LogEntriesLock = new object();
        private static Guid logId = Guid.NewGuid();

        private static void LoggedCallback(object sender,
            LoggedEventArgs e) =>
            AddLogEntry(e.Message);

        private static void AddLogEntry(string message)
        {
            lock (LogEntriesLock)
            {
                LogEntries.Add(DateTime.Now.ToString("s") + " " + message);
            }
        }

        internal static LogData GetLogEntries(Guid logToken,
            int start,
            int count)
        {
            lock (LogEntriesLock)
            {
                if (logToken != null && logToken != logId)
                {
                    return new LogData();
                    ;
                }

                if (start < 0 || start + count > LogEntries.Count() + 1)
                {
                    return new LogData();
                }

                return new LogData
                {
                    LogEntries = LogEntries.Skip(start).Take(count),
                };
            }
        }

        private static OperationInfo OperationInfo { get; set; }
            = null;

        private static void OperationUpdateCallback(object sender,
            OperationUpdateEventArgs e) =>
            OperationInfo = new OperationInfo
            {
                Message = string.Format(
                    OperationTypeTexts[e.Type],
                    e.Counter,
                    e.MaxCount),
                CurrentProgress = e.Counter,
                MaximumProgress = e.MaxCount,
                ProgressStyle = e.Style,
                StartTime = e.StartTime,
            };

        internal static StatusData GetCurrentStatus()
        {
            var statudData = new StatusData
            {
                Operation = OperationInfo,
            };

            lock (DuplicatesLock)
            {
                statudData.DuplicateCount = Duplicates.Count();
            }

            lock (LogEntriesLock)
            {
                statudData.LogCount = LogEntries.Count();
                statudData.LogToken = logId;
            }

            return statudData;
        }

        private static readonly DedupEngine Dedupper = new DedupEngine();

        internal static ConfigData LoadConfig()
        {
            var excludedDirectories = JsonConvert.DeserializeObject<List<string>>(
                Settings.Default.ExcludedDirectories);

            var fileExtensions = JsonConvert.DeserializeObject<List<string>>(
                Settings.Default.FileExtensions);
            if (fileExtensions == null || !fileExtensions.Any())
            {
                // Default value here, because it's stored as json.
                fileExtensions = new List<string>
                    {
                        ".mp4", ".mpg", ".avi", ".wmv", ".flv", ".m4v", ".mov",
                        ".mpeg", ".rm", ".mts", ".3gp"
                    };
            }

            DurationDifferenceType durationDifferenceType;
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

            return new ConfigData
            {
                SourcePath = Settings.Default.SourcePath,
                ExcludedDirectories = excludedDirectories,
                FileExtensions = fileExtensions,
                MaxThumbnailComparison = Settings.Default.MaxThumbnailComparison,
                MaxDifferentThumbnails = Settings.Default.MaxDifferentThumbnails,
                MaxDifferencePercentage = Settings.Default.MaxDifferencePercentage,
                MaxDurationDifferenceSeconds = Settings.Default.MaxDurationDifferenceSeconds,
                MaxDurationDifferencePercent = Settings.Default.MaxDurationDifferencePercent,
                DurationDifferenceType = durationDifferenceType,
                ThumbnailCount = Settings.Default.ThumbnailCount,
                Recursive = Settings.Default.Recursive,
                MonitorFileChanges = Settings.Default.MonitorFileChanges,
            };
        }

        internal static void SaveConfig(IDedupEngineSettings configuration)
        {
            Settings.Default.SourcePath = configuration.BasePath;
            Settings.Default.ExcludedDirectories =
                JsonConvert.SerializeObject(configuration.ExcludedDirectories);
            Settings.Default.FileExtensions =
                JsonConvert.SerializeObject(configuration.FileExtensions);

            Settings.Default.MaxThumbnailComparison = configuration.MaxCompares;
            Settings.Default.MaxDifferentThumbnails =
                configuration.MaxDifferentImages;
            Settings.Default.MaxDifferencePercentage =
                (configuration as IImageComparisonSettings).MaxDifferencePercent;
            Settings.Default.MaxDurationDifferenceSeconds =
                configuration.MaxDifferenceSeconds;
            Settings.Default.MaxDurationDifferencePercent =
                (configuration as IDurationComparisonSettings).MaxDifferencePercent;
            Settings.Default.DurationDifferenceType =
                configuration.DifferenceType.ToString();
            Settings.Default.ThumbnailCount = configuration.Count;
            Settings.Default.Recursive = configuration.Recursive;
            Settings.Default.MonitorFileChanges = configuration.MonitorChanges;
            Settings.Default.Save();
        }

        internal static void UpdateConfig(ConfigData config)
        {
            ThumbnailManager.Configuration = config;
            Dedupper.UpdateConfiguration(config);
            Dedupper.Stop();

            lock (LogEntriesLock)
            {
                LogEntries.Clear();
                logId = Guid.NewGuid();
            }

            try
            {
                Dedupper.Start();
            }
            catch (InvalidOperationException exc)
            {
                OperationInfo = new OperationInfo
                {
                    ProgressStyle = ProgressStyle.NoProgress,
                    Message = exc.Message,
                };
                AddLogEntry(exc.Message);
            }
        }

        private static void Main()
        {
            Dedupper.OperationUpdate += OperationUpdateCallback;
            Dedupper.DuplicateFound += DuplicateFoundCallback;
            Dedupper.Logged += LoggedCallback;

            var config = LoadConfig();
            Dedupper.UpdateConfiguration(config);
            ThumbnailManager.Configuration = config;

            OperationInfo = new OperationInfo
            {
                ProgressStyle = ProgressStyle.Marquee,
                Message = "Initializing...",
            };

            var baseAddress = new Uri("net.tcp://localhost:41721/VideoDedup");
            using (var serviceHost = new ServiceHost(typeof(WcfService), baseAddress))
            {
                serviceHost.Open();
                try
                {
                    Dedupper.Start();
                }
                catch (InvalidOperationException exc)
                {
                    OperationInfo = new OperationInfo
                    {
                        ProgressStyle = ProgressStyle.NoProgress,
                        Message = exc.Message,
                    };
                    AddLogEntry(exc.Message);
                }

                Console.WriteLine("Service running.  Please 'Enter' to exit...");
                _ = Console.ReadLine();
            }
        }
    }
}
