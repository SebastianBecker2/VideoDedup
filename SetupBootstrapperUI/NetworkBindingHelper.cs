namespace SetupBootstrapperUI
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;

    internal sealed class NetworkBindingEntry
    {
        public NetworkBindingEntry(string bindToken, string displayText, bool isAllNetworks)
        {
            BindToken = bindToken;
            DisplayText = displayText;
            IsAllNetworks = isAllNetworks;
        }

        public string BindToken { get; }

        public string DisplayText { get; }

        public bool IsAllNetworks { get; }

        public override string ToString() => DisplayText;
    }

    internal static class NetworkBindingHelper
    {
        /// <summary>Transport token (MSI/bundle/registry). Not [::] — brackets are MSI property syntax.</summary>
        public const string AllNetworksToken = "ALL";

        public const string AllNetworksDisplay = "All networks ([::])";

        public const string LegacyAllNetworksToken = "[::]";

        /// <summary>Must not be ';' — that delimiter is used inside MSI CustomActionData.</summary>
        public const char BindingsSeparator = '|';

        public static IReadOnlyList<NetworkBindingEntry> EnumerateAdapterAddresses()
        {
            var results = new List<NetworkBindingEntry>();

            foreach (var iface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (iface.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }

                if (iface.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }

                foreach (var address in iface.GetIPProperties().UnicastAddresses)
                {
                    var ip = address.Address;
                    if (ip.IsIPv6LinkLocal
                        || ip.IsIPv6Multicast
                        || ip.IsIPv6SiteLocal)
                    {
                        continue;
                    }

                    if (ip.AddressFamily != AddressFamily.InterNetwork
                        && ip.AddressFamily != AddressFamily.InterNetworkV6)
                    {
                        continue;
                    }

                    var token = ip.ToString();
                    results.Add(new NetworkBindingEntry(
                        token,
                        $"{iface.Name} — {token}",
                        isAllNetworks: false));
                }
            }

            return results;
        }

        public static string SerializeBindings(IEnumerable<string> bindTokens)
        {
            return string.Join(BindingsSeparator.ToString(), bindTokens);
        }

        public static IReadOnlyList<string> ParseBindings(string bindingsValue)
        {
            if (string.IsNullOrWhiteSpace(bindingsValue))
            {
                return new[] { AllNetworksToken };
            }

            var separators = new[] { BindingsSeparator, ';' };
            var tokens = bindingsValue.Split(separators, StringSplitOptions.None);
            var results = new List<string>();
            foreach (var token in tokens)
            {
                var trimmed = token.Trim();
                if (string.IsNullOrEmpty(trimmed))
                {
                    continue;
                }

                if (trimmed == AllNetworksToken
                    || trimmed == LegacyAllNetworksToken)
                {
                    return new[] { AllNetworksToken };
                }

                if (IPAddress.TryParse(trimmed, out _))
                {
                    results.Add(trimmed);
                }
            }

            if (results.Count > 0)
            {
                return results;
            }

            return new List<string> { AllNetworksToken };
        }
    }
}
