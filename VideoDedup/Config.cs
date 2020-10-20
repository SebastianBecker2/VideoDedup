using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoDedup.Properties;

namespace VideoDedup
{
    public partial class Config : Form
    {
        public Config()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            TxtSourcePath.Text = ConfigData.SourcePath;

            if (ConfigData.ExcludedDirectories != null)
            {
                LsbExcludedDirectories.Items.AddRange(ConfigData.ExcludedDirectories.ToArray());
            }

            if (ConfigData.FileExtensions != null)
            {
                LsbFileExtensions.Items.AddRange(ConfigData.FileExtensions.ToArray());
            }

            NumMaxThumbnailComparison.Value = ConfigData.MaxThumbnailComparison;
            NumMaxDifferentThumbnails.Value = ConfigData.MaxDifferentThumbnails;
            NumMaxDifferentPercentage.Value = ConfigData.MaxDifferencePercentage;

            RdbDurationDifferencePercent.Checked = ConfigData.DurationDifferenceType == DurationDifferenceType.Percent;
            RdbDurationDifferenceSeconds.Checked = ConfigData.DurationDifferenceType != DurationDifferenceType.Percent;
            NumMaxDurationDifferencePercent.Value = ConfigData.MaxDurationDifferencePercent;
            NumMaxDurationDifferenceSeconds.Value = ConfigData.MaxDurationDifferenceSeconds;

            NumThumbnailViewCount.Value = ConfigData.ThumbnailViewCount;

            base.OnLoad(e);
        }

        private void BtnOkay_Click(object sender, EventArgs e)
        {
            ConfigData.SourcePath = TxtSourcePath.Text;
            ConfigData.ExcludedDirectories = LsbExcludedDirectories.Items.Cast<string>().ToList();
            ConfigData.FileExtensions = LsbFileExtensions.Items.Cast<string>().ToList();

            ConfigData.MaxThumbnailComparison = (int)NumMaxThumbnailComparison.Value;
            ConfigData.MaxDifferentThumbnails = (int)NumMaxDifferentThumbnails.Value;
            ConfigData.MaxDifferencePercentage = (int)NumMaxDifferentPercentage.Value;

            if (RdbDurationDifferencePercent.Checked)
            {
                ConfigData.DurationDifferenceType = DurationDifferenceType.Percent;
            }
            else
            {
                ConfigData.DurationDifferenceType = DurationDifferenceType.Seconds;
            }
            ConfigData.MaxDurationDifferencePercent = (int)NumMaxDurationDifferencePercent.Value;
            ConfigData.MaxDurationDifferenceSeconds = (int)NumMaxDurationDifferenceSeconds.Value;

            ConfigData.ThumbnailViewCount = (int)NumThumbnailViewCount.Value;

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

            LsbFileExtensions.Items.Add(TxtFileExtension.Text);
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
