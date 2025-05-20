namespace VideoDedupSharedLib.ExtensionMethods.IVideoFileExtensions
{
    using System.Globalization;
    using System.Text;
    using Google.Protobuf.WellKnownTypes;
    using Interfaces;
    using VideoDedupGrpc;

    // ReSharper disable once InconsistentNaming
    public static class IVideoFileExtensions
    {
        /// <summary>
        /// Special case for FileSystemWatcher change events.
        /// When a large file has been written, the file might
        /// not be accessible yet when the event comes in.
        /// If we would try to read the duration right away
        /// it would fail.
        /// So we wait for read access to the file.
        /// For a maximum of 1 second.
        /// </summary>
        /// <param name="videoFile">The file we try to access.</param>
        /// <param name="cancelToken">A token to be able to cancel the wait.
        /// </param>
        /// <param name="timeout">For how long should we try to access the file?
        /// </param>
        /// <param name="interval">How long should we wait between each try?
        /// </param>
        /// <returns>
        /// true: File could be access.
        /// false: File could not be access in time.
        /// </returns>
        public static bool WaitForFileAccess(
            this IVideoFile videoFile,
            CancellationToken cancelToken,
            int timeout = 1000,
            int interval = 50) =>
            videoFile.WaitForFileAccess(timeout, interval, cancelToken);

        private static bool WaitForFileAccess(
            this IVideoFile videoFile,
            int timeout,
            int interval,
            CancellationToken? cancelToken = null)
        {
            if (string.IsNullOrWhiteSpace(videoFile.FilePath))
            {
                return false;
            }

            for (var i = 0; i < timeout / interval; i++)
            {
                try
                {
                    using var stream = File.Open(
                        videoFile.FilePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite);

                    return true;
                }
                catch (ArgumentOutOfRangeException) { throw; }
                catch (ArgumentNullException) { return false; }
                catch (ArgumentException) { return false; }
                catch (PathTooLongException) { return false; }
                catch (DirectoryNotFoundException) { return false; }
                catch (NotSupportedException) { return false; }
                catch (Exception) { }

                if (cancelToken is { IsCancellationRequested: true })
                {
                    return false;
                }
                Thread.Sleep(interval);
            }
            return false;
        }

        public static string GetInfoText(this IVideoFile videoFile)
        {
            var fileSize = videoFile.FileSize;
            var duration = videoFile.Duration;
            var codecInfo = videoFile.CodecInfo;
            var lastWrite = videoFile.LastWriteTime;
            var creation = videoFile.CreationTime;
            var lastAccess = videoFile.LastAccessTime;
            var durationFormat = duration.Hours >= 1 ? @"hh\:mm\:ss" : @"mm\:ss";
            var cul = CultureInfo.CurrentCulture;

            StringBuilder infoText = new();
            _ = infoText
                .AppendLine(cul, $"{videoFile.FilePath}")
                .AppendLine(cul, $"Size: {fileSize / (1024 * 1024)} MB")
                .AppendLine(cul, $"Duration: {duration.ToString(durationFormat, cul)}")
                .AppendLine(cul, $"Created: {creation:yyyy-MM-dd HH:mm:ss}")
                .AppendLine(cul, $"Modified: {lastWrite:yyyy-MM-dd HH:mm:ss}")
                .AppendLine(cul, $"Last Access: {lastAccess:yyyy-MM-dd HH:mm:ss}");

            if (codecInfo != null)
            {
                _ = infoText
                    .AppendLine(cul, $"Resolution: {codecInfo.Size.Width} x " +
                        $"{codecInfo.Size.Height} @ {codecInfo.FrameRate}")
                    .AppendLine(cul, $"Codec: {codecInfo.Name}");
            }

            return infoText.ToString().TrimEnd();
        }

        public static VideoFile ToVideoFile(this IVideoFile videoFile) =>
            new()
            {
                FilePath = videoFile.FilePath,
                FileSize = videoFile.FileSize,
                Duration = Duration.FromTimeSpan(videoFile.Duration),
                LastWriteTime = Timestamp.FromDateTime(
                    videoFile.LastWriteTime.ToUniversalTime()),
                CreationTime = Timestamp.FromDateTime(
                    videoFile.CreationTime.ToUniversalTime()),
                LastAccessTime = Timestamp.FromDateTime(
                    videoFile.LastAccessTime.ToUniversalTime()),
                CodecInfo = videoFile.CodecInfo,
            };
    }
}
