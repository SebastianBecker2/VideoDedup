namespace DuplicateManager
{
    using EventArgs;
    using VideoDedupGrpc;

    public class DuplicateManager
    {
        private readonly ThumbnailManager thumbnailManager;
        private readonly object duplicateLock = new();

        private ISet<DuplicateWrapper> duplicateList
            = new HashSet<DuplicateWrapper>();

        public ThumbnailSettings Settings => thumbnailManager.Settings;

        public int Count
        {
            get
            {
                lock (duplicateLock)
                {
                    return duplicateList.Count;
                }
            }
        }

        public event EventHandler<DuplicateAddedEventArgs>? DuplicateAdded;
        protected virtual void OnDuplicateAdded(DuplicateData duplicate) =>
            DuplicateAdded?.Invoke(
                this,
                new DuplicateAddedEventArgs(duplicate, Count));

        public event EventHandler<DuplicateRemovedEventArgs>? DuplicateRemoved;
        protected virtual void OnDuplicateRemoved(DuplicateData duplicate) =>
            DuplicateRemoved?.Invoke(
                this,
                new DuplicateRemovedEventArgs(duplicate, Count));

        public event EventHandler<DuplicateResolvedEventArgs>? DuplicateResolved;
        protected virtual void OnDuplicateResolved(
            DuplicateData duplicate,
            ResolveOperation operation) =>
            DuplicateResolved?.Invoke(
                this,
                new DuplicateResolvedEventArgs(duplicate, operation));

        public DuplicateManager(
            ThumbnailSettings settings) =>
            thumbnailManager = new ThumbnailManager(
                settings ?? throw new ArgumentNullException(nameof(settings)));

        public void UpdateSettings(
            ThumbnailSettings settings,
            UpdateSettingsResolution resolution =
                UpdateSettingsResolution.DiscardDuplicates)
        {
            thumbnailManager.Settings = settings;
            if (resolution == UpdateSettingsResolution.DiscardDuplicates)
            {
                DiscardAll();
                return;
            }

            lock (duplicateLock)
            {
                var oldDuplicateList = duplicateList;

                foreach (var d in duplicateList)
                {
                    thumbnailManager.RemoveVideoFileReference(d.File1);
                    thumbnailManager.RemoveVideoFileReference(d.File2);
                }

                duplicateList = new HashSet<DuplicateWrapper>(
                    oldDuplicateList.Select(d => new DuplicateWrapper(
                        thumbnailManager.AddVideoFileReference(d.File1),
                        thumbnailManager.AddVideoFileReference(d.File2),
                        d.DuplicateData.BasePath)));
            }
        }

        public DuplicateData GetDuplicate()
        {
            lock (duplicateLock)
            {
                while (true)
                {
                    if (!duplicateList.Any())
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
            lock (duplicateLock)
            {
                foreach (var duplicate in duplicateList)
                {
                    thumbnailManager.RemoveVideoFileReference(duplicate.File1);
                    thumbnailManager.RemoveVideoFileReference(duplicate.File2);
                }
                duplicateList.Clear();
            }
        }

        public void AddDuplicate(
            VideoFile file1,
            VideoFile file2,
            string basePath)
        {
            var videoFilePreview1 = thumbnailManager.AddVideoFileReference(file1);
            var videoFilePreview2 = thumbnailManager.AddVideoFileReference(file2);
            AddDuplicate(new DuplicateWrapper(
                videoFilePreview1,
                videoFilePreview2,
                basePath));
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

        private void RemoveDuplicate(
            DuplicateWrapper duplicate)
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

            File.Delete(file.FilePath);
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
    }
}
