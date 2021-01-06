namespace VideoDedup
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.Windows.Forms;
    using VideoDedupShared.ISynchronizeInvokeExtensions;
    using VideoDedup.Properties;
    using Microsoft.WindowsAPICodePack.Taskbar;
    using VideoDedupShared;

    public partial class VideoDedupDlg : Form
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
                        var binding = new NetTcpBinding
                        {
                            MaxReceivedMessageSize = int.MaxValue,
                            MaxBufferSize = int.MaxValue
                        };

                        var baseAddress = new Uri("net.tcp://localhost:41721/VideoDedup");
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

        private static LogToken logToken = null;

        private TimeSpan ElapsedTime { get; set; } = new TimeSpan();

        private int DuplicateCount { get; set; } = 0;

        private SmartTimer.Timer statusTimer = null;

        private ConfigData Configuration { get; set; }

        public VideoDedupDlg() => InitializeComponent();

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

                    DuplicateCount = status.DuplicateCount;
                    LblDuplicateCount.Text = string.Format(
                        StatusInfoDuplicateCount,
                        DuplicateCount);
                    BtnResolveDuplicates.Enabled = DuplicateCount > 0;
                    BtnDiscardDuplicates.Enabled = DuplicateCount > 0;

                    if (logToken != null && logToken.Id != logData.LogToken.Id)
                    {
                        TxtLog.Clear();
                    }
                    logToken = logData.LogToken;
                    foreach (var log in logData.LogItems)
                    {
                        if (!string.IsNullOrWhiteSpace(TxtLog.Text))
                        {
                            TxtLog.AppendText(Environment.NewLine + log);
                        }
                        else
                        {
                            TxtLog.AppendText(log);
                        }
                    }
                });
            }
            catch (Exception ex) when (
                ex is EndpointNotFoundException
                || ex is CommunicationException)
            {
                this.InvokeIfRequired(() =>
                {
                    BtnResolveDuplicates.Enabled = false;
                    UpdateProgress("Connecting...",
                        0, 0, ProgressStyle.Marquee);
                });
            }
            finally
            {
                _ = statusTimer.StartSingle(StatusTimerInterval);
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
            //Count = Settings.Default.ThumbnailViewCount,
        };

        private static void SaveConfig(ConfigData configuration)
        {
            //Settings.Default.ThumbnailViewCount = configuration.Count;
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

        private void BtnServerConfig_Click(object sender, EventArgs e)
        {
            using (var dlg = new ServerConfigDlg())
            {
                dlg.ServerConfig = WcfProxy.GetConfig();

                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                SaveConfig(Configuration);
                WcfProxy.SetConfig(dlg.ServerConfig);
            }
        }

        private void BtnResolveConflicts_Click(object sender, EventArgs e)
        {
            try
            {
                while (true)
                {
                    var duplicate = WcfProxy.GetDuplicate();
                    if (duplicate == null)
                    {
                        return;
                    }

                    using (var dlg = new FileComparisonDlg())
                    {
                        DialogResult result;
                        dlg.LeftFile = duplicate.File1;
                        dlg.RightFile = duplicate.File2;
                        result = dlg.ShowDialog();

                        if (result == DialogResult.Cancel)
                        {
                            WcfProxy.ResolveDuplicate(
                                duplicate.DuplicateId,
                                ResolveOperation.Cancel);
                            return;
                        }
                        WcfProxy.ResolveDuplicate(
                              duplicate.DuplicateId,
                              dlg.ResolveOperation);
                    }
                }
            }
            catch (Exception ex) when (
              ex is EndpointNotFoundException
              || ex is CommunicationException)
            {
                Debug.Print(ex.Message);
            }
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e) =>
            Application.Exit();

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new AboutDlg())
            {
                _ = dlg.ShowDialog();
            }
        }

        private void ServerConfigurationToolStripMenuItem_Click(object sender, EventArgs e) =>
            BtnServerConfig.PerformClick();

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

        private void BtnDiscard_Click(object sender, EventArgs e)
        {
            var selection = MessageBox.Show(
                $"There are {DuplicateCount} duplicates to resolve." +
                $"{Environment.NewLine}Are you sure you want to discard them?",
                "Discard duplicates?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (selection == DialogResult.No)
            {
                return;
            }

            try
            {
                WcfProxy.DiscardDuplicates();
            }
            catch (Exception ex) when (
               ex is EndpointNotFoundException
               || ex is CommunicationException)
            { }
        }
    }
}
