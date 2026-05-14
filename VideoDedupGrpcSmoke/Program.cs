using System.Net.Http;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using VideoDedupGrpc;
using static VideoDedupGrpc.VideoDedupGrpcService;

static string? Env(string key) => Environment.GetEnvironmentVariable(key);

var url = args.Length > 0
    ? args[0]
    : Env("VIDEODEDUP_GRPC_URL") ?? "http://127.0.0.1:51726";

if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
{
    AppContext.SetSwitch(
        "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport",
        true);
}

using var handler = new SocketsHttpHandler();
using var channel = GrpcChannel.ForAddress(
    url,
    new GrpcChannelOptions
    {
        HttpHandler = handler,
        MaxReceiveMessageSize = 128 * 1024 * 1024,
    });

var client = new VideoDedupGrpcServiceClient(channel);

var currentStep = "init";
try
{
    // 1) Basic calls + token source (status/log_token/progress_token).
    currentStep = "GetCurrentStatus";
    var status1 = await client.GetCurrentStatusAsync(new Empty());
    currentStep = "GetConfiguration";
    var cfg1 = await client.GetConfigurationAsync(new Empty());
    currentStep = "GetSystemInfo";
    var sys = await client.GetSystemInfoAsync(new Empty());

    if (string.IsNullOrEmpty(sys.MachineName))
        throw new InvalidOperationException("GetSystemInfo: empty machine name");

    // 2) Round-trip config — SetConfiguration must succeed (no soft-fail); verify read-back.
    currentStep = "SetConfiguration";
    await client.SetConfigurationAsync(cfg1.Clone());

    currentStep = "GetConfiguration (verify SetConfiguration)";
    var cfgVerified = await client.GetConfigurationAsync(new Empty());
    if (cfgVerified.DedupSettings.BasePath != cfg1.DedupSettings.BasePath
        || cfgVerified.VideoComparisonSettings.CompareCount
            != cfg1.VideoComparisonSettings.CompareCount)
    {
        throw new InvalidOperationException(
            "SetConfiguration verification failed: GetConfiguration after set "
            + "did not match sent BasePath or CompareCount.");
    }

    currentStep = "GetCurrentStatus (post-set)";
    var status2 = await client.GetCurrentStatusAsync(new Empty());

    // 3) GetLogEntries (token-sensitive).
    var logCount = Math.Max(0, status2.LogCount);
    currentStep = "GetLogEntries";
    var logReq = new GetLogEntriesRequest
    {
        LogToken = status2.LogToken,
        Start = 0,
        Count = Math.Min(32, logCount)
    };
    var logResp = await client.GetLogEntriesAsync(logReq);

    // 4) GetProgressInfo (token-sensitive).
    currentStep = "GetProgressInfo";
    var progressReq = new GetProgressInfoRequest
    {
        ProgressToken = status2.OperationInfo.ProgressToken,
        Start = 0,
        Count = 8
    };
    var progressResp = await client.GetProgressInfoAsync(progressReq);

    // 5) GetFolderContent (use empty path for "drives" on Windows and "/" on Linux).
    currentStep = "GetFolderContent";
    var folderReq = new GetFolderContentRequest
    {
        Path = "",
        TypeRestriction = FileType.Any
    };
    var folderResp = await client.GetFolderContentAsync(folderReq);
    if (folderResp.RequestFailed)
        throw new InvalidOperationException("GetFolderContent: request_failed=true");

    // 6) Duplicate management.
    currentStep = "GetDuplicate";
    _ = await client.GetDuplicateAsync(new Empty());
    currentStep = "DiscardDuplicates";
    await client.DiscardDuplicatesAsync(new Empty());

    var resolveReq = new ResolveDuplicateRequest
    {
        DuplicateId = Guid.NewGuid().ToString(),
        ResolveOperation = ResolveOperation.Skip,
        // DuplicateManager.ResolveDuplicate returns early when the duplicate_id
        // does not exist, so these fields do not need to be valid media metadata.
        File = new VideoFile()
    };
    currentStep = "ResolveDuplicate";
    var resolveResp = await client.ResolveDuplicateAsync(resolveReq);
    if (!resolveResp.Successful)
        throw new InvalidOperationException(
            $"ResolveDuplicate: Successful=false error='{resolveResp.ErrorMessage}'");

    // 7) Video comparison + comparison status/cancel RPCs.
    // Paths are interpreted by the gRPC server (paths on the server's filesystem). Default matches
    // packaging E2E: /tmp/vd-fixtures/grpc-smoke/{left,right}.mp4 inside the server container.
    // Override with VIDEODEDUP_SMOKE_COMPARE_LEFT / RIGHT (server-side paths; unset/blank → defaults below).
    const string defaultFixtureLeft = "/tmp/vd-fixtures/grpc-smoke/left.mp4";
    const string defaultFixtureRight = "/tmp/vd-fixtures/grpc-smoke/right.mp4";
    var rawCompareLeft = Env("VIDEODEDUP_SMOKE_COMPARE_LEFT");
    var rawCompareRight = Env("VIDEODEDUP_SMOKE_COMPARE_RIGHT");
    var compareLeft = string.IsNullOrWhiteSpace(rawCompareLeft) ? defaultFixtureLeft : rawCompareLeft.Trim();
    var compareRight = string.IsNullOrWhiteSpace(rawCompareRight) ? defaultFixtureRight : rawCompareRight.Trim();
    Console.Error.WriteLine("VideoDedupGrpcSmoke: VIDEODEDUP_SMOKE_COMPARE_LEFT=" + compareLeft);
    Console.Error.WriteLine("VideoDedupGrpcSmoke: VIDEODEDUP_SMOKE_COMPARE_RIGHT=" + compareRight);

    // Keep comparison work small for CI.
    var vc = cfg1.VideoComparisonSettings.Clone();
    if (vc.CompareCount <= 0) vc.CompareCount = 3;
    vc.CompareCount = Math.Min(vc.CompareCount, 5);
    if (vc.MaxDifferentFrames <= 0) vc.MaxDifferentFrames = 2;
    vc.MaxDifferentFrames = Math.Min(vc.MaxDifferentFrames, 2);
    if (vc.MaxDifference <= 0) vc.MaxDifference = 40;
    vc.MaxDifference = Math.Min(vc.MaxDifference, 40);

    var startReq = new VideoComparisonConfiguration
    {
        VideoComparisonSettings = vc,
        LeftFilePath = compareLeft,
        RightFilePath = compareRight,
        ForceLoadingAllFrames = false
    };

    currentStep = "StartVideoComparison";
    string comparisonToken;
    VideoComparisonStatus? startComparisonResponse = null;
    try
    {
        startComparisonResponse = await client.StartVideoComparisonAsync(startReq);
        comparisonToken = startComparisonResponse.ComparisonToken ?? string.Empty;
    }
    catch (RpcException ex)
    {
        Console.Error.WriteLine(
            $"WARNING: StartVideoComparison RPC failed (smoke continues): {ex.Status}");
        if (!string.IsNullOrWhiteSpace(ex.Status.Detail))
        {
            Console.Error.WriteLine(
                $"WARNING: StartVideoComparison Status.Detail: {ex.Status.Detail}");
        }

        Console.Error.WriteLine(
            $"WARNING: StartVideoComparison paths: left='{compareLeft}' right='{compareRight}'");
        comparisonToken = string.Empty;
    }

    if (startComparisonResponse is not null
        && string.IsNullOrWhiteSpace(comparisonToken))
    {
        var vr = startComparisonResponse.VideoComparisonResult;
        Console.Error.WriteLine(
            "WARNING: StartVideoComparison returned no comparison_token. "
            + $"paths: left='{compareLeft}' right='{compareRight}'");
        if (vr is null)
            Console.Error.WriteLine("WARNING: StartVideoComparison: VideoComparisonResult is null.");
        else
        {
            Console.Error.WriteLine(
                $"WARNING: StartVideoComparison: comparison_result={vr.ComparisonResult}, "
                + $"reason='{vr.Reason}'");
        }
    }

    var tokenForExtraCalls = string.IsNullOrWhiteSpace(comparisonToken)
        ? Guid.NewGuid().ToString()
        : comparisonToken;

    var statusReq = new VideoComparisonStatusRequest
    {
        ComparisonToken = tokenForExtraCalls,
        FrameComparisonIndex = 0
    };
    currentStep = "GetVideoComparisonStatus";
    try
    {
        _ = await client.GetVideoComparisonStatusAsync(statusReq);
    }
    catch (RpcException ex)
    {
        Console.Error.WriteLine(
            $"WARNING: GetVideoComparisonStatus failed during smoke: {ex.Status}.");
    }

    currentStep = "CancelVideoComparison";
    try
    {
        await client.CancelVideoComparisonAsync(new CancelVideoComparisonRequest
        {
            ComparisonToken = tokenForExtraCalls
        });
    }
    catch (RpcException ex)
    {
        Console.Error.WriteLine(
            $"WARNING: CancelVideoComparison failed during smoke: {ex.Status}.");
    }

    Console.WriteLine(
        "OK: gRPC {0} — host={1}, duplicates={2}, log_count={3}, base_path_len={4}, log_entries={5}, progress_infos={6}",
        url,
        sys.MachineName,
        status2.TotalDuplicatesCount,
        status2.LogCount,
        cfg1.DedupSettings.BasePath.Length,
        logResp.LogEntries.Count,
        progressResp.ProgressInfos.Count);
    return 0;
}
catch (RpcException ex)
{
    Console.Error.WriteLine("gRPC failed during {0}: {1}", currentStep, ex.Status);
    return 1;
}
catch (Exception ex)
{
    Console.Error.WriteLine("Smoke assertion failed during {0}: {1}", currentStep, ex.Message);
    return 2;
}
