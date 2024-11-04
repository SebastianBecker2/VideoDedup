namespace VideoDedupClient.Controls.ResolutionSettings
{
    using System.Windows.Forms;
    using VideoDedupGrpc;

    public partial class ResolutionSettingsCtrl : UserControl
    {
        public ResolutionSettingsCtrl() => InitializeComponent();

        public void ShowSettings(ResolutionSettings? resolutionSettings)
        {
            if (resolutionSettings is null)
            {
                return;
            }

            NumThumbnailViewCount.Value = resolutionSettings.ImageCount;
            RdbMoveToTrash.Checked = resolutionSettings.MoveToTrash;
            RdbDeleteFiles.Checked = !resolutionSettings.MoveToTrash;
        }

        public ResolutionSettings GetSettings() =>
            new()
            {
                ImageCount = (int)NumThumbnailViewCount.Value,
                MoveToTrash = RdbMoveToTrash.Checked,
            };
    }
}
