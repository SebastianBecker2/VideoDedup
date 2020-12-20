using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoDedup.TimeSpanExtension;

namespace VideoDedup
{
    class Duplicate : Tuple<VideoFile, VideoFile>
    {
        public Duplicate(VideoFile file1, VideoFile file2) : base(file1, file2)
        {
        }
    }

    class StoppedEventArgs : EventArgs { }
    class DuplicateCountChangedEventArgs : EventArgs
    {
        public int Count { get; set; }
    }
    class StatusChangedEventArgs : EventArgs { }
    class ProgressUpdateEventArgs : EventArgs
    {
        public string StatusInfo { get; set; }
        public int Counter { get; set; }
        public int MaxCount { get; set; }
    }
    class LoggedEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    class Dedupper : IDisposable
    {
        private readonly static string CacheFolderName = "VideoDedupCache";
        private readonly static string CacheFileName = "video_files.cache";
        private static string CacheFilePath
        {
            get
            {
                var cache_folder = Path.Combine(ConfigData.SourcePath, CacheFolderName);
                Directory.CreateDirectory(cache_folder);
                return Path.Combine(cache_folder, CacheFileName);
            }
        }

        private readonly static string StatusInfoComparing = "Comparing: {0}/{1}";
        private readonly static string StatusInfoLoading = "Loading media info: {0}/{1}";
        private readonly static string StatusInfoSearching = "Searching for files...";

        private readonly static string StatusInfoDuplicateCount = "Duplicates found {0}";
        private readonly static string StatusInfoChecking = "Checking: ";

        private bool disposedValue;

        private ConfigNonStatic Configuration { get; set; } = null;
        private Task DedupTask { get; set; } = null;
        private object DedupLock { get; set; } = new object { };
        private CancellationTokenSource CancelSource { get; set; }
            = new CancellationTokenSource { };
        private IProducerConsumerCollection<VideoFile> NewFiles { get; set; }
            = new ConcurrentQueue<VideoFile> { };
        private IProducerConsumerCollection<VideoFile> DeletedFiles { get; set; }
            = new ConcurrentQueue<VideoFile> { };
        private IEnumerable<VideoFile> VideoFiles { get; set; }
        private IProducerConsumerCollection<Duplicate> Duplicates { get; set; }
            = new ConcurrentQueue<Duplicate> { };

        public event EventHandler<StoppedEventArgs> Stopped;
        protected virtual void OnStopped()
        {
            Stopped?.Invoke(this, new StoppedEventArgs { });
        }

        public event EventHandler<DuplicateCountChangedEventArgs> DuplicateCountChanged;
        protected virtual void OnDuplicateCountChanged()
        {
            DuplicateCountChanged?.Invoke(this, new DuplicateCountChangedEventArgs
            {
                Count = Duplicates.Count
            });
        }

        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        protected virtual void OnStatusChanged()
        {
            StatusChanged?.Invoke(this, new StatusChangedEventArgs { });
        }

        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;
        protected virtual void OnProgressUpdate(string statusInfo,
            int counter,
            int maxCount)
        {
            ProgressUpdate?.Invoke(this, new ProgressUpdateEventArgs
            {
                StatusInfo = statusInfo,
                Counter = counter,
                MaxCount = maxCount
            });
        }

        public event EventHandler<LoggedEventArgs> Logged;
        protected virtual void OnLogged(string message)
        {
            Logged?.Invoke(this, new LoggedEventArgs { Message = message });
        }

        public Dedupper(ConfigNonStatic config)
        {
            Configuration = config;

            RestartProcessingFolder();
        }

        public void EnqueueDuplicate(Duplicate duplicate)
        {
            Duplicates.TryAdd(duplicate);
            OnDuplicateCountChanged();
        }

        public bool DequeueDuplcate(out Duplicate duplicate)
        {
            if (Duplicates.TryTake(out duplicate))
            {
                OnDuplicateCountChanged();
                return true;
            }
            return false;
        }

        public void UpdateConfiguration(ConfigNonStatic config)
        {
            // Depending on which cofigurations changed,
            // it should try to avoid extra work.
            // Changing the folder invalidates the video file list.
            // But changing the MaxDifferentThumbnails only requires
            // a new search for duplicates in the video file list.
        }

        private void RestartProcessingFolder()
        {
            CancelSource.Cancel();
            DedupTask?.Wait();
            CancelSource?.Dispose();
            lock (DedupLock)
            {
                CancelSource = new CancellationTokenSource();
                DedupTask = Task.Factory.StartNew(ProcessFolder);
            }
        }

        private void StartProcessingChanges()
        {
            lock (DedupLock)
            {
                // If the task is still running,
                // it will check for new files on it's own.
                if (DedupTask != null)
                {
                    return;
                }

                DedupTask = Task.Factory.StartNew(ProcessChanges);
            }
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

                foreach (string directory in Directory
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

        private IEnumerable<VideoFile> GetVideoFileList(string sourcePath)
        {
            var timer = Stopwatch.StartNew();

            OnProgressUpdate(StatusInfoSearching, 0, 0);

            // Get all video files in source path.
            var fileExtensions = ConfigData.FileExtensions.ToList();
            var found_files = GetAllAccessibleFilesIn(sourcePath, ConfigData.ExcludedDirectories)
                .Where(f => fileExtensions.Contains(Path.GetExtension(f), StringComparer.CurrentCultureIgnoreCase))
                .Select(f => new VideoFile(f));

            var cached_files = LoadVideoFilesCache(CacheFilePath);
            if (cached_files == null || !cached_files.Any())
            {
                cached_files = new HashSet<VideoFile>(found_files);
            }
            else
            {
                // Basically overwrite the found files with cached files
                // and make sure we don't take cached files that don't exist
                // anymore.
                cached_files.RemoveWhere(f => !File.Exists(f.FilePath));
                cached_files.UnionWith(found_files);
            }
            timer.Stop();

            OnProgressUpdate(StatusInfoLoading, 0, cached_files.Count());
            Debug.Print($"Found {cached_files.Count()} video files in {timer.ElapsedMilliseconds} ms");
            return cached_files;
        }

        private void PreloadFiles(
            IEnumerable<VideoFile> videoFiles,
            CancellationToken cancelToken)
        {
            int counter = 0;
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

            for (int index = 0; index < videoFileList.Count() - 1; index++)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                var file = videoFileList[index];

                OnLogged(StatusInfoChecking + $"{file.FilePath}{Environment.NewLine}" +
                    $"Duration: {file.Duration.ToPrettyString()}");
                OnProgressUpdate(StatusInfoComparing,
                    index + 1,
                    videoFileList.Count());

                for (int nextIndex = index + 1; nextIndex < videoFileList.Count; nextIndex++)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }

                    var nextFile = videoFileList[nextIndex];

                    if (!file.IsDurationEqual(nextFile))
                    {
                        break;
                    }

                    if (file.AreThumbnailsEqual(nextFile))
                    {
                        EnqueueDuplicate(new Duplicate(file, nextFile));
                    }
                }

                file.DisposeThumbnails();
            }

            OnProgressUpdate(StatusInfoComparing,
                videoFileList.Count(),
                videoFileList.Count());
        }

        private void ProcessChangesIfAny()
        {
            lock (DedupLock)
            {
                if (!NewFiles.Any() && !DeletedFiles.Any())
                {
                    DedupTask = null;
                    return;
                }

                DedupTask = Task.Factory.StartNew(ProcessChanges);
            }
        }

        private void ProcessFolder()
        {
            var cancelToken = CancelSource.Token;

            OnLogged(StatusInfoChecking);

            VideoFiles = GetVideoFileList(Configuration.SourcePath);

            // Cancallable preload of files
            PreloadFiles(VideoFiles, cancelToken);

            // Remove invalid files
            VideoFiles = VideoFiles.Where(f => f.Duration != TimeSpan.Zero);

            SaveVideoFilesCache(VideoFiles, CacheFilePath);

            FindDuplicates(VideoFiles, cancelToken);

            // Cleanup in case of cancel
            foreach (var file in VideoFiles)
            {
                file.DisposeThumbnails();
            }

            ProcessChangesIfAny();
        }

        private void ProcessChanges()
        {
            var cancelToken = CancelSource.Token;

            while (NewFiles.TryTake(out VideoFile file))
            {
                if (!file.WaitForFileAccess())
                {
                    continue;
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
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Dedupper()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

// Process Folder (initial processing)
// 1) GetVideoFileList()
// 2) PreloadFiles() // Cancellable and Statusable
// 3) videoFiles = videoFiles.Where(f => f.Duration != new TimeSpan())
// 4) SaveVideoFilesCache()
// 5) Remove files outside of MinDuration and MaxDuration (Not yet, maybe later on)
// 5) orderedVideoFiles = videoFiles.OrderBy(f => f.Duration) // Use List.sort()?
// 6) FindDuplicates()
// 7) DisposeThumbnails (in case of cancel in FindDuplicates??)

// Process Changes (adding new files)
// 1) PreloadFiles() // Of new files
// 2) newVideoFiles = newVideoFiles.Where(f => f.Duration != new TimeSpan())
// 3) videoFiles = orderedVideoFiles.Concat(newVideoFiles))
// 4) SaveVideoFilesCache(videoFiles)
// 5) Remove files outside of MinDuration and MaxDuration (Not yet, maybe later on)
// 5) orderedVideoFiles = videoFiles.OrderBy(f => f.Duration) // Use List.sort()?
// 6) FindDuplicates() // Of new files!!!
// 7) DisposeThumbnails (in case of cancel in FindDuplicates??)


// ProcessChangesIfAny() can't just be at the end.
// It won't be called on exception or cancel.
// But do we even want that?
// We don't just cancel it anymore just for fun.
// We cancel on config change or shutdown.
// In those cases, we don't want to process changes.

// ToDo:
// - Implement FileSystemWatcher
// - Handle New/Deleted files
// - VideoFile without ConfigData
// - https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/formatting-rules