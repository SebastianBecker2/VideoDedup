namespace VideoDedupClient
{
    using Dialogs;
    using Grpc.Net.Client;
    using Properties;
    using static VideoDedupGrpc.VideoDedupGrpcService;

    internal static class Program
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
                        $"http://{Configuration.ServerAddress}:41722",
                        new GrpcChannelOptions
                        {
                            MaxReceiveMessageSize = null,
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
