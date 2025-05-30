namespace VideoDedupClient.Controls.SystemInfo
{
    using System;
    using System.Text.Json;
    using System.Windows.Forms;
    using VideoDedupGrpc;

    public partial class SystemInfoCtrl : UserControl
    {
        private static readonly JsonSerializerOptions SerializationOptiosn =
            new()
            {
                WriteIndented = true
            };

        public SystemInfoCtrl() => InitializeComponent();

        public void ShowSystemInfo(GetSystemInfoResponse? systemInfo)
        {
            if (systemInfo is null)
            {
                return;
            }

            DgvSystemInfo.Rows.Clear();
            DgvSystemInfo.Tag = systemInfo;

            _ = DgvSystemInfo.Rows.Add("Processor Count", $"{systemInfo.ProcessorCount}");
            _ = DgvSystemInfo.Rows.Add("Machine Name", systemInfo.MachineName);
            _ = DgvSystemInfo.Rows.Add("OS Version", systemInfo.OsVersion);
            _ = DgvSystemInfo.Rows.Add("OS Description", systemInfo.OsDescription);
            _ = DgvSystemInfo.Rows.Add("OS Architecture", systemInfo.OsArchitecture);
            _ = DgvSystemInfo.Rows.Add("System Uptime", systemInfo.Uptime);
            _ = DgvSystemInfo.Rows.Add("Framework Description", systemInfo.FrameworkDescription);
            _ = DgvSystemInfo.Rows.Add("Runtime Identifier", systemInfo.RuntimeIdentifier);
            _ = DgvSystemInfo.Rows.Add("Process ID", systemInfo.ProcessId);
            _ = DgvSystemInfo.Rows.Add("Process Architecture", systemInfo.ProcessArchitecture);
            _ = DgvSystemInfo.Rows.Add("Process Path", systemInfo.ProcessPath);
            _ = DgvSystemInfo.Rows.Add("Username", systemInfo.Username);

            _ = DgvSystemInfo.Rows.Add("", "");

            _ = DgvSystemInfo.Rows.Add("Network Adapters", "");
            var firstAdapter = true;
            foreach (var networkAdapters in systemInfo.NetworkAdapters)
            {
                if (!firstAdapter)
                {
                    _ = DgvSystemInfo.Rows.Add("", "");
                }
                firstAdapter = false;
                _ = DgvSystemInfo.Rows.Add("    Name", networkAdapters.Name);
                _ = DgvSystemInfo.Rows.Add("    Status", networkAdapters.Status);
                _ = DgvSystemInfo.Rows.Add("    Type", networkAdapters.Type);
                _ = DgvSystemInfo.Rows.Add("    Mac", networkAdapters.Mac);
                var firstIp = true;
                foreach (var ipAddress in networkAdapters.IpAddresses)
                {
                    if (firstIp)
                    {
                        firstIp = false;
                        _ = DgvSystemInfo.Rows.Add("    IP Addresses", ipAddress);
                        continue;
                    }
                    _ = DgvSystemInfo.Rows.Add("", ipAddress);
                }
            }
        }

        private void BtnCopyToClipboard_Click(object sender, EventArgs e)
        {
            if (DgvSystemInfo.Tag is not GetSystemInfoResponse systemInfo)
            {
                return;
            }

            var json = JsonSerializer.Serialize(systemInfo, SerializationOptiosn);
            Clipboard.SetText(json);
        }
    }
}
