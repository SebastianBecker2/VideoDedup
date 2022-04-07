namespace VideoDedupClient.Dialogs
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Forms;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using Grpc.Net.Client;
    using VideoDedupGrpc;
    using VideoDedupSharedLib.ExtensionMethods.DataGridViewExtensions;
    using VideoDedupSharedLib.ExtensionMethods.ISynchronizeInvokeExtensions;
    using static VideoDedupGrpc.OperationInfo.Types;
    using static VideoDedupGrpc.VideoDedupGrpcService;

    public partial class VideoDedupDlg : Form
    {
        private static GrpcChannel? grpcChannel;
        private static readonly object GrpcClientLock = new();
        internal static VideoDedupGrpcServiceClient GrpcClient
        {
            get
            {
                lock (GrpcClientLock)
                {
                    grpcChannel ??= GrpcChannel.ForAddress(
                        $"http://{Settings.ServerAddress}:41722",
                        new GrpcChannelOptions
                        {
                            MaxReceiveMessageSize = null,
                        });
                    return new VideoDedupGrpcServiceClient(grpcChannel);
                }
            }
        }

        private string? logToken;
        private ConcurrentDictionary<int, LogEntry> LogEntries { get; } = new();

        private int DuplicateCount { get; set; }

        private SmartTimer.Timer StatusTimer { get; }

        private static ConfigData Settings { get; set; } = LoadConfig();

        public VideoDedupDlg()
        {
            InitializeComponent();
            StatusTimer = new SmartTimer.Timer(StatusTimerCallback);
        }

        protected override void OnLoad(EventArgs e)
        {
            StiProgress.UpdateStatusInfo(new OperationInfo
            {
                OperationType = OperationType.Connecting,
                ProgressStyle = ProgressStyle.Marquee,
            });
            _ = StatusTimer.StartSingle(0);

            base.OnLoad(e);
        }

        private static ConfigData LoadConfig() => new()
        {
            ServerAddress = Properties.Settings.Default.ServerAddress,
            StatusRequestInterval = TimeSpan.FromMilliseconds(
                Properties.Settings.Default.StatusRequestInterval),
            ClientSourcePath = Properties.Settings.Default.ClientSourcePath,
        };

        private static void SaveConfig(ConfigData settings)
        {
            Properties.Settings.Default.ServerAddress = settings.ServerAddress;
            Properties.Settings.Default.StatusRequestInterval =
                (int)settings.StatusRequestInterval.TotalMilliseconds;
            Properties.Settings.Default.ClientSourcePath =
                settings.ClientSourcePath;
            Properties.Settings.Default.Save();
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

                    DuplicateCount = status.DuplicateCount;
                    StiProgress.UpdateStatusInfo(
                        status.OperationInfo,
                        DuplicateCount);
                    BtnResolveDuplicates.Enabled = DuplicateCount > 0;
                    BtnDiscardDuplicates.Enabled = DuplicateCount > 0;
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
                    });
                });
            }
            finally
            {
                _ = StatusTimer.StartSingle(Settings.StatusRequestInterval);
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

        private void BtnClientConfig_Click(object sender, EventArgs e)
        {
            using var dlg = new ClientConfigDlg { Settings = Settings, };

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            Settings = dlg.Settings;
            SaveConfig(Settings);
            _ = StatusTimer.StartSingle(Settings.StatusRequestInterval);
        }

        private void BtnResolveConflicts_Click(object sender, EventArgs e)
        {
            try
            {
                while (true)
                {
                    var duplicate = GrpcClient.GetDuplicate(new Empty());
                    if (string.IsNullOrWhiteSpace(duplicate.DuplicateId))
                    {
                        return;
                    }

                    using var dlg = new VideoComparisonDlg();
                    dlg.LeftFile = duplicate.File1;
                    dlg.RightFile = duplicate.File2;
                    dlg.ServerSourcePath = duplicate.BasePath;
                    dlg.ClientSourcePath = Settings.ClientSourcePath;

                    var result = dlg.ShowDialog();

                    if (result == DialogResult.Cancel)
                    {
                        _ = GrpcClient.ResolveDuplicate(new ResolveDuplicateRequest
                        {
                            DuplicateId = duplicate.DuplicateId,
                            ResolveOperation =
                                ResolveDuplicateRequest.Types.ResolveOperation.Cancel,
                        });
                        return;
                    }

                    _ = GrpcClient.ResolveDuplicate(new ResolveDuplicateRequest
                    {
                        DuplicateId = duplicate.DuplicateId,
                        ResolveOperation = dlg.ResolveOperation,
                    });
                }
            }
            catch (Exception)
            {
                _ = MessageBox.Show(
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
                _ = GrpcClient.DiscardDuplicates(new Empty());
            }
            catch (Exception) { }
        }

        private void VideoDedupDlg_Load(object sender, EventArgs e)
        {

        }
    }
}
