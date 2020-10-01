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
        public string SourcePath { get; set; }
        public IList<string> ExcludedDirectories { get; set; }
        public IList<string> FileExtensions { get; set; }

        public Config()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            TxtSourcePath.Text = SourcePath;

            if (ExcludedDirectories != null)
            {
                LsbExcludedDirectories.Items.AddRange(ExcludedDirectories.ToArray());
            }

            if (FileExtensions != null)
            {
                LsbFileExtensions.Items.AddRange(FileExtensions.ToArray());
            }


            base.OnLoad(e);
        }

        private void BtnOkay_Click(object sender, EventArgs e)
        {
            SourcePath = TxtSourcePath.Text;
            ExcludedDirectories = LsbExcludedDirectories.Items.Cast<string>().ToList();
            FileExtensions = LsbFileExtensions.Items.Cast<string>().ToList();
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
