namespace VideoDedupConsole
{
    using System;
    using System.IO;
    using System.ServiceModel;
    using VideoDedupServer;

    internal class Program
    {
        // The folder name, settings are stored in.
        // Which is actually the company name.
        // Though company name is empty and he still somehow gets this name:"
        private static readonly string ApplicationName = "VideoDedupConsole";

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

        private static readonly Uri WcfBaseAddress =
            new Uri("net.tcp://localhost:41721/VideoDedup");

        private static void Main()
        {
            var dedupService = new Service(GetLocalAppPath());
            var serviceHost = new ServiceHost(dedupService, WcfBaseAddress);

            // Setting InstanceContextMode to single since we create
            // our own Service.
            var behaviour = serviceHost.Description.Behaviors
                .Find<ServiceBehaviorAttribute>();
            behaviour.InstanceContextMode = InstanceContextMode.Single;

            serviceHost.Open();

            Console.WriteLine("Service running.  Please 'Enter' to exit...");
            _ = Console.ReadLine();

            serviceHost.Abort();
            serviceHost.Close();
            dedupService.Dispose();
        }
    }
}
