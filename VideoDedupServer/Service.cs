namespace VideoDedupServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using DedupEngine;
    using Newtonsoft.Json;
    using CustomComparisonManager;
    using DuplicateManager;
    using VideoDedupShared;
    using Wcf.Contracts.Data;
    using OperationUpdateEventArgs = DedupEngine.OperationUpdateEventArgs;
    using Wcf.Contracts.Services;
    using VideoDedupServer.Properties;

    public class Service : IVideoDedupProvider, IDisposable
    {
        private static readonly IReadOnlyDictionary<OperationType, string>
            OperationTypeTexts = new Dictionary<OperationType, string>
        {
            { OperationType.Comparing, "Comparing: {0}/{1}" },
            { OperationType.LoadingMedia, "Loading media info: {0}/{1}" },
            { OperationType.Searching, "Searching for files..." },
            { OperationType.Monitoring, "Monitoring for file changes..." },
            { OperationType.Completed, "Finished comparison" },
        };

        private readonly DedupEngine dedupper;
        private readonly CancellationTokenSource cancelTokenSource =
            new CancellationTokenSource();
        private readonly DuplicateManager duplicateManager;
        private readonly List<string> logEntries = new List<string>();
        private readonly object logEntriesLock = new object();
        private readonly Task initializationTask;
        private readonly CustomComparisonManager comparisonManager
            = new CustomComparisonManager();

        private bool disposedValue;

        private OperationInfo OperationInfo { get; set; }
        private Guid LogId { get; set; } = Guid.NewGuid();

        public Service(string appDataFolder)
        {
            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            var config = GetConfig();

            dedupper = new DedupEngine(appDataFolder);
            dedupper.OperationUpdate += OperationUpdateCallback;
            dedupper.DuplicateFound += DuplicateFoundCallback;
            dedupper.Logged += LoggedCallback;
            dedupper.UpdateConfiguration(config);

            duplicateManager = new DuplicateManager(config);

            OperationInfo = new OperationInfo
            {
                ProgressStyle = ProgressStyle.Marquee,
                Message = "Initializing...",
            };

            initializationTask = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        dedupper.Start();
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
                });
        }

        private void OperationUpdateCallback(
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

        private void DuplicateFoundCallback(
            object sender,
            DuplicateFoundEventArgs e)
        {
            try
            {
                duplicateManager.AddDuplicate(e.File1, e.File2, e.BasePath);
            }
            catch (AggregateException exc)
            {
                lock (logEntriesLock)
                {
                    AddLogEntry(exc.Message);
                    AddLogEntry(exc.InnerException.Message);
                }
            }
        }

        private void LoggedCallback(object sender,
            LoggedEventArgs e) =>
            AddLogEntry(e.Message);

        private void AddLogEntry(string message)
        {
            lock (logEntriesLock)
            {
                logEntries.Add(DateTime.Now.ToString("s") + " " + message);
            }
        }

        public CustomVideoComparisonStartData StartCustomVideoComparison(
            CustomVideoComparisonData customVideoComparisonData) =>
            comparisonManager.StartCustomComparison(customVideoComparisonData);

        public void CancelCustomVideoComparison(Guid videoComparisonToken) =>
            _ = comparisonManager.CancelCustomComparison(videoComparisonToken);

        public CustomVideoComparisonStatusData GetVideoComparisonStatus(
            Guid videoComparisonToken,
            int imageComparisonIndex = 0) =>
            comparisonManager.GetStatus(
                videoComparisonToken,
                imageComparisonIndex);

        public StatusData GetCurrentStatus()
        {
            var statudData = new StatusData
            {
                Operation = OperationInfo,
            };

            statudData.DuplicateCount = duplicateManager.Count;

            lock (logEntriesLock)
            {
                statudData.LogCount = logEntries.Count();
                statudData.LogToken = LogId;
            }

            return statudData;
        }

        public LogData GetLogEntries(Guid logToken, int start, int count)
        {
            lock (logEntriesLock)
            {
                if (logToken != null && logToken != LogId)
                {
                    return new LogData();
                    ;
                }

                if (start < 0 || start + count > logEntries.Count() + 1)
                {
                    return new LogData();
                }

                return new LogData
                {
                    LogEntries = logEntries.Skip(start).Take(count),
                };
            }
        }

        public DuplicateData GetDuplicate() =>
            duplicateManager.GetDuplicate();

        public void ResolveDuplicate(
            Guid duplicateId,
            ResolveOperation resolveOperation) =>
            duplicateManager.ResolveDuplicate(duplicateId, resolveOperation);

        public void DiscardDuplicates() =>
            duplicateManager.DiscardAll();

        public ConfigData GetConfig()
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
                        ".mpeg", ".rm", ".3gp"
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
                MaxImageDifferencePercent =
                    Settings.Default.MaxDifferencePercentage,
                MaxDurationDifferenceSeconds =
                    Settings.Default.MaxDurationDifferenceSeconds,
                MaxDurationDifferencePercent =
                    Settings.Default.MaxDurationDifferencePercent,
                DifferenceType = durationDifferenceType,
                ThumbnailCount = Settings.Default.ThumbnailCount,
                Recursive = Settings.Default.Recursive,
                MonitorChanges = Settings.Default.MonitorFileChanges,
                SaveStateIntervalMinutes =
                    Settings.Default.SaveStateIntervalMinutes,
            };
        }

        public void SetConfig(ConfigData settings)
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
                (settings as IImageComparisonSettings).MaxImageDifferencePercent;
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
            Settings.Default.SaveStateIntervalMinutes =
                settings.SaveStateIntervalMinutes;
            Settings.Default.Save();

            UpdateConfig(settings);
        }

        private void UpdateConfig(ConfigData settings)
        {
            lock (logEntriesLock)
            {
                logEntries.Clear();
                LogId = Guid.NewGuid();
            }

            duplicateManager.DiscardAll();
            duplicateManager.Settings = settings;
            dedupper.Stop();
            dedupper.UpdateConfiguration(settings);

            try
            {
                dedupper.Start();
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cancelTokenSource.Cancel();
                    try
                    {
                        initializationTask.Wait();
                        dedupper.Stop();
                    }
                    catch (AggregateException exc)
                    {
                        exc.Handle(x => x is OperationCanceledException);
                    }

                    comparisonManager.Dispose();
                    dedupper.Dispose();
                    cancelTokenSource.Dispose();
                    initializationTask.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
