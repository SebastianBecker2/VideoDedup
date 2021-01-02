namespace VideoDedup
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.Windows.Forms;
    using VideoDedupShared.ISynchronizeInvokeExtensions;
    using global::VideoDedup.Properties;
    using Microsoft.WindowsAPICodePack.Taskbar;
    using Newtonsoft.Json;
    using VideoDedupShared;

    public partial class VideoDedup : Form
    {
        private static WcfProxy WcfProxy
        {
            get
            {
                lock (WcfProxyLock)
                {
                    // https://stackoverflow.com/questions/2008382/how-to-heal-faulted-wcf-channels
                    if (wcfProxy != null
                        && wcfProxy.InnerChannel.State
                        == CommunicationState.Faulted)
                    {
                        wcfProxy.Abort();
                        wcfProxy = null;
                    }

                    if (wcfProxy == null)
                    {
                        var binding = new NetTcpBinding();
                        var baseAddress = new Uri("net.tcp://localhost:41721/hello");
                        var address = new EndpointAddress(baseAddress);
                        wcfProxy = new WcfProxy(binding, address);
                    }
                    return wcfProxy;
                }
            }
        }
        private static WcfProxy wcfProxy = null;
        private static readonly object WcfProxyLock = new object();

        private static readonly TimeSpan StatusTimerInterval =
            TimeSpan.FromSeconds(1);

        private static readonly string StatusInfoDuplicateCount = "Duplicates found {0}";

        private string CurrentStatusInfo { get; set; }

        private static LogToken logToken = null;

        private DateTime? LastStatusUpdate { get; set; } = null;

        private TimeSpan ElapsedTime { get; set; } = new TimeSpan();

        private SmartTimer.Timer statusTimer = null;

        private ConfigData Configuration { get; set; }

        public VideoDedup() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
#if !DEBUG
            BtnToDoManager.Visible = false;
#endif
            Configuration = LoadConfig();

            statusTimer = new SmartTimer.Timer(StatusTimerCallback);
            UpdateProgress("Connecting...", 0, 0, ProgressStyle.Marquee);
            statusTimer.StartSingle(0);

            base.OnLoad(e);
        }

        private void StatusTimerCallback(object param)
        {
            try
            {
                var status = WcfProxy.GetCurrentStatus();
                var logData = WcfProxy.GetLogEvents(logToken);

                this.InvokeIfRequired(() =>
                {
                    ElapsedTime = ElapsedTime.Add(StatusTimerInterval);
                    LblTimer.Text = ElapsedTime.ToString();

                    UpdateProgress(status.StatusMessage,
                        status.CurrentProgress,
                        status.MaximumProgress,
                        status.ProgressStyle);
                    LblDuplicateCount.Text = string.Format(
                        StatusInfoDuplicateCount,
                        status.DuplicateCount);

                    if (logToken != null && logToken.Id != logData.LogToken.Id)
                    {
                        TxtLog.Clear();
                    }
                    logToken = logData.LogToken;
                    foreach (var log in logData.LogItems)
                    {
                        TxtLog.AppendText(log + Environment.NewLine);
                    }
                });
            }
            catch (EndpointNotFoundException)
            {
                this.InvokeIfRequired(() =>
                    UpdateProgress("Connecting...",
                    0, 0, ProgressStyle.Marquee));
            }
            catch (CommunicationObjectFaultedException)
            {
                this.InvokeIfRequired(() =>
                    UpdateProgress("Connecting...",
                    0, 0, ProgressStyle.Marquee));
            }
            finally
            {
                statusTimer.StartSingle(StatusTimerInterval);
            }
        }

        private void BtnToDoManager_Click(object sender, EventArgs e)
        {
            using (var dlg = new ToDoManager.ToDoManager())
            {
                _ = dlg.ShowDialog();
            }
        }

        private static ConfigData LoadConfig() => new ConfigData
        {
            ThumbnailViewCount = Settings.Default.ThumbnailViewCount,
        };

        private static void SaveConfig(ConfigData configuration)
        {
            Settings.Default.ThumbnailViewCount = configuration.ThumbnailViewCount;
            Settings.Default.Save();
        }

        private void UpdateProgress(
            string statusInfo,
            int counter,
            int maxCount,
            ProgressStyle style)
        {
            LblStatusInfo.Text = string.Format(
                statusInfo,
                counter,
                maxCount);

            // Regular progress (searching duplicates)
            // Marquee (conecting, monitoring) [style = marquee, max = 1]
            // Off (invalid configuration) [value = 0, max = 0]
            if (style == ProgressStyle.NoProgress)
            {
                TaskbarManager.Instance.SetProgressState(
                    TaskbarProgressBarState.NoProgress,
                    Handle);
                ProgressBar.Style = ProgressBarStyle.Continuous;
                ProgressBar.Maximum = 0;
                ProgressBar.Value = 0;
            }
            else if (style == ProgressStyle.Continuous)
            {
                TaskbarManager.Instance.SetProgressState(
                    TaskbarProgressBarState.Normal,
                    Handle);
                TaskbarManager.Instance.SetProgressValue(
                    counter,
                    maxCount,
                    Handle);
                ProgressBar.Style = ProgressBarStyle.Continuous;
                ProgressBar.Maximum = maxCount;
                ProgressBar.Value = counter;
            }
            else if (style == ProgressStyle.Marquee)
            {
                TaskbarManager.Instance.SetProgressState(
                    TaskbarProgressBarState.Indeterminate,
                    Handle);
                ProgressBar.Style = ProgressBarStyle.Marquee;
                ProgressBar.Maximum = 1;
                ProgressBar.Value = 0;
            }
            else
            {
                Debug.Assert(true);
            }
        }

        private void BtnDedup_Click(object sender, EventArgs e)
        {
            TxtLog.Clear();
            //Dedupper = new DedupEngine(Configuration);
            //Dedupper.ProgressUpdate += (s, args) => UpdateProgress(args.StatusInfo,
            //    args.Counter,
            //    args.MaxCount);
            //Dedupper.DuplicateCountChanged += (s, args) =>
            //{
            //    LblDuplicateCount.InvokeIfRequired(() =>
            //        LblDuplicateCount.Text = string.Format(StatusInfoDuplicateCount, args.Count));
            //    BtnResolveDuplicates.InvokeIfRequired(() =>
            //        BtnResolveDuplicates.Enabled = args.Count > 0);
            //};
            //Dedupper.Logged += (s, args) =>
            //    TxtLog.InvokeIfRequired(() =>
            //        TxtLog.AppendText(args.Message + Environment.NewLine));

            //BtnDedup.Enabled = false;
            //BtnCancel.Enabled = true;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            //ProgressBar.Style = ProgressBarStyle.Marquee;
            //ProgressBar.Value = 0;
            //ProgressBar.Maximum = 0;
            //Dedupper.Dispose();
            //BtnDedup.Enabled = true;
            //BtnCancel.Enabled = false;
        }

        private void BtnConfig_Click(object sender, EventArgs e)
        {
            using (var dlg = new ConfigDlg())
            {
                dlg.ClientConfig = Configuration;
                dlg.ServerConfig = WcfProxy.GetConfig();

                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                Configuration = dlg.ClientConfig;
                SaveConfig(Configuration);
                WcfProxy.SetConfig(dlg.ServerConfig);
            }
        }

        private void BtnResolveConflicts_Click(object sender, EventArgs e)
        {
            //while (Dedupper.DequeueDuplcate(out var duplicate))
            //{
            //    (var left, var right) = duplicate;
            //    // Mostely because we might have deleted
            //    // this file during the previous compare
            //    if (!File.Exists(left.FilePath))
            //    {
            //        Debug.Print($"{left.FilePath} doesn't exist anymore. Can't compare.");
            //        continue;
            //    }
            //    if (!File.Exists(right.FilePath))
            //    {
            //        Debug.Print($"{right.FilePath} doesn't exist anymore. Can't compare.");
            //        continue;
            //    }

            //    using (var dlg = new FileComparison())
            //    {
            //        DialogResult result;
            //        lock (left)
            //            lock (right)
            //            {
            //                dlg.LeftFile = left;
            //                dlg.RightFile = right;
            //                result = dlg.ShowDialog();
            //                left.DisposeThumbnails();
            //                right.DisposeThumbnails();
            //            }

            //        if (result == DialogResult.Yes)
            //        {
            //            continue;
            //        }
            //        if (result == DialogResult.Cancel)
            //        {
            //            Dedupper.EnqueueDuplicate(duplicate);
            //            return;
            //        }
            //        if (result == DialogResult.No) // Skip
            //        {
            //            Dedupper.EnqueueDuplicate(duplicate);
            //        }
            //    }
            //}
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

            //if (ElapsedTimer.Enabled)
            //{
            //    var selection = MessageBox.Show(
            //        $"VideoDedup is currently search for duplicates." +
            //        $"{Environment.NewLine}Are you sure you want to close?",
            //        "Cancel search?",
            //        MessageBoxButtons.YesNo,
            //        MessageBoxIcon.Warning);

            //    if (selection == DialogResult.No)
            //    {
            //        e.Cancel = true;
            //        return;
            //    }
            //}
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
