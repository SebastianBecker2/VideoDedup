namespace VideoDedupClient.Controls.DedupSettings
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Windows.Forms;
    using VideoDedupGrpc;

    public partial class DedupSettingsCtrl : UserControl
    {
        public DedupSettingsCtrl()
        {
            InitializeComponent();

            NudConcurrencyLevel.Maximum = int.MaxValue;

        }

        public void ShowSettings(DedupSettings? dedupSettings, int? processorCount)
        {
            if (dedupSettings is null)
            {
                return;
            }

            TxtSourcePath.Text = dedupSettings.BasePath;
            ChbRecursive.Checked = dedupSettings.Recursive;
            ChbMonitorFileChanges.Checked =
                dedupSettings.MonitorChanges;

            if (dedupSettings.ExcludedDirectories != null)
            {
                LsbExcludedDirectories.Items.AddRange(
                    [.. dedupSettings.ExcludedDirectories]);
            }

            if (dedupSettings.FileExtensions != null)
            {
                LsbFileExtensions.Items.AddRange(
                    [.. dedupSettings.FileExtensions]);
            }

            NudConcurrencyLevel.Value =
                dedupSettings.ConcurrencyLevel;


            var text = $"Set the concurrency level for parallel processing." +
                $"{Environment.NewLine}Higher values increase performance but " +
                $"may use more system resources.{Environment.NewLine}" +
                $"Default: Number of logical CPUs divided by 2.";

            if (processorCount is not null)
            {
                text += $" ({processorCount} / 2 = " +
                    $"{processorCount / 2})";
            }

            TipHints.SetToolTip(PibConcurrencyLevelHint, text);
        }

        public DedupSettings GetSettings()
        {
            var dedupSettings = new DedupSettings()
            {
                BasePath = TxtSourcePath.Text,
                Recursive = ChbRecursive.Checked,
                MonitorChanges = ChbMonitorFileChanges.Checked,
                ConcurrencyLevel = (int)NudConcurrencyLevel.Value,
            };

            dedupSettings.ExcludedDirectories.Clear();
            dedupSettings.ExcludedDirectories.AddRange(
                LsbExcludedDirectories.Items.Cast<string>());

            dedupSettings.FileExtensions.Clear();
            dedupSettings.FileExtensions.AddRange(
                [.. LsbFileExtensions.Items.Cast<string>()]);

            return dedupSettings;
        }

        private void BtnSelectSourcePath_Click(object sender, EventArgs e)
        {
            var newPath = SelectFileDialog.Show(
                TxtSourcePath.Text,
                "Select source directory",
                CustomSelectFileDlg.RequestedEntryType.Folders);
            if (newPath is null)
            {
                return;
            }

            TxtSourcePath.Text = newPath;
        }

        private void BtnAddExcludedDirectory_Click(object sender, EventArgs e)
        {
            var newPath = SelectFileDialog.Show(
                TxtSourcePath.Text,
                "Select excluded directory",
                CustomSelectFileDlg.RequestedEntryType.Folders);
            if (newPath is null)
            {
                return;
            }

            _ = LsbExcludedDirectories.Items.Add(newPath);
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

        private void PibConcurrencyLevelHint_Click(object sender, EventArgs e) =>
            TipHints.Show(
                TipHints.GetToolTip(PibConcurrencyLevelHint),
                PibConcurrencyLevelHint,
                0,
                PibConcurrencyLevelHint.Height,
                3000);
    }
}
