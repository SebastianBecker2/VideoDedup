namespace VideoDedupSharedLib.ExtensionMethods.StringExtensions
{
    public static class StringExtensions
    {
        public static string MakeRelativePath(
            this string absolutePath,
            string basePath)
        {
            var fullPath = NormalizePathSeparators(absolutePath);
            var rootPath = NormalizePathSeparators(basePath).TrimEnd('/');

            var comparison = UsesWindowsPathSemantics(rootPath)
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;

            if (!fullPath.StartsWith(rootPath, comparison)
                || (fullPath.Length > rootPath.Length
                    && fullPath[rootPath.Length] != '/'))
            {
                throw new ArgumentException(
                    $"Path '{absolutePath}' is not under base '{basePath}'.",
                    nameof(absolutePath));
            }

            if (fullPath.Length == rootPath.Length)
            {
                return string.Empty;
            }

            return fullPath[(rootPath.Length + 1)..];
        }

        private static string NormalizePathSeparators(string path) =>
            path.Replace('\\', '/').TrimEnd('/');

        private static bool UsesWindowsPathSemantics(string normalizedPath) =>
            normalizedPath.Length >= 2 && normalizedPath[1] == ':'
            || normalizedPath.StartsWith("//", StringComparison.Ordinal);
    }
}
