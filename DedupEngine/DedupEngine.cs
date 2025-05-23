namespace DedupEngine
{
    using System.Collections.Concurrent;
    using EventArgs;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.IVideoFileExtensions;
    using static VideoDedupGrpc.OperationInfo.Types;

    public class DedupEngine : IDisposable
    {
        private const string EngineDatastoreFileName = "engine.db";

        private static readonly int DamagedFileRetryCount = 5;

        private int operationType = (int)OperationType.Completed;
        public OperationType OperationType
        {
            get => (OperationType)Interlocked.CompareExchange(ref operationType, 0, 0);
            set => Interlocked.Exchange(ref operationType, (int)value);
        }

        private int fileCounter;
        public int FileCounter
        {
            get => Interlocked.CompareExchange(ref fileCounter, 0, 0);
            set => Interlocked.Exchange(ref fileCounter, value);
        }

        private int maxFileCount;
        public int MaxFileCount
        {
            get => Interlocked.CompareExchange(ref maxFileCount, 0, 0);
            set => Interlocked.Exchange(ref maxFileCount, value);
        }

        private long progressStyle = (int)ProgressStyle.Marquee;
        public ProgressStyle ProgressStyle
        {
            get => (ProgressStyle)Interlocked.CompareExchange(ref progressStyle, 0, 0);
            set => Interlocked.Exchange(ref progressStyle, (int)value);
        }

        private long operationStartTimeTicks = DateTime.MinValue.Ticks;
        public DateTime OperationStartTime
        {
            get => new(Interlocked.Read(ref operationStartTimeTicks));
            set => Interlocked.Exchange(ref operationStartTimeTicks, value.Ticks);
        }

        private bool disposedValue; // For IDisposable

        private object DedupLock { get; } = new();
        private Task? DedupTask { get; set; }
        private CancellationTokenSource CancelSource { get; set; } = new();

        private FileSystemWatcher FileWatcher { get; } = new();

        // ConcurrentDictionary is used as a hash set
        private readonly ConcurrentDictionary<VideoFile, byte> newFiles = new();
        private readonly ConcurrentDictionary<VideoFile, byte> deletedFiles = new();
        private DedupSettings dedupSettings;
        private DurationComparisonSettings durationComparisonSettings;
        private VideoComparisonSettings videoComparisonSettings;
        private List<VideoFile> videoFiles = [];
        private readonly EngineDatastore datastore;

        public event EventHandler<StartedEventArgs>? Started;
        protected virtual void OnStarted() =>
            Started?.Invoke(this, new StartedEventArgs());

        public event EventHandler<StoppedEventArgs>? Stopped;
        protected virtual void OnStopped() =>
            Stopped?.Invoke(this, new StoppedEventArgs());

        public event EventHandler<DuplicateFoundEventArgs>? DuplicateFound;
        protected virtual void OnDuplicateFound(
            VideoComparer.VideoFile file1,
            VideoComparer.VideoFile file2) =>
            DuplicateFound?.Invoke(
                this,
                new DuplicateFoundEventArgs(dedupSettings.BasePath, file1, file2));

        public event EventHandler<OperationUpdateEventArgs>? OperationUpdate;
        protected virtual void OnOperationUpdate(
            OperationType type,
            int counter,
            int maxCount)
        {
            OperationType = type;
            FileCounter = counter;
            MaxFileCount = maxCount;
            ProgressStyle = ProgressStyle.Continuous;

            OperationUpdate?.Invoke(this, new OperationUpdateEventArgs
            {
                Type = type,
                Counter = counter,
                MaxCount = maxCount,
                Style = ProgressStyle.Continuous,
                StartTime = OperationStartTime,
            });
        }
        protected virtual void OnOperationUpdate(
            OperationType type,
            ProgressStyle style)
        {
            OperationType = type;
            FileCounter = 0;
            MaxFileCount = 0;
            ProgressStyle = style;

            OperationUpdate?.Invoke(this, new OperationUpdateEventArgs
            {
                Type = type,
                Counter = 0,
                MaxCount = 0,
                Style = style,
                StartTime = OperationStartTime,
            });
        }

        public event EventHandler<LoggedEventArgs>? Logged;
        protected virtual void OnLogged(string message) =>
            Logged?.Invoke(this, new LoggedEventArgs(message));

        public DedupEngine(
            string appDataFolder,
            DedupSettings dedupSettings,
            DurationComparisonSettings durationComparisonSettings,
            VideoComparisonSettings videoComparisonSettings)
        {
            this.dedupSettings = dedupSettings;
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
            DedupSettings dedupSettings,
            DurationComparisonSettings durationComparisonSettings,
            VideoComparisonSettings videoComparisonSettings)
        {
            if (this.dedupSettings.Equals(dedupSettings)
                && this.durationComparisonSettings.Equals(durationComparisonSettings)
                && this.videoComparisonSettings.Equals(videoComparisonSettings))
            {
                return false;
            }
            Stop();
            this.dedupSettings = dedupSettings;
            this.durationComparisonSettings = durationComparisonSettings;
            this.videoComparisonSettings = videoComparisonSettings;
            return true;
        }

        public void Start()
        {
            OnLogged("Starting DedupEngine");

            if (!Directory.Exists(dedupSettings.BasePath))
            {
                throw new InvalidOperationException("Unable to start. " +
                    "Base path is not valid.");
            }

            if (DedupTask is { IsCompleted: false })
            {
                return;
            }

            FileWatcher.Path = dedupSettings.BasePath;
            FileWatcher.IncludeSubdirectories = dedupSettings.Recursive;
            FileWatcher.EnableRaisingEvents = dedupSettings.MonitorChanges;

            lock (DedupLock)
            {
                DedupTask?.Dispose();
                CancelSource?.Dispose();
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
            OnLogged($"{nameof(HandleFileWatcherDeletedEvent)} - '{e.FullPath}'");
            HandleDeletedFileEvent(e.FullPath);
        }

        private void HandleFileWatcherChangedEvent(
            object sender,
            FileSystemEventArgs e)
        {
            OnLogged($"{nameof(HandleFileWatcherChangedEvent)} - '{e.FullPath}'");
            HandleNewFileEvent(e.FullPath);
        }

        private void HandleFileWatcherCreatedEvent(
            object sender,
            FileSystemEventArgs e)
        {
            OnLogged($"{nameof(HandleFileWatcherCreatedEvent)} - '{e.FullPath}'");
            HandleNewFileEvent(e.FullPath);
        }

        private void HandleFileWatcherRenamedEvent(
            object sender,
            RenamedEventArgs e)
        {
            OnLogged($"{nameof(HandleFileWatcherRenamedEvent)} - '{e.FullPath}' " +
                $"- '{e.OldFullPath}'");
            HandleDeletedFileEvent(e.OldFullPath);
            HandleNewFileEvent(e.FullPath);
        }

        private void HandleFileWatcherErrorEvent(
            object sender,
            ErrorEventArgs e) =>
            OnLogged("FileWatcher crashed! Unable to continue monitoring the" +
                     $" source folder '{dedupSettings.BasePath}'.");

        private bool IsFilePathRelevant(
            string filePath,
            DedupSettings dedupSettings)
        {
            if (!filePath.StartsWith(
                dedupSettings.BasePath,
                StringComparison.OrdinalIgnoreCase))
            {
                OnLogged($"File not in source folder: '{filePath}'");
                return false;
            }

            if (dedupSettings.ExcludedDirectories?.Any(p => filePath.StartsWith(
                p,
                StringComparison.OrdinalIgnoreCase)) ?? false)
            {
                OnLogged($"File is in excluded directory: '{filePath}'");
                return false;
            }

            var extension = Path.GetExtension(filePath);
            if (!dedupSettings.FileExtensions?.Contains(extension) ?? false)
            {
                OnLogged($"File doesn't have proper file extension: '{filePath}'");
                return false;
            }

            return true;
        }

        private void HandleDeletedFileEvent(string filePath)
        {
            if (!IsFilePathRelevant(filePath, dedupSettings))
            {
                return;
            }

            _ = deletedFiles.TryAdd(new VideoFile(filePath), 0);
            OnLogged($"File deleted: '{filePath}'");
            StartProcessingChanges();
        }

        private void HandleNewFileEvent(string filePath)
        {
            if (!IsFilePathRelevant(filePath, dedupSettings))
            {
                return;
            }

            _ = newFiles.TryAdd(new VideoFile(filePath), 0);
            OnLogged($"File created: '{filePath}'");
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
                return [];
            }

            IEnumerable<string> files = [];
            excludedDirectories ??= [];

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

        private void PreloadFiles(
            IEnumerable<VideoFile> videoFiles,
            CancellationToken cancelToken)
        {
            var counter = 0;
            var fileCount = videoFiles.Count();

            OperationStartTime = DateTime.Now;
            OnOperationUpdate(
                OperationType.LoadingMedia,
                counter,
                fileCount);

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = dedupSettings.ConcurrencyLevel,
                CancellationToken = cancelToken,
            };
            _ = Parallel.ForEach(videoFiles, parallelOptions, file =>
            {
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

                OnOperationUpdate(
                    OperationType.LoadingMedia,
                    ++counter,
                    fileCount);

                cancelToken.ThrowIfCancellationRequested();
            });
        }

        private List<Candidate> PrepareCandidates(
            List<VideoFile> targetVideos,
            List<VideoFile> allVideos,
            DurationComparisonSettings durationComparisonSettings,
            CancellationToken cancelToken)
        {
            OnOperationUpdate(
                OperationType.Preparing,
                0,
                targetVideos.Count);

            var preparedCount = 0;
            ThrottledOperationUpdate throttledOperationUpdate =
                new(OnOperationUpdate)
                {
                    Time = TimeSpan.FromMilliseconds(50),
                };

            var pairs = new HashSet<Candidate>();
            foreach (var targetVideo in targetVideos)
            {
                cancelToken.ThrowIfCancellationRequested();

                foreach (var otherVideo in allVideos.Where(f => f != targetVideo))
                {
                    cancelToken.ThrowIfCancellationRequested();

                    if (!targetVideo.IsDurationEqual(
                        otherVideo,
                        durationComparisonSettings))
                    {
                        continue;
                    }
                    _ = pairs.Add(new Candidate(targetVideo, otherVideo));
                }


                throttledOperationUpdate.Raise(
                    OperationType.Preparing,
                    preparedCount++,
                    targetVideos.Count);
            }

            OnOperationUpdate(
                OperationType.Preparing,
                targetVideos.Count,
                targetVideos.Count);

            return [.. pairs];
        }

        private void FindDuplicates(
            List<Candidate> candidates,
            int individualFileCount,
            CancellationToken cancelToken)
        {
            OperationStartTime = DateTime.Now;
            OnOperationUpdate(
                OperationType.Comparing,
                0,
                individualFileCount);

            List<Candidate> blockedCandidates;
            var candidateCount = candidates.Count;

            var processedFiles = new ConcurrentDictionary<VideoFile, byte>(
                dedupSettings.ConcurrencyLevel,
                individualFileCount);
            object processingLock = new();

            ThrottledOperationUpdate throttledOperationUpdate =
                new(OnOperationUpdate)
                {
                    Time = TimeSpan.FromMilliseconds(100),
                };

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = dedupSettings.ConcurrencyLevel,
                CancellationToken = cancelToken,
            };

            void ProcessCandidate(Candidate candidate)
            {
                if (!candidate.TryStartProcessing(processingLock))
                {
                    blockedCandidates.Add(candidate);
                    return;
                }

                cancelToken.ThrowIfCancellationRequested();

                if (candidate.IsErrorCountExceeded(DamagedFileRetryCount))
                {
                    return;
                }

                CompareVideoFiles(candidate.File1, candidate.File2, cancelToken);
                _ = processedFiles.GetOrAdd(candidate.File1, 0);
                _ = processedFiles.GetOrAdd(candidate.File2, 0);

                throttledOperationUpdate.Raise(
                    OperationType.Comparing,
                    processedFiles.Count,
                    individualFileCount);

                candidate.StopProcessing();
            }

            // Process candidates in parallel
            // until all candidates are processed
            while (candidates.Count > 0)
            {
                blockedCandidates = new List<Candidate>(candidates.Count);

                _ = Parallel.ForEach(
                    candidates,
                    parallelOptions,
                    ProcessCandidate);

                candidates = blockedCandidates;
            }

            OnOperationUpdate(
                OperationType.Comparing,
                individualFileCount,
                individualFileCount);
        }

        private void CompareVideoFiles(
            VideoFile left,
            VideoFile right,
            CancellationToken cancelToken)
        {
            try
            {
                var comparer = new VideoComparer.VideoComparer(
                        videoComparisonSettings,
                        datastore.DatastoreFilePath,
                        left,
                        right);
                if (comparer.Compare(cancelToken)
                    != ComparisonResult.Duplicate)
                {
                    return;
                }
                OnLogged($"Found duplicate of '{left.FilePath}' and" +
                    $" '{right.FilePath}'");
                OnDuplicateFound(left, right);
            }
            catch (VideoComparer.ComparisonException exc)
            {
                exc.VideoFile.IncrementErrorCount();
                if (exc.VideoFile.ErrorCount > DamagedFileRetryCount)
                {
                    _ = deletedFiles.TryAdd(
                        new VideoFile(exc.VideoFile),
                        0);
                    OnLogged($"File corrupted: '{exc.VideoFile.FilePath}'");
                }
                else
                {
                    OnLogged($"Comparison failed. Couldn't access: " +
                        $"'{exc.VideoFile.FilePath}'");
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
                if (!newFiles.IsEmpty || !deletedFiles.IsEmpty)
                {
                    DedupTask = Task.Run(ProcessChanges, CancelSource.Token);
                    return;
                }

                if (!dedupSettings.MonitorChanges)
                {
                    OperationStartTime = DateTime.MinValue;
                    OnOperationUpdate(
                        OperationType.Completed,
                        ProgressStyle.NoProgress);
                    OnLogged("Finished comparison.");
                    return;
                }

                OperationStartTime = DateTime.Now;
                OnOperationUpdate(
                    OperationType.Monitoring,
                    ProgressStyle.Marquee);
                OnLogged("Monitoring for file changes...");
            }
        }

        private void ProcessFolder()
        {
            var cancelToken = CancelSource.Token;

            OperationStartTime = DateTime.Now;
            OnOperationUpdate(OperationType.Searching, ProgressStyle.Marquee);

            videoFiles = [.. GetAllAccessibleFilesIn(
                dedupSettings.BasePath,
                dedupSettings.ExcludedDirectories,
                dedupSettings.Recursive)
                .Where(f => dedupSettings.FileExtensions?.Contains(
                    Path.GetExtension(f),
                    StringComparer.InvariantCultureIgnoreCase) ?? true)
                .Select(f => new VideoFile(f))];

            cancelToken.ThrowIfCancellationRequested();

            // Cancellable preload of files
            OnLogged("Starting preloading media info of " +
                $"{videoFiles.Count} Files.");
            PreloadFiles(videoFiles, cancelToken);
            cancelToken.ThrowIfCancellationRequested();

            OnLogged("Finished preloading media info of " +
                $"{videoFiles.Count} Files.");

            // Remove invalid files
            foreach (var file in
                videoFiles.Where(f => f.Duration == TimeSpan.Zero))
            {
                OnLogged($"Discarding file '{file.FilePath}' " +
                    $"Unable to determine duration for file.");
            }
            videoFiles = [.. videoFiles.Where(f => f.Duration != TimeSpan.Zero)];
            cancelToken.ThrowIfCancellationRequested();

            OnLogged("Starting preparing candidates for " +
                $"{videoFiles.Count} Files.");
            var candidates = PrepareCandidates(
                videoFiles,
                videoFiles,
                durationComparisonSettings,
                cancelToken);
            OnLogged("Finished preparing candidates for " +
                $"{videoFiles.Count} Files.");

            OnLogged("Starting searching for duplicates of " +
                $"{videoFiles.Count} Files.");
            FindDuplicates(candidates, videoFiles.Count, cancelToken);
            cancelToken.ThrowIfCancellationRequested();
            OnLogged("Finished searching for duplicates of " +
                $"{videoFiles.Count} Files.");

            ProcessChangesIfAny();
        }

        private void ProcessChanges()
        {
            var cancelToken = CancelSource.Token;

            while (!deletedFiles.IsEmpty)
            {
                var deletedFile = deletedFiles.First().Key;
                _ = deletedFiles.TryRemove(deletedFile, out var _);

                if (videoFiles.Remove(deletedFile))
                {
                    OnLogged($"Removed file: '{deletedFile.FilePath}'");
                }
                else
                {
                    OnLogged("Deleted file not in VideoFile-List: " +
                        $"'{deletedFile.FilePath}'");
                }
                cancelToken.ThrowIfCancellationRequested();
            }

            OperationStartTime = DateTime.Now;
            OnOperationUpdate(OperationType.Comparing, 0, this.newFiles.Count);

            var newFiles = new List<VideoFile>();
            foreach (var newFile in this.newFiles.Keys)
            {
                if (!this.newFiles.TryRemove(newFile, out _))
                {
                    continue;
                }
                cancelToken.ThrowIfCancellationRequested();

                if (!newFile.WaitForFileAccess(cancelToken))
                {
                    OnLogged($"Unable to access new file: '{newFile.FileName}'");
                    continue;
                }

                cancelToken.ThrowIfCancellationRequested();

                if (newFile.Duration == TimeSpan.Zero)
                {
                    OnLogged($"New file has no duration: '{newFile.FilePath}'");
                    continue;
                }

                cancelToken.ThrowIfCancellationRequested();

                if (videoFiles.Contains(newFile))
                {
                    OnLogged($"New file already in VideoFile-List: " +
                        $"'{newFile.FilePath}'");
                    continue;
                }

                cancelToken.ThrowIfCancellationRequested();

                videoFiles.Add(newFile);
                datastore.InsertVideoFile(newFile);
                newFiles.Add(newFile);
            }

            var candidates = PrepareCandidates(
                videoFiles,
                videoFiles,
                durationComparisonSettings,
                cancelToken);

            FindDuplicates(candidates, newFiles.Count, cancelToken);

            OnOperationUpdate(
                    OperationType.Comparing,
                    newFiles.Count,
                    newFiles.Count);

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
