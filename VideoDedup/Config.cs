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

        public Config()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            TxtSourcePath.Text = SourcePath;

            if (ExcludedDirectories != null)
            {
                LsbExceptionPaths.Items.AddRange(ExcludedDirectories.ToArray());
            }

            base.OnLoad(e);
        }

        private void BtnOkay_Click(object sender, EventArgs e)
        {
            SourcePath = TxtSourcePath.Text;
            ExcludedDirectories = LsbExceptionPaths.Items.Cast<string>().ToList();
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

        private void BtnAddExceptionPath_Click(object sender, EventArgs e)
        {
            using (var dlg = new CommonOpenFileDialog())
            {
                dlg.IsFolderPicker = true;

                if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                LsbExceptionPaths.Items.Add(dlg.FileName);
            }
        }

        private void BtnRemoveExceptionPath_Click(object sender, EventArgs e)
        {
            foreach (var s in LsbExceptionPaths.SelectedItems.OfType<string>().ToList())
            {
                LsbExceptionPaths.Items.Remove(s);
            }
        }
    }
}
