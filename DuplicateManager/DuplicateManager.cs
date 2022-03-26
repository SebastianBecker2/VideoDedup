namespace DuplicateManager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    //using DedupEngine;
    using EventArgs;
    using KGySoft.CoreLibraries;
    using VideoDedupGrpc;
    using static VideoDedupGrpc.ResolveDuplicateRequest.Types;

    public class DuplicateManager
    {
        private readonly ThumbnailManager thumbnailManager;
        private readonly ISet<DuplicateWrapper> duplicateList
            = new HashSet<DuplicateWrapper>();
        private readonly object duplicateLock = new();

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
            if (resolution == UpdateSettingsResolution.DiscardDuplicates)
            {
                DiscardAll();
            }
            else
            {
                lock (duplicateLock)
                {
                    foreach (var duplicate in duplicateList)
                    {
                        thumbnailManager.RemoveVideoFileReference(duplicate.File1);
                        thumbnailManager.RemoveVideoFileReference(duplicate.File2);
                    }

                    var newDuplicateList = duplicateList.Select(d =>
                        new DuplicateWrapper(
                            thumbnailManager.AddVideoFileReference(new VideoFile(d.File1)),
                            thumbnailManager.AddVideoFileReference(new VideoFile(d.File2)),
                            d.DuplicateData.BasePath));
                    duplicateList.Clear();
                    duplicateList.AddRange(newDuplicateList);
                }
            }
            thumbnailManager.Settings = settings;
        }

        public DuplicateData? GetDuplicate()
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

        public void ResolveDuplicate(
            string duplicateId,
            ResolveOperation resolveOperation)
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

                static void DeleteFile(string path)
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
    }
}
