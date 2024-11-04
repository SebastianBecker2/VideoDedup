namespace VideoDedupClient.Controls.FolderSettings
{
    using System;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Forms;
    using CustomSelectFileDlg;
    using CustomSelectFileDlg.Exceptions;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.ByteStringExtensions;

    public partial class FolderSettingsCtrl : UserControl
    {
        public FolderSettingsCtrl() => InitializeComponent();

        public void ShowSettings(FolderSettings? folderSettings)
        {
            if (folderSettings is null)
            {
                return;
            }

            TxtSourcePath.Text = folderSettings.BasePath;
            ChbRecursive.Checked = folderSettings.Recursive;
            ChbMonitorFileChanges.Checked =
                folderSettings.MonitorChanges;

            if (folderSettings.ExcludedDirectories != null)
            {
                LsbExcludedDirectories.Items.AddRange(
                    [.. folderSettings.ExcludedDirectories]);
            }

            if (folderSettings.FileExtensions != null)
            {
                LsbFileExtensions.Items.AddRange(
                    [.. folderSettings.FileExtensions]);
            }
        }

        public FolderSettings GetSettings()
        {
            var folderSettings = new FolderSettings()
            {
                BasePath = TxtSourcePath.Text,
                Recursive = ChbRecursive.Checked,
                MonitorChanges = ChbMonitorFileChanges.Checked,
            };

            folderSettings.ExcludedDirectories.Clear();
            folderSettings.ExcludedDirectories.AddRange(
                LsbExcludedDirectories.Items.Cast<string>());

            folderSettings.FileExtensions.Clear();
            folderSettings.FileExtensions.AddRange(
                LsbFileExtensions.Items.Cast<string>().ToList());

            return folderSettings;
        }

        private void BtnSelectSourcePath_Click(object sender, EventArgs e)
        {
            static string? ShowFolderSelection(string initialFolder)
            {
                var serverAddress = Program.Configuration.ServerAddress.ToLower(
                    CultureInfo.InvariantCulture);

                if (serverAddress is "localhost" or "127.0.0.1")
                {
                    return ShowFolderSelectionLocally(initialFolder);
                }

                return ShowFolderSelectionRemote(initialFolder);
            }

            var newPath = ShowFolderSelection(TxtSourcePath.Text);
            if (newPath is null)
            {
                return;
            }

            TxtSourcePath.Text = newPath;
        }

        private void BtnAddExcludedDirectory_Click(object sender, EventArgs e)
        {
            using var dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true;

            if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            _ = LsbExcludedDirectories.Items.Add(dlg.FileName);
        }

        private void BtnRemoveExcludedDirectory_Click(object sender, EventArgs e)
        {
            foreach (var s in LsbExcludedDirectories
                         .SelectedItems
                         .OfType<string>()
                         .ToList())
            {
                LsbExcludedDirectories.Items.Remove(s);
            }
        }

        private void BtnAddFileExtension_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFileExtension.Text))
            {
                return;
            }

            _ = LsbFileExtensions.Items.Add(TxtFileExtension.Text);
        }

        private void BtnRemoveFileExtension_Click(object sender, EventArgs e)
        {
            foreach (var s in LsbFileExtensions
                         .SelectedItems
                         .OfType<string>()
                         .ToList())
            {
                LsbFileExtensions.Items.Remove(s);
            }
        }
        private static string? ShowFolderSelectionLocally(string initialPath)
        {
            using var dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = initialPath;

            if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return null;
            }

            return dlg.FileName;
        }

        private static string? ShowFolderSelectionRemote(string initialPath)
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

            using var dlg = new CustomSelectFileDialog();
            dlg.IsFolderSelector = true;
            dlg.CurrentPath = initialPath;
            dlg.ButtonUpEnabled = ButtonUpEnabledWhen.Always;
            dlg.EntryIconStyle = IconStyle.FallbackToExtensionSpecificIcons;

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

            dlg.ContentRequested += (_, args) =>
            {
                var contentRequest = Program.GrpcClient.GetFolderContent(
                    new GetFolderContentRequest
                    {
                        Path = args.Path,
                        TypeRestriction = FileType.Folder
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
    }
}
