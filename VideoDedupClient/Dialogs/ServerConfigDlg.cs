namespace VideoDedupClient.Dialogs
{
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
        private LogSettings? LogSettings => ConfigurationSettings?.LogSettings;

        public ConfigurationSettings? ConfigurationSettings { get; set; }

        public ServerConfigDlg() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            if (FolderSettings is not null)
            {
                VicVideoInput.TxtSourcePath.Text = FolderSettings.BasePath;
                VicVideoInput.ChbRecursive.Checked = FolderSettings.Recursive;
                VicVideoInput.ChbMonitorFileChanges.Checked =
                    FolderSettings.MonitorChanges;

                if (FolderSettings.ExcludedDirectories != null)
                {
                    VicVideoInput.LsbExcludedDirectories.Items.AddRange(
                        [.. FolderSettings.ExcludedDirectories]);
                }

                if (FolderSettings.FileExtensions != null)
                {
                    VicVideoInput.LsbFileExtensions.Items.AddRange(
                        [.. FolderSettings.FileExtensions]);
                }
            }

            if (VideoComparisonSettings is not null)
            {
                CscComparisonSettings.NumMaxImageComparison.Value =
                    VideoComparisonSettings.CompareCount;
                CscComparisonSettings.NumMaxDifferentImages.Value =
                    VideoComparisonSettings.MaxDifferentImages;
                CscComparisonSettings.NumMaxDifferentPercentage.Value =
                    VideoComparisonSettings.MaxDifference;
            }

            if (DurationComparisonSettings is not null)
            {
                CscComparisonSettings.RdbDurationDifferencePercent.Checked =
                    DurationComparisonSettings.DifferenceType
                    == DurationDifferenceType.Percent;
                CscComparisonSettings.RdbDurationDifferenceSeconds.Checked =
                    DurationComparisonSettings.DifferenceType
                    == DurationDifferenceType.Seconds;
                CscComparisonSettings.NumMaxDurationDifference.Value =
                    DurationComparisonSettings.MaxDifference;
            }

            if (ThumbnailSettings is not null)
            {
                RscResolutionSettings.NumThumbnailViewCount.Value =
                    ThumbnailSettings.ImageCount;
            }

            if (LogSettings is not null)
            {
                LscLogSettings.CmbVideoDedupServiceLogLevel.SelectedIndex =
                    (int)LogSettings.VideoDedupServiceLogLevel;
                LscLogSettings.CmbComparisonManagerLogLevel.SelectedIndex =
                    (int)LogSettings.ComparisonManagerLogLevel;
                LscLogSettings.CmbDedupEngineLogLevel.SelectedIndex =
                    (int)LogSettings.DedupEngineLogLevel;
            }

            base.OnLoad(e);
        }

        private void BtnOkay_Click(object sender, EventArgs e)
        {
            ConfigurationSettings ??= new ConfigurationSettings();
            FolderSettings!.BasePath = VicVideoInput.TxtSourcePath.Text;
            FolderSettings!.Recursive = VicVideoInput.ChbRecursive.Checked;
            FolderSettings!.MonitorChanges =
                VicVideoInput.ChbMonitorFileChanges.Checked;

            FolderSettings!.ExcludedDirectories.Clear();
            FolderSettings!.ExcludedDirectories.AddRange(
                VicVideoInput.LsbExcludedDirectories.Items.Cast<string>());

            FolderSettings!.FileExtensions.Clear();
            FolderSettings!.FileExtensions.AddRange(
                VicVideoInput.LsbFileExtensions.Items.Cast<string>().ToList());

            VideoComparisonSettings!.CompareCount =
                (int)CscComparisonSettings.NumMaxImageComparison.Value;
            VideoComparisonSettings!.MaxDifferentImages =
                (int)CscComparisonSettings.NumMaxDifferentImages.Value;
            VideoComparisonSettings!.MaxDifference =
                (int)CscComparisonSettings.NumMaxDifferentPercentage.Value;

            if (CscComparisonSettings.RdbDurationDifferencePercent.Checked)
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
                (int)CscComparisonSettings.NumMaxDurationDifference.Value;

            ThumbnailSettings!.ImageCount =
                (int)RscResolutionSettings.NumThumbnailViewCount.Value;

            LogSettings!.VideoDedupServiceLogLevel =
                (LogSettings.Types.LogLevel)
                    LscLogSettings.CmbVideoDedupServiceLogLevel.SelectedIndex;
            LogSettings!.ComparisonManagerLogLevel =
                (LogSettings.Types.LogLevel)
                    LscLogSettings.CmbComparisonManagerLogLevel.SelectedIndex;
            LogSettings!.DedupEngineLogLevel =
                (LogSettings.Types.LogLevel)
                    LscLogSettings.CmbDedupEngineLogLevel.SelectedIndex;

            DialogResult = DialogResult.OK;
        }

        private void CscComparisonSettings_TryComparisonClick(
            object sender,
            EventArgs e)
        {
            using var dlg = new CustomVideoComparisonDlg();
            dlg.VideoComparisonSettings = new VideoComparisonSettings
            {
                CompareCount =
                    (int)CscComparisonSettings.NumMaxImageComparison.Value,
                MaxDifferentImages =
                    (int)CscComparisonSettings.NumMaxDifferentImages.Value,
                MaxDifference =
                    (int)CscComparisonSettings.NumMaxDifferentPercentage.Value,
            };

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            CscComparisonSettings.NumMaxImageComparison.Value =
                dlg.VideoComparisonSettings.CompareCount;
            CscComparisonSettings.NumMaxDifferentImages.Value =
                dlg.VideoComparisonSettings.MaxDifferentImages;
            CscComparisonSettings.NumMaxDifferentPercentage.Value =
                dlg.VideoComparisonSettings.MaxDifference;
        }
    }
}
