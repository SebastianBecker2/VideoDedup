namespace VideoDedupSharedLib
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Runtime.InteropServices;

    // https://stackoverflow.com/a/1437451/2347040
#pragma warning disable IDE0079 // Remove unnecessary suppression
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
    public static class FileInfoProvider
    {
        [StructLayout(LayoutKind.Sequential)]
        private readonly struct SHFILEINFO
        {
            private readonly IntPtr hIcon;
            private readonly int iIcon;
            private readonly uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            private readonly string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
#pragma warning disable IDE1006 // Naming Styles
            public readonly string szTypeName;
#pragma warning restore IDE1006 // Naming Styles
        };

        private static class FILE_ATTRIBUTE
        {
            public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        }

        private static class SHGFI
        {
            public const uint SHGFI_TYPENAME = 0x000000400;
            public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
        }

#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments
        [DllImport(
            "shell32.dll",
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            CallingConvention = CallingConvention.Cdecl)]
#pragma warning restore CA2101 // Specify marshaling for P/Invoke string arguments
        private static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            out SHFILEINFO psfi,
            uint cbSizeFileInfo,
            uint uFlags);

        public static string? GetMimeType(string file)
        {
            try
            {
                if (SHGetFileInfo(
                        file,
                        FILE_ATTRIBUTE.FILE_ATTRIBUTE_NORMAL,
                        out var info,
                        (uint)Marshal.SizeOf<SHFILEINFO>(),
                        SHGFI.SHGFI_TYPENAME | SHGFI.SHGFI_USEFILEATTRIBUTES) !=
                    IntPtr.Zero)
                {
                    return info.szTypeName;
                }
            }
            catch (ArgumentException) { }

            return null;
        }

        private static readonly ConcurrentDictionary<string, Bitmap?>
            IconCache = new();

        public static Bitmap? GetIcon(string file)
        {
            var extension = Path.GetExtension(file);
            if (IconCache.TryGetValue(extension, out var icon))
            {
                return icon;
            }
            icon = Icon.ExtractAssociatedIcon(file)?.ToBitmap();
            _ = IconCache.TryAdd(extension, icon);
            return icon;
        }
    }
}
