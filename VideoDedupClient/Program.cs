namespace VideoDedupClient
{
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using Dialogs;
    using Grpc.Core;
    using Grpc.Net.Client;
    using Grpc.Net.Client.Configuration;
    using Properties;
    using static VideoDedupGrpc.VideoDedupGrpcService;

    internal static class Program
    {
        private static GrpcChannel? grpcChannel;
        private static SocketsHttpHandler? grpcHandler;
        private static readonly object GrpcClientLock = new();
        private static readonly MethodConfig GrpcDefaultBackoffConfig =
            new()
            {
                Names = { MethodName.Default },
                RetryPolicy = new RetryPolicy
                {
                    MaxAttempts = 5,
                    InitialBackoff = TimeSpan.FromSeconds(1),
                    MaxBackoff = TimeSpan.FromSeconds(5),
                    BackoffMultiplier = 1.2,
                    RetryableStatusCodes = { StatusCode.Unavailable },
                },
            };
        private static readonly MethodConfig GrpcGetCurrentStatusBackoffConfig =
            new()
            {
                Names =
                {
                    new MethodName
                    {
                        Service = "video_dedup_grpc.VideoDedupGrpcService",
                        Method = "GetCurrentStatus",
                    },
                },
                RetryPolicy = new RetryPolicy
                {
                    MaxAttempts = 2,
                    InitialBackoff = TimeSpan.FromMilliseconds(100),
                    MaxBackoff = TimeSpan.FromMilliseconds(100),
                    BackoffMultiplier = 1,
                    RetryableStatusCodes = { StatusCode.Unavailable },
                },
            };

        private static string BuildUrl()
        {
            var protocol = Configuration.Protocol;
            var serverAddress = Configuration.ServerAddress;
            var port = Configuration.Port;

            if (IPAddress.TryParse(serverAddress, out var ip)
                && ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                return $"{protocol}://[{serverAddress}]:{port}";
            }

            return $"{protocol}://{serverAddress}:{port}";
        }

        private static GrpcChannelOptions BuildGrpcChannelOptions(SocketsHttpHandler handler) =>
            new()
            {
                HttpHandler = handler,
                MaxReconnectBackoff = TimeSpan.FromSeconds(10),
                MaxReceiveMessageSize = null,
                ServiceConfig = new ServiceConfig
                {
                    MethodConfigs =
                    {
                        GrpcDefaultBackoffConfig,
                        GrpcGetCurrentStatusBackoffConfig,
                    },
                },
            };

        private static string? ResolvePinnedCertificateFilePath()
        {
            var configured = Configuration.PinnedCertificatePath?.Trim();
            if (!string.IsNullOrEmpty(configured) && File.Exists(configured))
            {
                return configured;
            }

            var alongside = Path.Combine(
                AppContext.BaseDirectory,
                "cert",
                "VideoDedup.crt");
            return File.Exists(alongside) ? alongside : null;
        }

        private static string? TryLoadPinnedThumbprint()
        {
            var path = ResolvePinnedCertificateFilePath();
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            try
            {
                using var cert = new X509Certificate2(path);
                return cert.Thumbprint;
            }
            catch
            {
                return null;
            }
        }

        private static GrpcChannel CreateGrpcChannel()
        {
            var url = BuildUrl();
            grpcHandler?.Dispose();
            grpcHandler = null;

            if (string.Equals(Configuration.Protocol, "http", StringComparison.OrdinalIgnoreCase))
            {
                grpcHandler = new SocketsHttpHandler();
                return GrpcChannel.ForAddress(url, BuildGrpcChannelOptions(grpcHandler));
            }

            var pinnedThumb = TryLoadPinnedThumbprint();
            grpcHandler = new SocketsHttpHandler();
            if (string.IsNullOrEmpty(pinnedThumb))
            {
                grpcHandler.SslOptions.RemoteCertificateValidationCallback =
                    static (_, _, _, _) => false;
            }
            else
            {
                grpcHandler.SslOptions.RemoteCertificateValidationCallback =
                    (_, certificate, _, _) =>
                    {
                        if (certificate is null)
                        {
                            return false;
                        }

                        using (var leaf = new X509Certificate2(certificate))
                        {
                            return string.Equals(
                                leaf.Thumbprint,
                                pinnedThumb,
                                StringComparison.OrdinalIgnoreCase);
                        }
                    };
            }

            return GrpcChannel.ForAddress(url, BuildGrpcChannelOptions(grpcHandler));
        }

        internal static VideoDedupGrpcServiceClient GrpcClient
        {
            get
            {
                lock (GrpcClientLock)
                {
                    grpcChannel ??= CreateGrpcChannel();
                    return new VideoDedupGrpcServiceClient(grpcChannel);
                }
            }
        }

        internal static ConfigData Configuration { get; set; } = LoadConfig();

        private static ConfigData LoadConfig() => new()
        {
            Protocol = Settings.Default.Protocol,
            ServerAddress = Settings.Default.ServerAddress,
            Port = Settings.Default.Port,
            StatusRequestInterval = TimeSpan.FromMilliseconds(
                Settings.Default.StatusRequestInterval),
            ClientSourcePath = Settings.Default.ClientSourcePath,
            PinnedCertificatePath = Settings.Default.PinnedCertificatePath ?? string.Empty,
        };

        internal static void SaveConfig() => SaveConfig(Configuration);

        private static void SaveConfig(ConfigData configuration)
        {
            Settings.Default.Protocol = configuration.Protocol;
            Settings.Default.ServerAddress = configuration.ServerAddress;
            Settings.Default.Port = configuration.Port;
            Settings.Default.StatusRequestInterval =
                (int)configuration.StatusRequestInterval.TotalMilliseconds;
            Settings.Default.ClientSourcePath =
                configuration.ClientSourcePath;
            Settings.Default.PinnedCertificatePath =
                configuration.PinnedCertificatePath ?? string.Empty;
            Settings.Default.Save();
            ResetGrpcChannel();
        }

        internal static void ResetGrpcChannel()
        {
            lock (GrpcClientLock)
            {
                grpcChannel?.Dispose();
                grpcChannel = null;
                grpcHandler?.Dispose();
                grpcHandler = null;
            }
        }

        internal static bool IsLikelyCertificateOrTlsFailure(Exception ex)
        {
            for (var c = ex; c != null; c = c.InnerException)
            {
                if (c is AuthenticationException)
                {
                    return true;
                }

                if (c is IOException io && io.Message.Contains("Unable to read data", StringComparison.Ordinal))
                {
                    return true;
                }

                var msg = c.Message;
                if (msg.Contains("SSL", StringComparison.OrdinalIgnoreCase)
                    || msg.Contains("TLS", StringComparison.OrdinalIgnoreCase)
                    || msg.Contains("certificate", StringComparison.OrdinalIgnoreCase)
                    || msg.Contains("remote certificate", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool TryImportPinnedServerCertificate(
            string sourcePath,
            out string? errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
            {
                errorMessage = "Certificate file was not found.";
                return false;
            }

            try
            {
                using var _ = new X509Certificate2(sourcePath);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }

            try
            {
                var destDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "VideoDedup");
                _ = Directory.CreateDirectory(destDir);
                var dest = Path.Combine(destDir, "VideoDedup.crt");
                File.Copy(sourcePath, dest, overwrite: true);
                Configuration.PinnedCertificatePath = dest;
                Settings.Default.PinnedCertificatePath = dest;
                Settings.Default.Save();
                ResetGrpcChannel();
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new VideoDedupDlg());
        }
    }
}
