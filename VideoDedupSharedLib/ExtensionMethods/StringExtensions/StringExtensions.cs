namespace VideoDedupSharedLib.ExtensionMethods.StringExtensions
{
    public static class StringExtensions
    {
        public static string MakeRelativePath(
            this string absolutePath,
            string basePath)
        {
            var fullPath = new Uri(absolutePath, UriKind.Absolute);

            if (!basePath.EndsWith(
                    Path.DirectorySeparatorChar.ToString(),
                    StringComparison.InvariantCulture)
                && !basePath.EndsWith(
                    Path.AltDirectorySeparatorChar.ToString(),
                    StringComparison.InvariantCulture))
            {
                basePath += Path.DirectorySeparatorChar;
            }
            var relRoot = new Uri(basePath, UriKind.Absolute);

            var relPath = relRoot.MakeRelativeUri(fullPath).ToString();
            return Uri.UnescapeDataString(relPath);
        }
    }
}
