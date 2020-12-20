namespace VideoDedup
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Windows.Forms;
    using Microsoft.WindowsAPICodePack.Dialogs;

    public partial class Config : Form
    {
        public Config() => this.InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            this.TxtSourcePath.Text = ConfigData.SourcePath;

            if (ConfigData.ExcludedDirectories != null)
            {
                this.LsbExcludedDirectories.Items.AddRange(ConfigData.ExcludedDirectories.ToArray());
            }

            if (ConfigData.FileExtensions != null)
            {
                this.LsbFileExtensions.Items.AddRange(ConfigData.FileExtensions.ToArray());
            }

            this.NumMaxThumbnailComparison.Value = ConfigData.MaxThumbnailComparison;
            this.NumMaxDifferentThumbnails.Value = ConfigData.MaxDifferentThumbnails;
            this.NumMaxDifferentPercentage.Value = ConfigData.MaxDifferencePercentage;

            this.RdbDurationDifferencePercent.Checked = ConfigData.DurationDifferenceType == DurationDifferenceType.Percent;
            this.RdbDurationDifferenceSeconds.Checked = ConfigData.DurationDifferenceType != DurationDifferenceType.Percent;
            this.NumMaxDurationDifferencePercent.Value = ConfigData.MaxDurationDifferencePercent;
            this.NumMaxDurationDifferenceSeconds.Value = ConfigData.MaxDurationDifferenceSeconds;

            this.NumThumbnailViewCount.Value = ConfigData.ThumbnailViewCount;

            base.OnLoad(e);
        }

        private void BtnOkay_Click(object sender, EventArgs e)
        {
            ConfigData.SourcePath = this.TxtSourcePath.Text;
            ConfigData.ExcludedDirectories = this.LsbExcludedDirectories.Items.Cast<string>().ToList();
            ConfigData.FileExtensions = this.LsbFileExtensions.Items.Cast<string>().ToList();

            ConfigData.MaxThumbnailComparison = (int)this.NumMaxThumbnailComparison.Value;
            ConfigData.MaxDifferentThumbnails = (int)this.NumMaxDifferentThumbnails.Value;
            ConfigData.MaxDifferencePercentage = (int)this.NumMaxDifferentPercentage.Value;

            if (this.RdbDurationDifferencePercent.Checked)
            {
                ConfigData.DurationDifferenceType = DurationDifferenceType.Percent;
            }
            else
            {
                ConfigData.DurationDifferenceType = DurationDifferenceType.Seconds;
            }
            ConfigData.MaxDurationDifferencePercent = (int)this.NumMaxDurationDifferencePercent.Value;
            ConfigData.MaxDurationDifferenceSeconds = (int)this.NumMaxDurationDifferenceSeconds.Value;

            ConfigData.ThumbnailViewCount = (int)this.NumThumbnailViewCount.Value;

            this.DialogResult = DialogResult.OK;
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

                this.TxtSourcePath.Text = dlg.FileName;
            }
        }

        private void BtnAddExcludedDirectory_Click(object sender, EventArgs e)
        {

        }

        private void BtnRemoveExcludedDirectory_Click(object sender, EventArgs e)
        {
            foreach (var s in this.LsbExcludedDirectories.SelectedItems.OfType<string>().ToList())
            {
                this.LsbExcludedDirectories.Items.Remove(s);
            }
        }

        private void BtnAddFileExtension_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.TxtFileExtension.Text))
            {
                return;
            }

            _ = this.LsbFileExtensions.Items.Add(this.TxtFileExtension.Text);
        }

        private void BtnRemoveFileExtension_Click(object sender, EventArgs e)
        {
            foreach (var s in this.LsbFileExtensions.SelectedItems.OfType<string>().ToList())
            {
                this.LsbFileExtensions.Items.Remove(s);
            }
        }
    }
}
