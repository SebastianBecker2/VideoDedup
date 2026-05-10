namespace VideoDedupService
{
    using System.IO.Compression;
    using Microsoft.Extensions.Hosting;
    using VideoDedupServer;

    internal sealed class Program
    {
        private static readonly string ApplicationName = "VideoDedupService";

        private static string GetLocalAppPath()
        {
            var overridePath = Environment.GetEnvironmentVariable("VIDEODEDUP_APP_DATA");
            if (!string.IsNullOrWhiteSpace(overridePath))
            {
                if (!Directory.Exists(overridePath))
                {
                    _ = Directory.CreateDirectory(overridePath);
                }

                return overridePath;
            }

            var path = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);
            if (!Directory.Exists(path))
            {
                _ = Directory.CreateDirectory(path);
            }

            path = Path.Combine(path, ApplicationName);
            if (!Directory.Exists(path))
            {
                _ = Directory.CreateDirectory(path);
            }

            return path;
        }

#pragma warning disable IDE0210 // Convert to top-level statements
        private static void Main(string[] args)
        {
            LinuxHostBootstrap.LoadEtcEnvironmentFile();

            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                ContentRootPath = AppContext.BaseDirectory,
                Args = args,
            });

            _ = builder.WebHost.ConfigureKestrel((context, options) =>
            {
                var env = context.HostingEnvironment.EnvironmentName;
                var section = env == "Development"
                    ? "KestrelDevelopment"
                    : "Kestrel";
                options.Configure(context.Configuration.GetSection(section));
            });

            _ = builder.Services.AddGrpc(options =>
            {
                options.ResponseCompressionAlgorithm = "gzip";
                options.ResponseCompressionLevel = CompressionLevel.Optimal;
            });
            _ = builder.Services.AddSingleton(
                _ => new VideoDedupService(GetLocalAppPath()));

            _ = builder.Services.Configure<HostOptions>(options =>
            {
                options.ShutdownTimeout = TimeSpan.FromSeconds(60);
            });
            _ = builder.Services.AddHostedService<GracefulShutdownHostedService>();

            if (OperatingSystem.IsWindows())
            {
                _ = builder.Host.UseWindowsService();
            }
            else if (OperatingSystem.IsLinux())
            {
                _ = builder.Host.UseSystemd();
            }

            var app = builder.Build();

            LinuxHostBootstrap.EnsureNotRunningAsRootUnlessAllowed();

            _ = app.MapGrpcService<VideoDedupService>();

            app.Run();
        }
#pragma warning restore IDE0210 // Convert to top-level statements
    }
}
