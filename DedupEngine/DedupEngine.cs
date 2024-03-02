namespace DedupEngine
{
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using EventArgs;
    using VideoComparer;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.IVideoFileExtensions;
    using VideoDedupSharedLib.ExtensionMethods.TimeSpanExtensions;
    using static VideoDedupGrpc.OperationInfo.Types;
    using VideoFile = VideoComparer.VideoFile;

    public class DedupEngine : IDisposable
    {
        private const string EngineDatastoreFileName = "engine.db";

        private static readonly int DamagedFileRetryCount = 5;

        private bool disposedValue; // For IDisposable

        private object DedupLock { get; } = new();
        private Task? DedupTask { get; set; }
        private CancellationTokenSource CancelSource { get; set; } = new();

        private FileSystemWatcher FileWatcher { get; } = new();

        // ConcurrentDictionary is used as a hash set
        private readonly ConcurrentDictionary<VideoFile, byte> newFiles = new();
        private readonly ConcurrentDictionary<VideoFile, byte> deletedFiles = new();
        private FolderSettings folderSettings;
        private DurationComparisonSettings durationComparisonSettings;
        private VideoComparisonSettings videoComparisonSettings;
        private IList<VideoFile> videoFiles = new List<VideoFile>();
        private readonly EngineDatastore datastore;

        public event EventHandler<StartedEventArgs>? Started;
        protected virtual void OnStarted() =>
            Started?.Invoke(this, new StartedEventArgs());

        public event EventHandler<StoppedEventArgs>? Stopped;
        protected virtual void OnStopped() =>
            Stopped?.Invoke(this, new StoppedEventArgs());

        public event EventHandler<DuplicateFoundEventArgs>? DuplicateFound;
        protected virtual void OnDuplicateFound(
            VideoFile file1,
            VideoFile file2) =>
            DuplicateFound?.Invoke(
                this,
                new DuplicateFoundEventArgs(folderSettings.BasePath, file1, file2));

        public event EventHandler<OperationUpdateEventArgs>? OperationUpdate;
        private DateTime operationStartTime = DateTime.MinValue;
        protected virtual void OnOperationUpdate(OperationType type,
            int counter,
            int maxCount) =>
            OperationUpdate?.Invoke(this, new OperationUpdateEventArgs
            {
                Type = type,
                Counter = counter,
                MaxCount = maxCount,
                Style = ProgressStyle.Continuous,
                StartTime = operationStartTime,
            });
        protected virtual void OnOperationUpdate(OperationType type,
            ProgressStyle style) =>
            OperationUpdate?.Invoke(this, new OperationUpdateEventArgs
            {
                Type = type,
                Counter = 0,
                MaxCount = 0,
                Style = style,
                StartTime = operationStartTime,
            });

        public event EventHandler<LoggedEventArgs>? Logged;
        protected virtual void OnLogged(string message) =>
            Logged?.Invoke(this, new LoggedEventArgs(message));

        public DedupEngine(
            string appDataFolder,
            FolderSettings folderSettings,
            DurationComparisonSettings durationComparisonSettings,
            VideoComparisonSettings videoComparisonSettings)
        {
            this.folderSettings = folderSettings;
            this.durationComparisonSettings = durationComparisonSettings;
            this.videoComparisonSettings = videoComparisonSettings;

            if (string.IsNullOrWhiteSpace(appDataFolder))
            {
                throw new ArgumentException($"'{nameof(appDataFolder)}' cannot" +
                    "be null or whitespace", nameof(appDataFolder));
            }

            datastore = new EngineDatastore(
                Path.Combine(appDataFolder, EngineDatastoreFileName));

            FileWatcher.Changed += HandleFileWatcherChangedEvent;
            FileWatcher.Deleted += HandleFileWatcherDeletedEvent;
            FileWatcher.Error += HandleFileWatcherErrorEvent;
            FileWatcher.Renamed += HandleFileWatcherRenamedEvent;
            FileWatcher.Created += HandleFileWatcherCreatedEvent;
        }

        public bool UpdateConfiguration(
            FolderSettings folderSettings,
            DurationComparisonSettings durationComparisonSettings,
            VideoComparisonSettings videoComparisonSettings)
        {
            if (this.folderSettings.Equals(folderSettings)
                && this.durationComparisonSettings.Equals(durationComparisonSettings)
                && this.videoComparisonSettings.Equals(videoComparisonSettings))
            {
                return false;
            }
            Stop();
            this.folderSettings = folderSettings;
            this.durationComparisonSettings = durationComparisonSettings;
            this.videoComparisonSettings = videoComparisonSettings;
            return true;
        }

        public void Start()
        {
            OnLogged("Starting DedupEngine");

            if (!Directory.Exists(folderSettings.BasePath))
            {
                throw new InvalidOperationException("Unable to start. " +
                    "Base path is not valid.");
            }

            if (DedupTask is { IsCompleted: false })
            {
                return;
            }

            FileWatcher.Path = folderSettings.BasePath;
            FileWatcher.IncludeSubdirectories = folderSettings.Recursive;
            FileWatcher.EnableRaisingEvents = folderSettings.MonitorChanges;

            lock (DedupLock)
            {
                DedupTask?.Dispose();
                CancelSource = new CancellationTokenSource();
                DedupTask = Task.Run(ProcessFolder, CancelSource.Token);
            }

            OnStarted();
            OnLogged("Started DedupEngine");
        }

        public void Stop()
        {
            OnLogged("Stopping DedupEngine");

            FileWatcher.EnableRaisingEvents = false;

            if (DedupTask is { IsCompleted: false })
            {
                CancelSource.Cancel();
                try
                {
                    DedupTask?.Wait();
                }
                catch (AggregateException exc)
                {
                    exc.Handle(x => x is OperationCanceledException);
                }
            }

            OnStopped();
            OnLogged("Stopped DedupEngine");
        }

        private void StartProcessingChanges()
        {
            lock (DedupLock)
            {
                // If the task is still running,
                // it will check for new files on it's own.
                if (DedupTask is { IsCompleted: false })
                {
                    return;
                }

                DedupTask = Task.Run(ProcessChanges, CancelSource.Token);
            }
        }

        private void HandleFileWatcherDeletedEvent(
            object sender,
            FileSystemEventArgs e)
        {
            OnLogged($"{nameof(HandleFileWatcherDeletedEvent)} - {e.FullPath}");
            HandleDeletedFileEvent(e.FullPath);
        }

        private void HandleFileWatcherChangedEvent(
            object sender,
            FileSystemEventArgs e)
        {
            OnLogged($"{nameof(HandleFileWatcherChangedEvent)} - {e.FullPath}");
            HandleNewFileEvent(e.FullPath);
        }

        private void HandleFileWatcherCreatedEvent(
            object sender,
            FileSystemEventArgs e)
        {
            OnLogged($"{nameof(HandleFileWatcherCreatedEvent)} - {e.FullPath}");
            HandleNewFileEvent(e.FullPath);
        }

        private void HandleFileWatcherRenamedEvent(
            object sender,
            RenamedEventArgs e)
        {
            OnLogged($"{nameof(HandleFileWatcherRenamedEvent)} - {e.FullPath} - {e.OldFullPath}");
            HandleDeletedFileEvent(e.OldFullPath);
            HandleNewFileEvent(e.FullPath);
        }

        private void HandleFileWatcherErrorEvent(
            object sender,
            ErrorEventArgs e) =>
            OnLogged("FileWatcher crashed! Unable to continue monitoring the" +
                     " source folder.");

        private bool IsFilePathRelevant(
            string filePath,
            FolderSettings folderSettings)
        {
            if (!filePath.StartsWith(
                folderSettings.BasePath,
                StringComparison.OrdinalIgnoreCase))
            {
                OnLogged($"File not in source folder: {filePath}");
                return false;
            }

            if (folderSettings.ExcludedDirectories?.Any(p => filePath.StartsWith(
                p,
                StringComparison.OrdinalIgnoreCase)) ?? false)
            {
                OnLogged($"File is in excluded directory: {filePath}");
                return false;
            }

            var extension = Path.GetExtension(filePath);
            if (!folderSettings.FileExtensions?.Contains(extension) ?? false)
            {
                OnLogged($"File doesn't have proper file extension: {filePath}");
                return false;
            }

            return true;
        }

        private void HandleDeletedFileEvent(string filePath)
        {
            if (!IsFilePathRelevant(filePath, folderSettings))
            {
                return;
            }

            _ = deletedFiles.TryAdd(new VideoFile(filePath), 0);
            OnLogged($"File deleted: {filePath}");
            StartProcessingChanges();
        }

        private void HandleNewFileEvent(string filePath)
        {
            if (!IsFilePathRelevant(filePath, folderSettings))
            {
                return;
            }

            _ = newFiles.TryAdd(new VideoFile(filePath), 0);
            OnLogged($"File created: {filePath}");
            StartProcessingChanges();
        }

        private static IEnumerable<string> GetAllAccessibleFilesIn(
            string rootDirectory,
            IEnumerable<string>? excludedDirectories = null,
            bool recursive = true,
            string searchPattern = "*.*")
        {
            if (Path.GetFileName(rootDirectory) == "$RECYCLE.BIN")
            {
                return new List<string>();
            }

            IEnumerable<string> files = new List<string>();
            excludedDirectories ??= new List<string>();

            try
            {
                files = files.Concat(Directory.EnumerateFiles(rootDirectory,
                    searchPattern, SearchOption.TopDirectoryOnly));

                if (recursive)
                {
                    foreach (var directory in Directory
                        .GetDirectories(rootDirectory)
                        .Where(d => !excludedDirectories.Contains(d,
                            StringComparer.InvariantCultureIgnoreCase)))
                    {
                        files = files.Concat(GetAllAccessibleFilesIn(directory,
                            excludedDirectories, recursive, searchPattern));
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Don't do anything if we cannot access a file.
            }

            return files;
        }

        private IEnumerable<VideoFile> GetVideoFileList(
            FolderSettings folderSettings)
        {
            operationStartTime = DateTime.Now;
            OnOperationUpdate(OperationType.Searching, ProgressStyle.Marquee);

            return GetAllAccessibleFilesIn(
                folderSettings.BasePath,
                folderSettings.ExcludedDirectories,
                folderSettings.Recursive)
                .Where(f => folderSettings.FileExtensions?.Contains(
                    Path.GetExtension(f),
                    StringComparer.InvariantCultureIgnoreCase) ?? true)
                .Select(f => new VideoFile(f));
        }

        private void PreloadFiles(
            IEnumerable<VideoFile> videoFiles,
            CancellationToken cancelToken)
        {
            var counter = 0;
            var fileCount = videoFiles.Count();

            operationStartTime = DateTime.Now;
            OnOperationUpdate(
                OperationType.LoadingMedia,
                counter,
                fileCount);

            foreach (var file in videoFiles)
            {
                OnOperationUpdate(OperationType.LoadingMedia,
                    ++counter,
                    fileCount);

                var duration = datastore.GetVideoFileDuration(file);
                if (duration.HasValue)
                {
                    file.Duration = duration.Value;
                }
                else
                {
                    _ = file.Duration;
                    datastore.InsertVideoFile(file);
                }

                if (cancelToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        private void FindDuplicates(CancellationToken cancelToken)
        {
            operationStartTime = DateTime.Now;
            OnOperationUpdate(
                OperationType.Comparing,
                0,
                videoFiles.Count);

            var timer = Stopwatch.StartNew();

            videoFiles = videoFiles
                .OrderBy(f => f.Duration)
                .ToList();

            foreach (var index in
                Enumerable.Range(0, Math.Max(videoFiles.Count - 1, 0)))
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                var file = videoFiles[index];

                OnLogged($"Checking: {file.FilePath} - Duration: " +
                    $"{file.Duration.ToPrettyString()}");
                OnOperationUpdate(
                    OperationType.Comparing,
                    index + 1,
                    videoFiles.Count);

                foreach (var other in videoFiles
                    .Skip(index + 1)
                    .TakeWhile(f =>
                        file.IsDurationEqual(f, durationComparisonSettings))
                    .Where(f => f.ErrorCount <= DamagedFileRetryCount))
                {
                    if (file.ErrorCount > DamagedFileRetryCount)
                    {
                        break;
                    }

                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }

                    CompareVideoFiles(file, other, cancelToken);
                }
            }

            timer.Stop();
            Debug.Print($"Dedup took {timer.ElapsedMilliseconds} ms");

            OnOperationUpdate(OperationType.Comparing,
                videoFiles.Count,
                videoFiles.Count);
        }

        private void FindDuplicatesOf(
            IEnumerable<VideoFile> videoFiles,
            VideoFile file,
            CancellationToken cancelToken)
        {
            OnLogged($"Checking: {file.FilePath} - Duration: " +
                    $"{file.Duration.ToPrettyString()}");

            foreach (var other in videoFiles
                .Where(f => f != file)
                .Where(f => f.IsDurationEqual(file, durationComparisonSettings))
                .Where(f => f.ErrorCount <= DamagedFileRetryCount))
            {
                if (file.ErrorCount > DamagedFileRetryCount)
                {
                    break;
                }

                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                CompareVideoFiles(file, other, cancelToken);
            }
        }

        private void CompareVideoFiles(
            VideoFile left,
            VideoFile right,
            CancellationToken cancelToken)
        {
            try
            {
                var comparer = new VideoComparer(
                        videoComparisonSettings,
                        datastore.DatastoreFilePath,
                        left,
                        right);
                if (comparer.Compare(cancelToken)
                    != ComparisonResult.Duplicate)
                {
                    return;
                }
                OnLogged($"Found duplicate of {left.FilePath} and" +
                    $" {right.FilePath}");
                OnDuplicateFound(left, right);
            }
            catch (ComparisonException exc)
            {
                if (++exc.VideoFile.ErrorCount > DamagedFileRetryCount)
                {
                    _ = deletedFiles.TryAdd(
                        new VideoFile(exc.VideoFile),
                        0);
                    OnLogged($"File corrupted: {exc.VideoFile.FilePath}");
                }
                else
                {
                    OnLogged($"Comparison failed. Couldn't access: " +
                        $"{exc.VideoFile.FilePath}");
                }
            }
            catch (Exception exc)
            {
                OnLogged($"Critical Error: {exc.Message}");
                throw;
            }
        }

        private void ProcessChangesIfAny()
        {
            lock (DedupLock)
            {
                if (newFiles.Any() || deletedFiles.Any())
                {
                    DedupTask = Task.Run(ProcessChanges, CancelSource.Token);
                    return;
                }

                if (!folderSettings.MonitorChanges)
                {
                    operationStartTime = DateTime.MinValue;
                    OnOperationUpdate(
                        OperationType.Completed,
                        ProgressStyle.NoProgress);
                    OnLogged("Finished comparison.");
                    return;
                }

                operationStartTime = DateTime.Now;
                OnOperationUpdate(
                    OperationType.Monitoring,
                    ProgressStyle.Marquee);
                OnLogged("Monitoring for file changes...");
            }
        }

        private void ProcessFolder()
        {
            var cancelToken = CancelSource.Token;

            videoFiles = GetVideoFileList(folderSettings).ToList();
            cancelToken.ThrowIfCancellationRequested();

            // Cancellable preload of files
            OnLogged("Starting preloading media info of " +
                $"{videoFiles.Count} Files.");
            PreloadFiles(videoFiles, cancelToken);
            if (cancelToken.IsCancellationRequested)
            {
                cancelToken.ThrowIfCancellationRequested();
            }
            OnLogged("Finished preloading media info of " +
                $"{videoFiles.Count} Files.");

            // Remove invalid files
            videoFiles = videoFiles
                .Where(f => f.Duration != TimeSpan.Zero)
                .ToList();
            cancelToken.ThrowIfCancellationRequested();

            OnLogged("Starting searching for duplicates of " +
                $"{videoFiles.Count} Files.");
            FindDuplicates(cancelToken);
            if (cancelToken.IsCancellationRequested)
            {
                cancelToken.ThrowIfCancellationRequested();
            }
            OnLogged("Finished searching for duplicates of " +
                $"{videoFiles.Count} Files.");

            ProcessChangesIfAny();
        }

        private void ProcessChanges()
        {
            var cancelToken = CancelSource.Token;

            while (deletedFiles.Any())
            {
                var deletedFile = deletedFiles.First().Key;
                _ = deletedFiles.TryRemove(deletedFile, out var _);

                if (videoFiles.Remove(deletedFile))
                {
                    OnLogged($"Removed file: {deletedFile.FilePath}");
                }
                else
                {
                    OnLogged("Deleted file not in VideoFile-List: " +
                        $"{deletedFile.FilePath}");
                }
                cancelToken.ThrowIfCancellationRequested();
            }

            operationStartTime = DateTime.Now;
            OnOperationUpdate(OperationType.Comparing, 0, newFiles.Count);
            // We need to count for the ProgressUpdate since we shrink
            // the Queue on every iteration.
            var filesProcessed = 1;
            while (newFiles.Any())
            {
                var newFile = newFiles.First().Key;
                _ = newFiles.TryRemove(newFile, out var _);
                OnOperationUpdate(OperationType.Comparing, filesProcessed,
                    filesProcessed + newFiles.Count);
                filesProcessed++;

                if (!newFile.WaitForFileAccess(cancelToken))
                {
                    OnLogged($"Unable to access new file: {newFile.FileName}");
                    cancelToken.ThrowIfCancellationRequested();
                    continue;
                }
                cancelToken.ThrowIfCancellationRequested();

                if (newFile.Duration == TimeSpan.Zero)
                {
                    OnLogged($"New file has no duration: {newFile.FilePath}");
                    continue;
                }
                cancelToken.ThrowIfCancellationRequested();

                if (!videoFiles.Contains(newFile))
                {
                    OnLogged($"New file added to VideoFile-List: {newFile.FilePath}");
                    videoFiles.Add(newFile);
                    datastore.InsertVideoFile(newFile);
                }
                else
                {
                    OnLogged($"New file already in VideoFile-List: {newFile.FilePath}");
                    continue;
                }
                cancelToken.ThrowIfCancellationRequested();

                FindDuplicatesOf(videoFiles, newFile, cancelToken);
                cancelToken.ThrowIfCancellationRequested();
            }

            ProcessChangesIfAny();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CancelSource.Cancel();
                    try
                    {
                        DedupTask?.Wait();
                    }
                    catch (AggregateException exc)
                    {
                        exc.Handle(x => x is OperationCanceledException);
                    }
                    CancelSource.Dispose();
                    DedupTask?.Dispose();
                    FileWatcher.Dispose();
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
