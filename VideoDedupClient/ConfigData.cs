namespace VideoDedupClient
{
    using System;

    public class ConfigData
    {
        public string ServerAddress { get; set; } = "";
        public TimeSpan StatusRequestInterval { get; set; }
        public string ClientSourcePath { get; set; } = "";
    }
}
