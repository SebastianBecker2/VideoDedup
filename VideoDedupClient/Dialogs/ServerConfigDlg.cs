namespace VideoDedupClient.Dialogs
{
    using System;
    using System.Linq;
    using System.Windows.Forms;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using VideoDedupGrpc;
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
                        FolderSettings.ExcludedDirectories.ToArray());
                }

                if (FolderSettings.FileExtensions != null)
                {
                    LsbFileExtensions.Items.AddRange(
                        FolderSettings.FileExtensions.ToArray());
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
                    DurationComparisonSettings.DifferenceType == DurationDifferenceType.Percent;
                RdbDurationDifferencePercent.Checked =
                    DurationComparisonSettings.DifferenceType != DurationDifferenceType.Percent;
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
            using var dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = TxtSourcePath.Text;

            if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            TxtSourcePath.Text = dlg.FileName;
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

        private void RdbDurationDifferenceSeconds_CheckedChanged(object sender, EventArgs e)
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
    }
}
