namespace VideoDedupShared.IVideoFileExtension
{
    using System;
    using System.IO;
    using System.Threading;

    public static class IVideoFileExtension
    {

        /// <summary>
        /// Special case for FileSystemWatcher change events.
        /// When a large file has been written, the file might
        /// not be accessable yet when the event comes in.
        /// If we would try to read the duration right away
        /// it would fail.
        /// So we wait for read access to the file.
        /// For a maximum of 1 second.
        /// </summary>
        /// <param name="timeout">For how long should we try
        /// to access the file?</param>
        /// <param name="interval">How long should we wait
        /// between each try?</param>
        /// <returns>
        /// true: File could be access.
        /// false: File could not be access in time.
        /// </returns>
        public static bool WaitForFileAccess(this IVideoFile videoFile, int timeout = 1000, int interval = 50) =>
            videoFile.WaitForFileAccess(timeout, interval, null);

        /// <summary>
        /// Special case for FileSystemWatcher change events.
        /// When a large file has been written, the file might
        /// not be accessable yet when the event comes in.
        /// If we would try to read the duration right away
        /// it would fail.
        /// So we wait for read access to the file.
        /// For a maximum of 1 second.
        /// </summary>
        /// <param name="cancelToken">A token to be able to
        /// cancel the wait.</param>
        /// <param name="timeout">For how long should we try
        /// to access the file?</param>
        /// <param name="interval">How long should we wait
        /// between each try?</param>
        /// <returns>
        /// true: File could be access.
        /// false: File could not be access in time.
        /// </returns>
        public static bool WaitForFileAccess(this IVideoFile videoFile, CancellationToken cancelToken,
            int timeout = 1000,
            int interval = 50) =>
            videoFile.WaitForFileAccess(timeout, interval, cancelToken);

        private static bool WaitForFileAccess(this IVideoFile videoFile, int timeout,
            int interval,
            CancellationToken? cancelToken = null)
        {
            for (var i = 0; i < (timeout / interval); i++)
            {
                try
                {
                    using (var stream = File.Open(videoFile.FilePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite))
                    {
                        if (stream != null)
                        {
                            return true;
                        }
                    }
                }
                catch (ArgumentOutOfRangeException) { throw; }
                catch (ArgumentNullException) { return false; }
                catch (ArgumentException) { return false; }
                catch (PathTooLongException) { return false; }
                catch (DirectoryNotFoundException) { return false; }
                catch (NotSupportedException) { return false; }
                catch (Exception) { }

                if (cancelToken.HasValue &&
                    cancelToken.Value.IsCancellationRequested)
                {
                    return false;
                }
                Thread.Sleep(interval);
            }
            return false;
        }

    }
}
