using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Taskbar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToDoManager;
using VideoDedup.ISynchronizeInvokeExtensions;
using VideoDedup.ProgressBarExtension;
using VideoDedup.Properties;
using VideoDedup.TimeSpanExtension;

namespace VideoDedup
{
    public partial class VideoDedup : Form
    {
        private readonly static string StatusInfoDuplicateCount = "Duplicates found {0}";

        private string CurrentStatusInfo { get; set; }

        private DateTime? lastStatusUpdate { get; set; } = null;

        private TimeSpan ElapsedTime { get; set; } = new TimeSpan();

        private FileSystemWatcher Watcher { get; set; } = new FileSystemWatcher();

        private Dedupper Dedupper { get; set; } = null;

        public VideoDedup()
        {
            Watcher.Changed += WatcherChangeEventHandler;
            Watcher.Created += WatcherCreatedEventHandler;
            Watcher.Renamed += WatcherRenamedEventHandler;
            Watcher.Deleted += WatcherDeletedEventHandler;
            //Watcher.NotifyFilter = NotifyFilters.LastWrite;
            //Watcher.NotifyFilter =
            //NotifyFilters.Attributes |
            //NotifyFilters.CreationTime |
            //NotifyFilters.DirectoryName |
            //NotifyFilters.FileName |
            //NotifyFilters.LastAccess |
            //NotifyFilters.LastWrite
            //;
            //NotifyFilters.Security |
            //NotifyFilters.Size;
            Watcher.Filter = "*.*";
            Watcher.IncludeSubdirectories = true;

            Watcher.Path = ConfigData.SourcePath;
            Watcher.EnableRaisingEvents = true;

            InitializeComponent();
        }

        private void WatcherDeletedEventHandler(object sender, FileSystemEventArgs e)
        {
            Debug.Print("Deleted");
            Debug.Print("File " + e.ChangeType.ToString() + ": " + e.Name);
        }

        private void WatcherRenamedEventHandler(object sender, RenamedEventArgs e)
        {
            if (!File.Exists(e.FullPath))
            {
                return;
            }
            Debug.Print("Renamed");
            Debug.Print("File " + e.ChangeType.ToString() + ": " + e.Name);
        }

        private void WatcherCreatedEventHandler(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(e.FullPath))
            {
                return;
            }
            Debug.Print("Created");
            Debug.Print("File " + e.ChangeType.ToString() + ": " + e.Name);
        }

        private void WatcherChangeEventHandler(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(e.FullPath))
            {
                return;
            }
            Debug.Print("Changed");
            Debug.Print("File " + e.ChangeType.ToString() + ": " + e.Name);
            var configuration = new ConfigNonStatic
            {
                DurationDifferenceType = ConfigData.DurationDifferenceType,
                SourcePath = ConfigData.SourcePath,
                ExcludedDirectories = ConfigData.ExcludedDirectories,
                FileExtensions = ConfigData.FileExtensions,
                MaxDifferentThumbnails = ConfigData.MaxDifferentThumbnails,
                MaxDifferencePercentage = ConfigData.MaxDifferencePercentage,
                MaxDurationDifferenceSeconds = ConfigData.MaxDurationDifferenceSeconds,
                MaxDurationDifferencePercent = ConfigData.MaxDurationDifferencePercent,
                MaxThumbnailComparison = ConfigData.MaxThumbnailComparison,
            };
            var f = new VideoFile(e.FullPath, configuration);
            Debug.Print("Duration: " + f.Duration.ToString());

        }

        protected override void OnLoad(EventArgs e)
        {
#if !DEBUG
            BtnToDoManager.Visible = false;
#endif
            base.OnLoad(e);
        }

        private void BtnToDoManager_Click(object sender, EventArgs e)
        {
            using (var dlg = new ToDoManager.ToDoManager())
            {
                dlg.ShowDialog();
            }
        }

        private void UpdateProgress(
            string statusInfo,
            int counter,
            int maxCount)
        {
            this.InvokeIfRequired(() =>
            {
                if (lastStatusUpdate.HasValue
                    && (DateTime.Now - lastStatusUpdate.Value).TotalMilliseconds < 100
                    && maxCount - counter > 2
                    && counter > 0)
                {
                    return;
                }
                lastStatusUpdate = DateTime.Now;

                if (!string.IsNullOrWhiteSpace(statusInfo))
                {
                    CurrentStatusInfo = statusInfo;
                }

                LblStatusInfo.Text = string.Format(
                    CurrentStatusInfo,
                    counter,
                    maxCount);

                if (maxCount > 0)
                {
                    TaskbarManager.Instance.SetProgressState(
                        TaskbarProgressBarState.Normal,
                        Handle);
                    ProgressBar.Style = ProgressBarStyle.Continuous;
                }
                else
                {
                    TaskbarManager.Instance.SetProgressState(
                        TaskbarProgressBarState.Indeterminate,
                        Handle);
                    ProgressBar.Style = ProgressBarStyle.Marquee;
                }

                ProgressBar.Value = counter;
                ProgressBar.Maximum = maxCount == 0 ? 1 : maxCount;
                TaskbarManager.Instance.SetProgressValue(
                    counter,
                    maxCount,
                    Handle);
            });
        }

        private void BtnDedup_Click(object sender, EventArgs e)
        {
            Watcher.Path = @"D:\VideoDedupTest2";

            var configuration = new ConfigNonStatic
            {
                DurationDifferenceType = ConfigData.DurationDifferenceType,
                SourcePath = ConfigData.SourcePath,
                ExcludedDirectories = ConfigData.ExcludedDirectories,
                FileExtensions = ConfigData.FileExtensions,
                MaxDifferentThumbnails = ConfigData.MaxDifferentThumbnails,
                MaxDifferencePercentage = ConfigData.MaxDifferencePercentage,
                MaxDurationDifferenceSeconds = ConfigData.MaxDurationDifferenceSeconds,
                MaxDurationDifferencePercent = ConfigData.MaxDurationDifferencePercent,
                MaxThumbnailComparison = ConfigData.MaxThumbnailComparison,
            };
            Dedupper = new Dedupper(configuration);
            Dedupper.ProgressUpdate += (s, args) => UpdateProgress(args.StatusInfo,
                args.Counter,
                args.MaxCount);
            Dedupper.DuplicateCountChanged += (s, args) =>
            {
                LblDuplicateCount.InvokeIfRequired(() =>
                    LblDuplicateCount.Text = string.Format(StatusInfoDuplicateCount, args.Count));
                BtnResolveDuplicates.InvokeIfRequired(() =>
                    BtnResolveDuplicates.Enabled = args.Count > 0);
            };
            Dedupper.Logged += (s, args) =>
                LblCurrentFile.InvokeIfRequired(() => LblCurrentFile.Text = args.Message);

            BtnDedup.Enabled = false;
            BtnCancel.Enabled = true;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Dedupper.Dispose();
            BtnDedup.Enabled = true;
            BtnCancel.Enabled = false;
        }

        private void BtnConfig_Click(object sender, EventArgs e)
        {
            using (var dlg = new Config())
            {
                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
            }
        }

        private void ElapsedTimer_Tick(object sender, EventArgs e)
        {
            ElapsedTime = ElapsedTime.Add(TimeSpan.FromSeconds(1));
            LblTimer.Text = ElapsedTime.ToString();
        }

        private void BtnResolveConflicts_Click(object sender, EventArgs e)
        {
            while (Dedupper.DequeueDuplcate(out Duplicate duplicate))
            {
                (var left, var right) = duplicate;
                // Mostely because we might have deleted
                // this file during the previous compare
                if (!File.Exists(left.FilePath))
                {
                    Debug.Print($"{left.FilePath} doesn't exist anymore. Can't compare.");
                    continue;
                }
                if (!File.Exists(right.FilePath))
                {
                    Debug.Print($"{right.FilePath} doesn't exist anymore. Can't compare.");
                    continue;
                }

                using (var dlg = new FileComparison())
                {
                    DialogResult result;
                    lock (left) lock (right)
                        {
                            dlg.LeftFile = left;
                            dlg.RightFile = right;
                            result = dlg.ShowDialog();
                            left.DisposeThumbnails();
                            right.DisposeThumbnails();
                        }

                    if (result == DialogResult.Yes)
                    {
                        continue;
                    }
                    if (result == DialogResult.Cancel)
                    {
                        Dedupper.EnqueueDuplicate(duplicate);
                        return;
                    }
                    if (result == DialogResult.No) // Skip
                    {
                        Dedupper.EnqueueDuplicate(duplicate);
                    }
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new About())
            {
                dlg.ShowDialog();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            NotifyIcon.Visible = false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
                NotifyIcon.Visible = true;
            }
        }

        private void VideoDedup_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (Duplicates.Any())
            //{
            //    var selection = MessageBox.Show(
            //        $"There are {Duplicates.Count()} duplicates to resolve." +
            //        $"{Environment.NewLine}Are you sure you want to close?",
            //        "Discard duplicates?",
            //        MessageBoxButtons.YesNo,
            //        MessageBoxIcon.Warning);

            //    if (selection == DialogResult.No)
            //    {
            //        e.Cancel = true;
            //        return;
            //    }
            //}

            if (ElapsedTimer.Enabled)
            {
                var selection = MessageBox.Show(
                    $"VideoDedup is currently search for duplicates." +
                    $"{Environment.NewLine}Are you sure you want to close?",
                    "Cancel search?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (selection == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void BtnDiscard_Click(object sender, EventArgs e)
        {
            //if (Duplicates.Any())
            //{
            //    var selection = MessageBox.Show(
            //                    $"There are {Duplicates.Count()} duplicates to resolve." +
            //                    $"{Environment.NewLine}Are you sure you want to discard them?",
            //                    "Discard duplicates?",
            //                    MessageBoxButtons.YesNo,
            //                    MessageBoxIcon.Warning);

            //    if (selection == DialogResult.No)
            //    {
            //        return;
            //    }

            //    ClearDuplicates();
            //}
        }
    }
}
