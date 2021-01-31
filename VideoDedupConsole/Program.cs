namespace VideoDedupConsole
{
    using DedupEngine;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using VideoDedupConsole.Properties;
    using VideoDedupShared;
    using Wcf.Contracts.Data;
    using MpvException = DedupEngine.MpvLib.MpvException;
    using OperationUpdateEventArgs = DedupEngine.OperationUpdateEventArgs;

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

        private static readonly DuplicateManager DuplicateManager =
            new DuplicateManager();

        public static int GetDuplicateCount() => DuplicateManager.Count;

        internal static DuplicateData GetDuplicate() =>
            DuplicateManager.GetDuplicate();

        internal static void DiscardDuplicates() =>
            DuplicateManager.DiscardAll();

        private static void DuplicateFoundCallback(
            object sender,
            DuplicateFoundEventArgs e)
        {
            try
            {
                DuplicateManager.AddDuplicate(e.File1, e.File2, e.BasePath);
            }
            catch (AggregateException exc)
            when (exc.InnerException is MpvException)
            {
                lock (LogEntriesLock)
                {
                    AddLogEntry(exc.Message);
                    AddLogEntry(exc.InnerException.Message);
                }
            }
        }

        internal static void ResolveDuplicate(
            Guid duplicateId,
            ResolveOperation resolveOperation) =>
            DuplicateManager.ResolveDuplicate(duplicateId, resolveOperation);

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

            statudData.DuplicateCount = DuplicateManager.Count;

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
            DuplicateManager.Settings = config;
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
            DuplicateManager.Settings = config;

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
