namespace VideoDedupClient.Controls.ComparisonSettings
{
    using System;
    using System.Windows.Forms;

    public partial class ComparisonSettingsCtrl : UserControl
    {
        public event EventHandler? TryComparisonClick;
        protected virtual void OnTryComparisonClick() =>
            TryComparisonClick?.Invoke(this, EventArgs.Empty);

        public ComparisonSettingsCtrl() => InitializeComponent();

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
