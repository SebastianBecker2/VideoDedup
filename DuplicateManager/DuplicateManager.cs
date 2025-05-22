namespace DuplicateManager
{
    using EventArgs;
    using VideoDedupGrpc;
    using VideoDedupSharedLib;

    public class DuplicateManager : IDisposable
    {
        private readonly ThumbnailManager thumbnailManager;
        private readonly object duplicateLock = new();
        private readonly HashSet<DuplicateWrapper> duplicateList = [];
        private readonly ActiveWorkProcessor<DuplicateWrapper> prepareProcessor;

        private int allDuplicatesCount;
        private bool disposedValue;

        public ResolutionSettings Settings => thumbnailManager.Settings;

        public int DuplicatesCount
        {
            get
            {
                lock (duplicateLock)
                {
                    return allDuplicatesCount;
                }
            }
        }
        public int PreparedDuplicatesCount
        {
            get
            {
                lock (duplicateLock)
                {
                    return duplicateList.Count;
                }
            }
        }
        public int UnpreparedDuplicatesCount
        {
            get
            {
                lock (duplicateLock)
                {
                    return prepareProcessor.Count;
                }
            }
        }

        public event EventHandler<DuplicateAddedEventArgs>? DuplicateAdded;
        protected virtual void OnDuplicateAdded(DuplicateData duplicate) =>
            DuplicateAdded?.Invoke(
                this,
                new DuplicateAddedEventArgs(duplicate, DuplicatesCount));

        public event EventHandler<DuplicateRemovedEventArgs>? DuplicateRemoved;
        protected virtual void OnDuplicateRemoved(DuplicateData duplicate) =>
            DuplicateRemoved?.Invoke(
                this,
                new DuplicateRemovedEventArgs(duplicate, DuplicatesCount));

        public event EventHandler<DuplicateResolvedEventArgs>? DuplicateResolved;
        protected virtual void OnDuplicateResolved(
            DuplicateData duplicate,
            ResolveOperation operation) =>
            DuplicateResolved?.Invoke(
                this,
                new DuplicateResolvedEventArgs(duplicate, operation));

        public DuplicateManager(ResolutionSettings settings)
        {
            thumbnailManager = new(settings);
            prepareProcessor = new(PrepareDuplicate);
        }

        public void UpdateSettings(ResolutionSettings settings)
        {
            thumbnailManager.Settings = settings;
            DiscardAll();
        }

        public DuplicateData GetDuplicate()
        {
            lock (duplicateLock)
            {
                while (true)
                {
                    if (duplicateList.Count == 0)
                    {
                        return new DuplicateData();
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
            prepareProcessor.Clear();
            lock (duplicateLock)
            {
                foreach (var duplicate in duplicateList)
                {
                    thumbnailManager.RemoveVideoFileReference(duplicate.File1);
                    thumbnailManager.RemoveVideoFileReference(duplicate.File2);
                }
                duplicateList.Clear();
                _ = Interlocked.Exchange(ref allDuplicatesCount, 0);
            }
        }

        public void AddDuplicate(
            VideoFile file1,
            VideoFile file2,
            string basePath)
        {
            prepareProcessor.Enqueue(new DuplicateWrapper(file1, file2, basePath));
            _ = Interlocked.Increment(ref allDuplicatesCount);
        }

        private void AddDuplicate(
            DuplicateWrapper duplicate)
        {
            lock (duplicateLock)
            {
                _ = duplicateList.Add(duplicate);
            }
            OnDuplicateAdded(duplicate.DuplicateData);
        }

        public void RemoveDuplicate(string duplicateId)
        {
            lock (duplicateLock)
            {
                var duplicate = duplicateList
                .FirstOrDefault(d => d.DuplicateId == duplicateId);

                if (duplicate is null)
                {
                    return;
                }

                RemoveDuplicate(duplicate);
            }
        }

        private void RemoveDuplicate(DuplicateWrapper duplicate)
        {
            lock (duplicateLock)
            {
                thumbnailManager.RemoveVideoFileReference(duplicate.File1);
                thumbnailManager.RemoveVideoFileReference(duplicate.File2);
                _ = duplicateList.Remove(duplicate);
            }
            OnDuplicateRemoved(duplicate.DuplicateData);
        }

        private void HandleDeleteFile(
            DuplicateWrapper duplicate,
            VideoFile? file)
        {
            if (file is null)
            {
                throw new InvalidOperationException(
                    "No file specified. Operation " +
                    $"\"{ResolveOperation.DeleteFile}\" requires a file to be " +
                    "specified.");
            }

            if (file.FilePath != duplicate.File1.FilePath
                && file.FilePath != duplicate.File2.FilePath)
            {
                throw new InvalidOperationException(
                    "Invalid file specified. Operation " +
                    $"\"{ResolveOperation.DeleteFile}\" requires a file to be " +
                    "specified that matches either of the files of " +
                    "the duplicate.");
            }

            if (Settings.MoveToTrash)
            {
                _ = Directory.CreateDirectory(Settings.TrashPath);

                var folder =
                    Path.Combine(Settings.TrashPath, $"{Guid.NewGuid()}");
                _ = Directory.CreateDirectory(folder);

                var trashFile =
                    Path.Combine(folder, Path.GetFileName(file.FilePath));
                File.Move(file.FilePath, trashFile);

                var metaFile = Path.Combine(folder, "meta.txt");
                File.WriteAllText(metaFile, file.FilePath);
            }
            else
            {
                File.Delete(file.FilePath);
            }

            RemoveDuplicate(duplicate);
        }

        public void ResolveDuplicate(
            string duplicateId,
            ResolveOperation resolveOperation,
            VideoFile? file)
        {
            lock (duplicateLock)
            {
                var duplicate = duplicateList
                    .FirstOrDefault(d =>
                        d.DuplicateId == duplicateId);

                if (duplicate is null)
                {
                    return;
                }

                switch (resolveOperation)
                {
                    case ResolveOperation.DeleteFile:
                        HandleDeleteFile(duplicate, file);
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

        private void PrepareDuplicate(
            DuplicateWrapper duplicate,
            CancellationToken cancelToken)
        {
            if (cancelToken.IsCancellationRequested)
            {
                return;
            }

            _ = thumbnailManager.AddVideoFileReference(
                duplicate.File1,
                cancelToken);

            if (cancelToken.IsCancellationRequested)
            {
                return;
            }

            _ = thumbnailManager.AddVideoFileReference(
                duplicate.File2,
                cancelToken);

            AddDuplicate(duplicate);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    prepareProcessor.Dispose();
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
