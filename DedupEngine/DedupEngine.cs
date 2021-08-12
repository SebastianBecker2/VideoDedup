namespace DedupEngine
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using VideoDedupShared.IVideoFileExtension;
    using VideoDedupShared.TimeSpanExtension;
    using IDedupEngineSettings = VideoDedupShared.IDedupEngineSettings;
    using IFolderSettings = VideoDedupShared.IFolderSettings;
    using OperationType = VideoDedupShared.OperationType;
    using ProgressStyle = VideoDedupShared.ProgressStyle;
    using ComparisonResult = VideoDedupShared.ComparisonResult;

    public class DedupEngine : IDisposable
    {
        private static readonly string LogCheckingFile = "Checking: {0} - Duration: {1}";
        private static readonly string LogDeletedFile = "File deleted: {0}";
        private static readonly string LogNewFile = "File created: {0}";

        private static readonly string EngineDatastoreFileName = "engine.db";
        private string AppDataFolder { get; }

        private bool disposedValue; // For IDisposable

        private object DedupLock { get; } = new object { };
        private Task DedupTask { get; set; } = null;
        private CancellationTokenSource CancelSource { get; set; }
            = new CancellationTokenSource { };

        private FileSystemWatcher FileWatcher { get; }
            = new FileSystemWatcher { };

        // ConcurrentDictionary is used as a hash set
        private ConcurrentDictionary<VideoFile, byte> NewFiles { get; set; }
        private ConcurrentDictionary<VideoFile, byte> DeletedFiles { get; set; }
        private EngineSettings Settings { get; set; }
        private IList<VideoFile> VideoFiles { get; set; }
        private EngineDatastore Datastore { get; set; }

        public event EventHandler<StoppedEventArgs> Stopped;
        protected virtual void OnStopped() =>
            Stopped?.Invoke(this, new StoppedEventArgs { });

        public event EventHandler<DuplicateFoundEventArgs> DuplicateFound;
        protected virtual void OnDuplicateFound(
            VideoFile file1,
            VideoFile file2) =>
            DuplicateFound?.Invoke(this, new DuplicateFoundEventArgs
            {
                File1 = file1,
                File2 = file2,
                BasePath = Settings.BasePath,
            });

        public event EventHandler<OperationUpdateEventArgs> OperationUpdate;
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

        public event EventHandler<LoggedEventArgs> Logged;
        protected virtual void OnLogged(string message) =>
            Logged?.Invoke(this, new LoggedEventArgs
            {
                Message = message,
            });

        public DedupEngine(string appDataFolder)
        {
            if (string.IsNullOrWhiteSpace(appDataFolder))
            {
                throw new ArgumentException($"'{nameof(appDataFolder)}' cannot" +
                    $"be null or whitespace", nameof(appDataFolder));
            }

            AppDataFolder = appDataFolder;

            Datastore = new EngineDatastore(
                Path.Combine(appDataFolder, EngineDatastoreFileName));

            FileWatcher.Changed += HandleFileWatcherChangedEvent;
            FileWatcher.Deleted += HandleFileWatcherDeletedEvent;
            FileWatcher.Error += HandleFileWatcherErrorEvent;
            FileWatcher.Renamed += HandleFileWatcherRenamedEvent;
            FileWatcher.Created += HandleFileWatcherCreatedEvent;
        }

        public DedupEngine(string appDataFolder, IDedupEngineSettings settings)
            : this(appDataFolder) =>
            UpdateConfiguration(settings);

        public void UpdateConfiguration(IDedupEngineSettings settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            Stop();
            Settings = new EngineSettings(settings);
        }

        public void Start()
        {
            if (Settings == null)
            {
                throw new InvalidOperationException("Unable to start. " +
                    "No configuration set.");
            }

            if (!Directory.Exists(Settings.BasePath))
            {
                throw new InvalidOperationException("Unable to start. " +
                    "Base path is not valid.");
            }

            if (DedupTask != null && !DedupTask.IsCompleted)
            {
                return;
            }

            FileWatcher.Path = Settings.BasePath;
            FileWatcher.IncludeSubdirectories = Settings.Recursive;
            FileWatcher.EnableRaisingEvents = Settings.MonitorChanges;

            NewFiles = new ConcurrentDictionary<VideoFile, byte> { };
            DeletedFiles = new ConcurrentDictionary<VideoFile, byte> { };

            lock (DedupLock)
            {
                DedupTask?.Dispose();
                CancelSource = new CancellationTokenSource();
                DedupTask = Task.Factory.StartNew(
                    ProcessFolder,
                    CancelSource.Token);
            }
        }

        public void Stop()
        {
            FileWatcher.EnableRaisingEvents = false;

            if (DedupTask == null || DedupTask.IsCompleted)
            {
                return;
            }

            CancelSource.Cancel();
            try
            {
                DedupTask?.Wait();
            }
            catch (AggregateException exc)
            {
                exc.Handle(x => x is OperationCanceledException);
            }
            OnStopped();
        }

        private void StartProcessingChanges()
        {
            lock (DedupLock)
            {
                // If the task is still running,
                // it will check for new files on it's own.
                if (!DedupTask.IsCompleted)
                {
                    return;
                }

                DedupTask = Task.Factory.StartNew(
                    ProcessChanges,
                    CancelSource.Token);
            }
        }

        private void HandleFileWatcherDeletedEvent(object sender, FileSystemEventArgs e)
        {
            OnLogged($"{nameof(HandleFileWatcherDeletedEvent)} - {e.FullPath}");
            HandleDeletedFileEvent(e.FullPath);
        }

        private void HandleFileWatcherChangedEvent(object sender, FileSystemEventArgs e)
        {
            OnLogged($"{nameof(HandleFileWatcherChangedEvent)} - {e.FullPath}");
            HandleNewFileEvent(e.FullPath);
        }

        private void HandleFileWatcherCreatedEvent(object sender, FileSystemEventArgs e)
        {
            OnLogged($"{nameof(HandleFileWatcherCreatedEvent)} - {e.FullPath}");
            HandleNewFileEvent(e.FullPath);
        }

        private void HandleFileWatcherRenamedEvent(object sender, RenamedEventArgs e)
        {
            OnLogged($"{nameof(HandleFileWatcherRenamedEvent)} - {e.FullPath} - {e.OldFullPath}");
            HandleDeletedFileEvent(e.OldFullPath);
            HandleNewFileEvent(e.FullPath);
        }

        private void HandleFileWatcherErrorEvent(object sender, ErrorEventArgs e) =>
            OnLogged("FileWatcher crashed! Unable to continue monitoring the source folder.");

        private bool IsFilePathRelevant(string filePath, IFolderSettings settings)
        {
            if (!filePath.StartsWith(settings.BasePath))
            {
                OnLogged($"File not in source folder: {filePath}");
                return false;
            }

            foreach (var excludedPath in settings.ExcludedDirectories)
            {
                if (filePath.StartsWith(excludedPath))
                {
                    OnLogged($"File is in excluded directory: {filePath}");
                    return false;
                }
            }

            var extension = Path.GetExtension(filePath);
            if (!settings.FileExtensions.Contains(extension))
            {
                OnLogged($"File doesn't have proper file extension: {filePath}");
                return false;
            }

            return true;
        }

        private void HandleDeletedFileEvent(string filePath)
        {
            if (!IsFilePathRelevant(filePath, Settings))
            {
                return;
            }

            _ = DeletedFiles.TryAdd(new VideoFile(filePath), 0);
            OnLogged(string.Format(LogDeletedFile, filePath));
            StartProcessingChanges();
        }

        private void HandleNewFileEvent(string filePath)
        {
            if (!IsFilePathRelevant(filePath, Settings))
            {
                return;
            }

            _ = NewFiles.TryAdd(new VideoFile(filePath), 0);
            OnLogged(string.Format(LogNewFile, filePath));
            StartProcessingChanges();
        }

        private static IEnumerable<string> GetAllAccessibleFilesIn(
            string rootDirectory,
            IEnumerable<string> excludedDirectories = null,
            bool recursive = true,
            string searchPattern = "*.*")
        {
            if (Path.GetFileName(rootDirectory) == "$RECYCLE.BIN")
            {
                return new List<string> { };
            }

            IEnumerable<string> files = new List<string> { };
            if (excludedDirectories == null)
            {
                excludedDirectories = new List<string> { };
            }

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
            IFolderSettings folderSettings)
        {
            operationStartTime = DateTime.Now;
            OnOperationUpdate(OperationType.Searching, ProgressStyle.Marquee);

            return GetAllAccessibleFilesIn(
                folderSettings.BasePath,
                folderSettings.ExcludedDirectories,
                folderSettings.Recursive)
                .Where(f => folderSettings.FileExtensions.Contains(
                    Path.GetExtension(f),
                    StringComparer.InvariantCultureIgnoreCase))
                .Select(f => new VideoFile(f));
        }

        private void PreloadFiles(
            IEnumerable<VideoFile> videoFiles,
            CancellationToken cancelToken)
        {
            var counter = 0;

            operationStartTime = DateTime.Now;
            OnOperationUpdate(
                OperationType.LoadingMedia,
                counter,
                videoFiles.Count());

            foreach (var file in videoFiles)
            {
                OnOperationUpdate(OperationType.LoadingMedia,
                    ++counter,
                    videoFiles.Count());

                var duration = Datastore.GetVideoFileDuration(file);
                if (duration.HasValue)
                {
                    file.Duration = duration.Value;
                }
                else
                {
                    _ = file.Duration;
                    Datastore.InsertVideoFile(file);
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
                VideoFiles.Count());

            var timer = Stopwatch.StartNew();

            VideoFiles = VideoFiles
                .OrderBy(f => f.Duration)
                .ToList();

            foreach (var index in Enumerable.Range(0, VideoFiles.Count - 1))
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                var file = VideoFiles[index];

                OnLogged(string.Format(LogCheckingFile,
                    file.FilePath,
                    file.Duration.ToPrettyString()));
                OnOperationUpdate(OperationType.Comparing,
                    index + 1,
                    VideoFiles.Count());

                foreach (var other in VideoFiles
                    .Skip(index + 1)
                    .TakeWhile(other =>
                        file.IsDurationEqual(other, Settings)))
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }

                    try
                    {
                        var comparer = new VideoComparer(Datastore)
                        {
                            LeftVideoFile = file,
                            RightVideoFile = other,
                            Settings = Settings,
                        };
                        if (comparer.Compare(cancelToken)
                            == ComparisonResult.Duplicate)
                        {
                            OnLogged($"Found duplicate of {file.FilePath} and " +
                                $"{other.FilePath}");
                            OnDuplicateFound(file, other);
                        }
                    }
                    catch (AggregateException exc)
                    when (exc.InnerException is MpvLib.MpvException)
                    {
                        OnLogged(exc.InnerException.Message);
                    }
                }
            }

            timer.Stop();
            Debug.Print($"Dedup took {timer.ElapsedMilliseconds} ms");

            OnOperationUpdate(OperationType.Comparing,
                VideoFiles.Count(),
                VideoFiles.Count());
        }

        private void FindDuplicatesOf(
            IEnumerable<VideoFile> videoFiles,
            VideoFile refFile,
            CancellationToken cancelToken)
        {
            OnLogged(string.Format(LogCheckingFile,
                    refFile.FilePath,
                    refFile.Duration.ToPrettyString()));

            foreach (var file in videoFiles
                .Where(f => f != refFile)
                .Where(f => f.IsDurationEqual(refFile, Settings)))
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                try
                {
                    var comparer = new VideoComparer(Datastore)
                    {
                        LeftVideoFile = file,
                        RightVideoFile = refFile,
                        Settings = Settings,
                    };
                    if (comparer.Compare(cancelToken)
                        == ComparisonResult.Duplicate)
                    {
                        OnLogged($"Found duplicate of {refFile.FilePath} and" +
                            $" {file.FilePath}");
                        OnDuplicateFound(refFile, file);
                    }
                }
                catch (AggregateException exc)
                when (exc.InnerException is MpvLib.MpvException)
                {
                    OnLogged(exc.InnerException.Message);
                }
            }
        }

        private void ProcessChangesIfAny()
        {
            lock (DedupLock)
            {
                if (NewFiles.Any() || DeletedFiles.Any())
                {
                    DedupTask = Task.Factory.StartNew(
                        ProcessChanges,
                        CancelSource.Token);
                    return;
                }

                if (!Settings.MonitorChanges)
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
                return;
            }
        }

        private void ProcessFolder()
        {
            var cancelToken = CancelSource.Token;

            VideoFiles = GetVideoFileList(Settings).ToList();
            cancelToken.ThrowIfCancellationRequested();

            // Cancellable preload of files
            OnLogged($"Starting preloading media info of " +
                $"{VideoFiles.Count()} Files.");
            PreloadFiles(VideoFiles, cancelToken);
            if (cancelToken.IsCancellationRequested)
            {
                cancelToken.ThrowIfCancellationRequested();
            }
            OnLogged($"Finished preloading media info of " +
                $"{VideoFiles.Count()} Files.");

            // Remove invalid files
            VideoFiles = VideoFiles
                .Where(f => f.Duration != TimeSpan.Zero)
                .ToList();
            cancelToken.ThrowIfCancellationRequested();

            FindDuplicates(cancelToken);
            if (cancelToken.IsCancellationRequested)
            {
                cancelToken.ThrowIfCancellationRequested();
            }
            OnLogged($"Finished searching for duplicates of " +
                $"{VideoFiles.Count()} Files.");

            ProcessChangesIfAny();
        }

        private void ProcessChanges()
        {
            var cancelToken = CancelSource.Token;

            while (DeletedFiles.Any())
            {
                var deletedFile = DeletedFiles.First().Key;
                _ = DeletedFiles.TryRemove(deletedFile, out var _);

                if (VideoFiles.Remove(deletedFile))
                {
                    OnLogged($"Removed file: {deletedFile.FilePath}");
                }
                else
                {
                    OnLogged($"Deleted file not in VideoFile-List: " +
                        $"{deletedFile.FilePath}");
                }
                cancelToken.ThrowIfCancellationRequested();
            }

            operationStartTime = DateTime.Now;
            OnOperationUpdate(OperationType.Comparing, 0, NewFiles.Count());
            // We need to count for the ProgressUpdate since we shrink
            // the Queue on every iteration.
            var filesProcessed = 1;
            while (NewFiles.Any())
            {
                var newFile = NewFiles.First().Key;
                _ = NewFiles.TryRemove(newFile, out var _);
                OnOperationUpdate(OperationType.Comparing, filesProcessed,
                    filesProcessed + NewFiles.Count());
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

                if (!VideoFiles.Contains(newFile))
                {
                    OnLogged($"New file added to VideoFile-List: {newFile.FilePath}");
                    VideoFiles.Add(newFile);
                }
                else
                {
                    OnLogged($"New file already in VideoFile-List: {newFile.FilePath}");
                    continue;
                }
                cancelToken.ThrowIfCancellationRequested();

                FindDuplicatesOf(VideoFiles, newFile, cancelToken);
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
                    CancelSource?.Dispose();
                    DedupTask?.Dispose();
                    FileWatcher?.Dispose();
                    // TODO: dispose managed state (managed objects)
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
