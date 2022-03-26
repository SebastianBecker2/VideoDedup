namespace VideoDedupClient.Controls.DnsTextBox
{
    public class ResolveFailedEventArgs : EventArgs
    {
        public string DnsName { get; set; }

        public ResolveFailedEventArgs(string dnsName) => DnsName = dnsName;
    }
}
