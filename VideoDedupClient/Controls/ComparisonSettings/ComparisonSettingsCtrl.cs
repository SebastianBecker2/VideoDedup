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

        public ComparisonSettingsCtrl()
        {
            InitializeComponent();

            var text = "The difference of two frames is provided on a scale " +
                $"of 0 to 200.{Environment.NewLine}Default: 80";
            TipHints.SetToolTip(PibMaxDifferentPercentageHint, text);
        }

        public void ShowSettings(
            VideoComparisonSettings? videoComparisonSettingsm,
            DurationComparisonSettings? durationComparisonSettings)
        {
            if (videoComparisonSettingsm is not null)
            {
                NumMaxFrameComparison.Value =
                    videoComparisonSettingsm.CompareCount;
                NumMaxDifferentFrames.Value =
                    videoComparisonSettingsm.MaxDifferentFrames;
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
                CompareCount = (int)NumMaxFrameComparison.Value,
                MaxDifferentFrames = (int)NumMaxDifferentFrames.Value,
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

        private void PibMaxDifferentPercentageHint_Click(
            object sender,
            EventArgs e) =>
            TipHints.Show(
                TipHints.GetToolTip(PibMaxDifferentPercentageHint),
                PibMaxDifferentPercentageHint,
                0,
                PibMaxDifferentPercentageHint.Height,
                3000);
    }
}
