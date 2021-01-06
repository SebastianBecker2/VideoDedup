using System.Collections.Generic;
using System.Net;

namespace VideoDedup.DnsTextBox
{
    public class ResolveSuccessfulEventArgs
    {
        public string DnsName { get; set; }
        public IEnumerable<IPAddress> IpAddress { get; set; }
    }
}
