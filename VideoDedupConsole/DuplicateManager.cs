namespace VideoDedupConsole
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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

        private readonly ThumbnailManager thumbnailManager =
            new ThumbnailManager();
        private readonly ISet<DuplicateWrapper> duplicateList =
            new HashSet<DuplicateWrapper>();
        private readonly object duplicateLock = new object();

        public IThumbnailSettings Settings
        {
            get => thumbnailManager.Configuration;
            set => thumbnailManager.Configuration = value;
        }

        public int Count
        {
            get
            {
                lock (duplicateLock)
                {
                    return duplicateList.Count();
                }
            }
        }

        public DuplicateManager() { }

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
            DedupEngine.VideoFile file1,
            DedupEngine.VideoFile file2,
            string basePath)
        {
            lock (duplicateLock)
            {
                _ = duplicateList.Add(new DuplicateWrapper(
                    thumbnailManager.AddVideoFileReference(file1),
                    thumbnailManager.AddVideoFileReference(file2),
                    basePath));
            }
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
                        // But since it would get lost int he ocean
                        // of logs, it's almost pointless.
                    }
                }

                switch (resolveOperation)
                {
                    case ResolveOperation.DeleteFile1:
                        DeleteFile(duplicate.File1.FilePath);
                        thumbnailManager.RemoveVideoFileReference(duplicate.File1);
                        thumbnailManager.RemoveVideoFileReference(duplicate.File2);
                        _ = duplicateList.Remove(duplicate);
                        break;
                    case ResolveOperation.DeleteFile2:
                        DeleteFile(duplicate.File2.FilePath);
                        thumbnailManager.RemoveVideoFileReference(duplicate.File1);
                        thumbnailManager.RemoveVideoFileReference(duplicate.File2);
                        _ = duplicateList.Remove(duplicate);
                        break;
                    case ResolveOperation.Skip:
                        // Do nothing.
                        // The duplicate is kept in the list for later.
                        break;
                    case ResolveOperation.Discard:
                        thumbnailManager.RemoveVideoFileReference(duplicate.File1);
                        thumbnailManager.RemoveVideoFileReference(duplicate.File2);
                        _ = duplicateList.Remove(duplicate);
                        break;
                    case ResolveOperation.Cancel:
                        duplicate.LastRequest = DateTime.MinValue;
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"\"{resolveOperation}\" is invalid"
                            + $"for enum {nameof(ResolveOperation)}");
                }
            }
        }
    }
}
