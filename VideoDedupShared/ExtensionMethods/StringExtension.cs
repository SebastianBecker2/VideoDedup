namespace VideoDedupShared.StringExtension
{
    using System;
    using System.IO;

    public static class StringExtension
    {
        public static string MakeRelativePath(this string absolutPath, string basePath)
        {
            var fullPath = new Uri(absolutPath, UriKind.Absolute);

            if (!basePath.EndsWith(Path.DirectorySeparatorChar.ToString())
                && !basePath.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                basePath += Path.DirectorySeparatorChar;
            }
            var relRoot = new Uri(basePath, UriKind.Absolute);

            var relPath = relRoot.MakeRelativeUri(fullPath).ToString();
            return Uri.UnescapeDataString(relPath);
        }
    }
}
