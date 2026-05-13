using System.Diagnostics;
using System.Net.Http;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using VideoDedupGrpc;
using static VideoDedupGrpc.VideoDedupGrpcService;

// Deep smoke: three VideoComparison runs with fixed settings; poll until a terminal result (or cancel) each.
// 1) Distinct left/right paths → expect DIFFERENT (E2E fixtures must differ visually).
// 2) Left path for both sides (self-compare) → expect DUPLICATE.
// 3) Distinct paths again → CancelVideoComparison immediately after start → GetVideoComparisonStatus until
//    protobuf CANCELLED or RpcException with StatusCode.Cancelled (server removes session; null response).
// StartVideoComparison must return a non-empty ComparisonToken; Rpc failures fail the run.
// Paths default to /tmp/vd-fixtures/grpc-smoke/{left,right}.mp4 (E2E server mount).
// Override with VIDEODEDUP_SMOKE_COMPARE_LEFT / RIGHT only when using other server-visible paths.
// Poll GetVideoComparisonStatus until a terminal result or timeout (default 60s; VIDEODEDUP_COMPARISON_POLL_TIMEOUT_SEC).

static string? Env(string key) => Environment.GetEnvironmentVariable(key);

const string DefaultFixtureLeft = "/tmp/vd-fixtures/grpc-smoke/left.mp4";
const string DefaultFixtureRight = "/tmp/vd-fixtures/grpc-smoke/right.mp4";

var url = args.Length > 0
    ? args[0]
    : Env("VIDEODEDUP_GRPC_URL") ?? "http://127.0.0.1:51726";

var compareLeft = ResolveSmokeComparePath(
    Env("VIDEODEDUP_SMOKE_COMPARE_LEFT"),
    DefaultFixtureLeft,
    "VIDEODEDUP_SMOKE_COMPARE_LEFT");
var compareRight = ResolveSmokeComparePath(
    Env("VIDEODEDUP_SMOKE_COMPARE_RIGHT"),
    DefaultFixtureRight,
    "VIDEODEDUP_SMOKE_COMPARE_RIGHT");

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
        // GetVideoComparisonStatus returns repeated frame_comparisons (image bytes); DUPLICATE/self-compare
        // can exceed the default ~4 MB client receive cap before the tool sees a terminal result.
        MaxReceiveMessageSize = 128 * 1024 * 1024,
    });

var client = new VideoDedupGrpcServiceClient(channel);

var currentStep = "init";
try
{
    currentStep = "GetSystemInfo";
    var sys = await client.GetSystemInfoAsync(new Empty());

    if (string.IsNullOrWhiteSpace(sys.MachineName)
        || sys.ProcessorCount < 1
        || string.IsNullOrWhiteSpace(sys.OsDescription))
    {
        throw new InvalidOperationException(
            "GetSystemInfo: expected non-empty MachineName and OsDescription, and ProcessorCount >= 1.");
    }

    var fw = sys.FrameworkDescription ?? string.Empty;
    if (!fw.Contains(".NET", StringComparison.OrdinalIgnoreCase))
    {
        throw new InvalidOperationException(
            $"GetSystemInfo: FrameworkDescription unlikely for this host: '{fw}'");
    }

    var expectMachine = Env("VIDEODEDUP_SMOKE_EXPECT_MACHINE_NAME");
    if (!string.IsNullOrEmpty(expectMachine)
        && !string.Equals(sys.MachineName, expectMachine, StringComparison.Ordinal))
    {
        throw new InvalidOperationException(
            $"GetSystemInfo: MachineName '{sys.MachineName}' does not match "
            + $"VIDEODEDUP_SMOKE_EXPECT_MACHINE_NAME '{expectMachine}'.");
    }

    var pollTimeoutSec = 60;
    if (int.TryParse(Env("VIDEODEDUP_COMPARISON_POLL_TIMEOUT_SEC"), out var parsedTimeout)
        && parsedTimeout > 0)
    {
        pollTimeoutSec = parsedTimeout;
    }

    async Task<int> runScenarioAsync(
        string leftPath,
        string rightPath,
        ComparisonResult expectedTerminal,
        string scenarioLabel)
    {
        var startReq = new VideoComparisonConfiguration
        {
            ForceLoadingAllFrames = false,
            LeftFilePath = leftPath,
            RightFilePath = rightPath,
            VideoComparisonSettings = new VideoComparisonSettings
            {
                CompareCount = 100,
                MaxDifferentFrames = 10,
                MaxDifference = 80,
            },
        };

        currentStep = "StartVideoComparison";
        var startResp = await client.StartVideoComparisonAsync(startReq);

        var comparisonToken = startResp.ComparisonToken ?? string.Empty;
        if (string.IsNullOrWhiteSpace(comparisonToken))
        {
            var vrStart = startResp.VideoComparisonResult;
            var reason = vrStart?.Reason ?? "(none)";
            var cr = vrStart?.ComparisonResult.ToString() ?? "null";
            throw new InvalidOperationException(
                "StartVideoComparison failed: empty ComparisonToken (deep smoke requires a started session). "
                + $"scenario={scenarioLabel} left='{leftPath}' right='{rightPath}' "
                + $"comparison_result={cr} reason='{reason}'");
        }

        var terminal = EffectiveComparisonResult(startResp);
        if (terminal != ComparisonResult.NoResult)
        {
            AssertExpectedComparisonTerminal(terminal, startResp.VideoComparisonResult, expectedTerminal, scenarioLabel);
            Console.WriteLine(
                "OK: VideoComparison deep smoke ({3}) — url={0}, result={1}, token_len={2}",
                url,
                terminal,
                comparisonToken.Length,
                scenarioLabel);
            return 0;
        }

        var deadline = Stopwatch.GetTimestamp() + Stopwatch.Frequency * (long)pollTimeoutSec;
        var statusReq = new VideoComparisonStatusRequest
        {
            ComparisonToken = comparisonToken,
            FrameComparisonIndex = 0,
        };

        while (Stopwatch.GetTimestamp() < deadline)
        {
            currentStep = "GetVideoComparisonStatus";
            VideoComparisonStatus pollResp;
            try
            {
                pollResp = await client.GetVideoComparisonStatusAsync(statusReq);
            }
            catch (RpcException ex)
            {
                Console.Error.WriteLine("gRPC failed during {0}: {1}", currentStep, ex.Status);
                return 1;
            }

            terminal = EffectiveComparisonResult(pollResp);
            if (terminal != ComparisonResult.NoResult)
            {
                AssertExpectedComparisonTerminal(terminal, pollResp.VideoComparisonResult, expectedTerminal, scenarioLabel);
                Console.WriteLine(
                    "OK: VideoComparison deep smoke ({3}) — url={0}, result={1}, host={2}",
                    url,
                    terminal,
                    sys.MachineName,
                    scenarioLabel);
                return 0;
            }

            await Task.Delay(100);
        }

        Console.Error.WriteLine(
            "Timed out after {0}s waiting for GetVideoComparisonStatus (scenario={1}, expected {2}). "
            + "Override with VIDEODEDUP_COMPARISON_POLL_TIMEOUT_SEC.",
            pollTimeoutSec,
            scenarioLabel,
            expectedTerminal);
        return 2;
    }

    async Task<int> runCancelAfterStartAsync()
    {
        const string scenarioLabel = "cancel after start";
        var startReq = new VideoComparisonConfiguration
        {
            ForceLoadingAllFrames = false,
            LeftFilePath = compareLeft,
            RightFilePath = compareRight,
            VideoComparisonSettings = new VideoComparisonSettings
            {
                CompareCount = 100,
                MaxDifferentFrames = 10,
                MaxDifference = 80,
            },
        };

        currentStep = "StartVideoComparison";
        var startResp = await client.StartVideoComparisonAsync(startReq);

        var comparisonToken = startResp.ComparisonToken ?? string.Empty;
        if (string.IsNullOrWhiteSpace(comparisonToken))
        {
            var vrStart = startResp.VideoComparisonResult;
            var reason = vrStart?.Reason ?? "(none)";
            var cr = vrStart?.ComparisonResult.ToString() ?? "null";
            throw new InvalidOperationException(
                "StartVideoComparison failed: empty ComparisonToken (deep smoke requires a started session). "
                + $"scenario={scenarioLabel} left='{compareLeft}' right='{compareRight}' "
                + $"comparison_result={cr} reason='{reason}'");
        }

        if (EffectiveComparisonResult(startResp) != ComparisonResult.NoResult)
        {
            throw new InvalidOperationException(
                "cancel-after-start: StartVideoComparison already returned a terminal result before "
                + $"CancelVideoComparison; cannot assert CANCELLED. scenario={scenarioLabel}");
        }

        currentStep = "CancelVideoComparison";
        await client.CancelVideoComparisonAsync(
            new CancelVideoComparisonRequest { ComparisonToken = comparisonToken });

        var deadline = Stopwatch.GetTimestamp() + Stopwatch.Frequency * (long)pollTimeoutSec;
        var statusReq = new VideoComparisonStatusRequest
        {
            ComparisonToken = comparisonToken,
            FrameComparisonIndex = 0,
        };

        while (Stopwatch.GetTimestamp() < deadline)
        {
            currentStep = "GetVideoComparisonStatus";
            try
            {
                var pollResp = await client.GetVideoComparisonStatusAsync(statusReq);
                var terminalPoll = EffectiveComparisonResult(pollResp);
                if (terminalPoll == ComparisonResult.Cancelled)
                {
                    Console.WriteLine(
                        "OK: VideoComparison deep smoke ({3}) — url={0}, result={1}, host={2}",
                        url,
                        terminalPoll,
                        sys.MachineName,
                        scenarioLabel);
                    return 0;
                }

                if (terminalPoll != ComparisonResult.NoResult)
                {
                    throw new InvalidOperationException(
                        $"scenario={scenarioLabel}: after CancelVideoComparison, expected protobuf CANCELLED, "
                        + $"gRPC Cancelled, or NO_RESULT while polling; got {terminalPoll}.");
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine(
                    "OK: VideoComparison deep smoke ({3}) — url={0}, gRPC={1}, host={2} (session ended after cancel)",
                    url,
                    ex.StatusCode,
                    sys.MachineName,
                    scenarioLabel);
                return 0;
            }
            catch (RpcException ex)
            {
                Console.Error.WriteLine("gRPC failed during {0}: {1}", currentStep, ex.Status);
                return 1;
            }

            await Task.Delay(100);
        }

        Console.Error.WriteLine(
            "Timed out after {0}s waiting for gRPC Cancelled or protobuf CANCELLED after CancelVideoComparison "
            + "(scenario={1}). Override with VIDEODEDUP_COMPARISON_POLL_TIMEOUT_SEC.",
            pollTimeoutSec,
            scenarioLabel);
        return 2;
    }

    var exitDistinct = await runScenarioAsync(
        compareLeft,
        compareRight,
        ComparisonResult.Different,
        "distinct fixtures");
    if (exitDistinct != 0)
    {
        return exitDistinct;
    }

    var exitSelf = await runScenarioAsync(
        compareLeft,
        compareLeft,
        ComparisonResult.Duplicate,
        "same file both sides");
    if (exitSelf != 0)
    {
        return exitSelf;
    }

    return await runCancelAfterStartAsync();
}
catch (RpcException ex)
{
    Console.Error.WriteLine("gRPC failed during {0}: {1}", currentStep, ex.Status);
    return 1;
}
catch (Exception ex)
{
    Console.Error.WriteLine("Comparison smoke failed during {0}: {1}", currentStep, ex.Message);
    return 2;
}

static ComparisonResult EffectiveComparisonResult(VideoComparisonStatus status)
{
    var vr = status.VideoComparisonResult;
    return vr is null ? ComparisonResult.NoResult : vr.ComparisonResult;
}

static void AssertExpectedComparisonTerminal(
    ComparisonResult actual,
    VideoComparisonResult? vr,
    ComparisonResult expected,
    string scenarioLabel)
{
    if (actual == expected)
    {
        return;
    }

    var reason = vr?.Reason ?? string.Empty;
    var msg = (expected, actual) switch
    {
        (ComparisonResult.Different, ComparisonResult.Duplicate) =>
            "Expected DIFFERENT but comparison reported DUPLICATE. "
            + $"scenario={scenarioLabel} E2E fixtures must be visually distinct. Reason: '{reason}'",
        (ComparisonResult.Duplicate, ComparisonResult.Different) =>
            "Expected DUPLICATE (same file both sides) but comparison reported DIFFERENT. "
            + $"scenario={scenarioLabel} Reason: '{reason}'",
        (ComparisonResult.Cancelled, ComparisonResult.Different) =>
            "Expected CANCELLED after CancelVideoComparison but got DIFFERENT. "
            + $"scenario={scenarioLabel} Reason: '{reason}'",
        (ComparisonResult.Cancelled, ComparisonResult.Duplicate) =>
            "Expected CANCELLED after CancelVideoComparison but got DUPLICATE. "
            + $"scenario={scenarioLabel} Reason: '{reason}'",
        _ =>
            $"scenario={scenarioLabel}: expected terminal {expected} but got {actual}. Reason: '{reason}'",
    };
    throw new InvalidOperationException(msg);
}

static string ResolveSmokeComparePath(string? fromEnv, string serverDefault, string label)
{
    if (string.IsNullOrWhiteSpace(fromEnv))
    {
        return serverDefault;
    }

    if (LooksLikeWindowsAbsolutePath(fromEnv))
    {
        Console.Error.WriteLine(
            "{0}='{1}' is a Windows-style path on this machine, not a path inside the gRPC server; "
            + "using server default '{2}' instead.",
            label,
            fromEnv,
            serverDefault);
        return serverDefault;
    }

    return fromEnv;
}

static bool LooksLikeWindowsAbsolutePath(string path)
{
    ReadOnlySpan<char> t = path.AsSpan().TrimStart();
    return t.Length >= 3
        && char.IsAsciiLetter(t[0])
        && t[1] == ':'
        && (t[2] == '\\' || t[2] == '/');
}
