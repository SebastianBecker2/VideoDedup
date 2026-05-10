namespace VideoDedupService
{
    internal sealed partial class GracefulShutdownHostedService : IHostedService, IDisposable
    {
        private readonly IHostApplicationLifetime lifetime;
        private readonly ILogger<GracefulShutdownHostedService> logger;
        private CancellationTokenRegistration stoppingRegistration;

        public GracefulShutdownHostedService(
            IHostApplicationLifetime lifetime,
            ILogger<GracefulShutdownHostedService> logger)
        {
            this.lifetime = lifetime;
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            stoppingRegistration = lifetime.ApplicationStopping.Register(() =>
                ShutdownMessage.Log(logger));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public void Dispose() => stoppingRegistration.Dispose();

        private static partial class ShutdownMessage
        {
            [LoggerMessage(
                EventId = 1,
                Level = LogLevel.Information,
                Message = "Shutdown requested; draining host (SIGTERM or IHostApplicationLifetime.StopApplication).")]
            public static partial void Log(ILogger logger);
        }
    }
}
