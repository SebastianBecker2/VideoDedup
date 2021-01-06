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
        private static readonly IReadOnlyDictionary<StatusType, string> StatusInfo
            = new Dictionary<StatusType, string>
        {
            { StatusType.Comparing, "Comparing: {0}/{1}" },
            { StatusType.Loading, "Loading media info: {0}/{1}" },
            { StatusType.Searching, "Searching for files..." },
            { StatusType.Monitoring, "Monitoring for file changes..." },
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

        public static DuplicateData GetDuplicate()
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

        public static void DiscardDuplicates()
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

                Duplicates.Add(new DuplicateWrapper(file1, file2));
            }
        }

        public static void ResolveDuplicate(Guid duplicateId,
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
                LogEntries.Add(message);
            }
        }

        public static LogData GetLogEntries(LogToken logToken)
        {
            lock (LogEntriesLock)
            {
                var logIndex = 0;
                if (logToken != null && logToken.Id == logId)
                {
                    logIndex = logToken.Index;
                }
                return new LogData
                {
                    LogToken = new LogToken
                    {
                        Id = logId,
                        Index = LogEntries.Count(),
                    },
                    LogItems = LogEntries.Skip(logIndex),
                };
            }
        }

        private static StatusData CurrentStatus { get; } = new StatusData();
        private static readonly object CurrentStatusLock = new object();

        public static StatusData GetCurrentStatus()
        {
            lock (CurrentStatusLock)
            {
                lock (DuplicatesLock)
                {
                    CurrentStatus.DuplicateCount = Duplicates.Count();
                }
                return CurrentStatus.Clone();
            }
        }

        private static void ProgressUpdateCallback(object sender,
            ProgressUpdateEventArgs e)
        {
            lock (CurrentStatusLock)
            {
                CurrentStatus.StatusMessage = string.Format(
                    StatusInfo[e.Type], e.Counter, e.MaxCount);
                CurrentStatus.CurrentProgress = e.Counter;
                CurrentStatus.MaximumProgress = e.MaxCount;
                CurrentStatus.ProgressStyle = e.Style;
            }
        }

        private static readonly DedupEngine Dedupper = new DedupEngine();

        public static ConfigData LoadConfig()
        {
            var excludedDirectories = JsonConvert.DeserializeObject<List<string>>(
                Settings.Default.ExcludedDirectories);

            var fileExtensions = JsonConvert.DeserializeObject<List<string>>(
                Settings.Default.FileExtensions);
            if (fileExtensions == null || !fileExtensions.Any())
            {
                fileExtensions = new List<string>
                    {
                        ".mp4", ".mpg", ".avi", ".wmv", ".flv", ".m4v", ".mov", ".mpeg", ".rm", ".mts", ".3gp"
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
            };
        }

        public static void SaveConfig(IDedupperSettings configuration)
        {
            Settings.Default.SourcePath =
                configuration.BasePath;
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
            Settings.Default.ThumbnailCount =
                configuration.Count;
            Settings.Default.Recursive = configuration.Recursive;
            Settings.Default.Save();
        }

        public static void UpdateConfig(ConfigData config)
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
                lock (CurrentStatusLock)
                {
                    CurrentStatus.ProgressStyle = ProgressStyle.NoProgress;
                    CurrentStatus.StatusMessage = exc.Message;
                }
                AddLogEntry(exc.Message);
            }
        }

        private static void Main()
        {
            Dedupper.ProgressUpdate += ProgressUpdateCallback;
            Dedupper.DuplicateFound += DuplicateFoundCallback;
            Dedupper.Logged += LoggedCallback;

            var config = LoadConfig();
            Dedupper.UpdateConfiguration(config);
            ThumbnailManager.Configuration = config;

            lock (CurrentStatusLock)
            {
                CurrentStatus.ProgressStyle = ProgressStyle.Marquee;
                CurrentStatus.StatusMessage = "Initializing...";
            }

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
                    lock (CurrentStatusLock)
                    {
                        CurrentStatus.ProgressStyle = ProgressStyle.NoProgress;
                        CurrentStatus.StatusMessage = exc.Message;
                    }
                    AddLogEntry(exc.Message);
                }

                Console.WriteLine("Service running.  Please 'Enter' to exit...");
                _ = Console.ReadLine();
            }
        }
    }
}
