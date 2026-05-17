namespace SetupActionCertificate
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.Win32;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using WixToolset.Dtf.WindowsInstaller;

    public class CustomActions
    {
        private const int DefaultServerPort = 51726;
        private const int MinServerPort = 1024;
        private const int MaxServerPort = 65535;

        private const string ServerRegistryKey =
            @"SOFTWARE\SebastianBecker\VideoDedup\Server";

        private const string ClientRegistryKey =
            @"SOFTWARE\SebastianBecker\VideoDedup\Client";

        private static void LoadAssemblies(Session session) =>
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var assemblyName = new AssemblyName(args.Name).Name + ".dll";
                var assemblyPath = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    assemblyName);

                if (File.Exists(assemblyPath))
                {
                    return Assembly.LoadFile(assemblyPath);
                }

                session.Log($"[SetupActionCertificate] Missing dependency: " +
                    $"{assemblyPath}");
                return null;
            };

        private static void InstallCertificate(
            Session session,
            string certPublicPath)
        {
            // Load the certificate
            var cert = new X509Certificate2(certPublicPath);

            // Open the Trusted Root Certification Authorities store
            using (var store = new X509Store(
                StoreName.Root,
                StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadWrite);
                store.Add(cert);
                store.Close();
            }

            session.Log($"[SetupActionCertificate] Public certificate " +
                $"installed to Trusted Root Certification Authorities");
        }

        private static void RemoveCertificate(
            Session session,
            string certPublicPath)
        {
            var thumbprint = new X509Certificate2(certPublicPath).Thumbprint;
            RemoveCertificateByThumbprint(session, thumbprint);
        }

        private static void RemoveCertificateByThumbprint(
            Session session,
            string thumbprint)
        {
            if (string.IsNullOrWhiteSpace(thumbprint))
            {
                session.Log(
                    "[SetupActionCertificate] No thumbprint; skipping cert store removal.");
                return;
            }

            using (var store = new X509Store(
                StoreName.Root,
                StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadWrite);

                var certs = store.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    thumbprint,
                    false);

                foreach (var cert in certs)
                {
                    store.Remove(cert);
                }

                store.Close();
            }

            session.Log(
                "[SetupActionCertificate] Public certificate removed from " +
                "Trusted Root Certification Authorities (thumbprint " +
                $"{thumbprint}).");
        }

        private static void UpdateAppSettings(
            Session session,
            string settingsPath,
            string certPrivatePath,
            string randomPassword)
        {
            var json = JObject.Parse(File.ReadAllText(settingsPath));
            json["Kestrel"]["Endpoints"]["gRPC"]["Certificate"]["Path"] =
                certPrivatePath;
            json["Kestrel"]["Endpoints"]["gRPC"]["Certificate"]["Password"] =
                randomPassword;
            File.WriteAllText(settingsPath, json.ToString(Formatting.Indented));

            session.Log($"[SetupActionCertificate] " +
                $"Updated appsettings.json with certificate path and password.");
        }

        private static int ParseListenPort(string portValue)
        {
            if (!int.TryParse(portValue, out var port)
                || port < MinServerPort
                || port > MaxServerPort)
            {
                return DefaultServerPort;
            }

            return port;
        }

        private const string AllNetworksTransportToken = "ALL";

        private const string AllNetworksUrlToken = "[::]";

        private static bool IsAllNetworksBindingToken(string token)
        {
            return string.Equals(
                token,
                AllNetworksTransportToken,
                StringComparison.OrdinalIgnoreCase)
                || token == AllNetworksUrlToken;
        }

        private static IReadOnlyList<string> ParseListenBindings(string bindingsValue)
        {
            if (string.IsNullOrWhiteSpace(bindingsValue))
            {
                return new[] { AllNetworksTransportToken };
            }

            var results = new List<string>();
            foreach (var part in bindingsValue.Split('|', ';'))
            {
                var token = part.Trim();
                if (string.IsNullOrEmpty(token))
                {
                    continue;
                }

                if (IsAllNetworksBindingToken(token))
                {
                    return new[] { AllNetworksTransportToken };
                }

                if (IPAddress.TryParse(token, out _))
                {
                    results.Add(token);
                }
            }

            if (results.Count > 0)
            {
                return results;
            }

            return new List<string> { AllNetworksTransportToken };
        }

        private static string BuildListenUrl(string bindToken, int port)
        {
            if (IsAllNetworksBindingToken(bindToken))
            {
                return $"https://{AllNetworksUrlToken}:{port}";
            }

            var ip = IPAddress.Parse(bindToken);
            if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return $"https://[{ip}]:{port}";
            }

            return $"https://{ip}:{port}";
        }

        private static void UpdateListenEndpointsInAppSettings(
            Session session,
            string settingsPath,
            int port,
            IReadOnlyList<string> bindings)
        {
            var json = JObject.Parse(File.ReadAllText(settingsPath));
            var endpoints = json["Kestrel"]?["Endpoints"] as JObject;
            var template = endpoints?["gRPC"] as JObject;
            if (template is null)
            {
                session.Log(
                    "[SetupActionCertificate] ConfigureServerListenPort: " +
                    "Kestrel.Endpoints.gRPC missing in appsettings.json");
                return;
            }

            var certificate = template["Certificate"]?.DeepClone();
            var protocols = template["Protocols"]?.ToString() ?? "Http2";

            foreach (var property in endpoints.Properties().ToList())
            {
                property.Remove();
            }

            for (var i = 0; i < bindings.Count; i++)
            {
                var endpointName = i == 0 ? "gRPC" : $"gRPC_{i}";
                var url = BuildListenUrl(bindings[i], port);
                endpoints[endpointName] = new JObject
                {
                    ["Url"] = url,
                    ["Protocols"] = protocols,
                    ["Certificate"] = certificate?.DeepClone(),
                };
            }

            File.WriteAllText(settingsPath, json.ToString(Formatting.Indented));
            session.Log(
                $"[SetupActionCertificate] Updated appsettings.json with " +
                $"{bindings.Count} listen endpoint(s) on port {port}.");
        }

        private static void WriteServerListenBindingsRegistry(
            Session session,
            string bindingsSerialized)
        {
            using (var key = Registry.LocalMachine.CreateSubKey(
                ServerRegistryKey,
                true))
            {
                key.SetValue(
                    "ListenBindings",
                    bindingsSerialized,
                    RegistryValueKind.String);
            }

            session.Log(
                $"[SetupActionCertificate] Server ListenBindings registry: " +
                $"{bindingsSerialized}");
        }

        private static void WriteServerListenPortRegistry(Session session, int port)
        {
            using (var key = Registry.LocalMachine.CreateSubKey(
                ServerRegistryKey,
                true))
            {
                key.SetValue("ListenPort", port, RegistryValueKind.DWord);
            }

            session.Log(
                $"[SetupActionCertificate] Server ListenPort registry: {port}");
        }

        private static void WriteClientInstallListenPortRegistry(
            Session session,
            int port)
        {
            using (var key = Registry.LocalMachine.CreateSubKey(
                ClientRegistryKey,
                true))
            {
                key.SetValue("InstallListenPort", port, RegistryValueKind.DWord);
            }

            session.Log(
                $"[SetupActionCertificate] Client InstallListenPort registry: {port}");
        }

        private static X509Extension BuildSubjectAlternativeName()
        {
            // Build SAN extension
            var sanBuilder = new SubjectAlternativeNameBuilder();
            sanBuilder.AddDnsName("localhost");
            sanBuilder.AddIpAddress(IPAddress.Loopback);
            sanBuilder.AddIpAddress(IPAddress.IPv6Loopback);
            sanBuilder.AddDnsName(Environment.MachineName);

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

                    // Ignore link-local and temporary addresses
                    if (ip.IsIPv6LinkLocal
                        || ip.IsIPv6Multicast
                        || ip.IsIPv6SiteLocal)
                    {
                        continue;
                    }

                    if (ip.AddressFamily == AddressFamily.InterNetwork
                        || ip.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        sanBuilder.AddIpAddress(ip);
                    }
                }
            }

            return sanBuilder.Build();
        }

        private static void CreateCertificate(
            Session session,
            string certPrivatePath,
            string certPublicPath,
            string randomPassword,
            out string thumbprint)
        {
            thumbprint = string.Empty;
            using (var rsa = RSA.Create(2048))
            {
                var request = new CertificateRequest(
                    "CN=VideoDedupServer, O=Sebastian Becker, C=DE",
                    rsa,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);

                request.CertificateExtensions.Add(
                    new X509BasicConstraintsExtension(false, false, 0, false));

                request.CertificateExtensions.Add(
                    new X509KeyUsageExtension(
                        X509KeyUsageFlags.DigitalSignature
                            | X509KeyUsageFlags.KeyEncipherment,
                        false));

                request.CertificateExtensions.Add(
                    new X509EnhancedKeyUsageExtension(
                        new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") },
                        false));

                request.CertificateExtensions.Add(BuildSubjectAlternativeName());

                using (var certificate = request.CreateSelfSigned(
                    DateTimeOffset.Now,
                    DateTimeOffset.Now.AddYears(10)))
                {
                    certificate.FriendlyName =
                        "VideoDedupServer Self-Signed Certificate";

                    thumbprint = certificate.Thumbprint;

                    // Export to PFX (private key)
                    File.WriteAllBytes(
                        certPrivatePath,
                        certificate.Export(X509ContentType.Pfx, randomPassword));

                    // Export CRT (public key for clients)
                    File.WriteAllBytes(
                        certPublicPath,
                        certificate.Export(X509ContentType.Cert));
                }
            }

            session.Log($"[SetupActionCertificate] Certificate generated and " +
                $"saved to {certPrivatePath} and {certPublicPath}");
        }

        private static void WriteServerCertDiscovery(
            Session session,
            string certPublicPath,
            string thumbprint)
        {
            var thumbprintSidecar = certPublicPath + ".thumbprint.txt";
            File.WriteAllText(thumbprintSidecar, thumbprint);

            using (var key = Registry.LocalMachine.CreateSubKey(
                ServerRegistryKey,
                true))
            {
                key.SetValue("CertPath", certPublicPath, RegistryValueKind.String);
                key.SetValue("CertThumbprint", thumbprint, RegistryValueKind.String);
            }

            session.Log(
                $"[SetupActionCertificate] Server cert discovery: " +
                $"{certPublicPath}, thumbprint {thumbprint}");
        }

        private static void RemoveServerCertDiscovery(Session session)
        {
            try
            {
                using (var parent = Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\SebastianBecker\VideoDedup",
                    true))
                {
                    parent?.DeleteSubKeyTree("Server", false);
                }
                session.Log("[SetupActionCertificate] Removed server cert registry key.");
            }
            catch (Exception ex)
            {
                session.Log(
                    $"[SetupActionCertificate] Warning removing server cert registry: {ex}");
            }
        }

        [CustomAction]
        public static ActionResult ConfigureServerListenPort(Session session)
        {
            LoadAssemblies(session);

            var serverfolder = session.CustomActionData["SERVERFOLDER"];
            var port = ParseListenPort(session.CustomActionData["SERVERLISTENPORT"]);
            var bindings = ParseListenBindings(
                session.CustomActionData["SERVERLISTENBINDINGS"]);
            var bindingsSerialized = string.Join("|", bindings);
            var syncClient =
                session.CustomActionData["VIDEODEDUP_SYNCCLIENTPORT"] == "1";
            var settingsPath = Path.Combine(serverfolder, "appsettings.json");

            if (!File.Exists(settingsPath))
            {
                session.Log(
                    $"[SetupActionCertificate] ConfigureServerListenPort: " +
                    $"appsettings.json not found at {settingsPath}");
                return ActionResult.Failure;
            }

            session.Log(
                $"[SetupActionCertificate] ConfigureServerListenPort: " +
                $"{bindings.Count} binding(s), port {port}.");

            UpdateListenEndpointsInAppSettings(
                session,
                settingsPath,
                port,
                bindings);
            WriteServerListenPortRegistry(session, port);
            WriteServerListenBindingsRegistry(session, bindingsSerialized);

            if (syncClient)
            {
                WriteClientInstallListenPortRegistry(session, port);
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult GenerateSelfSignedCert(Session session)
        {
            LoadAssemblies(session);

            var serverfolder = session.CustomActionData["SERVERFOLDER"];
            var certPath = Path.Combine(serverfolder, "cert");
            _ = Directory.CreateDirectory(certPath);
            var certPrivatePath = Path.Combine(certPath, "VideoDedup.pfx");
            var certPublicPath = Path.Combine(certPath, "VideoDedup.crt");
            var settingsPath = Path.Combine(serverfolder, "appsettings.json");

            var randomPassword =
                Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            CreateCertificate(
                session,
                certPrivatePath,
                certPublicPath,
                randomPassword,
                out var thumbprint);

            UpdateAppSettings(
                session,
                settingsPath,
                certPrivatePath,
                randomPassword);

            WriteServerCertDiscovery(session, certPublicPath, thumbprint);

            InstallCertificate(session, certPublicPath);

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RemoveSelfSignedCert(Session session)
        {
            LoadAssemblies(session);

            var serverfolder = session.CustomActionData["SERVERFOLDER"];
            var certPath = Path.Combine(serverfolder, "cert");
            var certPublicPath = Path.Combine(certPath, "VideoDedup.crt");

            string thumbprint = null;
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(ServerRegistryKey))
                {
                    thumbprint = key?.GetValue("CertThumbprint") as string;
                }
            }
            catch (Exception ex)
            {
                session.Log(
                    $"[SetupActionCertificate] Warning reading cert thumbprint: {ex}");
            }

            if (File.Exists(certPublicPath))
            {
                try
                {
                    RemoveCertificate(session, certPublicPath);
                }
                catch (Exception ex)
                {
                    session.Log(
                        $"[SetupActionCertificate] Warning removing cert from file: {ex}");
                    RemoveCertificateByThumbprint(session, thumbprint);
                }
            }
            else
            {
                session.Log(
                    $"[SetupActionCertificate] Cert file not found at {certPublicPath}; " +
                    "removing by thumbprint if available.");
                RemoveCertificateByThumbprint(session, thumbprint);
            }

            RemoveServerCertDiscovery(session);

            if (Directory.Exists(certPath))
            {
                Directory.Delete(certPath, true);
                session.Log($"Removed certificate directory: " +
                    $"{certPath}");
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ImportClientCertificate(Session session)
        {
            LoadAssemblies(session);

            var clientFolder = session.CustomActionData["CLIENTFOLDER"];
            var source = session.CustomActionData["CLIENTCERTSOURCE"];

            if (string.IsNullOrWhiteSpace(source))
            {
                session.Log(
                    "[SetupActionCertificate] ImportClientCertificate: " +
                    "CLIENTCERTSOURCE empty; skipping.");
                return ActionResult.Success;
            }

            if (!File.Exists(source))
            {
                session.Log(
                    $"[SetupActionCertificate] ImportClientCertificate: " +
                    $"source file not found: {source}. " +
                    "If the file is on a network share, copy it to a local folder " +
                    "and run setup again, or use a build of the bootstrapper that " +
                    "stages the certificate under ProgramData before install.");
                return ActionResult.Failure;
            }

            try
            {
                using (var _ = new X509Certificate2(source))
                {
                }
            }
            catch (Exception ex)
            {
                session.Log(
                    $"[SetupActionCertificate] ImportClientCertificate: " +
                    $"invalid certificate file: {ex}");
                return ActionResult.Failure;
            }

            var destDir = Path.Combine(clientFolder, "cert");
            _ = Directory.CreateDirectory(destDir);
            var destPath = Path.Combine(destDir, "VideoDedup.crt");
            File.Copy(source, destPath, overwrite: true);

            using (var pub = new X509Certificate2(destPath))
            {
                session.Log(
                    $"[SetupActionCertificate] Client certificate imported to " +
                    $"{destPath}, thumbprint {pub.Thumbprint}");
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RemoveClientCertificate(Session session)
        {
            LoadAssemblies(session);

            var clientFolder = session.CustomActionData["CLIENTFOLDER"];
            var certDir = Path.Combine(clientFolder, "cert");

            if (!Directory.Exists(certDir))
            {
                session.Log(
                    "[SetupActionCertificate] RemoveClientCertificate: " +
                    "cert directory absent.");
                return ActionResult.Success;
            }

            try
            {
                Directory.Delete(certDir, recursive: true);
                session.Log(
                    $"[SetupActionCertificate] Removed client cert directory: " +
                    $"{certDir}");
            }
            catch (Exception ex)
            {
                session.Log(
                    $"[SetupActionCertificate] RemoveClientCertificate: {ex}");
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }
    }
}
