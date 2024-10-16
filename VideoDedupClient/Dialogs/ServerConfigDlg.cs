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

        public ServerConfigDlg()
        {
            InitializeComponent();

            var logLevel = Enum.GetNames<LogSettings.Types.LogLevel>();
            CmbVideoDedupServiceLogLevel.Items.AddRange(logLevel);
            CmbComparisonManagerLogLevel.Items.AddRange(logLevel);
            CmbDedupEngineLogLevel.Items.AddRange(logLevel);
        }

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

            if (LogSettings is not null)
            {
                CmbVideoDedupServiceLogLevel.SelectedIndex =
                    (int)LogSettings.VideoDedupServiceLogLevel;
                CmbComparisonManagerLogLevel.SelectedIndex =
                    (int)LogSettings.ComparisonManagerLogLevel;
                CmbDedupEngineLogLevel.SelectedIndex =
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

            LogSettings!.VideoDedupServiceLogLevel =
                (LogSettings.Types.LogLevel)
                    CmbVideoDedupServiceLogLevel.SelectedIndex;
            LogSettings!.ComparisonManagerLogLevel =
                (LogSettings.Types.LogLevel)
                    CmbComparisonManagerLogLevel.SelectedIndex;
            LogSettings!.DedupEngineLogLevel =
                (LogSettings.Types.LogLevel)
                    CmbDedupEngineLogLevel.SelectedIndex;

            DialogResult = DialogResult.OK;
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
    }
}
