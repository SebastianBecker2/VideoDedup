namespace VideoDedupServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Google.Protobuf;
    using Google.Protobuf.WellKnownTypes;
    using VideoDedupGrpc;
    using VideoDedupServer.Properties;
    using VideoDedupSharedLib;
    using static VideoDedupGrpc.GetFolderContentResponse.Types;

    internal static class FileManager
    {
        private static readonly ByteString DriveIcon =
            ByteString.CopyFrom(Resources.drive);

        public static IEnumerable<FileAttributes> GetFolderContent(
            string path,
            FileType typeRestriction)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return DriveInfo.GetDrives().Select(drive =>
                    new FileAttributes
                    {
                        Name = drive.Name,
                        Type = FileType.Folder,
                        Icon = DriveIcon,
                    });
            }

            if (!Directory.Exists(path))
            {
                // If it's a UNC path to server, without subfolder, we show the
                // shares on that server.
                if (!Uri.TryCreate(path, UriKind.Absolute, out var uri)
                    || !uri.IsUnc
                    || path.Trim('\\').Contains('\\'))
                {
                    throw new FileNotFoundException();
                }

                return GetServerShares(path);
            }

            Func<string, IEnumerable<string>> enumerator =
                typeRestriction switch
                {
                    FileType.Folder => Directory.EnumerateDirectories,
                    FileType.File => Directory.EnumerateFiles,
                    FileType.Any => Directory.EnumerateFileSystemEntries,
                    _ => Directory.EnumerateFileSystemEntries
                };

            // Add a backslash at the end to make sure we never try to get
            // the content of the current drive without a backslash or slash
            // at the end. If the current working directory is "d:\subfolder"
            // and we try to get the file system entries from "d:", we would
            // get the file system entries from "d:\subfolder" instead.
            // Adding the backslash prevents that.
            return enumerator(path + "\\")
                .Select(ToFileAttributes)
                .Where(e => e is not null)!;
        }

        private static FileAttributes? ToFileAttributes(string path)
        {
            try
            {
                var attr = File.GetAttributes(path);
                var type = attr.HasFlag(System.IO.FileAttributes.Directory)
                    ? FileType.Folder
                    : FileType.File;
                var info = new FileInfo(path);
                var size = type == FileType.Folder ? 0 : info.Length;
                var mimeType = type == FileType.Folder
                    ? "File folder"
                    : FileInfoProvider.GetMimeType(path) ?? "";
                var dateModified =
                    Timestamp.FromDateTime(info.LastWriteTimeUtc);
                return new()
                {
                    Name = Path.GetFileName(path),
                    Size = size,
                    Type = type,
                    DateModified = dateModified,
                    MimeType = mimeType,
                    Icon = null,
                };
            }
            catch
            {
                return null;
            }
        }

        private static IEnumerable<FileAttributes> GetServerShares(
            string serverName) =>
            new Vanara.SharedDevices(serverName)
                .Where(kvp => !kvp.Value.IsSpecial && kvp.Value.IsDiskVolume)
                .Select(kvp =>
                    new FileAttributes
                    {
                        Name = kvp.Key,
                        Type = FileType.Folder
                    });

    }
}
