namespace VideoDedupClient.Shell
{
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Opens File Explorer with a file selected. Uses the shell API instead of
    /// <c>explorer.exe /select</c>, which is unreliable for UNC paths and mixed separators.
    /// </summary>
    internal static class ShellExplorer
    {
        public static bool TryShowInExplorer(string filePath)
        {
            var normalizedPath = NormalizeExistingFilePath(filePath);
            if (normalizedPath is null)
            {
                return false;
            }

            var pidl = ILCreateFromPathW(normalizedPath);
            if (pidl == IntPtr.Zero)
            {
                return TryOpenContainingFolder(normalizedPath);
            }

            try
            {
                if (SHOpenFolderAndSelectItems(pidl, 0, null, 0) == 0)
                {
                    return true;
                }

                return TryOpenContainingFolder(normalizedPath);
            }
            finally
            {
                ILFree(pidl);
            }
        }

        private static string? NormalizeExistingFilePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            path = path.Trim().Replace('/', '\\');

            try
            {
                return Path.GetFullPath(path);
            }
            catch (Exception) when (File.Exists(path))
            {
                return path;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool TryOpenContainingFolder(string filePath)
        {
            var folder = Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
            {
                return false;
            }

            _ = Process.Start(new ProcessStartInfo
            {
                FileName = folder,
                UseShellExecute = true,
            });

            return true;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr ILCreateFromPathW(string path);

        [DllImport("shell32.dll")]
        private static extern void ILFree(IntPtr pidl);

        [DllImport("shell32.dll")]
        private static extern int SHOpenFolderAndSelectItems(
            IntPtr pidlFolder,
            uint cidl,
            IntPtr[]? apidl,
            uint dwFlags);
    }
}
