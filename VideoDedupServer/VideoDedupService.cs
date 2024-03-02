namespace VideoDedupServer
{
    using System.IO;
    using CustomComparisonManager;
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
        private readonly DedupEngine dedupEngine;
        private readonly DuplicateManager duplicateManager;
        private readonly List<string> logEntries = new();
        private readonly object logEntriesLock = new();
        private readonly CustomComparisonManager comparisonManager;
        private readonly CancellationTokenSource cancelTokenSource = new();
        private readonly Task initializationTask;
        private OperationInfo operationInfo;
        private Guid logToken = Guid.NewGuid();
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

            comparisonManager = new(logManager.CustomComparisonManagerLogger);

            dedupEngine = new DedupEngine(
                appDataFolderPath,
                settings.FolderSettings,
                settings.DurationComparisonSettings,
                settings.VideoComparisonSettings);
            dedupEngine.OperationUpdate += OperationUpdateCallback;
            dedupEngine.DuplicateFound += DuplicateFoundCallback;
            dedupEngine.Logged += (_, e) =>
                AddLogEntry(e.Message, LogSource.DedupEngine);
            dedupEngine.Started += (_, _) => AddLogEntry("Started DedupEngine");
            dedupEngine.Stopped += DedupEngine_Stopped;

            duplicateManager = new DuplicateManager(settings.ThumbnailSettings);

            operationInfo = new OperationInfo
            {
                ProgressStyle = ProgressStyle.Marquee,
                OperationType = OperationType.Initializing,
            };

            initializationTask = Task.Run(() => StartDedupEngine());

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
                CurrentDuplicateCount = duplicateManager.Count,
                DuplicatesFound = duplicatesFound,
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

        public override Task<GetFolderContentResponse> GetFolderContent(
            GetFolderContentRequest request,
            ServerCallContext context)
        {
            try
            {
                var result = new GetFolderContentResponse();
                result.Files.AddRange(
                    FileManager.GetFolderContent(request.Path, request.TypeRestriction));
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

        private void LoggedCallback(
            object? sender,
            LoggedEventArgs e) =>
            AddLogEntry(
                e.Message,
                LogSource.DedupEngine);

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
                    logManager.CustomComparisonManagerLogger.Write(
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
                    logManager.CustomComparisonManagerLogger.Write(level, message);
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
                ThumbnailSettings = ConfigurationManager.GetThumbnailSettings(),
                LogSettings = ConfigurationManager.GetLogSettings(),
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
            ConfigurationManager.SetThumbnailSettings(
                settings.ThumbnailSettings);
            ConfigurationManager.SetLogSettings(settings.LogSettings);

            Settings.Default.Save();

            AddLogEntry("Saved configuration");
        }

        private void UpdateConfiguration(ConfigurationSettings settings)
        {
            AddLogEntry("Updating configuration");

            logManager.UpdateConfiguration(settings.LogSettings);

            duplicateManager.UpdateSettings(
                settings.ThumbnailSettings,
                UpdateSettingsResolution.DiscardDuplicates);

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
            duplicatesFound = 0;
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
