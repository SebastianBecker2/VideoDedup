namespace VideoDedupClient.Dialogs
{
    using System.Globalization;
    using CustomSelectFileDlg;
    using CustomSelectFileDlg.Exceptions;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.ByteStringExtensions;
    using static VideoDedupGrpc.DurationComparisonSettings.Types;

    public partial class ServerConfigDlg : Form
    {
        private FolderSettings? FolderSettings =>
            ConfigurationSettings?.FolderSettings;
        private DurationComparisonSettings? DurationComparisonSettings =>
            ConfigurationSettings?.DurationComparisonSettings;
        private VideoComparisonSettings? VideoComparisonSettings =>
            ConfigurationSettings?.VideoComparisonSettings;
        private ThumbnailSettings? ThumbnailSettings =>
            ConfigurationSettings?.ThumbnailSettings;

        public ConfigurationSettings? ConfigurationSettings { get; set; }

        public ServerConfigDlg() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            if (FolderSettings is not null)
            {
                TxtSourcePath.Text = FolderSettings.BasePath;
                ChbRecursive.Checked = FolderSettings.Recursive;
                ChbMonitorFileChanges.Checked = FolderSettings.MonitorChanges;

                if (FolderSettings.ExcludedDirectories != null)
                {
                    LsbExcludedDirectories.Items.AddRange(
                        FolderSettings.ExcludedDirectories.ToArray<object>());
                }

                if (FolderSettings.FileExtensions != null)
                {
                    LsbFileExtensions.Items.AddRange(
                        FolderSettings.FileExtensions.ToArray<object>());
                }
            }

            if (VideoComparisonSettings is not null)
            {
                NumMaxImageComparison.Value =
                    VideoComparisonSettings.CompareCount;
                NumMaxDifferentImages.Value =
                    VideoComparisonSettings.MaxDifferentImages;
                NumMaxDifferentPercentage.Value =
                    VideoComparisonSettings.MaxDifference;
            }

            if (DurationComparisonSettings is not null)
            {
                RdbDurationDifferencePercent.Checked =
                    DurationComparisonSettings.DifferenceType
                    == DurationDifferenceType.Percent;
                RdbDurationDifferenceSeconds.Checked =
                    DurationComparisonSettings.DifferenceType
                    == DurationDifferenceType.Seconds;
                NumMaxDurationDifference.Value =
                    DurationComparisonSettings.MaxDifference;
            }

            if (ThumbnailSettings is not null)
            {
                NumThumbnailViewCount.Value = ThumbnailSettings.ImageCount;
            }

            base.OnLoad(e);
        }

        private void BtnOkay_Click(object sender, EventArgs e)
        {
            ConfigurationSettings ??= new ConfigurationSettings();
            FolderSettings!.BasePath = TxtSourcePath.Text;
            FolderSettings!.Recursive = ChbRecursive.Checked;
            FolderSettings!.MonitorChanges = ChbMonitorFileChanges.Checked;

            FolderSettings!.ExcludedDirectories.Clear();
            FolderSettings!.ExcludedDirectories.AddRange(
                LsbExcludedDirectories.Items.Cast<string>());

            FolderSettings!.FileExtensions.Clear();
            FolderSettings!.FileExtensions.AddRange(
                LsbFileExtensions.Items.Cast<string>().ToList());

            VideoComparisonSettings!.CompareCount =
                (int)NumMaxImageComparison.Value;
            VideoComparisonSettings!.MaxDifferentImages =
                (int)NumMaxDifferentImages.Value;
            VideoComparisonSettings!.MaxDifference =
                (int)NumMaxDifferentPercentage.Value;

            if (RdbDurationDifferencePercent.Checked)
            {
                DurationComparisonSettings!.DifferenceType =
                    DurationDifferenceType.Percent;
            }
            else
            {
                DurationComparisonSettings!.DifferenceType =
                    DurationDifferenceType.Seconds;
            }
            DurationComparisonSettings!.MaxDifference =
                (int)NumMaxDurationDifference.Value;

            ThumbnailSettings!.ImageCount = (int)NumThumbnailViewCount.Value;

            DialogResult = DialogResult.OK;
        }

        private void BtnSelectSourcePath_Click(object sender, EventArgs e)
        {
            static string? ShowFolderSelection(string initialFolder)
            {
                var serverAddress = Program.Configuration.ServerAddress.ToLower(
                    CultureInfo.InvariantCulture);
#if DEBUG
                return ShowFolderSelectionRemote(initialFolder);
#endif
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

        private void BtnCustomVideoComparison_Click(object sender, EventArgs e)
        {
            using var dlg = new CustomVideoComparisonDlg();
            dlg.VideoComparisonSettings = new VideoComparisonSettings
            {
                CompareCount = (int)NumMaxImageComparison.Value,
                MaxDifferentImages = (int)NumMaxDifferentImages.Value,
                MaxDifference = (int)NumMaxDifferentPercentage.Value,
            };

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            NumMaxImageComparison.Value = dlg.VideoComparisonSettings.CompareCount;
            NumMaxDifferentImages.Value =
                dlg.VideoComparisonSettings.MaxDifferentImages;
            NumMaxDifferentPercentage.Value =
                dlg.VideoComparisonSettings.MaxDifference;
        }

        private void HandleDurationDifferenceTypeChanged(
            object sender,
            EventArgs e)
        {
            if (RdbDurationDifferenceSeconds.Checked)
            {
                LblMaxDurationDifferenceUnit.Text = "Seconds";
            }
            else
            {
                LblMaxDurationDifferenceUnit.Text = "Percent";
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
                    args.Entries = Array.Empty<Entry>();
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
