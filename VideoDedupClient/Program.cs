namespace VideoDedupClient
{
    using Grpc.Core;
    using Grpc.Net.Client;
    using Grpc.Net.Client.Configuration;
    using Properties;
    using Dialogs;
    using static VideoDedupGrpc.VideoDedupGrpcService;
    using System.Net;

    internal static class Program
    {
        private static GrpcChannel? grpcChannel;
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
                    RetryableStatusCodes = { StatusCode.Unavailable }
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
                    RetryableStatusCodes = { StatusCode.Unavailable }
                },
            };

        private static string BuildUrl(string serverAddress, int port)
        {
            if (IPAddress.TryParse(Configuration.ServerAddress, out var ip)
                && ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                return $"http://[{serverAddress}]:{port}";
            }
            return $"http://{serverAddress}:{port}";
        }

        internal static VideoDedupGrpcServiceClient GrpcClient
        {
            get
            {
                lock (GrpcClientLock)
                {
                    grpcChannel ??= GrpcChannel.ForAddress(
                        BuildUrl(Configuration.ServerAddress, 41722),
                        new GrpcChannelOptions
                        {
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
                        });
                    return new VideoDedupGrpcServiceClient(grpcChannel);
                }
            }
        }

        internal static ConfigData Configuration { get; set; } = LoadConfig();

        private static ConfigData LoadConfig() => new()
        {
            ServerAddress = Settings.Default.ServerAddress,
            StatusRequestInterval = TimeSpan.FromMilliseconds(
                    Settings.Default.StatusRequestInterval),
            ClientSourcePath = Settings.Default.ClientSourcePath,
        };

        internal static void SaveConfig() => SaveConfig(Configuration);

        private static void SaveConfig(ConfigData configuration)
        {
            Settings.Default.ServerAddress = configuration.ServerAddress;
            Settings.Default.StatusRequestInterval =
                (int)configuration.StatusRequestInterval.TotalMilliseconds;
            Settings.Default.ClientSourcePath =
                configuration.ClientSourcePath;
            Settings.Default.Save();
            lock (GrpcClientLock)
            {
                grpcChannel = null;
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

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new VideoDedupDlg());
        }
    }
}
