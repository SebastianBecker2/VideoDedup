using System.Diagnostics;
using System.Net.Http;
using Grpc.Core;
using Grpc.Net.Client;
using Google.Protobuf.WellKnownTypes;
using VideoDedupGrpc;
using static VideoDedupGrpc.OperationInfo.Types;
using static VideoDedupGrpc.VideoDedupGrpcService;

// Dedup deep smoke: point engine at fixture directory (4 mp4s including *_copy_dedup), poll until
// monitoring state with expected duplicate counts, then exercise progress, log, folder, resolve, discard.

static string? Env(string key) => Environment.GetEnvironmentVariable(key);

const string DefaultFixtureDir = "/tmp/vd-fixtures/grpc-smoke";

var url = args.Length > 0
    ? args[0]
    : Env("VIDEODEDUP_GRPC_URL") ?? "http://127.0.0.1:51726";

var fixtureDir = (Env("VIDEODEDUP_SMOKE_FIXTURE_DIR") ?? DefaultFixtureDir).Trim();
if (string.IsNullOrEmpty(fixtureDir))
    fixtureDir = DefaultFixtureDir;

var pollTimeoutSec = 60;
if (int.TryParse(Env("VIDEODEDUP_DEDUP_POLL_TIMEOUT_SEC"), out var t) && t > 0)
    pollTimeoutSec = t;

Console.Error.WriteLine($"VideoDedupGrpcDedupSmoke: fixture_dir={fixtureDir} poll_timeout_sec={pollTimeoutSec}");

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
        MaxReceiveMessageSize = 64 * 1024 * 1024,
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

    currentStep = "GetConfiguration";
    var cfg = await client.GetConfigurationAsync(new Empty());
    var updated = cfg.Clone();
    updated.DedupSettings.BasePath = fixtureDir;

    currentStep = "SetConfiguration";
    await client.SetConfigurationAsync(updated);

    currentStep = "poll GetCurrentStatus";
    var deadline = Stopwatch.GetTimestamp() + Stopwatch.Frequency * (long)pollTimeoutSec;
    StatusData? lastStatus = null;
    while (Stopwatch.GetTimestamp() < deadline)
    {
        var st = await client.GetCurrentStatusAsync(new Empty());
        lastStatus = st;
        var op = st.OperationInfo;
        if (op is null || st.LogCount <= 0)
        {
            await Task.Delay(150);
            continue;
        }

        if (st.TotalDuplicatesCount == 2
            && st.PreparedDuplicatesCount == 2
            && op.OperationType == OperationType.Monitoring
            && op.MaximumFiles == 4
            && op.ProgressCount >= 4)
        {
            break;
        }

        await Task.Delay(150);
    }

    if (lastStatus is null)
        throw new InvalidOperationException("GetCurrentStatus: never received a response.");

    var finalCheck = lastStatus;
    var opInfo = finalCheck.OperationInfo;
    if (opInfo is null
        || finalCheck.LogCount <= 0
        || finalCheck.TotalDuplicatesCount != 2
        || finalCheck.PreparedDuplicatesCount != 2
        || opInfo.OperationType != OperationType.Monitoring
        || opInfo.MaximumFiles != 4
        || opInfo.ProgressCount < 4)
    {
        Console.Error.WriteLine(
            "GetCurrentStatus poll timed out or expected values not reached. Last response:");
        Console.Error.WriteLine(finalCheck.ToString());
        throw new InvalidOperationException(
            "Timed out or condition mismatch: expected logCount>0, totalDuplicates=2, prepared=2, "
            + "MONITORING, maximum_files=4, progress_count>=4.");
    }

    currentStep = "GetProgressInfo";
    var progressResp = await client.GetProgressInfoAsync(new GetProgressInfoRequest
    {
        ProgressToken = opInfo.ProgressToken,
        Start = 0,
        Count = 10,
    });
    if (progressResp.ProgressInfos.Count < 4)
    {
        throw new InvalidOperationException(
            $"GetProgressInfo: expected at least 4 entries, got {progressResp.ProgressInfos.Count}.");
    }

    currentStep = "GetLogEntries";
    var logCount = finalCheck.LogCount;
    var logResp = await client.GetLogEntriesAsync(new GetLogEntriesRequest
    {
        LogToken = finalCheck.LogToken,
        Start = 0,
        Count = logCount,
    });
    if (logResp.LogEntries.Count != logCount)
    {
        throw new InvalidOperationException(
            $"GetLogEntries: expected {logCount} entries, got {logResp.LogEntries.Count}.");
    }

    currentStep = "GetFolderContent";
    var folderResp = await client.GetFolderContentAsync(new GetFolderContentRequest
    {
        Path = fixtureDir,
        TypeRestriction = FileType.File,
    });
    if (folderResp.RequestFailed || folderResp.Files.Count != 4)
    {
        throw new InvalidOperationException(
            $"GetFolderContent: request_failed={folderResp.RequestFailed}, files.Count={folderResp.Files.Count} (expected 4).");
    }

    currentStep = "GetDuplicate";
    var dup = await client.GetDuplicateAsync(new Empty());
    if (string.IsNullOrWhiteSpace(dup.DuplicateId))
        throw new InvalidOperationException("GetDuplicate: duplicate_id empty.");
    if (!PathsEqualNorm(dup.BasePath, fixtureDir))
    {
        throw new InvalidOperationException(
            $"GetDuplicate: base_path '{dup.BasePath}' does not match fixture dir '{fixtureDir}'.");
    }

    if (!DuplicatePairMatchesSideRule(dup))
    {
        throw new InvalidOperationException(
            "GetDuplicate: file1/file2 names must both contain 'left' or both contain 'right'.");
    }

    var pathToDelete = PickCopyDedupPath(dup);
    if (string.IsNullOrEmpty(pathToDelete))
    {
        throw new InvalidOperationException(
            "GetDuplicate: no file path with 'copy_dedup' in the file name.");
    }

    currentStep = "ResolveDuplicate";
    var resolveResp = await client.ResolveDuplicateAsync(new ResolveDuplicateRequest
    {
        DuplicateId = dup.DuplicateId,
        ResolveOperation = ResolveOperation.DeleteFile,
        File = new VideoFile { FilePath = pathToDelete },
    });
    if (!resolveResp.Successful
        || !string.IsNullOrEmpty(resolveResp.ErrorMessage)
        || resolveResp.ResolveOperation != ResolveOperation.DeleteFile)
    {
        throw new InvalidOperationException(
            $"ResolveDuplicate: successful={resolveResp.Successful} error='{resolveResp.ErrorMessage}' op={resolveResp.ResolveOperation}");
    }

    currentStep = "DiscardDuplicates";
    await client.DiscardDuplicatesAsync(new Empty());

    currentStep = "GetDuplicate (after discard)";
    var dup2 = await client.GetDuplicateAsync(new Empty());
    if (!string.IsNullOrEmpty(dup2.DuplicateId) || !string.IsNullOrEmpty(dup2.BasePath))
    {
        throw new InvalidOperationException(
            $"After DiscardDuplicates: expected empty duplicate_id and base_path; got id='{dup2.DuplicateId}' base='{dup2.BasePath}'.");
    }

    Console.WriteLine($"OK: VideoDedupGrpcDedupSmoke — url={url}, fixture_dir={fixtureDir}");
    return 0;
}
catch (RpcException ex)
{
    Console.Error.WriteLine("gRPC failed during {0}: {1}", currentStep, ex.Status);
    return 1;
}
catch (Exception ex)
{
    Console.Error.WriteLine("Dedup smoke failed during {0}: {1}", currentStep, ex.Message);
    return 2;
}

static bool PathsEqualNorm(string a, string b) =>
    string.Equals(a.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
        b.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
        StringComparison.Ordinal);

static bool DuplicatePairMatchesSideRule(DuplicateData dup)
{
    var f1 = dup.File1;
    var f2 = dup.File2;
    if (f1 is null || f2 is null)
        return false;
    var n1 = Path.GetFileName(f1.FilePath);
    var n2 = Path.GetFileName(f2.FilePath);
    bool bothLeft = n1.Contains("left", StringComparison.OrdinalIgnoreCase)
        && n2.Contains("left", StringComparison.OrdinalIgnoreCase);
    bool bothRight = n1.Contains("right", StringComparison.OrdinalIgnoreCase)
        && n2.Contains("right", StringComparison.OrdinalIgnoreCase);
    return bothLeft || bothRight;
}

static string PickCopyDedupPath(DuplicateData dup)
{
    foreach (var p in new[] { dup.File1?.FilePath, dup.File2?.FilePath })
    {
        if (string.IsNullOrEmpty(p))
            continue;
        if (Path.GetFileName(p).Contains("copy_dedup", StringComparison.OrdinalIgnoreCase))
            return p;
    }

    return string.Empty;
}
