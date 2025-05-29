namespace VideoDedupClient
{
    using System.Globalization;
    using CustomSelectFileDlg;
    using CustomSelectFileDlg.Exceptions;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.ByteStringExtensions;

    internal static class SelectFileDialog
    {
        public static string? Show(
            string initialPath,
            string title = "Select file dialog",
            RequestedEntryType fileType = RequestedEntryType.Files)
        {
            var serverAddress = Program.Configuration.ServerAddress.ToLower(
                    CultureInfo.InvariantCulture);

            var isFolderSelector = fileType is RequestedEntryType.Folders;

            if (serverAddress is "localhost" or "127.0.0.1")
            {
                return ShowFolderSelectionLocally(
                    initialPath,
                    title,
                    isFolderSelector);
            }
            return ShowFolderSelectionRemote(
                initialPath,
                title,
                isFolderSelector);
        }

        private static string? ShowFolderSelectionLocally(
            string initialPath,
            string title,
            bool isFolderSelector)
        {
            using var dlg = new CommonOpenFileDialog
            {
                Title = title,
                IsFolderPicker = isFolderSelector,
                InitialDirectory = initialPath,
            };

            if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return null;
            }

            return dlg.FileName;
        }

        private static string? ShowFolderSelectionRemote(
            string initialPath,
            string title,
            bool isFolderSelector)
        {
            static Entry ToEntry(
                GetFolderContentResponse.Types.FileAttributes f) =>
                new(f.Name)
                {
                    DateModified = f.DateModified?.ToDateTime(),
                    Size = f.Type == FileType.Folder ? null : f.Size,
                    Type = f.Type == FileType.Folder
                        ? EntryType.Folder
                        : EntryType.File,
                    MimeType = f.MimeType,
                    Icon = f.Icon?.ToImage(),
                };

            using var dlg = new CustomSelectFileDialog
            {
                Text = title,
                IsFolderSelector = isFolderSelector,
                CurrentPath = initialPath,
                ButtonUpEnabled = ButtonUpEnabledWhen.Always,
                EntryIconStyle = IconStyle.FallbackToExtensionSpecificIcons,
            };

            var rootFolderRequest = Program.GrpcClient.GetFolderContent(
                new GetFolderContentRequest
                {
                    Path = "",
                    TypeRestriction = FileType.Folder
                });
            if (!rootFolderRequest.RequestFailed)
            {
                dlg.RootEntries = rootFolderRequest.Files.Select(ToEntry);
            }

            dlg.ContentRequested += (obj, args) =>
            {
                if (args.Path is null)
                {
                    if (obj is not CustomSelectFileDialog dlg)
                    {
                        return;
                    }
                    dlg.CurrentPath = dlg.RootEntries?.FirstOrDefault()?.Name;
                    return;
                }
                var contentRequest = Program.GrpcClient.GetFolderContent(
                    new GetFolderContentRequest
                    {
                        Path = args.Path ?? "",
                        TypeRestriction = isFolderSelector
                            ? FileType.Folder
                            : ToFileType(args.RequestedEntryType),
                    });
                if (contentRequest.RequestFailed)
                {
                    args.Entries = [];
                    throw new InvalidContentRequestException();
                }
                args.Entries = contentRequest.Files.Select(ToEntry);
            };

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return null;
            }

            return dlg.SelectedPath;
        }

        private static FileType ToFileType(
            RequestedEntryType requestedEntryType) =>
            requestedEntryType switch
            {
                RequestedEntryType.Files => FileType.File,
                RequestedEntryType.Folders => FileType.Folder,
                RequestedEntryType.FilesAndFolders => FileType.Any,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(requestedEntryType)),
            };
    }
}
