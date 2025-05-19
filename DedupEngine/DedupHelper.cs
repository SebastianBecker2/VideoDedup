namespace DedupEngine
{
    using VideoDedupGrpc;

    internal class DedupHelper
    {
        public static IEnumerable<string> GetAllAccessibleFilesIn(
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

        public static List<Candidate> PrepareCandidates(
            List<VideoFile> targetVideos,
            List<VideoFile> allVideos,
            DurationComparisonSettings durationComparisonSettings)
        {
            var pairs = new HashSet<Candidate>();
            foreach (var targetVideo in targetVideos)
            {
                foreach (var otherVideo in allVideos.Where(f => f != targetVideo))
                {
                    if (!targetVideo.IsDurationEqual(
                        otherVideo,
                        durationComparisonSettings))
                    {
                        continue;
                    }
                    _ = pairs.Add(new Candidate(targetVideo, otherVideo));
                }
            }

            return [.. pairs];
        }
    }
}
