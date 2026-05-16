namespace VideoDedupService
{
    using System.Runtime.InteropServices;

    internal static class LinuxHostBootstrap
    {
        internal const string EtcEnvPath = "/etc/videodedupserver/env";

        internal const string EtcTlsEnvPath = "/etc/videodedupserver/tls.env";

        /// <summary>
        /// Loads KEY=value lines from distro env files before configuration binds.
        /// Later sources (e.g. systemd EnvironmentFile) may override; this covers manual runs.
        /// </summary>
        internal static void LoadEtcEnvironmentFile()
        {
            if (!OperatingSystem.IsLinux())
            {
                return;
            }

            LoadKeyValueEnvFile(EtcEnvPath);
            LoadKeyValueEnvFile(EtcTlsEnvPath);
        }

        private static void LoadKeyValueEnvFile(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            foreach (var line in File.ReadLines(path))
            {
                var trimmed = line.Trim();
                if (trimmed.Length == 0 || trimmed[0] == '#')
                {
                    continue;
                }

                var eq = trimmed.IndexOf('=');
                if (eq <= 0)
                {
                    continue;
                }

                var key = trimmed[..eq].Trim();
                var value = Unquote(trimmed[(eq + 1)..].Trim());
                if (key.Length > 0)
                {
                    Environment.SetEnvironmentVariable(key, value);
                }
            }
        }

        /// <summary>
        /// Refuse accidental root execution on Linux (packaged service runs as videodedup).
        /// </summary>
        internal static void EnsureNotRunningAsRootUnlessAllowed()
        {
            if (!OperatingSystem.IsLinux())
            {
                return;
            }

            if (geteuid() != 0)
            {
                return;
            }

            if (string.Equals(
                    Environment.GetEnvironmentVariable("VIDEODEDUP_ALLOW_ROOT"),
                    "1",
                    StringComparison.Ordinal))
            {
                return;
            }

            Console.Error.WriteLine(
                "videodedupserver: refusing to run as root. Run as user 'videodedup' " +
                "or set VIDEODEDUP_ALLOW_ROOT=1 for debugging only.");
            Environment.Exit(78);
        }

        private static string Unquote(string value)
        {
            if (value.Length >= 2 &&
                ((value[0] == '"' && value[^1] == '"') ||
                 (value[0] == '\'' && value[^1] == '\'')))
            {
                return value[1..^1];
            }

            return value;
        }

        [DllImport("libc", EntryPoint = "geteuid")]
        private static extern uint geteuid();
    }
}
