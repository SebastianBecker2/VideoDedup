namespace VideoDedupServer
{
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using CustomComparisonManager;
    using DedupEngine;
    using DedupEngine.EventArgs;
    using DuplicateManager;
    using Google.Protobuf;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using Newtonsoft.Json;
    using Properties;
    using Serilog;
    using Serilog.Core;
    using Serilog.Events;
    using VideoDedupGrpc;
    using VideoDedupSharedLib;
    using VideoDedupSharedLib.ExtensionMethods.ImageExtensions;
    using VideoDedupSharedLib.ExtensionMethods.IVideoFileExtensions;
    using static VideoDedupGrpc.DurationComparisonSettings.Types;
    using static VideoDedupGrpc.OperationInfo.Types;

    public class VideoDedupService :
        VideoDedupGrpcService.VideoDedupGrpcServiceBase,
        IDisposable
    {
        private static readonly ByteString DriveIcon =
            ByteString.FromStream(Resources.drive.ToMemoryStream(ImageFormat.Png));

        private readonly string appDataFolderPath;
        private readonly DedupEngine dedupEngine;
        private readonly DuplicateManager duplicateManager;
        private readonly List<string> logEntries = new();
        private readonly object logEntriesLock = new();
        private readonly CustomComparisonManager comparisonManager = new();
        private readonly CancellationTokenSource cancelTokenSource = new();
        private readonly Task initializationTask;
        private OperationInfo operationInfo;
        private Guid logToken = Guid.NewGuid();
        private int duplicatesFound;
        private readonly Logger videoDedupServiceLog;
        private readonly Logger comparisonManagerLog;
        private Logger? dedupEngineLog;
        private bool disposedValue;

        private static Logger CreateVideoDedupServiceLogger(
            string appDataFolder) =>
            new LoggerConfiguration()
                .WriteTo.File(
                    Path.Combine(appDataFolder, "VideoDedupService-.log"),
                    formatProvider: CultureInfo.InvariantCulture,
                    rollOnFileSizeLimit: true,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: null,
                    retainedFileTimeLimit: null)
                .CreateLogger();

        private static Logger CreateComparisonManagerLogger(
            string appDataFolder) =>
            new LoggerConfiguration()
                .WriteTo.File(
                    Path.Combine(appDataFolder, "CustomComparisonManager-.log"),
                    formatProvider: CultureInfo.InvariantCulture,
                    rollOnFileSizeLimit: true,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: null,
                    retainedFileTimeLimit: null)
                .CreateLogger();

        private static Logger CreateDedupEngineLogger(
            string appDataFolder) =>
            new LoggerConfiguration()
                .WriteTo.File(
                    Path.Combine(
                        appDataFolder,
                        $"DedupEngine-{DateTime.Now:s}.log".Replace(':', '-')),
                    formatProvider: CultureInfo.InvariantCulture,
                    rollOnFileSizeLimit: true,
                    rollingInterval: RollingInterval.Infinite,
                    retainedFileCountLimit: null,
                    retainedFileTimeLimit: null)
                .CreateLogger();

        public VideoDedupService(
            string appDataFolderPath)
        {
            this.appDataFolderPath = appDataFolderPath;

            videoDedupServiceLog =
                CreateVideoDedupServiceLogger(appDataFolderPath);
            comparisonManagerLog =
                CreateComparisonManagerLogger(appDataFolderPath);

            AddLogEntry("Starting VideoDedupService");

            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            var settings = LoadConfiguration();

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
            dedupEngine.Stopped += (_, _) => AddLogEntry("Stopped DedupEngine");

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
                    GetFolderContent(request.Path, request.TypeRestriction));
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

        private static IEnumerable<GetFolderContentResponse.Types.FileAttributes>
            GetFolderContent(string path, FileType typeRestriction)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return DriveInfo.GetDrives().Select(drive =>
                    new GetFolderContentResponse.Types.FileAttributes
                    {
                        Name = drive.Name,
                        Type = FileType.Folder,
                        Icon = DriveIcon,
                    });
            }

            if (!Directory.Exists(path))
            {
                // If it's a UNC path to server, without subfolder, we show the
                // shares on that server.
                if (!Uri.TryCreate(path, UriKind.Absolute, out var uri)
                    || !uri.IsUnc
                    || path.Trim('\\').Contains('\\'))
                {
                    throw new FileNotFoundException();
                }

                return GetServerShares(path);
            }

            Func<string, IEnumerable<string>> enumerator =
                typeRestriction switch
                {
                    FileType.Folder => Directory.EnumerateDirectories,
                    FileType.File => Directory.EnumerateFiles,
                    FileType.Any => Directory.EnumerateFileSystemEntries,
                    _ => Directory.EnumerateFileSystemEntries
                };

            // Add a backslash at the end to make sure we never try to get
            // the content of the current drive without a backslash or slash
            // at the end. If the current working directory is "d:\subfolder"
            // and we try to get the file system entries from "d:", we would
            // get the file system entries from "d:\subfolder" instead.
            // Adding the backslash prevents that.
            return enumerator(path + "\\")
                .Select(ToFileAttributes)
                .Where(e => e is not null)!;
        }

        private static GetFolderContentResponse.Types.FileAttributes?
            ToFileAttributes(string path)
        {
            try
            {
                var attr = File.GetAttributes(path);
                var type = attr.HasFlag(FileAttributes.Directory)
                    ? FileType.Folder
                    : FileType.File;
                var info = new FileInfo(path);
                var size = type == FileType.Folder ? 0 : info.Length;
                var mimeType = type == FileType.Folder
                    ? "File folder"
                    : FileInfoProvider.GetMimeType(path) ?? "";
                var dateModified =
                    Timestamp.FromDateTime(info.LastWriteTimeUtc);
                return new()
                {
                    Name = Path.GetFileName(path),
                    Size = size,
                    Type = type,
                    DateModified = dateModified,
                    MimeType = mimeType,
                    Icon = null,
                };
            }
            catch
            {
                return null;
            }
        }

        private static IEnumerable<GetFolderContentResponse.Types.FileAttributes>
            GetServerShares(string serverName) =>
            new Vanara.SharedDevices(serverName)
                .Where(kvp => !kvp.Value.IsSpecial && kvp.Value.IsDiskVolume)
                .Select(kvp =>
                    new GetFolderContentResponse.Types.FileAttributes
                    {
                        Name = kvp.Key,
                        Type = FileType.Folder
                    });

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
                    videoDedupServiceLog.Write(level, exc, message);
                    break;
                case LogSource.ComparisonManager:
                    comparisonManagerLog.Write(level, exc, message);
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
                        dedupEngineLog?.Write(level, exc, message);
                    }
                    break;
                default:
                    videoDedupServiceLog.Write(
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
                    videoDedupServiceLog.Write(level, message);
                    break;
                case LogSource.ComparisonManager:
                    comparisonManagerLog.Write(level, message);
                    break;
                case LogSource.DedupEngine:
                    lock (logEntriesLock)
                    {
                        logEntries.Add($"{DateTime.Now:s} {message}");
                        dedupEngineLog?.Write(level, message);
                    }
                    break;
                default:
                    videoDedupServiceLog.Write(
                        level,
                        $"Invalid LogSource {source} for message '{message}'");
                    break;
            }
        }

        public ConfigurationSettings LoadConfiguration()
        {
            AddLogEntry("Loading configuration");

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

            AddLogEntry("Loaded configuration");

            return configData;
        }

        public void SaveConfiguration(ConfigurationSettings settings)
        {
            AddLogEntry("Saving configuration");

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

            AddLogEntry("Saved configuration");
        }

        private void UpdateConfiguration(ConfigurationSettings settings)
        {
            AddLogEntry("Updating configuration");
            dedupEngine.Stop();

            lock (logEntriesLock)
            {
                logEntries.Clear();
                logToken = Guid.NewGuid();
                dedupEngineLog?.Dispose();
                dedupEngineLog = null;
            }

            duplicateManager.UpdateSettings(
                settings.ThumbnailSettings,
                UpdateSettingsResolution.DiscardDuplicates);

            dedupEngine.UpdateConfiguration(
                settings.FolderSettings,
                settings.DurationComparisonSettings,
                settings.VideoComparisonSettings);

            StartDedupEngine();

            AddLogEntry("Updated configuration");
        }

        private void StartDedupEngine()
        {
            dedupEngineLog = CreateDedupEngineLogger(appDataFolderPath);
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

                    dedupEngineLog?.Dispose();
                    dedupEngineLog = null;
                    comparisonManagerLog.Dispose();

                    AddLogEntry("Stopped VideoDedupService");

                    videoDedupServiceLog.Dispose();
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
