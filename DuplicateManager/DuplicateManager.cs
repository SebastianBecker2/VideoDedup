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
            if (file is null || string.IsNullOrWhiteSpace(file.FilePath))
            {
                throw new InvalidOperationException(
                    "No file specified. Operation " +
                    $"\"{ResolveOperation.DeleteFile}\" requires a file to be " +
                    "specified.");
            }

            var sourcePath = GetMatchingDuplicateFilePath(duplicate, file.FilePath);
            if (sourcePath is null)
            {
                throw new InvalidOperationException(
                    "Invalid file specified. Operation " +
                    $"\"{ResolveOperation.DeleteFile}\" requires a file to be " +
                    "specified that matches either of the files of " +
                    "the duplicate.");
            }

            if (Settings.MoveToTrash)
            {
                if (string.IsNullOrWhiteSpace(Settings.TrashPath))
                {
                    throw new InvalidOperationException(
                        "Trash path is not configured.");
                }

                _ = Directory.CreateDirectory(Settings.TrashPath);

                var folder =
                    Path.Combine(Settings.TrashPath, $"{Guid.NewGuid()}");
                _ = Directory.CreateDirectory(folder);

                var trashFile =
                    Path.Combine(folder, Path.GetFileName(sourcePath));
                MoveFile(sourcePath, trashFile);

                var metaFile = Path.Combine(folder, "meta.txt");
                File.WriteAllText(metaFile, sourcePath);
            }
            else
            {
                File.Delete(sourcePath);
            }

            RemoveDuplicate(duplicate);
        }

        private static string? GetMatchingDuplicateFilePath(
            DuplicateWrapper duplicate,
            string filePath)
        {
            if (PathsEqual(filePath, duplicate.File1.FilePath))
            {
                return duplicate.File1.FilePath;
            }

            if (PathsEqual(filePath, duplicate.File2.FilePath))
            {
                return duplicate.File2.FilePath;
            }

            return null;
        }

        private static string NormalizePathForComparison(string path)
        {
            path = path.Trim();
            try
            {
                return Path.GetFullPath(path);
            }
            catch (Exception)
            {
                return path.Replace('\\', '/');
            }
        }

        private static bool PathsEqual(string left, string right) =>
            string.Equals(
                NormalizePathForComparison(left),
                NormalizePathForComparison(right),
                OperatingSystem.IsWindows()
                    ? StringComparison.OrdinalIgnoreCase
                    : StringComparison.Ordinal);

        private static void MoveFile(string sourcePath, string destinationPath)
        {
            var destinationDirectory = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destinationDirectory))
            {
                _ = Directory.CreateDirectory(destinationDirectory);
            }

            try
            {
                File.Move(sourcePath, destinationPath);
            }
            catch (IOException ex) when (IsCrossDeviceMove(ex))
            {
                File.Copy(sourcePath, destinationPath, overwrite: false);
                File.Delete(sourcePath);
            }
        }

        private static bool IsCrossDeviceMove(IOException ex) =>
            ex.Message.Contains("cross-device", StringComparison.OrdinalIgnoreCase)
            || (OperatingSystem.IsWindows()
                && ex.HResult == unchecked((int)0x80070011));

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
                    throw new InvalidOperationException(
                        $"Duplicate \"{duplicateId}\" was not found.");
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
