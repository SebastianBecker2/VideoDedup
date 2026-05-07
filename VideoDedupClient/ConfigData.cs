namespace VideoDedupClient
{
    public class ConfigData
    {
        public string ServerAddress { get; set; } = "";
        public TimeSpan StatusRequestInterval { get; set; }
        public string ClientSourcePath { get; set; } = "";
        public string Protocol { get; set; } = "https";
        public int Port { get; set; } = 51726;

        /// <summary>Optional path to <c>VideoDedup.crt</c> for TLS pinning. Empty uses the copy next to the client executable.</summary>
        public string PinnedCertificatePath { get; set; } = "";
    }
}
