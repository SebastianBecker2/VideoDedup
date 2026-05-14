using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using Google.Protobuf;
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
// Override with VIDEODEDUP_SMOKE_COMPARE_LEFT / RIGHT for paths on the gRPC server. Windows absolute
// paths are kept when VIDEODEDUP_GRPC_URL points at this machine (localhost, 127.0.0.1, hostname);
// otherwise they are replaced by the POSIX defaults (Linux/Docker). Set VIDEODEDUP_SMOKE_COMPARE_ASSUME_REMOTE_POSIX=1
// to always drop Windows paths even for localhost (Linux server in Docker on the same host).
// Poll GetVideoComparisonStatus until a terminal result or timeout (default 60s; VIDEODEDUP_COMPARISON_POLL_TIMEOUT_SEC).
// Completed scenarios use ForceLoadingAllFrames=true and CompareCount=100. Polling advances FrameComparisonIndex
// like CustomVideoComparisonDlg (Max(frame index)+1); exit when terminal is set and max frame index >= CompareCount-1,
// then a single request with FrameComparisonIndex=0 validates the full frame list length
// and that each FrameSet carries non-empty payloads (JPEG stills vs raw grey bytes; see VideoComparer
// CacheableFrameSet.ToFrameSet and VideoDedupClient FrameSet.cs / FrameComparisonResultViewCtl).

static string? Env(string key) => Environment.GetEnvironmentVariable(key);

const string DefaultFixtureLeft = "/tmp/vd-fixtures/grpc-smoke/left.mp4";
const string DefaultFixtureRight = "/tmp/vd-fixtures/grpc-smoke/right.mp4";

/// <summary>
/// Must match <see cref="VideoComparisonSettings.CompareCount"/> in deep-smoke requests.
/// With <c>ForceLoadingAllFrames</c> true, VideoComparer runs all load levels to completion,
/// producing exactly this many <c>FrameComparisonResult</c> rows (indices 0 .. count-1).
/// </summary>
const int SmokeCompareCount = 100;

var url = args.Length > 0
    ? args[0]
    : Env("VIDEODEDUP_GRPC_URL") ?? "http://127.0.0.1:51726";

var compareLeft = ResolveSmokeComparePath(
    Env("VIDEODEDUP_SMOKE_COMPARE_LEFT"),
    DefaultFixtureLeft,
    "VIDEODEDUP_SMOKE_COMPARE_LEFT",
    url);
var compareRight = ResolveSmokeComparePath(
    Env("VIDEODEDUP_SMOKE_COMPARE_RIGHT"),
    DefaultFixtureRight,
    "VIDEODEDUP_SMOKE_COMPARE_RIGHT",
    url);

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
            ForceLoadingAllFrames = true,
            LeftFilePath = leftPath,
            RightFilePath = rightPath,
            VideoComparisonSettings = new VideoComparisonSettings
            {
                CompareCount = SmokeCompareCount,
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
            AssertFullComparisonFrameCount(startResp, SmokeCompareCount, scenarioLabel);
            Console.WriteLine(
                "OK: VideoComparison deep smoke ({3}) — url={0}, result={1}, token_len={2}, frames={4}",
                url,
                terminal,
                comparisonToken.Length,
                scenarioLabel,
                startResp.FrameComparisons.Count);
            return 0;
        }

        // Same indexing idea as CustomVideoComparisonDlg.HandleStatusTimerTick: request only
        // frame rows with index >= FrameComparisonIndex, then set next request to Max(index)+1.
        // The dialog stops (cancels) when the next index is >= CompareCount; equivalently we have
        // seen every slot when maxIndexSeen >= CompareCount - 1 (indices are 0-based).
        const int lastExpectedFrameIndex = SmokeCompareCount - 1;

        var deadline = Stopwatch.GetTimestamp() + Stopwatch.Frequency * (long)pollTimeoutSec;
        var statusReq = new VideoComparisonStatusRequest
        {
            ComparisonToken = comparisonToken,
            FrameComparisonIndex = 0,
        };

        var maxFrameIndexSeen = -1;

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

            if (pollResp.FrameComparisons.Count > 0)
            {
                var batchMax = pollResp.FrameComparisons.Max(static f => f.Index);
                maxFrameIndexSeen = Math.Max(maxFrameIndexSeen, batchMax);
                statusReq.FrameComparisonIndex = batchMax + 1;
            }

            terminal = EffectiveComparisonResult(pollResp);
            if (terminal != ComparisonResult.NoResult
                && maxFrameIndexSeen >= lastExpectedFrameIndex)
            {
                // Incremental responses only contain a slice; fetch from 0 once for full list checks.
                currentStep = "GetVideoComparisonStatus (final snapshot)";
                VideoComparisonStatus finalResp;
                try
                {
                    finalResp = await client.GetVideoComparisonStatusAsync(
                        new VideoComparisonStatusRequest
                        {
                            ComparisonToken = comparisonToken,
                            FrameComparisonIndex = 0,
                        });
                }
                catch (RpcException ex)
                {
                    Console.Error.WriteLine("gRPC failed during {0}: {1}", currentStep, ex.Status);
                    return 1;
                }

                terminal = EffectiveComparisonResult(finalResp);
                AssertExpectedComparisonTerminal(
                    terminal,
                    finalResp.VideoComparisonResult,
                    expectedTerminal,
                    scenarioLabel);
                AssertFullComparisonFrameCount(finalResp, SmokeCompareCount, scenarioLabel);
                Console.WriteLine(
                    "OK: VideoComparison deep smoke ({3}) — url={0}, result={1}, host={2}, frames={4}",
                    url,
                    terminal,
                    sys.MachineName,
                    scenarioLabel,
                    finalResp.FrameComparisons.Count);
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
            ForceLoadingAllFrames = true,
            LeftFilePath = compareLeft,
            RightFilePath = compareRight,
            VideoComparisonSettings = new VideoComparisonSettings
            {
                CompareCount = SmokeCompareCount,
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

static void AssertFullComparisonFrameCount(
    VideoComparisonStatus status,
    int expectedCount,
    string scenarioLabel)
{
    var n = status.FrameComparisons.Count;
    if (n != expectedCount)
    {
        var maxIdx = status.FrameComparisons.Count == 0
            ? -1
            : status.FrameComparisons.Max(f => f.Index);
        throw new InvalidOperationException(
            $"scenario={scenarioLabel}: expected {expectedCount} frame comparison(s) in status "
            + $"(ForceLoadingAllFrames + CompareCount), got {n} (max index {maxIdx}).");
    }

    AssertAllFrameSetsHaveImagePayloads(status, scenarioLabel);
}

/// <summary>
/// Encoded stills (original from FFmpeg MJPEG, cropped/resized/greyscaled from SkiaSharp JPEG) must
/// start with a JPEG SOI marker. <c>Bytes</c> is the raw per-pixel strip used for numeric difference
/// (see VideoComparer.GetFrameBytes), not a second JPEG — the UI never displays it as an image.
/// </summary>
static void AssertAllFrameSetsHaveImagePayloads(
    VideoComparisonStatus status,
    string scenarioLabel)
{
    foreach (var f in status.FrameComparisons)
    {
        var prefix = $"{scenarioLabel} frame_index={f.Index}";
        AssertFrameSetPayloads(f.LeftFrames, $"{prefix} left");
        AssertFrameSetPayloads(f.RightFrames, $"{prefix} right");
    }
}

static void AssertFrameSetPayloads(FrameSet fs, string context)
{
    AssertJpegStill($"{context}.original", fs.Original);
    AssertJpegStill($"{context}.cropped", fs.Cropped);
    AssertJpegStill($"{context}.resized", fs.Resized);
    AssertJpegStill($"{context}.greyscaled", fs.Greyscaled);
    AssertRawComparisonBytes($"{context}.bytes", fs.Bytes);
}

static void AssertJpegStill(string label, ByteString? data)
{
    if (data is null || data.IsEmpty)
    {
        throw new InvalidOperationException($"{label}: missing or empty payload.");
    }

    if (!LooksLikeJpeg(data))
    {
        throw new InvalidOperationException(
            $"{label}: expected JPEG SOI (0xFF 0xD8); got length={data.Length}, "
            + $"first_bytes={FormatFirstBytes(data)}.");
    }
}

/// <summary>Raw greyscale strip for VideoComparer difference (16×16 → 256 single-channel samples).</summary>
static void AssertRawComparisonBytes(string label, ByteString? data)
{
    const int minLen = 200;
    if (data is null || data.Length < minLen)
    {
        throw new InvalidOperationException(
            $"{label}: expected at least {minLen} raw byte(s); got {(data is null ? "null" : data.Length.ToString(CultureInfo.InvariantCulture))}.");
    }

    if (LooksLikeJpeg(data))
    {
        throw new InvalidOperationException(
            $"{label}: payload looks like JPEG but this field must be raw pixel bytes, not an encoded still.");
    }
}

static bool LooksLikeJpeg(ByteString data) =>
    data.Length >= 2 && data[0] == 0xFF && data[1] == 0xD8;

static string FormatFirstBytes(ByteString data, int max = 8)
{
    var n = Math.Min(max, data.Length);
    return string.Join(
        " ",
        Enumerable.Range(0, n).Select(i => data[i].ToString("X2", CultureInfo.InvariantCulture)));
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

static string ResolveSmokeComparePath(
    string? fromEnv,
    string serverDefault,
    string label,
    string grpcUrl)
{
    if (string.IsNullOrWhiteSpace(fromEnv))
    {
        return serverDefault;
    }

    if (LooksLikeWindowsAbsolutePath(fromEnv))
    {
        var assumeRemotePosix = !string.IsNullOrEmpty(
            Env("VIDEODEDUP_SMOKE_COMPARE_ASSUME_REMOTE_POSIX"));
        if (!assumeRemotePosix && IsGrpcUrlLikelySameMachineAsClient(grpcUrl))
        {
            return fromEnv;
        }

        Console.Error.WriteLine(
            "{0}='{1}' is a Windows absolute path; gRPC URL '{2}' is not treated as this machine "
            + "(or VIDEODEDUP_SMOKE_COMPARE_ASSUME_REMOTE_POSIX is set). Using server default '{3}'.",
            label,
            fromEnv,
            grpcUrl,
            serverDefault);
        return serverDefault;
    }

    return fromEnv;
}

/// <summary>
/// True when the client URL points at a service expected to share this machine's filesystem
/// (native Windows server, or dev loopback). False for typical remote Linux hosts.
/// </summary>
static bool IsGrpcUrlLikelySameMachineAsClient(string grpcUrl)
{
    if (!Uri.TryCreate(grpcUrl, UriKind.Absolute, out var uri))
    {
        return false;
    }

    var host = uri.IdnHost;
    if (string.IsNullOrEmpty(host))
    {
        return false;
    }

    if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase))
    {
        return true;
    }

    if (host is "127.0.0.1" or "::1")
    {
        return true;
    }

    if (OperatingSystem.IsWindows())
    {
        if (string.Equals(host, Environment.MachineName, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        try
        {
            var dnsHost = System.Net.Dns.GetHostName();
            if (string.Equals(host, dnsHost, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        catch
        {
            // ignore DNS failures
        }
    }

    return false;
}

static bool LooksLikeWindowsAbsolutePath(string path)
{
    ReadOnlySpan<char> t = path.AsSpan().TrimStart();
    return t.Length >= 3
        && char.IsAsciiLetter(t[0])
        && t[1] == ':'
        && (t[2] == '\\' || t[2] == '/');
}
