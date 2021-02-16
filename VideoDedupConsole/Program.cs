namespace VideoDedupConsole
{
    using DedupEngine;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;
    using VideoDedupConsole.DuplicateManagement;
    using VideoDedupConsole.Properties;
    using VideoDedupShared;
    using Wcf.Contracts.Data;
    using MpvException = DedupEngine.MpvLib.MpvException;
    using OperationUpdateEventArgs = DedupEngine.OperationUpdateEventArgs;

    internal class Program
    {
        // The folder name, settings are stored in.
        // Which is actually the company name.
        // Though company name is empty and he still somehow gets this name:"
        private static readonly string ApplicationName = "VideoDedupConsole";

        private static string GetLocalAppPath()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, ApplicationName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        private static readonly Uri WcfBaseAddress =
            new Uri("net.tcp://localhost:41721/VideoDedup");

        private static readonly IReadOnlyDictionary<OperationType, string>
            OperationTypeTexts = new Dictionary<OperationType, string>
        {
            { OperationType.Comparing, "Comparing: {0}/{1}" },
            { OperationType.LoadingMedia, "Loading media info: {0}/{1}" },
            { OperationType.Searching, "Searching for files..." },
            { OperationType.Monitoring, "Monitoring for file changes..." },
            { OperationType.Completed, "Finished comparison" },
            { OperationType.LoadingDuplicates, "Loading duplicates: {0}/{1}" },
        };

        private static DuplicateManager DuplicateManager { get; set; }
         = new DuplicateManager(GetLocalAppPath());

        private static void DuplicateFileLoadedProgressCallback(
            object sender,
            FileLoadedProgressEventArgs e) =>
            OperationInfo = new OperationInfo
            {
                Message = string.Format(
                    OperationTypeTexts[OperationType.LoadingDuplicates],
                    e.Count,
                    e.MaxCount),
                CurrentProgress = e.Count,
                MaximumProgress = e.MaxCount,
                ProgressStyle = ProgressStyle.Continuous,
                StartTime = e.StartTime,
            };

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

        private static void OperationUpdateCallback(
            object sender,
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

        private static readonly DedupEngine Dedupper =
            new DedupEngine(GetLocalAppPath());

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
                BasePath = Settings.Default.SourcePath,
                ExcludedDirectories = excludedDirectories,
                FileExtensions = fileExtensions,
                MaxImageCompares = Settings.Default.MaxThumbnailComparison,
                MaxDifferentImages = Settings.Default.MaxDifferentThumbnails,
                MaxImageDifferencePercent = Settings.Default.MaxDifferencePercentage,
                MaxDurationDifferenceSeconds = Settings.Default.MaxDurationDifferenceSeconds,
                MaxDurationDifferencePercent = Settings.Default.MaxDurationDifferencePercent,
                DifferenceType = durationDifferenceType,
                ThumbnailCount = Settings.Default.ThumbnailCount,
                Recursive = Settings.Default.Recursive,
                MonitorChanges = Settings.Default.MonitorFileChanges,
            };
        }

        internal static void SaveConfig(ConfigData settings)
        {
            Settings.Default.SourcePath = settings.BasePath;
            Settings.Default.ExcludedDirectories =
                JsonConvert.SerializeObject(settings.ExcludedDirectories);
            Settings.Default.FileExtensions =
                JsonConvert.SerializeObject(settings.FileExtensions);

            Settings.Default.MaxThumbnailComparison =
                settings.MaxImageCompares;
            Settings.Default.MaxDifferentThumbnails =
                settings.MaxDifferentImages;
            Settings.Default.MaxDifferencePercentage =
                (settings as IImageComparisonSettings).MaxDifferencePercent;
            Settings.Default.MaxDurationDifferenceSeconds =
                settings.MaxDurationDifferenceSeconds;
            Settings.Default.MaxDurationDifferencePercent =
                (settings as IDurationComparisonSettings).MaxDifferencePercent;
            Settings.Default.DurationDifferenceType =
                settings.DifferenceType.ToString();
            Settings.Default.ThumbnailCount = settings.ThumbnailCount;
            Settings.Default.Recursive = settings.Recursive;
            Settings.Default.MonitorFileChanges =
                settings.MonitorChanges;
            Settings.Default.Save();
        }

        internal static void UpdateConfig(ConfigData settings)
        {
            DuplicateManager.Settings = settings;
            Dedupper.Stop();
            Dedupper.UpdateConfiguration(settings);

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

            DuplicateManager.FileLoadedProgress +=
                DuplicateFileLoadedProgressCallback;

            var config = LoadConfig();
            Dedupper.UpdateConfiguration(config);
            DuplicateManager.Settings = config;

            OperationInfo = new OperationInfo
            {
                ProgressStyle = ProgressStyle.Marquee,
                Message = "Initializing...",
            };

            using (var serviceHost = new ServiceHost(
                typeof(WcfService),
                WcfBaseAddress))
            using (var cancelTokenSource = new CancellationTokenSource())
            {
                serviceHost.Open();

                var startTask = Task.Factory.StartNew(
                    () => DuplicateManager.LoadFromFile(cancelTokenSource.Token),
                    cancelTokenSource.Token)
                    .ContinueWith(t =>
                {
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
                }, TaskContinuationOptions.NotOnCanceled);

                Console.WriteLine("Service running.  Please 'Enter' to exit...");
                _ = Console.ReadLine();

                cancelTokenSource.Cancel();
                try
                {
                    startTask.Wait();
                    Dedupper.Stop();
                }
                catch (AggregateException exc)
                {
                    exc.Handle(x => x is OperationCanceledException);
                }

                serviceHost.Abort();
                serviceHost.Close();
            }
        }
    }
}
