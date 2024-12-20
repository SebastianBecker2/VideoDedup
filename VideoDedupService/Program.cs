namespace VideoDedupService
{
    using System.IO.Compression;
    using VideoDedupServer;

    internal sealed class Program
    {
        // The folder name, settings are stored in.
        // Which is actually the company name.
        // Though company name is empty and he still somehow gets this name:"
        private static readonly string ApplicationName = "VideoDedupService";

        private static string GetLocalAppPath()
        {
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
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                // ContentRootPath is important for use as Windows Service.
                ContentRootPath = AppContext.BaseDirectory,
                Args = args,
            });

            _ = builder.Services.AddGrpc(options =>
            {
                options.ResponseCompressionAlgorithm = "gzip";
                options.ResponseCompressionLevel = CompressionLevel.Optimal;
            });
            _ = builder.Services.AddSingleton(
                _ => new VideoDedupService(GetLocalAppPath()));

            _ = builder.Host.UseWindowsService();

            var app = builder.Build();

            var videoDedupService =
                app.Services.GetRequiredService<VideoDedupService>();

            _ = app.MapGrpcService<VideoDedupService>();

            app.Run();

        }
#pragma warning restore IDE0210 // Convert to top-level statements
    }
}


