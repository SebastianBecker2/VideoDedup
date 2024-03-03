namespace VideoDedupClient.Controls.DnsTextBox
{
    using System.Net;

    public class ResolveSuccessfulEventArgs(
        string dnsName,
        IEnumerable<IPAddress> ipAddress) : EventArgs
    {
        public string DnsName { get; set; } = dnsName;
        public IEnumerable<IPAddress> IpAddress { get; set; } = ipAddress;
    }
}
