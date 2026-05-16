using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;

namespace VideoDedupGrpcSmoke.Common;

/// <summary>
/// gRPC channel for smoke tests: HTTPS with optional <c>VideoDedup.crt</c> thumbprint pinning.
/// </summary>
public static class GrpcSmokeChannel
{
    public const string DefaultUrl = "https://127.0.0.1:51726";

    public static GrpcChannel Create(string url, long maxReceiveMessageSize = 64 * 1024 * 1024)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL is required.", nameof(url));

        var handler = new SocketsHttpHandler();
        if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
        {
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",
                true);
        }
        else if (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            var pinnedThumb = TryLoadPinnedThumbprint();
            if (string.IsNullOrEmpty(pinnedThumb))
            {
                throw new InvalidOperationException(
                    "HTTPS smoke URL requires a pinned certificate. Set VIDEODEDUP_SMOKE_PINNED_CERT "
                    + "to the server VideoDedup.crt path, or place cert/VideoDedup.crt next to the smoke binary.");
            }

            handler.SslOptions.RemoteCertificateValidationCallback =
                (_, certificate, _, _) =>
                {
                    if (certificate is null)
                        return false;
                    using var leaf = new X509Certificate2(certificate);
                    return string.Equals(
                        leaf.Thumbprint,
                        pinnedThumb,
                        StringComparison.OrdinalIgnoreCase);
                };
        }

        return GrpcChannel.ForAddress(
            url,
            new GrpcChannelOptions
            {
                HttpHandler = handler,
                MaxReceiveMessageSize = (int)maxReceiveMessageSize,
            });
    }

    public static string? ResolvePinnedCertificatePath()
    {
        var fromEnv = Environment.GetEnvironmentVariable("VIDEODEDUP_SMOKE_PINNED_CERT")?.Trim();
        if (!string.IsNullOrEmpty(fromEnv) && File.Exists(fromEnv))
            return fromEnv;

        var alongside = Path.Combine(AppContext.BaseDirectory, "cert", "VideoDedup.crt");
        return File.Exists(alongside) ? alongside : null;
    }

    private static string? TryLoadPinnedThumbprint()
    {
        var path = ResolvePinnedCertificatePath();
        if (string.IsNullOrEmpty(path))
            return null;

        try
        {
            using var cert = new X509Certificate2(path);
            return cert.Thumbprint;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to load pinned certificate from '{path}': {ex.Message}",
                ex);
        }
    }
}
