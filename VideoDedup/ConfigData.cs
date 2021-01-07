namespace VideoDedup
{
    using System;

    public class ConfigData
    {
        public string ServerAddress { get; set; }
        public TimeSpan StatusRequestInterval { get; set; }
    }
}
