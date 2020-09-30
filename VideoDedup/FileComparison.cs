using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XnaFan.ImageComparison;

namespace VideoDedup
{
    public partial class FileComparison : Form
    {
        public VideoFile LeftFile { get; set; }
        public VideoFile RightFile { get; set; }

        public FileComparison()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            SplitterContainer.SplitterDistance = SplitterContainer.Width / 2;

            FpvLeft.VideoFile = LeftFile;
            FpvRight.VideoFile = RightFile;

            base.OnLoad(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }

        private void btnDeleteLeft_Click(object sender, EventArgs e)
        {
            try
            {
                File.Delete(LeftFile.FilePath);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void btnDeleteRight_Click(object sender, EventArgs e)
        {
            try
            {
                File.Delete(RightFile.FilePath);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void OpenFileInExplorer(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            // combine the arguments together
            // it doesn't matter if there is a space after ','
            string argument = "/select, \"" + filePath + "\"";

            Process.Start("explorer.exe", argument);
        }

        private void BtnShowRight_Click(object sender, EventArgs e)
        {
            OpenFileInExplorer(RightFile.FilePath);
        }

        private void BtnShowLeft_Click(object sender, EventArgs e)
        {
            OpenFileInExplorer(LeftFile.FilePath);
        }
    }
}
