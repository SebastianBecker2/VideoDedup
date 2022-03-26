namespace VideoDedupService
{
    using VideoDedupServer;

    internal class Program
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

        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
            {
                // ContentRootPath is important for use as Windows Service.
                ContentRootPath = AppContext.BaseDirectory,
                Args = args,
            });

            _ = builder.Services.AddGrpc();
            _ = builder.Services.AddSingleton(
                x => new VideoDedupService(GetLocalAppPath()));

            _ = builder.Host.UseWindowsService();

            var app = builder.Build();

            _ = app.MapGrpcService<VideoDedupService>();
            //app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();

        }
    }
}


