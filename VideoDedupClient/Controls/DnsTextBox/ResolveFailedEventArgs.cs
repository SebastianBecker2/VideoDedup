namespace VideoDedupClient.Controls.DnsTextBox
{
    public class ResolveFailedEventArgs(string dnsName) : EventArgs
    {
        public string DnsName { get; set; } = dnsName;
    }
}
