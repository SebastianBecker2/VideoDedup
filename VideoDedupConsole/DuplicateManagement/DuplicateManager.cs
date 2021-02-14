namespace VideoDedupConsole.DuplicateManagement
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Newtonsoft.Json;
    using VideoDedupShared;
    using Wcf.Contracts.Data;

    internal class DuplicateManager
    {
        private class DuplicateWrapper : IEquatable<DuplicateWrapper>
        {
            public DuplicateData DuplicateData { get; }
            public Guid DuplicateId => DuplicateData.DuplicateId;
            public VideoFile File1 => DuplicateData.File1;
            public VideoFile File2 => DuplicateData.File2;
            public DateTime LastRequest { get; set; }

            public DuplicateWrapper(
                VideoFile file1,
                VideoFile file2,
                string basePath)
            {
                _ = file1.FileSize;
                _ = file1.Duration;
                _ = file1.CodecInfo;
                _ = file2.FileSize;
                _ = file2.Duration;
                _ = file2.CodecInfo;
                DuplicateData = new DuplicateData
                {
                    DuplicateId = Guid.NewGuid(),
                    File1 = file1,
                    File2 = file2,
                    BasePath = basePath,
                };
            }

            public override bool Equals(object obj) =>
                Equals(obj as DuplicateWrapper);

            public bool Equals(DuplicateWrapper other) =>
                other != null
                && ((EqualityComparer<VideoFile>.Default.Equals(File1, other.File1)
                && EqualityComparer<VideoFile>.Default.Equals(File2, other.File2))
                || (EqualityComparer<VideoFile>.Default.Equals(File1, other.File2)
                && EqualityComparer<VideoFile>.Default.Equals(File2, other.File1)));

            public override int GetHashCode()
            {
                var hashCode = 1051446793;
                hashCode = (hashCode * -1521134295)
                    + EqualityComparer<VideoFile>.Default.GetHashCode(File1);
                hashCode = (hashCode * -1521134295)
                    + EqualityComparer<VideoFile>.Default.GetHashCode(File2);
                return hashCode;
            }

            public static bool operator ==(
                DuplicateWrapper left,
                DuplicateWrapper right) =>
                EqualityComparer<DuplicateWrapper>.Default.Equals(left, right);

            public static bool operator !=(
                DuplicateWrapper left,
                DuplicateWrapper right) =>
                !(left == right);
        }

        private class ThumbnailManager
        {
            private class VideoFileRefCounter
            {
                public VideoFile VideoFile { get; set; }
                public int RefCount { get; set; }

            }

            private Dictionary<IVideoFile, VideoFileRefCounter> UniqueVideoFiles { get; } =
                new Dictionary<IVideoFile, VideoFileRefCounter>();

            public IThumbnailSettings Configuration { get; set; }

            public VideoFile AddVideoFileReference(
                DedupEngine.VideoFile videoFile)
            {
                if (UniqueVideoFiles.TryGetValue(videoFile, out var refCounter))
                {
                    refCounter.RefCount++;
                    return refCounter.VideoFile;
                }

                if (videoFile.ImageCount == Configuration.Count)
                {
                    var videoFilePreview = new VideoFile(videoFile);
                    UniqueVideoFiles.Add(videoFile, new VideoFileRefCounter
                    {
                        VideoFile = videoFilePreview,
                        RefCount = 1,
                    });
                    return videoFilePreview;
                }

                using (var videoFileWithImages = new DedupEngine.VideoFile(
                        videoFile, Configuration.Count))
                {
                    var videoFilePreview = new VideoFile(videoFileWithImages);
                    UniqueVideoFiles.Add(videoFile, new VideoFileRefCounter
                    {
                        VideoFile = videoFilePreview,
                        RefCount = 1,
                    });
                    return videoFilePreview;
                }
            }

            public void RemoveVideoFileReference(IVideoFile videoFile)
            {
                var refCounter = UniqueVideoFiles[videoFile];

                if (refCounter.RefCount == 1)
                {
                    refCounter.VideoFile.Dispose();
                    _ = UniqueVideoFiles.Remove(videoFile);
                    return;
                }

                refCounter.RefCount--;
            }
        }

        private static readonly string DuplicateFilename = "Duplicate.list";
        private string DuplicateFilePath =>
            Path.Combine(AppDataFolder, DuplicateFilename);
        private string AppDataFolder { get; }

        private readonly ThumbnailManager thumbnailManager =
            new ThumbnailManager();
        private readonly ISet<DuplicateWrapper> duplicateList
            = new HashSet<DuplicateWrapper>();
        private readonly object duplicateLock = new object();
        private volatile int count;

        public IThumbnailSettings Settings
        {
            get => thumbnailManager.Configuration;
            set => thumbnailManager.Configuration = value;
        }
        public int Count => count;

        public bool AutoSave { get; set; }

        public event EventHandler<DuplicateAddedEventArgs> DuplicateAdded;
        protected virtual void OnDuplicateAdded(DuplicateData duplicate) =>
            DuplicateAdded?.Invoke(this, new DuplicateAddedEventArgs
            {
                Duplicate = duplicate,
                DuplicateCount = Count,
            });

        public event EventHandler<DuplicateRemovedEventArgs> DuplicateRemoved;
        protected virtual void OnDuplicateRemoved(DuplicateData duplicate) =>
            DuplicateRemoved?.Invoke(this, new DuplicateRemovedEventArgs
            {
                Duplicate = duplicate,
                DuplicateCount = Count,
            });

        public event EventHandler<DuplicateResolvedEventArgs> DuplicateResolved;
        protected virtual void OnDuplicateResolved(
            DuplicateData duplicate,
            ResolveOperation operation) =>
            DuplicateResolved?.Invoke(this, new DuplicateResolvedEventArgs
            {
                Duplicate = duplicate,
                Operation = operation,
            });

        public event EventHandler<FileLoadedProgressEventArgs> FileLoadedProgress;
        protected virtual void OnFileLoadedProgress(
            Func<FileLoadedProgressEventArgs> eventArgsCreator) =>
            FileLoadedProgress?.Invoke(this, eventArgsCreator.Invoke());

        public event EventHandler<FileLoadedEventArgs> FileLoaded;
        protected virtual void OnFileLoaded(int duplicateCount) =>
            FileLoaded?.Invoke(this, new FileLoadedEventArgs
            {
                DuplicateCount = duplicateCount,
                FilePath = DuplicateFilePath,
            });

        public event EventHandler<FileSavedEventArgs> FileSaved;
        protected virtual void OnFileSaved(int duplicateCount) =>
            FileSaved?.Invoke(this, new FileSavedEventArgs
            {
                DuplicateCount = duplicateCount,
                FilePath = DuplicateFilePath,
            });

        public DuplicateManager(string appDataFolder,
            bool autoSave = true)
        {
            if (string.IsNullOrWhiteSpace(appDataFolder))
            {
                throw new ArgumentException($"'{nameof(appDataFolder)}' cannot" +
                    $"be null or whitespace", nameof(appDataFolder));
            }
            AppDataFolder = appDataFolder;
            AutoSave = autoSave;
        }

        public DuplicateManager(
            IThumbnailSettings settings,
            string appDataFolder,
            bool autoSave = true)
            : this(appDataFolder, autoSave) =>
            Settings = settings
                ?? throw new ArgumentNullException(nameof(settings));

        public DuplicateData GetDuplicate()
        {
            lock (duplicateLock)
            {
                while (true)
                {
                    if (!duplicateList.Any())
                    {
                        return null;
                    }

                    // OrderBy is stable, so we get the first if multiple
                    // don't have a real LastRequest time-stamp
                    var duplicate = duplicateList
                        .OrderBy(d => d.LastRequest)
                        .First();

                    if (!File.Exists(duplicate.File1.FilePath)
                        || !File.Exists(duplicate.File2.FilePath))
                    {
                        _ = duplicateList.Remove(duplicate);
                        if (AutoSave)
                        {
                            SaveToFile();
                        }
                        continue;
                    }

                    // To preserve specific order
                    // even when using multiple clients.
                    // The most recently requested will be last
                    // next time (when skipped). Or first when canceled.
                    duplicate.LastRequest = DateTime.Now;
                    return duplicate.DuplicateData;
                }
            }
        }

        public void DiscardAll()
        {
            lock (duplicateLock)
            {
                foreach (var duplicate in duplicateList)
                {
                    thumbnailManager.RemoveVideoFileReference(duplicate.File1);
                    thumbnailManager.RemoveVideoFileReference(duplicate.File2);
                }
                duplicateList.Clear();
                count = 0;
                SaveToFile();
            }
        }

        public void AddDuplicate(
            DedupEngine.VideoFile file1,
            DedupEngine.VideoFile file2,
            string basePath)
        {
            lock (duplicateLock)
            {
                AddDuplicate(new DuplicateWrapper(
                    thumbnailManager.AddVideoFileReference(file1),
                    thumbnailManager.AddVideoFileReference(file2),
                    basePath));
            }
        }

        private void AddDuplicate(
            DuplicateWrapper duplicate,
            bool preventSave = false)
        {
            lock (duplicateLock)
            {
                _ = duplicateList.Add(duplicate);
            }
            count++;

            if (!preventSave && AutoSave)
            {
                SaveToFile();
            }

            OnDuplicateAdded(duplicate.DuplicateData);
        }

        public void RemoveDuplicate(Guid duplicateId)
        {
            lock (duplicateLock)
            {
                var duplicate = duplicateList
                .FirstOrDefault(d =>
                    d.DuplicateId == duplicateId);

                if (duplicate == null)
                {
                    return;
                }

                RemoveDuplicate(duplicate);
            }
        }

        private void RemoveDuplicate(
            DuplicateWrapper duplicate,
            bool preventSave = false)
        {
            lock (duplicateLock)
            {
                thumbnailManager.RemoveVideoFileReference(duplicate.File1);
                thumbnailManager.RemoveVideoFileReference(duplicate.File2);
                _ = duplicateList.Remove(duplicate);
            }
            count--;

            if (!preventSave && AutoSave)
            {
                SaveToFile();
            }

            OnDuplicateRemoved(duplicate.DuplicateData);
        }

        public void ResolveDuplicate(
            Guid duplicateId,
            ResolveOperation resolveOperation)
        {
            lock (duplicateLock)
            {
                var duplicate = duplicateList
                    .FirstOrDefault(d =>
                        d.DuplicateId == duplicateId);

                if (duplicate == null)
                {
                    return;
                }

                void DeleteFile(string path)
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch (Exception)
                    {
                        // Would be nice to make a log entry here.
                        // But since it would get lost in the ocean
                        // of logs, it's almost pointless.
                    }
                }

                switch (resolveOperation)
                {
                    case ResolveOperation.DeleteFile1:
                        DeleteFile(duplicate.File1.FilePath);
                        RemoveDuplicate(duplicate);
                        break;
                    case ResolveOperation.DeleteFile2:
                        DeleteFile(duplicate.File2.FilePath);
                        RemoveDuplicate(duplicate);
                        break;
                    case ResolveOperation.Skip:
                        // Do nothing.
                        // The duplicate is kept in the list for later.
                        break;
                    case ResolveOperation.Discard:
                        RemoveDuplicate(duplicate);
                        break;
                    case ResolveOperation.Cancel:
                        duplicate.LastRequest = DateTime.MinValue;
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"\"{resolveOperation}\" is invalid"
                            + $"for enum {nameof(ResolveOperation)}");
                }

                OnDuplicateResolved(duplicate.DuplicateData, resolveOperation);
            }
        }

        public void LoadFromFile(CancellationToken? cancellationToken = null)
        {
            var startTime = DateTime.Now;

            var duplicateFilePairs = JsonConvert.DeserializeObject<List<Tuple<
                    DedupEngine.VideoFile,
                    DedupEngine.VideoFile,
                    string>>>(
                File.ReadAllText(DuplicateFilePath));

            var duplciateLoaded = 0;
            foreach (var tuple in duplicateFilePairs)
            {
                cancellationToken?.ThrowIfCancellationRequested();

                lock (duplicateLock)
                {
                    var duplicate = new DuplicateWrapper(
                        thumbnailManager.AddVideoFileReference(tuple.Item1),
                        thumbnailManager.AddVideoFileReference(tuple.Item2),
                        tuple.Item3);

                    AddDuplicate(duplicate, true);

                    duplciateLoaded++;
                    OnFileLoadedProgress(() =>
                        new FileLoadedProgressEventArgs
                        {
                            Duplicate = duplicate.DuplicateData,
                            Count = duplciateLoaded,
                            MaxCount = duplicateFilePairs.Count(),
                            FilePath = DuplicateFilePath,
                            StartTime = startTime,
                        });
                }
            }

            OnFileLoaded(duplicateFilePairs.Count());
        }

        public void SaveToFile()
        {
            lock (duplicateLock)
            {
                var duplicateFilePairs = duplicateList
                    .Select(d => (
                        File1: new DedupEngine.VideoFile(d.File1),
                        File2: new DedupEngine.VideoFile(d.File2),
                        d.DuplicateData.BasePath));

                File.WriteAllText(DuplicateFilePath, JsonConvert.SerializeObject(
                    duplicateFilePairs,
                    Formatting.Indented));

                OnFileSaved(duplicateFilePairs.Count());
            }
        }
    }
}
