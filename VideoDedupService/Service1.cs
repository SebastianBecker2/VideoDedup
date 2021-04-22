namespace VideoDedupService
{
    using System;
    using System.IO;
    using System.ServiceModel;
    using System.ServiceProcess;
    using VideoDedupServer;

    public partial class Service1 : ServiceBase
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

        private static readonly Uri WcfBaseAddress =
            new Uri("net.tcp://localhost:41721/VideoDedup");

        private Service DedupService { get; set; }
        private ServiceHost ServiceHost { get; set; }

        public Service1() => InitializeComponent();

        protected override void OnStart(string[] args)
        {
            DedupService = new Service(GetLocalAppPath());
            ServiceHost = new ServiceHost(DedupService, WcfBaseAddress);

            // Setting InstanceContextMode to single since we create
            // our own Service.
            var behaviour = ServiceHost.Description.Behaviors
                .Find<ServiceBehaviorAttribute>();
            behaviour.InstanceContextMode = InstanceContextMode.Single;

            ServiceHost.Open();

            base.OnStart(args);
        }

        protected override void OnStop()
        {
            ServiceHost.Abort();
            ServiceHost.Close();
            DedupService.Dispose();

            base.OnStop();
        }
    }
}
