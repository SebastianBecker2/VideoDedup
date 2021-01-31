namespace VideoDedup.DnsTextBox
{
    using System.Collections.Generic;
    using System.Net;

    public class ResolveSuccessfulEventArgs
    {
        public string DnsName { get; set; }
        public IEnumerable<IPAddress> IpAddress { get; set; }
    }
}
