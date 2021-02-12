namespace VideoDedup
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Windows.Forms;
    using Microsoft.WindowsAPICodePack.Dialogs;
    using VideoDedupShared;
    public partial class ServerConfigDlg : Form
    {
        public Wcf.Contracts.Data.ConfigData ServerConfig { get; set; }

        public ServerConfigDlg() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            TxtSourcePath.Text = ServerConfig.BasePath;
            ChbRecursive.Checked = ServerConfig.Recursive;
            ChbMonitorFileChanges.Checked = ServerConfig.MonitorChanges;

            if (ServerConfig.ExcludedDirectories != null)
            {
                LsbExcludedDirectories.Items.AddRange(ServerConfig.ExcludedDirectories.ToArray());
            }

            if (ServerConfig.FileExtensions != null)
            {
                LsbFileExtensions.Items.AddRange(ServerConfig.FileExtensions.ToArray());
            }

            NumMaxThumbnailComparison.Value = ServerConfig.MaxImageCompares;
            NumMaxDifferentThumbnails.Value = ServerConfig.MaxDifferentImages;
            NumMaxDifferentPercentage.Value = ServerConfig.MaxImageDifferencePercent;

            RdbDurationDifferencePercent.Checked = ServerConfig.DifferenceType == DurationDifferenceType.Percent;
            RdbDurationDifferenceSeconds.Checked = ServerConfig.DifferenceType != DurationDifferenceType.Percent;
            NumMaxDurationDifferencePercent.Value = ServerConfig.MaxDurationDifferencePercent;
            NumMaxDurationDifferenceSeconds.Value = ServerConfig.MaxDurationDifferenceSeconds;

            NumThumbnailViewCount.Value = ServerConfig.ThumbnailCount;

            base.OnLoad(e);
        }

        private void BtnOkay_Click(object sender, EventArgs e)
        {
            ServerConfig.BasePath = TxtSourcePath.Text;
            ServerConfig.Recursive = ChbRecursive.Checked;
            ServerConfig.MonitorChanges = ChbMonitorFileChanges.Checked;

            ServerConfig.ExcludedDirectories = LsbExcludedDirectories.Items.Cast<string>().ToList();
            ServerConfig.FileExtensions = LsbFileExtensions.Items.Cast<string>().ToList();

            ServerConfig.MaxImageCompares = (int)NumMaxThumbnailComparison.Value;
            ServerConfig.MaxDifferentImages = (int)NumMaxDifferentThumbnails.Value;
            ServerConfig.MaxImageDifferencePercent = (int)NumMaxDifferentPercentage.Value;

            if (RdbDurationDifferencePercent.Checked)
            {
                ServerConfig.DifferenceType = DurationDifferenceType.Percent;
            }
            else
            {
                ServerConfig.DifferenceType = DurationDifferenceType.Seconds;
            }
            ServerConfig.MaxDurationDifferencePercent = (int)NumMaxDurationDifferencePercent.Value;
            ServerConfig.MaxDurationDifferenceSeconds = (int)NumMaxDurationDifferenceSeconds.Value;

            ServerConfig.ThumbnailCount = (int)NumThumbnailViewCount.Value;

            DialogResult = DialogResult.OK;
        }

        private void BtnSelectSourcePath_Click(object sender, EventArgs e)
        {
            using (var dlg = new CommonOpenFileDialog())
            {
                dlg.IsFolderPicker = true;
                dlg.InitialDirectory = TxtSourcePath.Text;

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
