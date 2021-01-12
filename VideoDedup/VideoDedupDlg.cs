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
    using System.Threading.Tasks;
    using System.Collections.Concurrent;
    using System.Linq;
    using VideoDedupShared.DataGridViewExtension;
    using VideoDedupShared.TimeSpanExtension;

    public partial class VideoDedupDlg : Form
    {
        private static readonly string StatusInfoDuplicateCount =
            "Duplicates found {0}";

        private static readonly TimeSpan WcfTimeout = TimeSpan.FromSeconds(10);

        private WcfProxy WcfProxy
        {
            get
            {
                lock (wcfProxyLock)
                {

                    // If server address changed
                    // or connection issue occurred.
                    if (wcfProxy != null
                        && (wcfProxy.Endpoint.Address.Uri.Host
                            != Configuration.ServerAddress
                        || wcfProxy.InnerChannel.State
                            == CommunicationState.Faulted))
                    {
                        // https://stackoverflow.com/questions/2008382/how-to-heal-faulted-wcf-channels
                        wcfProxy.Abort();
                        wcfProxy = null;
                    }


                    if (wcfProxy == null)
                    {
                        var binding = new NetTcpBinding
                        {
                            MaxReceivedMessageSize = int.MaxValue,
                            MaxBufferSize = int.MaxValue,
                            OpenTimeout = WcfTimeout,
                            CloseTimeout = WcfTimeout,
                            ReceiveTimeout = WcfTimeout,
                            SendTimeout = WcfTimeout,
                        };

                        var baseAddress = new Uri(
                            $"net.tcp://{Configuration.ServerAddress}:41721/VideoDedup");
                        var address = new EndpointAddress(baseAddress);
                        wcfProxy = new WcfProxy(binding, address);
                    }
                    return wcfProxy;
                }
            }
        }
        private WcfProxy wcfProxy = null;
        private readonly object wcfProxyLock = new object();

        private Guid? logToken;
        private ConcurrentDictionary<int, LogEntry> LogEntries { get; } =
            new ConcurrentDictionary<int, LogEntry>();

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
            UpdateOperation(new OperationInfo
            {
                Message = "Connecting...",
                ProgressStyle = ProgressStyle.Marquee,
            });
            statusTimer.StartSingle(0);

            base.OnLoad(e);
        }

        private static ConfigData LoadConfig() => new ConfigData
        {
            ServerAddress = Settings.Default.ServerAddress,
            StatusRequestInterval = TimeSpan.FromMilliseconds(
                Settings.Default.StatusRequestInterval),
            ClientSourcePath = Settings.Default.ClientSourcePath,
        };

        private static void SaveConfig(ConfigData configuration)
        {
            Settings.Default.ServerAddress = configuration.ServerAddress;
            Settings.Default.StatusRequestInterval =
                (int)configuration.StatusRequestInterval.TotalMilliseconds;
            Settings.Default.ClientSourcePath = configuration.ClientSourcePath;
            Settings.Default.Save();
        }

        private void StatusTimerCallback(object param)
        {
            try
            {
                var status = WcfProxy.GetCurrentStatus();

                this.InvokeIfRequired(() =>
                {
                    if (logToken != status.LogToken)
                    {
                        // Has to be in this order
                        // Otherwise clearing the DGV causes cell value requests
                        // in this thread before continuing here.
                        // Which has a high chance of adding items in the
                        // LogEntries list, which are subsequently cleared,
                        // leaving empty cells in the DGV that won't be filled.
                        DgvLog.RowCount = 0;
                        LogEntries.Clear();
                    }
                    logToken = status.LogToken;

                    // If we are scrolled down, we auto scroll
                    var prevRowCount = DgvLog.RowCount;
                    DgvLog.RowCount = status.LogCount;
                    if (DgvLog.GetLastDisplayedScrollingRowIndex(false)
                        == prevRowCount - 1)
                    {
                        DgvLog.FirstDisplayedScrollingRowIndex =
                            DgvLog.RowCount - 1;
                    }

                    UpdateOperation(status.Operation);

                    DuplicateCount = status.DuplicateCount;
                    LblDuplicateCount.Text = string.Format(
                        StatusInfoDuplicateCount,
                        DuplicateCount);
                    BtnResolveDuplicates.Enabled = DuplicateCount > 0;
                    BtnDiscardDuplicates.Enabled = DuplicateCount > 0;
                });
            }
            catch (Exception ex) when (
                ex is EndpointNotFoundException
                || ex is CommunicationException
                || ex is TimeoutException)
            {
                Debug.Print("Status request failed with: " + ex.Message);
                this.InvokeIfRequired(() =>
                {
                    BtnResolveDuplicates.Enabled = false;
                    UpdateOperation(new OperationInfo
                    {
                        Message = "Connecting...",
                        ProgressStyle = ProgressStyle.Marquee,
                    });
                });
            }
            finally
            {
                _ = statusTimer.StartSingle(Configuration.StatusRequestInterval);
            }
        }

        private void UpdateOperation(OperationInfo operation)
        {
            LblStatusInfo.Text = string.Format(
                operation.Message,
                operation.CurrentProgress,
                operation.MaximumProgress);

            if (operation.StartTime == DateTime.MinValue)
            {
                LblTimer.Text = "";
            }
            else
            {
                var duration = DateTime.Now - operation.StartTime;
                LblTimer.Text = duration.ToPrettyString();
            }

            // Off (invalid configuration) [value = 0, max = 0]
            // Continuous (searching duplicates)
            // Marquee (conecting, monitoring) [style = marquee, max = 1]
            if (operation.ProgressStyle == ProgressStyle.NoProgress)
            {
                TaskbarManager.Instance.SetProgressState(
                    TaskbarProgressBarState.NoProgress,
                    Handle);
                ProgressBar.Style = ProgressBarStyle.Continuous;
                ProgressBar.Maximum = 0;
                ProgressBar.Value = 0;
            }
            else if (operation.ProgressStyle == ProgressStyle.Continuous)
            {
                TaskbarManager.Instance.SetProgressState(
                    TaskbarProgressBarState.Normal,
                    Handle);
                TaskbarManager.Instance.SetProgressValue(
                    operation.CurrentProgress,
                    operation.MaximumProgress,
                    Handle);
                ProgressBar.Style = ProgressBarStyle.Continuous;
                ProgressBar.Maximum = operation.MaximumProgress;
                ProgressBar.Value = operation.CurrentProgress;
            }
            else if (operation.ProgressStyle == ProgressStyle.Marquee)
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

        private void DgvLog_CellValueNeeded(object sender,
            DataGridViewCellValueEventArgs e)
        {
            var logIndex = e.RowIndex;

            if (LogEntries.TryGetValue(logIndex, out var logEntry))
            {
                if (logEntry.Status == LogEntryStatus.Present)
                {
                    e.Value = logEntry.Message;
                }
                return;
            }

            e.Value = "";

            if (!logToken.HasValue)
            {
                return;
            }

            // Find first displayed row that has no log present or requested
            var start = logIndex;
            for (; start > DgvLog.FirstDisplayedScrollingRowIndex; start--)
            {
                if (LogEntries.TryGetValue(start - 1, out var _))
                {
                    break;
                }
            }

            // Find last displayed row that has no log present or requested
            var end = logIndex;
            for (; end < DgvLog.GetLastDisplayedScrollingRowIndex(true); end++)
            {
                if (LogEntries.TryGetValue(end + 1, out var _))
                {
                    break;
                }
            }

            var count = end - start + 1;

            RequestLogEntries(logToken.Value, start, count);
        }

        private async void RequestLogEntries(Guid logToken, int start, int count)
        {
            foreach (var index in Enumerable.Range(start, count))
            {
                _ = LogEntries.TryAdd(index, new LogEntry
                {
                    Status = LogEntryStatus.Requested,
                    Message = "",
                });
            }

            try
            {
                var logData = await Task.Factory.StartNew(() =>
                    WcfProxy.GetLogEntries(logToken, start, count));

                if (start + count > DgvLog.RowCount)
                {
                    return;
                }

                var logIndex = start;
                foreach (var logEntry in logData.LogEntries)
                {
                    _ = LogEntries[logIndex++] = new LogEntry
                    {
                        Status = LogEntryStatus.Present,
                        Message = logEntry,
                    };
                }

                foreach (var index in Enumerable.Range(start, count))
                {
                    DgvLog.UpdateCellValue(0, index);
                }
            }
            catch (Exception ex) when (
                ex is EndpointNotFoundException
                || ex is CommunicationException)
            {
                foreach (var index in Enumerable.Range(start, count))
                {
                    LogEntries.TryRemove(index, out var _);
                }
            }
        }

        private void BtnServerConfig_Click(object sender, EventArgs e)
        {
            using (var dlg = new ServerConfigDlg())
            {
                try
                {
                    dlg.ServerConfig = WcfProxy.GetConfig();
                }
                catch (Exception ex) when (
                    ex is EndpointNotFoundException
                    || ex is CommunicationException)
                {
                    MessageBox.Show(
                        "Unable to retrieve configuration from server.",
                        "Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                while (true)
                {
                    try
                    {
                        WcfProxy.SetConfig(dlg.ServerConfig);
                        return;
                    }
                    catch (Exception ex) when (
                        ex is EndpointNotFoundException
                        || ex is CommunicationException)
                    {
                        if (MessageBox.Show(
                            $"Unable to send configuration to the server." +
                                $"{Environment.NewLine}Do you want to try again?",
                            "Connection Error",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Error)
                            == DialogResult.No)
                        {
                            return;
                        }
                    }
                }
            }
        }

        private void BtnClientConfig_Click(object sender, EventArgs e)
        {
            using (var dlg = new ClientConfigDlg())
            {
                dlg.Configuration = Configuration;

                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                Configuration = dlg.Configuration;
                SaveConfig(Configuration);
                _ = statusTimer.StartSingle(Configuration.StatusRequestInterval);
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
                        dlg.ServerSourcePath = duplicate.BasePath;
                        dlg.Configuration = Configuration;
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
                MessageBox.Show(
                    $"Unable to process duplicate.{Environment.NewLine}" +
                        $"Connection to server failed.",
                    "Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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

        private void ServerConfigurationToolStripMenuItem_Click(object sender,
            EventArgs e) =>
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

        private void BtnToDoManager_Click(object sender, EventArgs e)
        {
            using (var dlg = new ToDoManager.ToDoManager())
            {
                _ = dlg.ShowDialog();
            }
        }
    }
}
