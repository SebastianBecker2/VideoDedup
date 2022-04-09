#undef UseLogger

namespace VideoDedupServer
{
    using System.Threading.Tasks;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
#if UseLogger
    using Microsoft.Extensions.Logging;
#endif
    using CustomComparisonManager;
    using DedupEngine;
    using DedupEngine.EventArgs;
    using DuplicateManager;
    using Newtonsoft.Json;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.IVideoFileExtensions;
    using static VideoDedupGrpc.DurationComparisonSettings.Types;
    using static VideoDedupGrpc.OperationInfo.Types;
    using VideoComparer.MpvLib;

    public class VideoDedupService :
        VideoDedupGrpcService.VideoDedupGrpcServiceBase,
        IDisposable
    {
        private readonly DedupEngine dedupEngine;
        private readonly DuplicateManager duplicateManager;
        private readonly List<string> logEntries = new();
        private readonly object logEntriesLock = new();
        private readonly CustomComparisonManager comparisonManager = new();
        private readonly CancellationTokenSource cancelTokenSource = new();
        private readonly Task initializationTask;
        private OperationInfo operationInfo;
        private Guid logToken = Guid.NewGuid();
#if UseLogger
        private readonly ILogger<VideoDedupService> logger;
#endif
        private bool disposedValue;

        public VideoDedupService(
#if UseLogger
            ILogger<VideoDedupService> logger,
#endif
            string appDataFolder)
        {
#if UseLogger
            this.logger = logger;
#endif

            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            var settings = LoadConfiguration();

            dedupEngine = new DedupEngine(
                appDataFolder,
                settings.FolderSettings,
                settings.DurationComparisonSettings,
                settings.VideoComparisonSettings);
            dedupEngine.OperationUpdate += OperationUpdateCallback;
            dedupEngine.DuplicateFound += DuplicateFoundCallback;
            dedupEngine.Logged += LoggedCallback;

            duplicateManager = new DuplicateManager(settings.ThumbnailSettings);

            operationInfo = new OperationInfo
            {
                ProgressStyle = ProgressStyle.Marquee,
                OperationType = OperationType.Initializing,
            };

            initializationTask = Task.Run(() =>
            {
                try
                {
                    dedupEngine.Start();
                }
                catch (InvalidOperationException exc)
                {
                    operationInfo = new OperationInfo
                    {
                        ProgressStyle = ProgressStyle.NoProgress,
                        OperationType = OperationType.Error,
                    };
                    AddLogEntry(exc.Message);
                }
            });
        }

        public override Task<ConfigurationSettings> GetConfiguration(
            Empty request,
            ServerCallContext context) =>
            Task.FromResult(LoadConfiguration());

        public override Task<Empty> SetConfiguration(
            ConfigurationSettings request,
            ServerCallContext context)
        {
            SaveConfiguration(request);
            return Task.FromResult(new Empty());
        }

        public override Task<StatusData> GetCurrentStatus(
            Empty request,
            ServerCallContext context)
        {
            var statusData = new StatusData
            {
                OperationInfo = operationInfo,
                DuplicateCount = duplicateManager.Count
            };

            lock (logEntriesLock)
            {
                statusData.LogCount = logEntries.Count;
                statusData.LogToken = logToken.ToString();
            }

            return Task.FromResult(statusData);
        }

        public override Task<ResolveDuplicateResponse> ResolveDuplicate(
            ResolveDuplicateRequest request,
            ServerCallContext context)
        {
            try
            {
                duplicateManager.ResolveDuplicate(
                    request.DuplicateId,
                    request.ResolveOperation,
                    request.File);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ResolveDuplicateResponse
                {
                    Successful = false,
                    ErrorMessage = ex.Message,
                    ResolveOperation = request.ResolveOperation,
                });
            }

            return Task.FromResult(new ResolveDuplicateResponse
            {
                Successful = true,
                ResolveOperation = request.ResolveOperation,
            });
        }

        public override Task<DuplicateData?> GetDuplicate(
            Empty request,
            ServerCallContext context) =>
            Task.FromResult(duplicateManager.GetDuplicate());

        public override Task<Empty> DiscardDuplicates(
            Empty request,
            ServerCallContext context)
        {
            duplicateManager.DiscardAll();
            return Task.FromResult(new Empty());
        }

        public override Task<GetLogEntriesResponse> GetLogEntries(
            GetLogEntriesRequest request,
            ServerCallContext context)
        {
            lock (logEntriesLock)
            {
                if (request.LogToken != null
                    && request.LogToken != logToken.ToString())
                {
                    return Task.FromResult(new GetLogEntriesResponse());
                }

                if (request.Start < 0
                    || request.Start + request.Count > logEntries.Count + 1)
                {
                    return Task.FromResult(new GetLogEntriesResponse());
                }

                var response = new GetLogEntriesResponse();
                response.LogEntries.AddRange(
                    logEntries.Skip(request.Start).Take(request.Count).ToList());
                return Task.FromResult(response);
            }
        }

        public override Task<CustomVideoComparisonStatus> StartCustomVideoComparison(
            CustomVideoComparisonConfiguration request,
            ServerCallContext context) =>
            Task.FromResult(comparisonManager.StartCustomComparison(
                request.VideoComparisonSettings,
                request.LeftFilePath,
                request.RightFilePath,
                request.ForceLoadingAllImages));

        public override Task<CustomVideoComparisonStatus?> GetVideoComparisonStatus(
            CustomVideoComparisonStatusRequest request,
            ServerCallContext context) =>
            Task.FromResult(comparisonManager.GetStatus(
                Guid.Parse(request.ComparisonToken),
                request.ImageComparisonIndex));

        public override Task<Empty> CancelCustomVideoComparison(
            CancelCustomVideoComparisonRequest request,
            ServerCallContext context)
        {
            _ = comparisonManager.CancelCustomComparison(
                Guid.Parse(request.ComparisonToken));
            return Task.FromResult(new Empty());
        }

        private void OperationUpdateCallback(
            object? sender,
            OperationUpdateEventArgs e) =>
            operationInfo = new OperationInfo
            {
                OperationType = e.Type,
                CurrentProgress = e.Counter,
                MaximumProgress = e.MaxCount,
                ProgressStyle = e.Style,
                StartTime = Timestamp.FromDateTime(e.StartTime.ToUniversalTime()),
            };

        private void DuplicateFoundCallback(
            object? sender,
            DuplicateFoundEventArgs e)
        {
            try
            {
                duplicateManager.AddDuplicate(
                    e.File1.ToVideoFile(),
                    e.File2.ToVideoFile(),
                    e.BasePath);
            }
            catch (MpvException exc)
            {
                lock (logEntriesLock)
                {
                    AddLogEntry($"{exc.Message} {exc.VideoFilePath}");
                    if (exc.InnerException != null)
                    {
                        AddLogEntry(exc.InnerException.Message);
                    }
                }
            }
            catch (Exception exc)
            {
                lock (logEntriesLock)
                {
                    AddLogEntry(exc.Message);
                    if (exc.InnerException != null)
                    {
                        AddLogEntry(exc.InnerException.Message);
                    }
                }
            }
        }

        private void LoggedCallback(
            object? sender,
            LoggedEventArgs e) =>
            AddLogEntry(e.Message);

        private void AddLogEntry(string message)
        {
            lock (logEntriesLock)
            {
                logEntries.Add(DateTime.Now.ToString("s") + " " + message);
            }
        }

        public static ConfigurationSettings LoadConfiguration()
        {
            var excludedDirectories = JsonConvert.DeserializeObject<List<string>>(
                Settings.Default.ExcludedDirectories) ?? new List<string>();

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
            if (System.Enum.TryParse(
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

            var configData = new ConfigurationSettings
            {
                FolderSettings = new FolderSettings
                {
                    BasePath = Settings.Default.BasePath,
                    Recursive = Settings.Default.Recursive,
                    MonitorChanges = Settings.Default.MonitorFileChanges,
                },
                VideoComparisonSettings = new VideoComparisonSettings
                {
                    CompareCount = Settings.Default.ImageCompareCount,
                    MaxDifferentImages = Settings.Default.MaxDifferentImages,
                    MaxDifference = Settings.Default.MaxImageDifference,
                },
                DurationComparisonSettings = new DurationComparisonSettings
                {
                    MaxDifference = Settings.Default.MaxDurationDifference,
                    DifferenceType = durationDifferenceType,
                },
                ThumbnailSettings = new ThumbnailSettings
                {
                    ImageCount = Settings.Default.ThumbnailImageCount,
                },
            };

            configData.FolderSettings.ExcludedDirectories.AddRange(
                excludedDirectories);
            configData.FolderSettings.FileExtensions.AddRange(fileExtensions);

            return configData;
        }

        public void SaveConfiguration(ConfigurationSettings settings)
        {
            Settings.Default.BasePath = settings.FolderSettings.BasePath;
            Settings.Default.ExcludedDirectories =
                JsonConvert.SerializeObject(
                    settings.FolderSettings.ExcludedDirectories);
            Settings.Default.FileExtensions =
                JsonConvert.SerializeObject(
                    settings.FolderSettings.FileExtensions);
            Settings.Default.Recursive = settings.FolderSettings.Recursive;
            Settings.Default.MonitorFileChanges =
                settings.FolderSettings.MonitorChanges;

            Settings.Default.ImageCompareCount =
                settings.VideoComparisonSettings.CompareCount;
            Settings.Default.MaxDifferentImages =
                settings.VideoComparisonSettings.MaxDifferentImages;
            Settings.Default.MaxImageDifference =
                settings.VideoComparisonSettings.MaxDifference;

            Settings.Default.MaxDurationDifference =
                settings.DurationComparisonSettings.MaxDifference;
            Settings.Default.DurationDifferenceType =
                settings.DurationComparisonSettings.DifferenceType.ToString();

            Settings.Default.ThumbnailImageCount = settings.ThumbnailSettings.ImageCount;

            Settings.Default.Save();

            UpdateConfig(settings);
        }

        private void UpdateConfig(ConfigurationSettings settings)
        {
            lock (logEntriesLock)
            {
                logEntries.Clear();
                logToken = Guid.NewGuid();
            }

            duplicateManager.UpdateSettings(
                settings.ThumbnailSettings,
                UpdateSettingsResolution.DiscardDuplicates);

            dedupEngine.UpdateConfiguration(
                settings.FolderSettings,
                settings.DurationComparisonSettings,
                settings.VideoComparisonSettings);

            try
            {
                dedupEngine.Start();
            }
            catch (InvalidOperationException exc)
            {
                operationInfo = new OperationInfo
                {
                    ProgressStyle = ProgressStyle.NoProgress,
                    OperationType = OperationType.Error,
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
                        dedupEngine.Stop();
                    }
                    catch (AggregateException exc)
                    {
                        exc.Handle(x => x is OperationCanceledException);
                    }

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
