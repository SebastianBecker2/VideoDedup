# VideoDedup gRPC interface

This document describes the public gRPC API exposed by **VideoDedupServer** and consumed by **VideoDedupClient** and the smoke-test clients. The canonical schema is `VideoDedupSharedLib/Protos/video_dedup.proto` (C# namespace `VideoDedupGrpc`, protobuf package `video_dedup_grpc`).

## Scope and assumptions

- **Paths are always on the server machine.** `GetFolderContent`, dedup `base_path`, and video comparison paths refer to filesystem locations visible to the server process, not the client (unless the client happens to run on the same host).
- **Unary RPCs only.** There are no server-streaming methods; long-running work is polled via `GetCurrentStatus`, `GetLogEntries`, `GetProgressInfo`, and `GetVideoComparisonStatus`.
- **TLS:** Production clients typically pin the server certificate. For plaintext HTTP/2 to local servers, clients may need `AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true)` on .NET (see `VideoDedupGrpcSmoke/Program.cs`).
- **Large payloads:** Frame comparison responses can include many JPEG thumbnails. Clients should configure a sufficient `MaxReceiveMessageSize` on the channel (the smoke tests use 128 MiB).

## Service

| Property | Value |
|----------|--------|
| Service (protobuf) | `video_dedup_grpc.VideoDedupGrpcService` |
| Generated C# client | `VideoDedupGrpc.VideoDedupGrpcServiceClient` |

---

## Enums

### `ComparisonResult`

| Name | Value | Meaning |
|------|------:|---------|
| `NO_RESULT` | 0 | No decisive outcome yet (default for unset fields). |
| `DUPLICATE` | 1 | Treated as duplicate / match for the scope where this enum appears. |
| `DIFFERENT` | 2 | Treated as different. |
| `CANCELLED` | 3 | User or client cancelled the operation. |
| `ABORTED` | 4 | Failed or stopped with error context often in `reason` (where applicable). |

Used inside **`VideoComparisonResult`** (overall job) and **`FrameComparisonResult`** (single frame pair).

### `ResolveOperation`

| Name | Value |
|------|------:|
| `DELETE_FILE` | 0 |
| `SKIP` | 1 |
| `DISCARD` | 2 |
| `CANCEL` | 3 |

### `FileType` (folder listing filter)

| Name | Value |
|------|------:|
| `ANY` | 0 |
| `FILE` | 1 |
| `FOLDER` | 2 |

### `DurationComparisonSettings.DurationDifferenceType`

| Name | Value |
|------|------:|
| `DURATION_DIFFERENCE_TYPE_SECONDS` | 0 |
| `DURATION_DIFFERENCE_TYPE_PERCENT` | 1 |

### `LogSettings.LogLevel`

| Name | Value |
|------|------:|
| `VERBOSE` | 0 |
| `DEBUG` | 1 |
| `INFORMATION` | 2 |
| `WARNING` | 3 |
| `ERROR` | 4 |
| `FATAL` | 5 |

### `OperationInfo.OperationType`

| Name | Value |
|------|------:|
| `COMPARING` | 0 |
| `LOADING_MEDIA` | 1 |
| `SEARCHING` | 2 |
| `MONITORING` | 3 |
| `COMPLETED` | 4 |
| `INITIALIZING` | 5 |
| `ERROR` | 6 |
| `CONNECTING` | 7 |
| `PREPARING` | 8 |

### `OperationInfo.ProgressStyle`

| Name | Value |
|------|------:|
| `MARQUEE` | 0 |
| `NO_PROGRESS` | 1 |
| `CONTINUOUS` | 2 |

---

## Message reference

All shapes below match `video_dedup.proto`. **Google well-known types** appear as imported messages: `google.protobuf.Empty`, `Timestamp`, `Duration`, `StringValue`, `BytesValue` (wrapper messages; unset means “no value”).

### `ConfigurationSettings`

Root configuration document for get/set configuration RPCs.

| Field | Type | Description |
|-------|------|-------------|
| `dedup_settings` | `DedupSettings` | Scan roots, filters, and engine behavior. |
| `duration_comparison_settings` | `DurationComparisonSettings` | How much duration may differ between candidates. |
| `video_comparison_settings` | `VideoComparisonSettings` | Frame sampling and difference thresholds for duplicate detection. |
| `log_settings` | `LogSettings` | Serilog-style levels per subsystem. |
| `resolution_settings` | `ResolutionSettings` | Thumbnails, delete vs trash. |

Note: protobuf field numbers jump from `3` to `5` for `log_settings` (there is no field `4` in the schema).

### `DedupSettings`

| Field | Type | Description |
|-------|------|-------------|
| `base_path` | `string` | Root directory the dedup engine scans. |
| `excluded_directories` | `repeated string` | Absolute or relative paths excluded from scanning. |
| `file_extensions` | `repeated string` | Extensions to include (server-defined interpretation). |
| `recursive` | `bool` | Whether to recurse into subdirectories. |
| `monitor_changes` | `bool` | Watch filesystem for changes after initial pass. |
| `concurrency_level` | `int32` | Parallelism hint for the engine. |

### `DurationComparisonSettings`

| Field | Type | Description |
|-------|------|-------------|
| `difference_type` | `DurationDifferenceType` | Compare absolute seconds vs percent. |
| `max_difference` | `int32` | Upper bound for “still duplicate” in the chosen unit. |

### `VideoComparisonSettings`

| Field | Type | Description |
|-------|------|-------------|
| `compare_count` | `int32` | How many frame positions to compare. |
| `max_different_frames` | `int32` | Stop / decide when this many frames differ. |
| `max_difference` | `int32` | Per-frame difference threshold (percentage semantics in the comparer). |

### `LogSettings`

| Field | Type | Description |
|-------|------|-------------|
| `video_dedup_service_log_level` | `LogLevel` | Host / gRPC service logging. |
| `comparison_manager_log_level` | `LogLevel` | Ad-hoc comparison jobs. |
| `dedup_engine_log_level` | `LogLevel` | Dedup engine (also feeds in-memory log lines exposed via `GetLogEntries`). |

### `ResolutionSettings`

| Field | Type | Description |
|-------|------|-------------|
| `thumbnail_count` | `int32` | How many preview frames to prepare per file when resolving duplicates. |
| `move_to_trash` | `bool` | If true, deletes go to `trash_path` structure; otherwise permanent delete. |
| `trash_path` | `string` | Folder for trashed files. The server may overwrite this on `SetConfiguration`. |

### `StatusData`

Returned by `GetCurrentStatus`.

| Field | Type | Description |
|-------|------|-------------|
| `operation_info` | `OperationInfo` | What the engine is doing and how progress is reported. |
| `total_duplicates_count` | `int32` | All duplicates tracked (prepared + unprepared). |
| `log_count` | `int32` | Current number of in-memory dedup log lines. |
| `log_token` | `string` | Opaque token; must be passed to `GetLogEntries`. Changes when the log is reset. |
| `unprepared_duplicates_count` | `int32` | Duplicates still being prepared (e.g. thumbnails). |
| `prepared_duplicates_count` | `int32` | Duplicates ready for `GetDuplicate` / resolve UI. |

### `OperationInfo`

| Field | Type | Description |
|-------|------|-------------|
| `operation_type` | `OperationType` | High-level phase (searching, comparing, monitoring, …). |
| `maximum_files` | `int32` | Scale for continuous progress (e.g. total files to process). |
| `progress_style` | `ProgressStyle` | Whether UI should show marquee, nothing, or a numeric bar. |
| `start_time` | `Timestamp` | When the current operation phase started (UTC in practice). |
| `progress_count` | `int32` | How many `ProgressInfo` snapshots exist for the current `progress_token`. |
| `progress_token` | `string` | Opaque token for `GetProgressInfo`; rotates when progress history resets. |

### `ProgressInfo`

One snapshot in the progress history (batched via `GetProgressInfo`).

| Field | Type | Description |
|-------|------|-------------|
| `file_count` | `int32` | Files processed so far in this phase. |
| `file_count_speed` | `double` | Derived files-per-second. |
| `duplicates_found` | `int32` | Duplicate counter at this snapshot. |
| `duplicates_found_speed` | `double` | Derived duplicates-per-second. |

### `GetLogEntriesRequest` / `GetLogEntriesResponse`

**Request**

| Field | Type | Description |
|-------|------|-------------|
| `log_token` | `string` | Must match `StatusData.log_token`. |
| `start` | `int32` | Zero-based index into the circular log buffer. |
| `count` | `int32` | Number of lines to return. |

**Response**

| Field | Type | Description |
|-------|------|-------------|
| `log_entries` | `repeated string` | Lines in order from `start`. Empty if token or range is invalid. |

### `GetProgressInfoRequest` / `GetProgressInfoResponse`

**Request**

| Field | Type | Description |
|-------|------|-------------|
| `progress_token` | `string` | Must match `OperationInfo.progress_token`. |
| `start` | `int32` | Zero-based index into stored `ProgressInfo` rows. |
| `count` | `int32` | Max rows to return. |

**Response**

| Field | Type | Description |
|-------|------|-------------|
| `progress_infos` | `repeated ProgressInfo` | Slice starting at `start`. Empty if token or range is invalid. |

### `DuplicateData`

Returned by `GetDuplicate`. When there is nothing to show, the server returns a message with **empty** `duplicate_id` (other fields may be unset).

| Field | Type | Description |
|-------|------|-------------|
| `duplicate_id` | `string` | Id for this pair until resolved; empty means “no duplicate available”. |
| `file1` | `VideoFile` | First file of the pair (server paths). |
| `file2` | `VideoFile` | Second file of the pair. |
| `base_path` | `string` | Dedup base path when the duplicate was recorded (client path mapping). |

### `ResolveDuplicateRequest` / `ResolveDuplicateResponse`

**Request**

| Field | Type | Description |
|-------|------|-------------|
| `duplicate_id` | `string` | From `DuplicateData`. |
| `resolve_operation` | `ResolveOperation` | Action to perform. |
| `file` | `VideoFile` | For `DELETE_FILE`, must identify one of the two files (`file_path` match). |

**Response**

| Field | Type | Description |
|-------|------|-------------|
| `successful` | `bool` | `false` if the server caught an exception (e.g. invalid delete). |
| `error_message` | `string` | Set when `successful` is false. |
| `resolve_operation` | `ResolveOperation` | Echo of the request. |

### `VideoComparisonConfiguration`

Input to `StartVideoComparison`.

| Field | Type | Description |
|-------|------|-------------|
| `video_comparison_settings` | `VideoComparisonSettings` | Thresholds for this run. |
| `left_file_path` | `string` | Absolute path on the server. |
| `right_file_path` | `string` | Absolute path on the server. |
| `force_loading_all_frames` | `bool` | Heavier mode for full frame payloads in `FrameSet`. |

### `VideoComparisonStatus`

**Primary envelope** for both `StartVideoComparison` and `GetVideoComparisonStatus`. It carries token, both sides as rich `VideoFile` values, **per-frame** rows, and an optional **aggregate** summary.

| Field | Type | Description |
|-------|------|-------------|
| `comparison_token` | `string` | Guid string for polling and cancel. Empty when the comparison failed to start (see `video_comparison_result`). |
| `left_file` | `VideoFile` | Metadata and thumbnails for the left input. |
| `right_file` | `VideoFile` | Metadata and thumbnails for the right input. |
| `frame_comparisons` | `repeated FrameComparisonResult` | Incremental frame-by-frame results (see polling RPC). |
| `video_comparison_result` | `VideoComparisonResult` | Optional overall outcome and index; see next message. |

### `VideoComparisonResult`

**Aggregate summary only** for the whole comparison job. The proto defines **exactly three** fields—there are no file paths or frame bytes here; those live on `VideoComparisonStatus` and `FrameComparisonResult`.

| Field | Type | Description |
|-------|------|-------------|
| `last_compared_index` | `int32` | Last `FrameComparisonResult.index` reflected in the overall decision (when applicable). |
| `comparison_result` | `ComparisonResult` | Outcome for the job (`NO_RESULT` until finished, then e.g. `DUPLICATE` / `DIFFERENT` / `CANCELLED` / `ABORTED`). |
| `reason` | `string` | Human-readable detail, especially for `ABORTED` or validation failures. |

**Relation to `VideoComparisonStatus`:** Clients should read `VideoComparisonStatus.video_comparison_result` for the rolling/final headline state, and `VideoComparisonStatus.frame_comparisons` for per-frame images, scores, and per-frame `comparison_result`.

### `FrameComparisonResult`

One compared frame pair (may appear many times per job).

| Field | Type | Description |
|-------|------|-------------|
| `index` | `int32` | Monotonic frame slot index for this job. |
| `left_frames` | `FrameSet` | Left side imagery at various resolutions / color spaces. |
| `right_frames` | `FrameSet` | Right side imagery. |
| `difference` | `double` | Numeric difference metric between the compared representations. |
| `load_level` | `int32` | Which quality pass produced this row (multi-resolution pipeline). |
| `comparison_result` | `ComparisonResult` | Result for **this frame** (may differ from the overall job while still in progress). |

### `FrameSet`

Binary image payloads for one side at one timeline position.

| Field | Type | Description |
|-------|------|-------------|
| `index` | `FrameIndex` | Normalized position along the video timeline. |
| `original` | `BytesValue` | Raw decoded frame bytes (optional). |
| `cropped` | `BytesValue` | Cropped variant (optional). |
| `resized` | `BytesValue` | Resized variant (optional). |
| `greyscaled` | `BytesValue` | Greyscale variant (optional). |
| `bytes` | `BytesValue` | Primary payload used for display when set (optional). |

### `FrameIndex`

Rational position: logical time ≈ `numerator / denominator` of the clip duration (see client code that multiplies duration by the quotient).

| Field | Type | Description |
|-------|------|-------------|
| `numerator` | `int32` | Timeline numerator. |
| `denominator` | `int32` | Timeline denominator (non-zero in valid data). |

### `VideoFile`

| Field | Type | Description |
|-------|------|-------------|
| `file_path` | `string` | Full server-side path. |
| `file_size` | `int64` | Length in bytes. |
| `duration` | `Duration` | Media duration. |
| `codec_info` | `CodecInfo` | Resolution, frame rate, codec name. |
| `last_write_time` | `Timestamp` | Last write time (UTC). |
| `frames` | `repeated BytesValue` | Optional preview thumbnails (e.g. JPEG). |
| `creation_time` | `Timestamp` | Creation time when available. |
| `last_access_time` | `Timestamp` | Last access time when available. |

### `CodecInfo`

| Field | Type | Description |
|-------|------|-------------|
| `size` | `Size` | Picture size. |
| `frame_rate` | `float` | Nominal frame rate. |
| `name` | `StringValue` | Codec name when known. |

### `Size`

| Field | Type | Description |
|-------|------|-------------|
| `width` | `int32` | Pixels wide. |
| `height` | `int32` | Pixels high. |

### `VideoComparisonStatusRequest`

| Field | Type | Description |
|-------|------|-------------|
| `comparison_token` | `string` | From `VideoComparisonStatus.comparison_token`. |
| `frame_comparison_index` | `int32` | Server returns `frame_comparisons` with `index` ≥ this value. |

### `CancelVideoComparisonRequest`

| Field | Type | Description |
|-------|------|-------------|
| `comparison_token` | `string` | Token to cancel. |

### `GetFolderContentRequest` / `GetFolderContentResponse`

**Request**

| Field | Type | Description |
|-------|------|-------------|
| `path` | `string` | Directory to list; empty lists roots (`/` or drives). |
| `type_restriction` | `FileType` | Limit to files, folders, or both. |

**Response**

| Field | Type | Description |
|-------|------|-------------|
| `files` | `repeated FileAttributes` | Entries in the directory. |
| `request_failed` | `bool` | `true` on missing path, permission, or IO errors. |

**Nested `FileAttributes`**

| Field | Type | Description |
|-------|------|-------------|
| `name` | `string` | Entry name (not full path). |
| `type` | `FileType` | File vs folder. |
| `size` | `int64` | File size; folders may be `0`. |
| `date_modified` | `Timestamp` | Last write (UTC). |
| `mime_type` | `string` | MIME or display type string. |
| `icon` | `BytesValue` | Optional icon bitmap (e.g. for drives). |

### `GetSystemInfoResponse`

**Top-level**

| Field | Type | Description |
|-------|------|-------------|
| `processor_count` | `int32` | `Environment.ProcessorCount`. |
| `machine_name` | `string` | Host name. |
| `os_version` | `string` | `Environment.OSVersion` string. |
| `os_description` | `string` | `RuntimeInformation.OSDescription`. |
| `process_id` | `int32` | Server process id. |
| `process_path` | `string` | Main module path. |
| `uptime` | `string` | Tick-based uptime string. |
| `username` | `string` | Process user name. |
| `framework_description` | `string` | .NET runtime description. |
| `os_architecture` | `string` | OS architecture. |
| `process_architecture` | `string` | Process architecture. |
| `runtime_identifier` | `string` | RID string. |
| `network_adapters` | `repeated NetworkAdapterInfo` | Per-interface summary. |

**Nested `NetworkAdapterInfo`**

| Field | Type | Description |
|-------|------|-------------|
| `name` | `string` | Interface name. |
| `status` | `string` | Operational status text. |
| `type` | `string` | Interface type text. |
| `mac` | `string` | Physical address string. |
| `ip_addresses` | `repeated string` | Addresses with prefix length, e.g. `192.168.1.2/24`. |

---

## RPC reference

Each RPC lists request/response types only; field details are in **Message reference** unless noted.

### `GetConfiguration`

| Request | Response |
|---------|----------|
| `google.protobuf.Empty` | `ConfigurationSettings` |

Returns persisted configuration. **Use:** Load server settings in an admin UI (client loads with `GetSystemInfo` for the same dialog).

### `SetConfiguration`

| Request | Response |
|---------|----------|
| `ConfigurationSettings` | `google.protobuf.Empty` |

Saves and applies settings. Server overwrites `resolution_settings.trash_path` with `{dedup_settings.base_path}/VideoDedupTrash` and injects trash into excluded directories internally. **Use:** Save settings editor.

### `GetCurrentStatus`

| Request | Response |
|---------|----------|
| `google.protobuf.Empty` | `StatusData` |

**Use:** Timer-driven heartbeat: refresh operation text, duplicate counts, `log_token` / `log_count`, and `operation_info` for progress polling.

### `GetLogEntries`

| Request | Response |
|---------|----------|
| `GetLogEntriesRequest` | `GetLogEntriesResponse` |

Invalid token or range → empty `log_entries` (not necessarily an RPC error). **Use:** Virtualized log grid: fetch visible row range with latest `log_token`. Clear cache when `log_token` changes (engine stop clears logs and rotates token).

### `GetProgressInfo`

| Request | Response |
|---------|----------|
| `GetProgressInfoRequest` | `GetProgressInfoResponse` |

Wrong token or bad indices → empty list. **Use:** After `GetCurrentStatus`, when `progress_count` increases, batch-fetch new `ProgressInfo` rows (client uses 500 per call).

### `GetDuplicate`

| Request | Response |
|---------|----------|
| `google.protobuf.Empty` | `DuplicateData` |

Empty `duplicate_id` means no prepared duplicate. **Use:** Resolution wizard loop.

### `ResolveDuplicate`

| Request | Response |
|---------|----------|
| `ResolveDuplicateRequest` | `ResolveDuplicateResponse` |

Unknown `duplicate_id` → success with no-op. **Use:** One RPC per user action; `DELETE_FILE` requires `file` matching one path.

### `DiscardDuplicates`

| Request | Response |
|---------|----------|
| `google.protobuf.Empty` | `google.protobuf.Empty` |

Clears the duplicate queue without deleting files. **Use:** Bulk dismiss from main UI.

### `StartVideoComparison`

| Request | Response |
|---------|----------|
| `VideoComparisonConfiguration` | `VideoComparisonStatus` |

On invalid paths, response may carry `video_comparison_result` with `ABORTED` and `reason`, and **empty** `comparison_token` (do not poll). **Use:** Start ad-hoc compare, then poll.

### `GetVideoComparisonStatus`

| Request | Response |
|---------|----------|
| `VideoComparisonStatusRequest` | `VideoComparisonStatus` |

Returns `frame_comparisons` from `frame_comparison_index` onward plus current `left_file` / `right_file` / `video_comparison_result`. Unknown token: server implementation may yield null / error—clients should stop polling invalid tokens.

**Use:** Timer poll; bump `frame_comparison_index` after consuming the max `FrameComparisonResult.index`.

### `CancelVideoComparison`

| Request | Response |
|---------|----------|
| `CancelVideoComparisonRequest` | `google.protobuf.Empty` |

**Use:** Dialog close, new comparison, or after enough frames—always cancel to free server state (idle comparisons time out after minutes).

### `GetFolderContent`

| Request | Response |
|---------|----------|
| `GetFolderContentRequest` | `GetFolderContentResponse` |

Check `request_failed`. **Use:** Remote file picker when the server is not localhost.

### `GetSystemInfo`

| Request | Response |
|---------|----------|
| `google.protobuf.Empty` | `GetSystemInfoResponse` |

**Use:** Diagnostics next to configuration.

---

## Typical client flows (VideoDedupClient)

1. **Dashboard:** Poll `GetCurrentStatus`. On `log_token` change, clear cached log lines.
2. **Log:** `GetLogEntries` with `(log_token, start, count)` for visible indices.
3. **Progress:** When `operation_info.progress_count` grows, `GetProgressInfo` with `progress_token`.
4. **Resolve:** `GetDuplicate` → UI → `ResolveDuplicate` → repeat until empty `duplicate_id`. Optional deep compare: `GetConfiguration` → `StartVideoComparison` → poll `GetVideoComparisonStatus` → `CancelVideoComparison`.
5. **Discard all:** `DiscardDuplicates` after confirmation.
6. **Remote browse:** `GetFolderContent` with `path == ""` then per-folder `path`.
7. **Settings:** `GetConfiguration` + `GetSystemInfo` → edit → `SetConfiguration`.

---

## Code map

| Artifact | Location |
|----------|----------|
| Proto | `VideoDedupSharedLib/Protos/video_dedup.proto` |
| Server implementation | `VideoDedupServer/VideoDedupService.cs` |
| Folder listing | `VideoDedupServer/FileManager.cs` |
| Duplicate queue semantics | `DuplicateManager/DuplicateManager.cs` |
| Comparison lifecycle | `ComparisonManager/ComparisonManager.cs` |
| Reference client | `VideoDedupClient/…` (dialogs, `SelectFileDialog`, `StatusInfoCtl`) |
| Smoke tests | `VideoDedupGrpcSmoke/Program.cs` |
