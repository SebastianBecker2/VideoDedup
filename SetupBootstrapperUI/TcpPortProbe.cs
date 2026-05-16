namespace SetupBootstrapperUI
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.ServiceProcess;
    using Microsoft.Win32;

    internal static class TcpPortProbe
    {
        /// <summary>Windows service name; matches <c>ServerComponent</c> in Setup/Config.wxi.</summary>
        private const string ServerServiceName = "VideoDedupServer";

        private const string VideoDedupServiceProcessName = "VideoDedupService";

        private const string ServerRegistryKey =
            @"SOFTWARE\SebastianBecker\VideoDedup\Server";

        public static bool IsTcpPortInUse(int port)
        {
            try
            {
                var listeners = IPGlobalProperties.GetIPGlobalProperties()
                    .GetActiveTcpListeners();
                return listeners.Any(endpoint => endpoint.Port == port);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// True when the port is held by VideoDedup (running service on the same port).
        /// </summary>
        public static bool IsTcpPortUsedByVideoDedupServer(int port)
        {
            if (TryGetListeningProcessId(port) is int pid
                && IsVideoDedupServiceProcess(pid))
            {
                return true;
            }

            return IsVideoDedupServerServiceRunning()
                && TryGetInstalledListenPort() == port;
        }

        public static bool ShouldWarnPortInUse(int port) =>
            IsTcpPortInUse(port) && !IsTcpPortUsedByVideoDedupServer(port);

        private static bool IsVideoDedupServerServiceRunning()
        {
            try
            {
                using (var service = new ServiceController(ServerServiceName))
                {
                    return service.Status == ServiceControllerStatus.Running
                        || service.Status == ServiceControllerStatus.StartPending;
                }
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static int? TryGetInstalledListenPort()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(
                    ServerRegistryKey,
                    false))
                {
                    var value = key?.GetValue("ListenPort");
                    if (value is int port)
                    {
                        return port;
                    }

                    if (value != null
                        && int.TryParse(value.ToString(), out port))
                    {
                        return port;
                    }
                }
            }
            catch
            {
                // ignore
            }

            return null;
        }

        private static bool IsVideoDedupServiceProcess(int processId)
        {
            try
            {
                using (var process = Process.GetProcessById(processId))
                {
                    return process.ProcessName.Equals(
                        VideoDedupServiceProcessName,
                        StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
                return false;
            }
        }

        private static int? TryGetListeningProcessId(int port)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "netstat.exe",
                    Arguments = "-ano",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using (var process = Process.Start(startInfo))
                {
                    if (process is null)
                    {
                        return null;
                    }

                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit(5000);

                    foreach (var line in output.Split(
                        new[] { '\r', '\n' },
                        StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (line.IndexOf("LISTENING", StringComparison.OrdinalIgnoreCase) < 0
                            && line.IndexOf("ABH", StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            continue;
                        }

                        var parts = line.Split(
                            new[] { ' ' },
                            StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 4)
                        {
                            continue;
                        }

                        var localEndpoint = parts[1];
                        if (!TryParsePortFromLocalEndpoint(localEndpoint, out var localPort)
                            || localPort != port)
                        {
                            continue;
                        }

                        if (int.TryParse(parts[parts.Length - 1], out var pid))
                        {
                            return pid;
                        }
                    }
                }
            }
            catch
            {
                // ignore
            }

            return null;
        }

        private static bool TryParsePortFromLocalEndpoint(
            string localEndpoint,
            out int port)
        {
            port = 0;
            if (string.IsNullOrEmpty(localEndpoint))
            {
                return false;
            }

            var endpoint = localEndpoint;
            if (endpoint.StartsWith("[", StringComparison.Ordinal))
            {
                var closingBracket = endpoint.IndexOf(']');
                if (closingBracket < 0)
                {
                    return false;
                }

                endpoint = endpoint.Substring(closingBracket + 1);
            }

            var colonIndex = endpoint.LastIndexOf(':');
            if (colonIndex < 0 || colonIndex == endpoint.Length - 1)
            {
                return false;
            }

            return int.TryParse(
                endpoint.Substring(colonIndex + 1),
                out port);
        }
    }
}
