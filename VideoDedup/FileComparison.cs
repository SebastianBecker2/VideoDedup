using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

        private Task LeftThumbnailTask;
        private Task RightThumbnailTask;

        public FileComparison()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            SplitterContainer.SplitterDistance = SplitterContainer.Width / 2;

            FpvLeft.VideoFile = LeftFile;
            LeftThumbnailTask = FpvLeft.UpdateDisplay();
            FpvRight.VideoFile = RightFile;
            RightThumbnailTask = FpvRight.UpdateDisplay();

            base.OnLoad(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var result = DialogResult;
            if (!LeftThumbnailTask.IsCompleted)
            {
                e.Cancel = true;
                FpvLeft.CancelThumbnails();
                LeftThumbnailTask.ContinueWith(t => DialogResult = result,
                    TaskScheduler.FromCurrentSynchronizationContext());
            }

            if (!RightThumbnailTask.IsCompleted)
            {
                e.Cancel = true;
                FpvRight.CancelThumbnails();
                RightThumbnailTask.ContinueWith(t => DialogResult = result,
                    TaskScheduler.FromCurrentSynchronizationContext());
            }

            base.OnFormClosing(e);
        }

        private void btnDeleteLeft_Click(object sender, EventArgs e)
        {
            Action delete_and_close = () =>
            {
                try
                {
                    File.Delete(LeftFile.FilePath);
                    DialogResult = DialogResult.OK;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            };

            if (!LeftThumbnailTask.IsCompleted)
            {
                FpvLeft.CancelThumbnails();
                LeftThumbnailTask.ContinueWith(t => delete_and_close(),
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                delete_and_close();
            }
        }

        private void btnDeleteRight_Click(object sender, EventArgs e)
        {
            Action delete_and_close = () =>
            {
                try
                {
                    File.Delete(RightFile.FilePath);
                    DialogResult = DialogResult.OK;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            };

            if (!RightThumbnailTask.IsCompleted)
            {
                FpvRight.CancelThumbnails();
                RightThumbnailTask.ContinueWith(t => delete_and_close(),
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                delete_and_close();
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
