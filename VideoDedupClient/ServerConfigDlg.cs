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
                LsbExcludedDirectories.Items.AddRange(
                    ServerConfig.ExcludedDirectories.ToArray());
            }

            if (ServerConfig.FileExtensions != null)
            {
                LsbFileExtensions.Items.AddRange(
                    ServerConfig.FileExtensions.ToArray());
            }

            NumMaxImageComparison.Value = ServerConfig.MaxImageCompares;
            NumMaxDifferentImages.Value = ServerConfig.MaxDifferentImages;
            NumMaxDifferentPercentage.Value =
                ServerConfig.MaxImageDifferencePercent;

            RdbDurationDifferencePercent.Checked =
                ServerConfig.DifferenceType == DurationDifferenceType.Percent;
            RdbDurationDifferenceSeconds.Checked =
                ServerConfig.DifferenceType != DurationDifferenceType.Percent;
            NumMaxDurationDifferencePercent.Value =
                ServerConfig.MaxDurationDifferencePercent;
            NumMaxDurationDifferenceSeconds.Value =
                ServerConfig.MaxDurationDifferenceSeconds;

            NumThumbnailViewCount.Value = ServerConfig.ThumbnailCount;

            NumSaveStateIntervalMinutes.Value =
                ServerConfig.SaveStateIntervalMinutes;

            base.OnLoad(e);
        }

        private void BtnOkay_Click(object sender, EventArgs e)
        {
            ServerConfig.BasePath = TxtSourcePath.Text;
            ServerConfig.Recursive = ChbRecursive.Checked;
            ServerConfig.MonitorChanges = ChbMonitorFileChanges.Checked;

            ServerConfig.ExcludedDirectories =
                LsbExcludedDirectories.Items.Cast<string>().ToList();
            ServerConfig.FileExtensions =
                LsbFileExtensions.Items.Cast<string>().ToList();

            ServerConfig.MaxImageCompares = (int)NumMaxImageComparison.Value;
            ServerConfig.MaxDifferentImages = (int)NumMaxDifferentImages.Value;
            ServerConfig.MaxImageDifferencePercent =
                (int)NumMaxDifferentPercentage.Value;

            if (RdbDurationDifferencePercent.Checked)
            {
                ServerConfig.DifferenceType = DurationDifferenceType.Percent;
            }
            else
            {
                ServerConfig.DifferenceType = DurationDifferenceType.Seconds;
            }
            ServerConfig.MaxDurationDifferencePercent =
                (int)NumMaxDurationDifferencePercent.Value;
            ServerConfig.MaxDurationDifferenceSeconds =
                (int)NumMaxDurationDifferenceSeconds.Value;

            ServerConfig.ThumbnailCount = (int)NumThumbnailViewCount.Value;

            ServerConfig.SaveStateIntervalMinutes =
                (int)NumSaveStateIntervalMinutes.Value;

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

        private void BtnVideoComparisonPreview_Click(object sender, EventArgs e)
        {
            using (var dlg = new VideoComparisonPreviewDlg())
            {
                dlg.ServerConfig = new Wcf.Contracts.Data.ConfigData()
                {
                    MaxImageCompares = (int)NumMaxImageComparison.Value,
                    MaxDifferentImages = (int)NumMaxDifferentImages.Value,
                    MaxImageDifferencePercent =
                        (int)NumMaxDifferentPercentage.Value,
                };

                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                NumMaxImageComparison.Value = dlg.ServerConfig.MaxImageCompares;
                NumMaxDifferentImages.Value =
                    dlg.ServerConfig.MaxDifferentImages;
                NumMaxDifferentPercentage.Value =
                    dlg.ServerConfig.MaxImageDifferencePercent;
            }
        }
    }
}
