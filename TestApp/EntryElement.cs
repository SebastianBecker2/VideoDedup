namespace TestApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CustomSelectFileDlg;
    using VideoDedupSharedLib;

    internal class EntryElement
    {
        public EntryElement(string path)
        {
            Entry = EntryFromFile(path);

            if (!Directory.Exists(path))
            {
                return;
            }

            try
            {
                SubEntries = Directory.GetFileSystemEntries(path)
                    .Select(p => new EntryElement(p))
                    .ToList();
            }
            catch (IOException)
            {
                SubEntries = new List<EntryElement>();
            }
            catch (UnauthorizedAccessException)
            {
                SubEntries = new List<EntryElement>();
            }
        }

        public Entry Entry { get; set; }
        public List<EntryElement>? SubEntries { get; set; }

        public static Entry EntryFromFile(string entry)
        {
            var attr = File.GetAttributes(entry);
            var info = new FileInfo(entry);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                return new Entry(entry.Length <= 3 ? entry : Path.GetFileName(entry))
                {
                    Size = null,
                    DateModified = info.LastWriteTimeUtc,
                    MimeType = "File folder",
                    Type = EntryType.Folder,
                };
            }

            return new Entry(Path.GetFileName(entry))
            {
                Icon = FileInfoProvider.GetIcon(entry),
                Size = info.Length,
                DateModified = info.LastWriteTimeUtc,
                MimeType = FileInfoProvider.GetMimeType(entry),
                Type = EntryType.File,
            };
        }

        public EntryElement? FindEntryElement(string path)
        {
            if (!path.StartsWith(
                    Entry.Name,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            path = path[Entry.Name.Length..];
            while (path.StartsWith(Path.DirectorySeparatorChar)
                   || path.StartsWith(Path.AltDirectorySeparatorChar))
            {
                path = path[1..];
            }

            var sepIndex = path.IndexOfAny(new[]
            {
                    Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar
                });
            if (sepIndex == -1)
            {
                if (string.IsNullOrEmpty(path))
                {
                    return this;
                }
                return SubEntries!.FirstOrDefault(e => e.Entry.Name == path);
            }

            return SubEntries!
                .FirstOrDefault(e => e.Entry.Name == path[0..sepIndex])
                ?.FindEntryElement(path);
        }
    }
}
