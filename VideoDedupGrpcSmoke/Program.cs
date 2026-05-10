using System.Net.Http;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using VideoDedupGrpc;
using static VideoDedupGrpc.VideoDedupGrpcService;

var url = args.Length > 0
    ? args[0]
    : Environment.GetEnvironmentVariable("VIDEODEDUP_GRPC_URL") ?? "http://127.0.0.1:51726";

if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
{
    AppContext.SetSwitch(
        "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",
        true);
}

using var handler = new SocketsHttpHandler();
using var channel = GrpcChannel.ForAddress(
    url,
    new GrpcChannelOptions { HttpHandler = handler });
var client = new VideoDedupGrpcServiceClient(channel);

try
{
    var status = await client.GetCurrentStatusAsync(new Empty());
    var cfg = await client.GetConfigurationAsync(new Empty());
    var sys = await client.GetSystemInfoAsync(new Empty());

    if (string.IsNullOrEmpty(sys.MachineName))
    {
        Console.Error.WriteLine("GetSystemInfo: empty machine name");
        return 2;
    }

    Console.WriteLine(
        "OK: gRPC {0} — host={1}, duplicates={2}, log_count={3}, base_path_len={4}",
        url,
        sys.MachineName,
        status.TotalDuplicatesCount,
        status.LogCount,
        cfg.DedupSettings.BasePath.Length);
    return 0;
}
catch (RpcException ex)
{
    Console.Error.WriteLine("gRPC failed: {0}", ex.Status);
    return 1;
}
