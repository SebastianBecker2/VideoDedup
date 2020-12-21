namespace VideoDedup
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using global::VideoDedup.ISynchronizeInvokeExtensions;
    using global::VideoDedup.Properties;
    using Microsoft.WindowsAPICodePack.Taskbar;
    using Newtonsoft.Json;

    public partial class VideoDedup : Form
    {
        private static readonly string StatusInfoDuplicateCount = "Duplicates found {0}";

        private string CurrentStatusInfo { get; set; }

        private DateTime? LastStatusUpdate { get; set; } = null;

        private TimeSpan ElapsedTime { get; set; } = new TimeSpan();

        private Dedupper Dedupper { get; set; } = null;

        private ConfigData Configuration { get; set; }

        public VideoDedup() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
#if !DEBUG
            BtnToDoManager.Visible = false;
#endif
            Configuration = LoadConfig();
            base.OnLoad(e);
        }

        private void BtnToDoManager_Click(object sender, EventArgs e)
        {
            using (var dlg = new ToDoManager.ToDoManager())
            {
                _ = dlg.ShowDialog();
            }
        }

        private ConfigData LoadConfig()
        {
            var excludedDirectories = JsonConvert.DeserializeObject<List<string>>(
                Settings.Default.ExcludedDirectories);

            var fileExtensions = JsonConvert.DeserializeObject<List<string>>(
                Settings.Default.FileExtensions);
            if (!fileExtensions.Any())
            {
                fileExtensions = new List<string>
                    {
                        ".mp4", ".mpg", ".avi", ".wmv", ".flv", ".m4v", ".mov", ".mpeg", ".rm", ".mts", ".3gp"
                    };
            }

            DurationDifferenceType durationDifferenceType;
            if (Enum.TryParse(
                        Settings.Default.DurationDifferenceType,
                        true,
                        out DurationDifferenceType value))
            {
                durationDifferenceType = value;
            }
            else
            {
                durationDifferenceType = DurationDifferenceType.Seconds;
            }

            return new ConfigData
            {
                SourcePath = Settings.Default.SourcePath,
                ExcludedDirectories = excludedDirectories,
                FileExtensions = fileExtensions,
                MaxThumbnailComparison = Settings.Default.MaxThumbnailComparison,
                MaxDifferentThumbnails = Settings.Default.MaxDifferentThumbnails,
                MaxDifferencePercentage = Settings.Default.MaxDifferencePercentage,
                MaxDurationDifferenceSeconds = Settings.Default.MaxDurationDifferenceSeconds,
                MaxDurationDifferencePercent = Settings.Default.MaxDurationDifferencePercent,
                DurationDifferenceType = durationDifferenceType,
                ThumbnailViewCount = Settings.Default.ThumbnailViewCount,
            };
        }

        private void SaveConfig(ConfigData configuration)
        {
            Settings.Default.SourcePath =
                Configuration.SourcePath;
            Settings.Default.ExcludedDirectories =
                JsonConvert.SerializeObject(configuration.ExcludedDirectories);
            Settings.Default.FileExtensions =
                JsonConvert.SerializeObject(configuration.FileExtensions);
            Settings.Default.MaxThumbnailComparison =
                Configuration.MaxThumbnailComparison;
            Settings.Default.MaxDifferentThumbnails = configuration.MaxDifferentThumbnails;
            Settings.Default.MaxDifferencePercentage = configuration.MaxDifferencePercentage;
            Settings.Default.MaxDurationDifferenceSeconds = configuration.MaxDurationDifferenceSeconds;
            Settings.Default.MaxDurationDifferencePercent = configuration.MaxDurationDifferencePercent;
            Settings.Default.DurationDifferenceType = configuration.DurationDifferenceType.ToString();
            Settings.Default.ThumbnailViewCount = configuration.ThumbnailViewCount;
            Settings.Default.Save();
        }

        private void UpdateProgress(
            string statusInfo,
            int counter,
            int maxCount) => this.InvokeIfRequired(() =>
            {
                if (LastStatusUpdate.HasValue
                    && (DateTime.Now - LastStatusUpdate.Value).TotalMilliseconds < 100
                    && maxCount - counter > 2
                    && counter > 0)
                {
                    return;
                }
                LastStatusUpdate = DateTime.Now;

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

        private void BtnDedup_Click(object sender, EventArgs e)
        {
            Dedupper = new Dedupper(Configuration);
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
                TxtLog.InvokeIfRequired(() =>
                    TxtLog.AppendText(args.Message + Environment.NewLine));

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
            using (var dlg = new ConfigDlg())
            {
                dlg.Configuration = Configuration;
                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                Configuration = dlg.Configuration;
                SaveConfig(Configuration);
                Dedupper?.UpdateConfiguration(Configuration);
            }
        }

        private void ElapsedTimer_Tick(object sender, EventArgs e)
        {
            ElapsedTime = ElapsedTime.Add(TimeSpan.FromSeconds(1));
            LblTimer.Text = ElapsedTime.ToString();
        }

        private void BtnResolveConflicts_Click(object sender, EventArgs e)
        {
            while (Dedupper.DequeueDuplcate(out var duplicate))
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
                    lock (left)
                        lock (right)
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

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new About())
            {
                _ = dlg.ShowDialog();
            }
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Visible = true;
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            NotifyIcon.Visible = false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Visible = false;
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
