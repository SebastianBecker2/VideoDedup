namespace VideoDedupServer
{
    using System;
    using System.Globalization;
    using Serilog.Core;
    using Serilog.Sinks.SystemConsole.Themes;
    using Serilog;
    using Serilog.Events;
    using VideoDedupGrpc;

    internal sealed class LogManager : IDisposable
    {
        private bool disposedValue;

        private string AppDataFolderPath { get; }

        public Logger VideoDedupServiceLogger { get; }
        private LoggingLevelSwitch VideoDedupServiceLogLevelSwitch { get; } =
            new(ToLogEventLevel(Settings.Default.VideoDedupServiceLogLevel));
        public Logger ComparisonManagerLogger { get; }
        private LoggingLevelSwitch ComparisonManagerLogLevelSwitch { get; } =
            new(ToLogEventLevel(Settings.Default.ComparisonManagerLogLevel));
        public Logger? DedupEngineLogger { get; private set; }
        private LoggingLevelSwitch DedupEngineLogLevelSwitch { get; } =
            new(ToLogEventLevel(Settings.Default.DedupEngineLogLevel));

        public LogManager(string appDataFolderPath)
        {
            AppDataFolderPath = appDataFolderPath;

            VideoDedupServiceLogger = CreateVideoDedupServiceLogger(
                appDataFolderPath,
                VideoDedupServiceLogLevelSwitch);

            ComparisonManagerLogger = CreateComparisonManagerLogger(
                appDataFolderPath,
                ComparisonManagerLogLevelSwitch);
        }

        public void CreateDedupEngineLogger() =>
            DedupEngineLogger ??= CreateDedupEngineLogger(
                AppDataFolderPath,
                DedupEngineLogLevelSwitch);

        public void DeleteDedupEngineLogger()
        {
            DedupEngineLogger?.Dispose();
            DedupEngineLogger = null;
        }

        public void UpdateConfiguration(LogSettings settings)
        {
            VideoDedupServiceLogLevelSwitch.MinimumLevel =
                ToLogEventLevel(settings.VideoDedupServiceLogLevel.ToString());

            ComparisonManagerLogLevelSwitch.MinimumLevel =
                ToLogEventLevel(
                    settings.ComparisonManagerLogLevel.ToString());

            DedupEngineLogLevelSwitch.MinimumLevel =
                ToLogEventLevel(settings.DedupEngineLogLevel.ToString());
        }

        private static Logger CreateVideoDedupServiceLogger(
            string appDataFolderPath, LoggingLevelSwitch levelSwitch) =>
            new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .WriteTo.File(
                    Path.Combine(appDataFolderPath, "VideoDedupService-.log"),
                    formatProvider: CultureInfo.InvariantCulture,
                    rollOnFileSizeLimit: true,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: null,
                    retainedFileTimeLimit: null)
                .WriteTo.Console(
                    formatProvider: CultureInfo.InvariantCulture,
                    theme: AnsiConsoleTheme.Sixteen)
                .CreateLogger();

        private static Logger CreateComparisonManagerLogger(
            string appDataFolderPath, LoggingLevelSwitch levelSwitch) =>
            new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .WriteTo.File(
                    Path.Combine(appDataFolderPath, "ComparisonManager-.log"),
                    formatProvider: CultureInfo.InvariantCulture,
                    rollOnFileSizeLimit: true,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: null,
                    retainedFileTimeLimit: null)
                .CreateLogger();

        private static Logger CreateDedupEngineLogger(
            string appDataFolderPath, LoggingLevelSwitch levelSwitch) =>
            new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .WriteTo.File(
                    Path.Combine(
                        appDataFolderPath,
                        $"DedupEngine-{DateTime.Now:s}.log".Replace(':', '-')),
                    formatProvider: CultureInfo.InvariantCulture,
                    rollOnFileSizeLimit: true,
                    rollingInterval: RollingInterval.Infinite,
                    retainedFileCountLimit: null,
                    retainedFileTimeLimit: null)
                .CreateLogger();

        private static LogEventLevel ToLogEventLevel(string logLevel)
        {
            if (!Enum.TryParse(
                logLevel,
                true,
                out LogEventLevel logEventLevel))
            {
                logEventLevel = LogEventLevel.Information;
            }
            return logEventLevel;
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    VideoDedupServiceLogger.Dispose();
                    ComparisonManagerLogger.Dispose();
                    VideoDedupServiceLogger?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
