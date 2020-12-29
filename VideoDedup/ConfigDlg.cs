namespace VideoDedup
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Windows.Forms;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using VideoDedupShared;
    public partial class ConfigDlg : Form
    {
        public ConfigData ClientConfig { get; set; }
        public Wcf.Contracts.Data.ConfigData ServerConfig { get; set; }

        public ConfigDlg() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            TxtSourcePath.Text = ServerConfig.SourcePath;

            if (ServerConfig.ExcludedDirectories != null)
            {
                LsbExcludedDirectories.Items.AddRange(ServerConfig.ExcludedDirectories.ToArray());
            }

            if (ServerConfig.FileExtensions != null)
            {
                LsbFileExtensions.Items.AddRange(ServerConfig.FileExtensions.ToArray());
            }

            NumMaxThumbnailComparison.Value = ServerConfig.MaxThumbnailComparison;
            NumMaxDifferentThumbnails.Value = ServerConfig.MaxDifferentThumbnails;
            NumMaxDifferentPercentage.Value = ServerConfig.MaxDifferencePercentage;

            RdbDurationDifferencePercent.Checked = ServerConfig.DurationDifferenceType == DurationDifferenceType.Percent;
            RdbDurationDifferenceSeconds.Checked = ServerConfig.DurationDifferenceType != DurationDifferenceType.Percent;
            NumMaxDurationDifferencePercent.Value = ServerConfig.MaxDurationDifferencePercent;
            NumMaxDurationDifferenceSeconds.Value = ServerConfig.MaxDurationDifferenceSeconds;

            NumThumbnailViewCount.Value = ClientConfig.ThumbnailViewCount;

            base.OnLoad(e);
        }

        private void BtnOkay_Click(object sender, EventArgs e)
        {
            ServerConfig.SourcePath = TxtSourcePath.Text;
            ServerConfig.ExcludedDirectories = LsbExcludedDirectories.Items.Cast<string>().ToList();
            ServerConfig.FileExtensions = LsbFileExtensions.Items.Cast<string>().ToList();

            ServerConfig.MaxThumbnailComparison = (int)NumMaxThumbnailComparison.Value;
            ServerConfig.MaxDifferentThumbnails = (int)NumMaxDifferentThumbnails.Value;
            ServerConfig.MaxDifferencePercentage = (int)NumMaxDifferentPercentage.Value;

            if (RdbDurationDifferencePercent.Checked)
            {
                ServerConfig.DurationDifferenceType = DurationDifferenceType.Percent;
            }
            else
            {
                ServerConfig.DurationDifferenceType = DurationDifferenceType.Seconds;
            }
            ServerConfig.MaxDurationDifferencePercent = (int)NumMaxDurationDifferencePercent.Value;
            ServerConfig.MaxDurationDifferenceSeconds = (int)NumMaxDurationDifferenceSeconds.Value;

            ClientConfig.ThumbnailViewCount = (int)NumThumbnailViewCount.Value;

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
