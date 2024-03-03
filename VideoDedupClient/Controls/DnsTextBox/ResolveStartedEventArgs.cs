namespace VideoDedupClient.Controls.DnsTextBox
{
    public class ResolveStartedEventArgs(string dnsName) : EventArgs
    {
        public string DnsName { get; set; } = dnsName;
    }
}
