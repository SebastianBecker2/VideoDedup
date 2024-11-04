namespace VideoDedupClient.Controls.ComparisonSettings
{
    using System;
    using System.Windows.Forms;
    using VideoDedupGrpc;
    using static VideoDedupGrpc.DurationComparisonSettings.Types;

    public partial class ComparisonSettingsCtrl : UserControl
    {
        public event EventHandler? TryComparisonClick;
        protected virtual void OnTryComparisonClick() =>
            TryComparisonClick?.Invoke(this, EventArgs.Empty);

        public ComparisonSettingsCtrl() => InitializeComponent();

        public void ShowSettings(
            VideoComparisonSettings? videoComparisonSettingsm,
            DurationComparisonSettings? durationComparisonSettings)
        {
            if (videoComparisonSettingsm is not null)
            {
                NumMaxImageComparison.Value =
                    videoComparisonSettingsm.CompareCount;
                NumMaxDifferentImages.Value =
                    videoComparisonSettingsm.MaxDifferentImages;
                NumMaxDifferentPercentage.Value =
                    videoComparisonSettingsm.MaxDifference;
            }

            if (durationComparisonSettings is not null)
            {
                RdbDurationDifferencePercent.Checked =
                    durationComparisonSettings.DifferenceType
                    == DurationDifferenceType.Percent;
                RdbDurationDifferenceSeconds.Checked =
                    durationComparisonSettings.DifferenceType
                    == DurationDifferenceType.Seconds;
                NumMaxDurationDifference.Value =
                    durationComparisonSettings.MaxDifference;
            }
        }

        public VideoComparisonSettings GetVideoComparisonSettings() =>
            new()
            {
                CompareCount = (int)NumMaxImageComparison.Value,
                MaxDifferentImages = (int)NumMaxDifferentImages.Value,
                MaxDifference = (int)NumMaxDifferentPercentage.Value,
            };

        public DurationComparisonSettings GetDurationComparisonSettings() =>
            new()
            {
                DifferenceType =
                        RdbDurationDifferencePercent.Checked
                        ? DurationDifferenceType.Percent
                        : DurationDifferenceType.Seconds,
                MaxDifference = (int)NumMaxDurationDifference.Value,
            };

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

        private void BtnCustomVideoComparison_Click(object sender, EventArgs e) =>
            OnTryComparisonClick();
    }
}
