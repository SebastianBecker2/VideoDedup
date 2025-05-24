namespace VideoDedupClient.Dialogs
{
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using SmartTimer;
    using VideoDedupGrpc;
    using VideoDedupClient.ExtensionMethods.DataGridViewExtensions;
    using VideoDedupClient.ExtensionMethods.ISynchronizeInvokeExtensions;
    using static VideoDedupGrpc.OperationInfo.Types;
    using static VideoDedupGrpc.VideoDedupGrpcService;

    public partial class VideoDedupDlg : Form
    {
        private static VideoDedupGrpcServiceClient GrpcClient =>
            Program.GrpcClient;

        private string? logToken;
        private ConcurrentDictionary<int, LogEntry> LogEntries { get; } = new();

        private int CurrentDuplicateCount { get; set; }

        private Timer StatusTimer { get; }

        public VideoDedupDlg()
        {
            InitializeComponent();
            StatusTimer = new Timer(StatusTimerCallback);
        }

        protected override void OnLoad(EventArgs e)
        {
            StiProgress.UpdateStatusInfo(new OperationInfo
            {
                OperationType = OperationType.Connecting,
                ProgressStyle = ProgressStyle.Marquee,
                StartTime = Timestamp.FromDateTime(DateTime.UtcNow),
            });
            _ = StatusTimer.StartSingle(0);

            base.OnLoad(e);
        }

        private void StatusTimerCallback(object? param)
        {
            try
            {
                var status = GrpcClient.GetCurrentStatus(new Empty());

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
                    if (DgvLog.GetLastDisplayedScrollingRowIndex(false) + 1
                        >= prevRowCount)
                    {
                        DgvLog.FirstDisplayedScrollingRowIndex =
                            DgvLog.RowCount - 1;
                    }

                    StiProgress.UpdateStatusInfo(
                        status.OperationInfo,
                        status.TotalDuplicatesCount,
                        status.PreparedDuplicatesCount);

                    CurrentDuplicateCount = status.TotalDuplicatesCount;
                    BtnResolveDuplicates.Enabled =
                        status.PreparedDuplicatesCount > 0;
                    BtnDiscardDuplicates.Enabled = BtnResolveDuplicates.Enabled;
                });
            }
            catch (RpcException ex)
            {
                Debug.Print("Status request failed with: " + ex.Message);
                this.InvokeIfRequired(() =>
                {
                    BtnResolveDuplicates.Enabled = false;
                    StiProgress.UpdateStatusInfo(new OperationInfo
                    {
                        OperationType = OperationType.Connecting,
                        ProgressStyle = ProgressStyle.Marquee,
                        StartTime = Timestamp.FromDateTime(DateTime.UtcNow),
                    });
                });
            }
            finally
            {
                _ = StatusTimer.StartSingle(
                    Program.Configuration.StatusRequestInterval);
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

            if (logToken is null)
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

            RequestLogEntries(logToken, start, count);
        }

        private async void RequestLogEntries(string logToken, int start, int count)
        {
            foreach (var index in Enumerable.Range(start, count))
            {
                _ = LogEntries.TryAdd(index, new LogEntry());
            }

            try
            {
                var logData = await
                    GrpcClient.GetLogEntriesAsync(new GetLogEntriesRequest
                    {
                        Count = count,
                        LogToken = logToken,
                        Start = start,
                    });

                if (start + count > DgvLog.RowCount)
                {
                    return;
                }

                var logIndex = start;
                foreach (var logEntry in logData.LogEntries)
                {
                    _ = LogEntries[logIndex++] = new LogEntry(logEntry);
                }

                foreach (var index in Enumerable.Range(start, count))
                {
                    DgvLog.UpdateCellValue(0, index);
                }
            }
            catch (Exception)
            {
                foreach (var index in Enumerable.Range(start, count))
                {
                    _ = LogEntries.TryRemove(index, out _);
                }
            }
        }

        private void BtnServerConfig_Click(object sender, EventArgs e)
        {
            using var dlg = new ServerConfigDlg();
            try
            {
                dlg.ConfigurationSettings =
                    GrpcClient.GetConfiguration(new Empty());
            }
            catch (Exception)
            {
                _ = MessageBox.Show(
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
                    _ = GrpcClient.SetConfiguration(dlg.ConfigurationSettings);
                    return;
                }
                catch (Exception)
                {
                    if (MessageBox.Show(
                            "Unable to send configuration to the server." +
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

        private void BtnClientConfig_Click(object sender, EventArgs e)
        {
            using var dlg = new ClientConfigDlg
            {
                Configuration = Program.Configuration,
            };

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            Program.Configuration = dlg.Configuration;
            Program.SaveConfig();
            _ = StatusTimer.StartSingle(
                Program.Configuration.StatusRequestInterval);
        }

        private void BtnResolveConflicts_Click(object sender, EventArgs e)
        {
            using var dlg = new ResolveDuplicateDlg();
            var result = dlg.ShowDialog();
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e) =>
            Application.Exit();

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var dlg = new AboutDlg();
            _ = dlg.ShowDialog();
        }

        private void ClientConfigurationToolStripMenuItem_Click(
            object sender,
            EventArgs e) =>
            BtnClientConfig.PerformClick();

        private void ServerConfigurationToolStripMenuItem_Click(
            object sender,
            EventArgs e) =>
            BtnServerConfig.PerformClick();

        private void BtnDiscard_Click(object sender, EventArgs e)
        {
            var selection = MessageBox.Show(
                $"There are {CurrentDuplicateCount} duplicates to resolve." +
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
                _ = GrpcClient.DiscardDuplicates(new Empty());
            }
            catch (Exception) { }
        }

        private void VideoDedupDlg_Load(object sender, EventArgs e)
        {

        }
    }
}
