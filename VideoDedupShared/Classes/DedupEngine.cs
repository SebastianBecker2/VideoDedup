namespace VideoDedupShared
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TimeSpanExtension;
    using Newtonsoft.Json;

    public class StoppedEventArgs : EventArgs { }

    public class DuplicateFoundEventArgs : EventArgs
    {
        public VideoFile File1 { get; set; }
        public VideoFile File2 { get; set; }
    }

    public class StatusChangedEventArgs : EventArgs { }

    public class ProgressUpdateEventArgs : EventArgs
    {
        public string StatusInfo { get; set; }
        public int Counter { get; set; }
        public int MaxCount { get; set; }
    }

    public class LoggedEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public class DedupEngine : IDisposable
    {
        private static readonly string StatusInfoComparing = "Comparing: {0}/{1}";
        private static readonly string StatusInfoLoading = "Loading media info: {0}/{1}";
        private static readonly string StatusInfoSearching = "Searching for files...";

        private static readonly string LogCheckingFile = "Checking: {0} - Duration: {1}";
        private static readonly string LogDeletedFile = "File deleted: {0}";
        private static readonly string LogNewFile = "File created: {0}";

        private bool disposedValue; // For IDisposable

        private IDedupperSettings Configuration { get; set; } = null;
        private Task DedupTask { get; set; } = null;
        private object DedupLock { get; set; } = new object { };
        private CancellationTokenSource CancelSource { get; set; }
            = new CancellationTokenSource { };
        private FileSystemWatcher FileWatcher { get; set; }
            = new FileSystemWatcher { };
        private IProducerConsumerCollection<VideoFile> NewFiles { get; set; }
            = new ConcurrentQueue<VideoFile> { };
        private IProducerConsumerCollection<VideoFile> DeletedFiles { get; set; }
            = new ConcurrentQueue<VideoFile> { };
        private IList<VideoFile> VideoFiles { get; set; }

        public event EventHandler<StoppedEventArgs> Stopped;
        protected virtual void OnStopped() =>
            Stopped?.Invoke(this, new StoppedEventArgs { });

        public event EventHandler<DuplicateFoundEventArgs> DuplicateFound;
        protected virtual void OnDuplicateFound(VideoFile file1,
            VideoFile file2) =>
            DuplicateFound?.Invoke(this, new DuplicateFoundEventArgs
            {
                File1 = file1,
                File2 = file2,
            });

        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        protected virtual void OnStatusChanged() =>
            StatusChanged?.Invoke(this, new StatusChangedEventArgs { });

        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;
        protected virtual void OnProgressUpdate(string statusInfo,
            int counter,
            int maxCount) =>
            ProgressUpdate?.Invoke(this, new ProgressUpdateEventArgs
            {
                StatusInfo = statusInfo,
                Counter = counter,
                MaxCount = maxCount
            });

        public event EventHandler<LoggedEventArgs> Logged;
        protected virtual void OnLogged(string message) =>
            Logged?.Invoke(this, new LoggedEventArgs
            {
                Message = DateTime.Now.ToString("s") + " " + message,
            });

        public DedupEngine()
        {
            FileWatcher.Changed += HandleFileWatcherChangedEvent;
            FileWatcher.Deleted += HandleFileWatcherDeletedEvent;
            FileWatcher.Error += HandleFileWatcherErrorEvent;
            FileWatcher.Renamed += HandleFileWatcherRenamedEvent;
            FileWatcher.Created += HandleFileWatcherCreatedEvent;
            FileWatcher.IncludeSubdirectories = true;
        }

        public DedupEngine(IDedupperSettings config) : this() =>
            UpdateConfiguration(config);

        public void UpdateConfiguration(IDedupperSettings config)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            Configuration = config.Copy();

            FileWatcher.Path = Configuration.BasePath;
            FileWatcher.EnableRaisingEvents = true;
        }

        public void Start()
        {
            if (Configuration == null)
            {
                throw new InvalidOperationException("Unable to start. No configuration set");
            }

            if (DedupTask != null && !DedupTask.IsCompleted)
            {
                return;
            }

            NewFiles = new ConcurrentQueue<VideoFile> { };
            DeletedFiles = new ConcurrentQueue<VideoFile> { };

            lock (DedupLock)
            {
                DedupTask?.Dispose();
                CancelSource = new CancellationTokenSource();
                DedupTask = Task.Factory.StartNew(ProcessFolder);
            }
        }

        public void Stop()
        {
            if (DedupTask.IsCompleted)
            {
                return;
            }

            CancelSource.Cancel();
            DedupTask?.Wait();
            CancelSource?.Dispose();
        }

        private void StartProcessingChanges()
        {
            lock (DedupLock)
            {
                // If the task is still running,
                // it will check for new files on it's own.
                if (DedupTask.IsCompleted)
                {
                    return;
                }

                DedupTask = Task.Factory.StartNew(ProcessChanges);
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
            if (!IsFilePathRelevant(filePath, Configuration))
            {
                return;
            }

            _ = DeletedFiles.TryAdd(new VideoFile(filePath));
            OnLogged(string.Format(LogDeletedFile, filePath));
            StartProcessingChanges();
        }

        private void HandleNewFileEvent(string filePath)
        {
            if (!IsFilePathRelevant(filePath, Configuration))
            {
                return;
            }

            _ = NewFiles.TryAdd(new VideoFile(filePath));
            OnLogged(string.Format(LogNewFile, filePath));
            StartProcessingChanges();
        }

        private static IEnumerable<string> GetAllAccessibleFilesIn(
            string rootDirectory,
            IEnumerable<string> excludedDirectories = null,
            string searchPattern = "*.*")
        {
            IEnumerable<string> files = new List<string> { };
            if (excludedDirectories == null)
            {
                excludedDirectories = new List<string> { };
            }

            try
            {
                files = files.Concat(Directory.EnumerateFiles(rootDirectory, searchPattern, SearchOption.TopDirectoryOnly));

                foreach (var directory in Directory
                    .GetDirectories(rootDirectory)
                    .Where(d => !excludedDirectories.Contains(d)))
                {
                    files = files.Concat(GetAllAccessibleFilesIn(directory, excludedDirectories, searchPattern));
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Don't do anything if we cannot access a file.
            }

            return files;
        }

        private static HashSet<VideoFile> LoadVideoFilesCache(string cachePath)
        {
            try
            {
                return JsonConvert.DeserializeObject<HashSet<VideoFile>>(File.ReadAllText(cachePath));
            }
            catch
            {
                return null;
            }
        }

        private IEnumerable<VideoFile> GetVideoFileList(
            IFolderSettings settings)
        {
            var timer = Stopwatch.StartNew();

            OnProgressUpdate(StatusInfoSearching, 0, 0);

            // Get all video files in source path.
            var fileExtensions = settings.FileExtensions.ToList();
            var found_files = GetAllAccessibleFilesIn(
                settings.BasePath,
                settings.ExcludedDirectories)
                .Where(f => fileExtensions.Contains(
                    Path.GetExtension(f),
                    StringComparer.CurrentCultureIgnoreCase))
                .Select(f => new VideoFile(f));

            var cached_files = LoadVideoFilesCache(settings.CachePath);
            if (cached_files == null || !cached_files.Any())
            {
                cached_files = new HashSet<VideoFile>(found_files);
            }
            else
            {
                // Basically overwrite the found files with cached files
                // and make sure we don't take cached files that don't exist
                // anymore.
                _ = cached_files.RemoveWhere(f => !File.Exists(f.FilePath));
                cached_files.UnionWith(found_files);
            }
            timer.Stop();

            OnProgressUpdate(StatusInfoLoading, 0, cached_files.Count());
            OnLogged($"Found {cached_files.Count()} video files in {timer.ElapsedMilliseconds} ms");
            return cached_files;
        }

        private void PreloadFiles(
            IEnumerable<VideoFile> videoFiles,
            CancellationToken cancelToken)
        {
            var counter = 0;
            foreach (var f in videoFiles)
            {
                OnProgressUpdate(StatusInfoLoading,
                    ++counter,
                    videoFiles.Count());

                // For now we only preload the duration
                // since the size is only rarely used
                // in the comparison dialog. No need
                // to preload it.
                var duration = f.Duration;
                //var size = f.FileSize;
                if (cancelToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        private static void SaveVideoFilesCache(
            IEnumerable<VideoFile> videoFiles,
            string cache_path)
        {
            var timer = Stopwatch.StartNew();
            File.WriteAllText(cache_path, JsonConvert.SerializeObject(videoFiles, Formatting.Indented));
            timer.Stop();
            Debug.Print($"Writing cache file took {timer.ElapsedMilliseconds} ms");
        }

        private void FindDuplicates(
            IEnumerable<VideoFile> videoFiles,
            CancellationToken cancelToken)
        {
            OnProgressUpdate(StatusInfoComparing, 0, videoFiles.Count());

            var videoFileList = videoFiles.OrderBy(f => f.Duration).ToList();

            for (var index = 0; index < videoFileList.Count() - 1; index++)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                var file = videoFileList[index];

                OnLogged(string.Format(LogCheckingFile,
                    file.FilePath,
                    file.Duration.ToPrettyString()));
                OnProgressUpdate(StatusInfoComparing,
                    index + 1,
                    videoFileList.Count());

                for (var nextIndex = index + 1; nextIndex < videoFileList.Count; nextIndex++)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }

                    var nextFile = videoFileList[nextIndex];

                    if (!file.IsDurationEqual(nextFile, Configuration))
                    {
                        break;
                    }

                    if (file.AreThumbnailsEqual(nextFile, Configuration, cancelToken))
                    {
                        OnLogged($"Found duplicate of {file.FilePath} and {nextFile.FilePath}");
                        OnDuplicateFound(file, nextFile);
                    }
                }

                file.DisposeThumbnails();
            }

            OnProgressUpdate(StatusInfoComparing,
                videoFileList.Count(),
                videoFileList.Count());
        }

        private void FindDuplicatesOf(
            IEnumerable<VideoFile> videoFiles,
            VideoFile refFile,
            CancellationToken cancelToken)
        {
            OnLogged($"Searching duplicates of {refFile.FileName}");

            foreach (var file in videoFiles)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                if (file == refFile)
                {
                    continue;
                }

                if (!file.IsDurationEqual(refFile, Configuration))
                {
                    continue;
                }

                if (file.AreThumbnailsEqual(refFile, Configuration, cancelToken))
                {
                    OnLogged($"Found duplicate of {refFile.FilePath} and {file.FilePath}");
                    OnDuplicateFound(refFile, file);
                }
            }
        }

        private void ProcessChangesIfAny()
        {
            lock (DedupLock)
            {
                if (!NewFiles.Any() && !DeletedFiles.Any())
                {
                    OnLogged("Going to sleep");
                    return;
                }
                DedupTask = Task.Factory.StartNew(ProcessChanges);
            }
        }

        private void ProcessFolder()
        {
            var cancelToken = CancelSource.Token;

            var videoFiles = GetVideoFileList(Configuration);
            if (cancelToken.IsCancellationRequested)
            {
                return;
            }

            // Cancellable preload of files
            PreloadFiles(videoFiles, cancelToken);
            if (cancelToken.IsCancellationRequested)
            {
                return;
            }

            // Remove invalid files
            VideoFiles = videoFiles
                .Where(f => f.Duration != TimeSpan.Zero)
                .ToList();
            SaveVideoFilesCache(VideoFiles, Configuration.CachePath);
            if (cancelToken.IsCancellationRequested)
            {
                return;
            }

            FindDuplicates(VideoFiles, cancelToken);
            // Cleanup in case of cancel
            foreach (var file in VideoFiles)
            {
                file.DisposeThumbnails();
            }
            if (cancelToken.IsCancellationRequested)
            {
                return;
            }

            ProcessChangesIfAny();
        }

        private void ProcessChanges()
        {
            var cancelToken = CancelSource.Token;

            while (DeletedFiles.TryTake(out var deletedFile))
            {
                if (VideoFiles.Remove(deletedFile))
                {
                    OnLogged($"Removed file: {deletedFile.FilePath}");
                }
                else
                {
                    OnLogged($"Deleted file not in VideoFile-List: {deletedFile.FilePath}");
                }
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }
            }

            while (NewFiles.TryTake(out var newFile))
            {
                if (!newFile.WaitForFileAccess(cancelToken))
                {
                    OnLogged($"Unable to access new file: {newFile.FileName}");
                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }
                    continue;
                }
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                if (newFile.Duration == TimeSpan.Zero)
                {
                    OnLogged($"New file has no duration: {newFile.FilePath}");
                    continue;
                }
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

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

                SaveVideoFilesCache(VideoFiles, Configuration.CachePath);
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                FindDuplicatesOf(VideoFiles, newFile, cancelToken);
                // Cleanup in case of cancel
                foreach (var file in VideoFiles)
                {
                    file.DisposeThumbnails();
                }
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }
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
                    DedupTask?.Wait();
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
