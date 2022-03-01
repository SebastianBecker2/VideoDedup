namespace VideoDedup.DnsTextBox
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Windows.Forms;
    using VideoDedupShared.ISynchronizeInvokeExtensions;

    public class DnsTextBoxCtrl : TextBox
    {
        private static string ValidCharacters =>
            ".-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly int MinimumLength = 3;
        // Complete domain name must not be longer than 253 chars
        private static readonly int MaximumLength = 253;

        //public int MyProperty { get; set; }
        private new Color? DefaultForeColor { get; set; }

        [Browsable(true)]
        [DefaultValue(typeof(Color), "Red")]
        [Category("Appearance")]
        [Description("Sets the color that inidicates that the name " +
            "or address could not be resolved")]
        public Color ErrorForeColor { get; set; } = Color.Red;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public IEnumerable<IPAddress> IpAddresses { get; set; }

        [Browsable(false)]
        public bool Resolving { get; set; } = false;

        [Browsable(false)]
        public bool ResolvedSuccessfully { get; set; } = false;

        public event EventHandler<ResolveStartedEventArgs> ResolveStarted;
        protected virtual void OnResolveStarted(string dnsName) =>
            ResolveStarted?.Invoke(this, new ResolveStartedEventArgs
            {
                DnsName = dnsName
            });

        public event EventHandler<ResolveSuccessfulEventArgs> ResolveSuccessful;
        protected virtual void OnResolveSuccessful(
            string dnsName,
            IEnumerable<IPAddress> ipAddresses) =>
            ResolveSuccessful?.Invoke(this, new ResolveSuccessfulEventArgs
            {
                DnsName = dnsName,
                IpAddress = ipAddresses,
            });

        public event EventHandler<ResolveFailedEventArgs> ResolveFailed;
        protected virtual void OnResolveFailed(string dnsName) =>
            ResolveFailed?.Invoke(this, new ResolveFailedEventArgs
            {
                DnsName = dnsName
            });

        public DnsTextBoxCtrl() { }

        protected override void OnTextChanged(EventArgs e)
        {
            Resolving = false;
            ResolvedSuccessfully = false;
            base.OnTextChanged(e);

            if (DesignMode)
            {
                return;
            }

            if (!DefaultForeColor.HasValue)
            {
                DefaultForeColor = ForeColor;
            }
            ForeColor = DefaultForeColor.Value;

            if (Text.Length < MinimumLength || !IsDnsNameValid(Text))
            {
                OnResolveFailed(Text);
                ForeColor = ErrorForeColor;
                return;
            }

            Resolving = true;
            OnResolveStarted(Text);
            try
            {
                _ = Dns.BeginGetHostAddresses(
                    Text,
                    ar => this.InvokeIfRequired(() => GetHostAddressesCallback(ar)),
                    Text);
            }
            catch
            {
                Resolving = false;
            }
        }

        private void GetHostAddressesCallback(IAsyncResult ar)
        {
            // Make sure the request was still relevant
            if ((ar.AsyncState as string) != Text)
            {
                try
                {
                    _ = Dns.EndGetHostAddresses(ar);
                }
                catch { }
                return;
            }

            Resolving = false;
            IpAddresses = null;
            try
            {
                IpAddresses = Dns.EndGetHostAddresses(ar).Where(a =>
                    a.AddressFamily == AddressFamily.InterNetwork
                    || a.AddressFamily == AddressFamily.InterNetworkV6);
            }
            catch (SocketException) { }

            if (IpAddresses == null || !IpAddresses.Any())
            {
                OnResolveFailed(Text);
                ForeColor = ErrorForeColor;
                return;
            }

            ResolvedSuccessfully = true;
            OnResolveSuccessful(Text, IpAddresses);
        }

        public static bool IsDnsNameValid(string dnsName)
        {
            if (dnsName.Length > MaximumLength)
            {
                return false;
            }

            if (dnsName.Any(c => !ValidCharacters.Contains(c)))
            {
                return false;
            }

            if (dnsName.First() == '-' || dnsName.Last() == '-')
            {
                return false;
            }

            return true;
        }
    }
}
