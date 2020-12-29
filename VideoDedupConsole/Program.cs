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
        private static IList<DuplicateWrapper> Duplicates { get; } =
            new List<DuplicateWrapper>();
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
                var duplicate = Duplicates.FirstOrDefault(d => d.LastRequest == null);
                if (duplicate == null)
                {
                    duplicate = Duplicates.OrderBy(d => d.LastRequest).FirstOrDefault();
                }
                return duplicate?.InnerDuplicate;
            }
        }

        private static void HandleDuplicateFoundEvent(object sender,
            DuplicateFoundEventArgs e)
        {
            lock (DuplicatesLock)
            {
                var duplicate = new DuplicateData
                {
                    DuplicateId = Guid.NewGuid(),
                    File1 = e.File1,
                    File2 = e.File2,
                };
                Duplicates.Add(new DuplicateWrapper
                {
                    InnerDuplicate = duplicate,
                });
            }
        }

        public static void ResolveDuplicate(Guid duplicateId,
            ResolveOperation resolveOperation)
        {
            lock (DuplicatesLock)
            {
                var duplicate = Duplicates
                    .FirstOrDefault(d =>
                        d.InnerDuplicate.DuplicateId == duplicateId);

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
                        DeleteFile(duplicate.InnerDuplicate.File1.FilePath);
                        Duplicates.Remove(duplicate);
                        break;
                    case ResolveOperation.DeleteFile2:
                        DeleteFile(duplicate.InnerDuplicate.File2.FilePath);
                        Duplicates.Remove(duplicate);
                        break;
                    case ResolveOperation.Skip:
                        // Do nothing.
                        // The duplicate is kept in the list for later.
                        break;
                    case ResolveOperation.Ignore:
                        Duplicates.Remove(duplicate);
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

        private static void HandleLoggedEvent(object sender,
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

        private static void HandleProgressUpdateEvent(object sender,
            ProgressUpdateEventArgs e)
        {
            lock (CurrentStatusLock)
            {
                CurrentStatus.CurrentProgress = e.Counter;
                CurrentStatus.MaximumProgress = e.MaxCount;
                CurrentStatus.StatusMessage = e.StatusInfo;
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

            return new Wcf.Contracts.Data.ConfigData
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
            Settings.Default.MaxDifferentThumbnails = configuration.MaxDifferentThumbnails;
            Settings.Default.MaxDifferencePercentage =
                (configuration as IThumbnailComparisonSettings).MaxDifferencePercent;
            Settings.Default.MaxDurationDifferenceSeconds = configuration.MaxDifferenceSeconds;
            Settings.Default.MaxDurationDifferencePercent =
                (configuration as IDurationComparisonSettings).MaxDifferencePercent;
            Settings.Default.DurationDifferenceType = configuration.DifferenceType.ToString();
            Settings.Default.Save();
        }

        public static void UpdateConfig(ConfigData config)
        {
            Dedupper.UpdateConfiguration(config);
            Dedupper.Stop();
            lock (LogEntriesLock)
            {
                LogEntries.Clear();
                logId = Guid.NewGuid();
            }
            Dedupper.Start();
        }

        private static void Main()
        {
            Dedupper.ProgressUpdate += HandleProgressUpdateEvent;
            Dedupper.DuplicateFound += HandleDuplicateFoundEvent;
            Dedupper.Logged += HandleLoggedEvent;
            Dedupper.UpdateConfiguration(LoadConfig());

            var baseAddress = new Uri("net.tcp://localhost:41721/hello");
            using (var serviceHost = new ServiceHost(typeof(WcfService), baseAddress))
            {
                serviceHost.Open();
                Dedupper.Start();

                Console.WriteLine("Service running.  Please 'Enter' to exit...");
                _ = Console.ReadLine();
            }
        }
    }
}
