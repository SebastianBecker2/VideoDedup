namespace VideoDedupClient.Controls.DnsTextBox
{
    public class ResolveStartedEventArgs : EventArgs
    {
        public string DnsName { get; set; }

        public ResolveStartedEventArgs(string dnsName) => DnsName = dnsName;
    }
}
