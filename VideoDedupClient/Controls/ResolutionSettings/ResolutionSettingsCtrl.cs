namespace VideoDedupClient.Controls.ResolutionSettings
{
    using System.Windows.Forms;
    using VideoDedupGrpc;

    public partial class ResolutionSettingsCtrl : UserControl
    {
        public ResolutionSettingsCtrl()
        {
            InitializeComponent();

            var text = "Deleted files will be moved to the subfolder " +
                "'VideoDedupTrash' in the Source Directory.";
            TipHints.SetToolTip(PibMoveToTrashHint, text);
        }

        public void ShowSettings(ResolutionSettings? resolutionSettings)
        {
            if (resolutionSettings is null)
            {
                return;
            }

            NumThumbnailViewCount.Value = resolutionSettings.ThumbnailCount;
            RdbMoveToTrash.Checked = resolutionSettings.MoveToTrash;
            RdbDeleteFiles.Checked = !resolutionSettings.MoveToTrash;
        }

        public ResolutionSettings GetSettings() =>
            new()
            {
                ThumbnailCount = (int)NumThumbnailViewCount.Value,
                MoveToTrash = RdbMoveToTrash.Checked,
            };

        private void PibMoveToTrashHint_Click(object sender, EventArgs e) =>
            TipHints.Show(
                TipHints.GetToolTip(PibMoveToTrashHint),
                PibMoveToTrashHint,
                0,
                PibMoveToTrashHint.Height,
                3000);
    }
}
