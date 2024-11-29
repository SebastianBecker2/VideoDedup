namespace VideoDedupServer
{
    using System.IO;
    using ComparisonManager;
    using DedupEngine;
    using DedupEngine.EventArgs;
    using DuplicateManager;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using Serilog.Events;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.IVideoFileExtensions;
    using static VideoDedupGrpc.OperationInfo.Types;

    public class VideoDedupService :
        VideoDedupGrpcService.VideoDedupGrpcServiceBase,
        IDisposable
    {
        private static readonly string TrashFolderName = "VideoDedupTrash";

        private readonly DedupEngine dedupEngine;
        private readonly DuplicateManager duplicateManager;
        private readonly List<string> logEntries = [];
        private readonly object logEntriesLock = new();
        private readonly ComparisonManager comparisonManager;
        private readonly CancellationTokenSource cancelTokenSource = new();
        private readonly Task initializationTask;
        private readonly List<ProgressInfo> progressInfos = [];
        private readonly object progressInfoLock = new();
        private OperationInfo operationInfo;
        private Guid logToken = Guid.NewGuid();
        private Guid progressToken = Guid.NewGuid();
        private int duplicatesFound;
        private readonly LogManager logManager;
        private bool disposedValue;

        public VideoDedupService(
            string appDataFolderPath)
        {
            logManager = new(appDataFolderPath);

            AddLogEntry("Starting VideoDedupService");

            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            var settings = LoadConfiguration();

            comparisonManager = new(logManager.ComparisonManagerLogger);

            // Adding TrashPath right before we create DedupEngine to avoid
            // having it permanently in the FolderSettings (so the user won't
            // see it).
            settings.FolderSettings.ExcludedDirectories.Add(
                settings.ResolutionSettings.TrashPath);

            dedupEngine = new DedupEngine(
                appDataFolderPath,
                settings.FolderSettings,
                settings.DurationComparisonSettings,
                settings.VideoComparisonSettings);
            dedupEngine.OperationUpdate += DedupEngine_OperationUpdate;
            dedupEngine.DuplicateFound += DedupEngine_DuplicateFound;
            dedupEngine.Logged += (_, e) =>
                AddLogEntry(e.Message, LogSource.DedupEngine);
            dedupEngine.Started += (_, _) => AddLogEntry("Started DedupEngine");
            dedupEngine.Stopped += DedupEngine_Stopped;

            duplicateManager = new DuplicateManager(settings.ResolutionSettings);

            operationInfo = new OperationInfo
            {
                ProgressStyle = ProgressStyle.Marquee,
                OperationType = OperationType.Initializing,
                StartTime = Timestamp.FromDateTime(DateTime.UtcNow),
            };

            initializationTask = Task.Run(StartDedupEngine);

            AddLogEntry("Started VideoDedupService");
        }

        public override Task<ConfigurationSettings> GetConfiguration(
            Empty request,
            ServerCallContext context) =>
            Task.FromResult(LoadConfiguration());

        public override Task<Empty> SetConfiguration(
            ConfigurationSettings request,
            ServerCallContext context)
        {
            var fs = request.FolderSettings;
            var rs = request.ResolutionSettings;
            rs.TrashPath = Path.Combine(fs.BasePath, TrashFolderName);

            SaveConfiguration(request);
            UpdateConfiguration(request);
            return Task.FromResult(new Empty());
        }

        public override Task<StatusData> GetCurrentStatus(
            Empty request,
            ServerCallContext context)
        {
            var statusData = new StatusData
            {
                OperationInfo = operationInfo,
                CurrentDuplicatesCount = duplicateManager.Count,
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

        public override Task<DuplicateData> GetDuplicate(
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
                if (request.LogToken != logToken.ToString())
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

        public override Task<VideoComparisonStatus> StartVideoComparison(
            VideoComparisonConfiguration request,
            ServerCallContext context) =>
            Task.FromResult(comparisonManager.StartComparison(
                request.VideoComparisonSettings,
                request.LeftFilePath,
                request.RightFilePath,
                request.ForceLoadingAllImages));

        public override Task<VideoComparisonStatus?> GetVideoComparisonStatus(
            VideoComparisonStatusRequest request,
            ServerCallContext context) =>
            Task.FromResult(comparisonManager.GetStatus(
                Guid.Parse(request.ComparisonToken),
                request.ImageComparisonIndex));

        public override Task<Empty> CancelVideoComparison(
            CancelVideoComparisonRequest request,
            ServerCallContext context)
        {
            _ = comparisonManager.CancelComparison(
                Guid.Parse(request.ComparisonToken));
            return Task.FromResult(new Empty());
        }

        public override Task<GetFolderContentResponse> GetFolderContent(
            GetFolderContentRequest request,
            ServerCallContext context)
        {
            try
            {
                var result = new GetFolderContentResponse();
                result.Files.AddRange(
                    FileManager.GetFolderContent(
                        request.Path,
                        request.TypeRestriction));
                return Task.FromResult(result);
            }
            catch (FileNotFoundException)
            {
                return Task.FromResult(
                    new GetFolderContentResponse { RequestFailed = true });
            }
            catch (IOException)
            {
                return Task.FromResult(
                    new GetFolderContentResponse { RequestFailed = true });
            }
            catch (UnauthorizedAccessException)
            {
                return Task.FromResult(
                    new GetFolderContentResponse { RequestFailed = true });
            }
        }

        public override Task<GetProgressInfoResponse> GetProgressInfo(
            GetProgressInfoRequest request,
            ServerCallContext context)
        {
            lock (progressInfoLock)
            {
                if (request.ProgressToken != progressToken.ToString())
                {
                    return Task.FromResult(new GetProgressInfoResponse());
                }

                if (request.Start < 0 || request.Count < 0)
                {
                    return Task.FromResult(new GetProgressInfoResponse());
                }

                var response = new GetProgressInfoResponse();
                response.ProgressInfos.AddRange(progressInfos
                    .Skip(request.Start)
                    .Take(request.Count)
                    .ToList());
                return Task.FromResult(response);
            }
        }

        private void DedupEngine_OperationUpdate(
            object? sender,
            OperationUpdateEventArgs e)
        {
            lock (progressInfoLock)
            {
                var maximumFiles = e.MaxCount;

                if (e.Style == ProgressStyle.Continuous)
                {
                    if (e.Counter == 0)
                    {
                        progressInfos.Clear();
                        progressToken = Guid.NewGuid();
                    }

                    var duration = (DateTime.Now - e.StartTime).TotalSeconds;
                    var fileSpeed = e.Counter / duration;
                    var duplicatesFoundSpeed = duplicatesFound / duration;

                    progressInfos.Add(new ProgressInfo()
                    {
                        FileCount = e.Counter,
                        FileCountSpeed = fileSpeed,
                        DuplicatesFound = duplicatesFound,
                        DuplicatesFoundSpeed = duplicatesFoundSpeed,
                    });
                }
                else
                {
                    maximumFiles = progressInfos.LastOrDefault()?.FileCount ?? 0;
                }

                operationInfo = new OperationInfo()
                {
                    OperationType = e.Type,
                    MaximumFiles = maximumFiles,
                    ProgressStyle = e.Style,
                    StartTime = Timestamp.FromDateTime(e.StartTime.ToUniversalTime()),
                    ProgressCount = progressInfos.Count,
                    ProgressToken = progressToken.ToString(),
                };
            }
        }

        private void DedupEngine_DuplicateFound(
            object? sender,
            DuplicateFoundEventArgs e)
        {
            try
            {
                duplicateManager.AddDuplicate(
                    e.File1.ToVideoFile(),
                    e.File2.ToVideoFile(),
                    e.BasePath);
                duplicatesFound++;
            }
            catch (Exception exc)
            {
                AddLogEntry(
                    exc,
                    "Error adding duplicate to DuplicateManager",
                    LogSource.DedupEngine);
            }
        }

        private void DedupEngine_Stopped(object? sender, StoppedEventArgs e)
        {
            AddLogEntry("Stopped DedupEngine");
            lock (logEntriesLock)
            {
                logEntries.Clear();
                logToken = Guid.NewGuid();
                logManager.DeleteDedupEngineLogger();
            }
        }

        private void AddLogEntry(
            Exception exc,
            string message,
            LogSource source = LogSource.VideoDedupService,
            LogEventLevel level = LogEventLevel.Error)
        {
            switch (source)
            {
                case LogSource.VideoDedupService:
                    logManager.VideoDedupServiceLogger.Write(
                        level,
                        exc,
                        message);
                    break;
                case LogSource.ComparisonManager:
                    logManager.ComparisonManagerLogger.Write(
                        level,
                        exc,
                        message);
                    break;
                case LogSource.DedupEngine:
                    lock (logEntriesLock)
                    {
                        logEntries.Add($"{DateTime.Now:s} {message}");
                        logEntries.Add($"{DateTime.Now:s} {exc.Message}");
                        if (exc.InnerException is not null)
                        {
                            logEntries.Add(
                                $"{DateTime.Now:s} {exc.InnerException.Message}");
                        }
                        logManager.DedupEngineLogger?.Write(level, exc, message);
                    }
                    break;
                default:
                    logManager.VideoDedupServiceLogger.Write(
                        level,
                        exc,
                        $"Invalid LogSource {source} for message '{message}'");
                    break;
            }
        }

        private void AddLogEntry(
            string message,
            LogSource source = LogSource.VideoDedupService,
            LogEventLevel level = LogEventLevel.Information)
        {
            switch (source)
            {
                case LogSource.VideoDedupService:
                    logManager.VideoDedupServiceLogger.Write(level, message);
                    break;
                case LogSource.ComparisonManager:
                    logManager.ComparisonManagerLogger.Write(level, message);
                    break;
                case LogSource.DedupEngine:
                    lock (logEntriesLock)
                    {
                        logEntries.Add($"{DateTime.Now:s} {message}");
                        logManager.DedupEngineLogger?.Write(level, message);
                    }
                    break;
                default:
                    logManager.VideoDedupServiceLogger.Write(
                        level,
                        $"Invalid LogSource {source} for message '{message}'");
                    break;
            }
        }

        public ConfigurationSettings LoadConfiguration()
        {
            AddLogEntry("Loading configuration");

            var configuration = new ConfigurationSettings()
            {
                FolderSettings = ConfigurationManager.GetFolderSettings(),
                VideoComparisonSettings =
                    ConfigurationManager.GetVideoComparisonSettings(),
                DurationComparisonSettings =
                    ConfigurationManager.GetDurationComparisonSettings(),
                LogSettings = ConfigurationManager.GetLogSettings(),
                ResolutionSettings =
                    ConfigurationManager.GetResolutionSettings(),
            };

            AddLogEntry("Loaded configuration");

            return configuration;
        }

        public void SaveConfiguration(ConfigurationSettings settings)
        {
            AddLogEntry("Saving configuration");

            ConfigurationManager.SetFolderSettings(settings.FolderSettings);
            ConfigurationManager.SetVideoComparisonSettings(
                settings.VideoComparisonSettings);
            ConfigurationManager.SetDurationComparisonSettings(
                settings.DurationComparisonSettings);
            ConfigurationManager.SetLogSettings(settings.LogSettings);
            ConfigurationManager.SetResolutionSettings(
                settings.ResolutionSettings);

            Settings.Default.Save();

            AddLogEntry("Saved configuration");
        }

        private void UpdateConfiguration(ConfigurationSettings settings)
        {
            AddLogEntry("Updating configuration");

            logManager.UpdateConfiguration(settings.LogSettings);

            duplicateManager.UpdateSettings(
                settings.ResolutionSettings,
                UpdateSettingsResolution.DiscardDuplicates);

            // Adding TrashPath right before we update the configuration
            // to avoid having it permanently in the FolderSettings (so the user
            // won't see it).
            settings.FolderSettings.ExcludedDirectories.Add(
                settings.ResolutionSettings.TrashPath);

            if (dedupEngine.UpdateConfiguration(
                settings.FolderSettings,
                settings.DurationComparisonSettings,
                settings.VideoComparisonSettings))
            {
                StartDedupEngine();
            }

            AddLogEntry("Updated configuration");
        }

        private void StartDedupEngine()
        {
            logManager.CreateDedupEngineLogger();
            try
            {
                dedupEngine.Start();
                duplicatesFound = 0;
            }
            catch (InvalidOperationException exc)
            {
                operationInfo = new OperationInfo
                {
                    ProgressStyle = ProgressStyle.NoProgress,
                    OperationType = OperationType.Error,
                    StartTime = Timestamp.FromDateTime(DateTime.UtcNow),
                };
                AddLogEntry(
                    exc,
                    "Error starting DedupEngine",
                    LogSource.DedupEngine);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    AddLogEntry("Stopping VideoDedupService");

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

                    AddLogEntry("Stopped VideoDedupService");

                    logManager.Dispose();
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
