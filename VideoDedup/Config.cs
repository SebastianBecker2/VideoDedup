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


            base.OnLoad(e);
        }

        private void BtnOkay_Click(object sender, EventArgs e)
        {
            ConfigData.SourcePath = TxtSourcePath.Text;
            ConfigData.ExcludedDirectories = LsbExcludedDirectories.Items.Cast<string>().ToList();
            ConfigData.FileExtensions = LsbFileExtensions.Items.Cast<string>().ToList();
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

                LsbExcludedDirectories.Items.Add(dlg.FileName);
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
