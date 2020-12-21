namespace VideoDedup
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Windows.Forms;
    using Microsoft.WindowsAPICodePack.Dialogs;

    public partial class ConfigDlg : Form
    {
        public ConfigData Configuration { get; set; }

        public ConfigDlg() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            TxtSourcePath.Text = Configuration.SourcePath;

            if (Configuration.ExcludedDirectories != null)
            {
                LsbExcludedDirectories.Items.AddRange(Configuration.ExcludedDirectories.ToArray());
            }

            if (Configuration.FileExtensions != null)
            {
                LsbFileExtensions.Items.AddRange(Configuration.FileExtensions.ToArray());
            }

            NumMaxThumbnailComparison.Value = Configuration.MaxThumbnailComparison;
            NumMaxDifferentThumbnails.Value = Configuration.MaxDifferentThumbnails;
            NumMaxDifferentPercentage.Value = Configuration.MaxDifferencePercentage;

            RdbDurationDifferencePercent.Checked = Configuration.DurationDifferenceType == DurationDifferenceType.Percent;
            RdbDurationDifferenceSeconds.Checked = Configuration.DurationDifferenceType != DurationDifferenceType.Percent;
            NumMaxDurationDifferencePercent.Value = Configuration.MaxDurationDifferencePercent;
            NumMaxDurationDifferenceSeconds.Value = Configuration.MaxDurationDifferenceSeconds;

            NumThumbnailViewCount.Value = Configuration.ThumbnailViewCount;

            base.OnLoad(e);
        }

        private void BtnOkay_Click(object sender, EventArgs e)
        {
            Configuration.SourcePath = TxtSourcePath.Text;
            Configuration.ExcludedDirectories = LsbExcludedDirectories.Items.Cast<string>().ToList();
            Configuration.FileExtensions = LsbFileExtensions.Items.Cast<string>().ToList();

            Configuration.MaxThumbnailComparison = (int)NumMaxThumbnailComparison.Value;
            Configuration.MaxDifferentThumbnails = (int)NumMaxDifferentThumbnails.Value;
            Configuration.MaxDifferencePercentage = (int)NumMaxDifferentPercentage.Value;

            if (RdbDurationDifferencePercent.Checked)
            {
                Configuration.DurationDifferenceType = DurationDifferenceType.Percent;
            }
            else
            {
                Configuration.DurationDifferenceType = DurationDifferenceType.Seconds;
            }
            Configuration.MaxDurationDifferencePercent = (int)NumMaxDurationDifferencePercent.Value;
            Configuration.MaxDurationDifferenceSeconds = (int)NumMaxDurationDifferenceSeconds.Value;

            Configuration.ThumbnailViewCount = (int)NumThumbnailViewCount.Value;

            DialogResult = DialogResult.OK;
        }

        private void BtnSelectSourcePath_Click(object sender, EventArgs e)
        {
            using (var dlg = new CommonOpenFileDialog())
            {
                dlg.IsFolderPicker = true;

                if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                TxtSourcePath.Text = dlg.FileName;
            }
        }

        private void BtnAddExcludedDirectory_Click(object sender, EventArgs e)
        {
            using (var dlg = new CommonOpenFileDialog())
            {
                dlg.IsFolderPicker = true;

                if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                _ = LsbExcludedDirectories.Items.Add(dlg.FileName);
            }
        }

        private void BtnRemoveExcludedDirectory_Click(object sender, EventArgs e)
        {
            foreach (var s in LsbExcludedDirectories.SelectedItems.OfType<string>().ToList())
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
            foreach (var s in LsbFileExtensions.SelectedItems.OfType<string>().ToList())
            {
                LsbFileExtensions.Items.Remove(s);
            }
        }
    }
}
