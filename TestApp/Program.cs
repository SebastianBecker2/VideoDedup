
using System.Diagnostics;
using System.Windows.Forms;
using CustomSelectFileDlg;
using CustomSelectFileDlg.EventArgs;
using CustomSelectFileDlg.Exceptions;
using TestApp;
using VideoDedupSharedLib;
//using Entry = CustomSelectFileDlg.Entry;

var rootPath = @"F:\";
EntryElement? rootElement = new(rootPath);

CustomSelectFileDialog selectFileDialog = new()
{
    CurrentPath = @"F:\_Test\Test6\Test5\Test4\Test3\Test2\Test1\Test1\Test2\Test3\Test4\Test5\Test6",
    EntryIconStyle = IconStyle.FallbackToSimpleIcons,
    IsFolderSelector = true,
    ButtonUpEnabled = ButtonUpEnabledWhen.Always,
    RootFolders = new[]
    {
        new Entry("C:"), new Entry("D:"), new Entry("E:"), new Entry("F:")
    },
};
selectFileDialog.ContentRequested += HandleContentRequest;

Entry ToEntry(string path)
{
    var attr = File.GetAttributes(path);
    var info = new FileInfo(path);

    if (attr.HasFlag(FileAttributes.Directory))
    {
        return new Entry(path.Length <= 3 ? path : Path.GetFileName(path))
        {
            Size = null,
            DateModified = info.LastWriteTimeUtc,
            MimeType = "File folder",
            Type = EntryType.Folder,
        };
    }

    return new Entry(Path.GetFileName(path))
    {
        Icon = FileInfoProvider.GetIcon(path),
        Size = info.Length,
        DateModified = info.LastWriteTimeUtc,
        MimeType = FileInfoProvider.GetMimeType(path),
        Type = EntryType.File,
    };
}

IEnumerable<Entry> GetShares(string server) =>
    new Vanara.SharedDevices(server)
        .Where(kvp => !kvp.Value.IsSpecial && kvp.Value.IsDiskVolume)
        .Select(kvp => new Entry(kvp.Key) { Type = EntryType.Folder });

IEnumerable<Entry> GetFolderContent(
    string path,
    RequestedEntryType requestedEntryType)
{
    if (!Directory.Exists(path))
    {
        if (!Uri.TryCreate(path, UriKind.Absolute, out var uri)
            || !uri.IsUnc
            || path.Trim('\\').Contains('\\'))
        {
            throw new FileNotFoundException();
        }

        // If it's a UNC path to server, without subfolder, we show the
        // shares on that server.
        return GetShares(path);
    }

    // Add a backslash at the end to make sure we never try to get
    // the content of the current drive without a backslash or slash
    // at the end. If the current working directory is "d:\subfolder"
    // and we try to get the file system entries from "d:", we would
    // get the file system entries from "d:\subfolder" instead.
    // Adding the backslash prevents that.
    path += "\\";

    if (requestedEntryType == RequestedEntryType.Folders)
    {
        return Directory.EnumerateDirectories(path).Select(ToEntry);
    }
    else if (requestedEntryType == RequestedEntryType.Files)
    {
        return Directory.EnumerateFiles(path).Select(ToEntry);
    }
    return Directory.EnumerateFileSystemEntries(path).Select(ToEntry);
}

void HandleContentRequest(object? sender, ContentRequestedEventArgs e)
{
    Debug.Print("Content Requested");
    if (string.IsNullOrWhiteSpace(e.Path))
    {
        Debug.Print($"Path is null or white space, setting it back to {rootPath}");
        selectFileDialog.CurrentPath = rootPath;
        return;
    }

    var entryElement = rootElement.FindEntryElement(e.Path);

    if (entryElement == null)
    {
        try
        {
            e.Entries = GetFolderContent(e.Path, e.RequestedEntryType);
            return;
        }
        catch (FileNotFoundException)
        {
            e.Entries = Array.Empty<Entry>();
            throw new InvalidContentRequestException();
        }
    }

    if (e.Path == rootPath + @"_Test\Test3")
    {
        var testElement = rootElement.FindEntryElement(rootPath + @"_Test");
        _ = testElement!.SubEntries!.Remove(entryElement);
    }

    e.Entries = (entryElement?.SubEntries ?? new List<EntryElement>())
        .Where(se => se.Entry.Type.Matches(e.RequestedEntryType))
        .Select(se => se.Entry);
}


foreach (var i in Enumerable.Range(0, 100))
{
    if (selectFileDialog.ShowDialog() == DialogResult.OK)
    {
        _ = MessageBox.Show(selectFileDialog.SelectedPath);
    }
}
