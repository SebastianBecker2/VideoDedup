namespace SetupBootstrapperUI
{
    using System;
    using System.IO;

    /// <summary>
    /// Copies client certificates to a machine-local path before MSI deferred custom actions run.
    /// Deferred actions run as SYSTEM and cannot read UNC or user-only network paths.
    /// </summary>
    internal static class ClientCertInstallStaging
    {
        private const string StagingFolderName = "ClientCertInstall";
        private const string StagedFileName = "VideoDedup.crt";

        public static string PrepareForMsi(string sourcePath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                return string.Empty;
            }

            sourcePath = sourcePath.Trim();
            if (!File.Exists(sourcePath))
            {
                return sourcePath;
            }

            if (!RequiresStaging(sourcePath))
            {
                return sourcePath;
            }

            var stagingDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "VideoDedup",
                StagingFolderName);
            _ = Directory.CreateDirectory(stagingDir);
            var destPath = Path.Combine(stagingDir, StagedFileName);
            File.Copy(sourcePath, destPath, overwrite: true);
            return destPath;
        }

        private static bool RequiresStaging(string path)
        {
            if (path.StartsWith(@"\\", StringComparison.Ordinal))
            {
                return true;
            }

            try
            {
                var root = Path.GetPathRoot(path);
                if (string.IsNullOrEmpty(root) || root.Length < 3 || root[1] != ':')
                {
                    return false;
                }

                var drive = new DriveInfo(root);
                return drive.DriveType == DriveType.Network;
            }
            catch (IOException)
            {
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return true;
            }
        }
    }
}
