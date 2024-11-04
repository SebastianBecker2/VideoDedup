namespace VideoDedupClient.Dialogs
{
    using VideoDedupGrpc;

    public partial class ServerConfigDlg : Form
    {
        private FolderSettings? FolderSettings =>
            ConfigurationSettings?.FolderSettings;
        private DurationComparisonSettings? DurationComparisonSettings =>
            ConfigurationSettings?.DurationComparisonSettings;
        private VideoComparisonSettings? VideoComparisonSettings =>
            ConfigurationSettings?.VideoComparisonSettings;
        private LogSettings? LogSettings => ConfigurationSettings?.LogSettings;
        private ResolutionSettings? ResolutionSettings =>
            ConfigurationSettings?.ResolutionSettings;

        public ConfigurationSettings? ConfigurationSettings { get; set; }

        public ServerConfigDlg() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            FscFolderSettings.ShowSettings(FolderSettings);

            CscComparisonSettings.ShowSettings(
                VideoComparisonSettings,
                DurationComparisonSettings);

            LscLogSettings.ShowSettings(LogSettings);

            RscResolutionSettings.ShowSettings(ResolutionSettings);

            base.OnLoad(e);
        }

        private void BtnOkay_Click(object sender, EventArgs e)
        {
            ConfigurationSettings ??= new ConfigurationSettings();

            ConfigurationSettings.FolderSettings =
                FscFolderSettings.GetSettings();

            ConfigurationSettings.VideoComparisonSettings =
                CscComparisonSettings.GetVideoComparisonSettings();
            ConfigurationSettings.DurationComparisonSettings =
                CscComparisonSettings.GetDurationComparisonSettings();

            ConfigurationSettings.ResolutionSettings =
                RscResolutionSettings.GetSettings();

            ConfigurationSettings.LogSettings = LscLogSettings.GetSettings();

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
