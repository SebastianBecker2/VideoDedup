namespace VideoDedupClient.Controls.DnsTextBox
{
    using System.Net;

    public class ResolveSuccessfulEventArgs : EventArgs
    {
        public string DnsName { get; set; }
        public IEnumerable<IPAddress> IpAddress { get; set; }

        public ResolveSuccessfulEventArgs(
            string dnsName,
            IEnumerable<IPAddress> ipAddress)
        {
            DnsName = dnsName;
            IpAddress = ipAddress;
        }
    }
}
