namespace MpvLib
{
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;
    using Exceptions;
    using VideoDedupGrpc;

    public class MpvWrapper : IDisposable
    {
        private const string LibPath = @"mpv-1.dll";
        private static readonly TimeSpan EventIdTimeout =
            TimeSpan.FromSeconds(10);
        private static readonly int MaxReadRetryCount = 10;

        public static CodecInfo GetCodecInfo(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new MpvFileNotFoundException(
                    "Unable to extract images. File not found.",
                    filePath);
            }

            var mpvHandle = PreparePropertyHandle(filePath);

            try
            {
                while (true)
                {
                    var eventId = GetEventId(mpvHandle, EventIdTimeout);

                    if (eventId == EventId.FileLoaded)
                    {
                        try
                        {
                            return ReadCodecInfo(mpvHandle);
                        }
                        catch (ApplicationException)
                        {
                            // Try again.
                            // It seems, sometimes the height property (or any
                            // property) isn't available when the event comes
                            // in. So we either wait longer, wait for the next
                            // event or just try again.
                            return ReadCodecInfo(mpvHandle);
                        }
                    }

                    if (eventId == EventId.EndFile)
                    {
                        throw new MpvOperationException(
                            "Unable to read codec information from video.",
                            filePath);
                    }
                    if (eventId == EventId.Shutdown)
                    {
                        throw new MpvOperationException(
                            "Unable to read codec information from video.",
                            filePath);
                    }
                    if (eventId == EventId.None)
                    {
                        throw new MpvOperationException(
                            "Unable to read codec information from video.",
                            filePath);
                    }
                }
            }
            catch (MpvException exc)
            {
                exc.VideoFilePath = filePath;
                throw;
            }
            finally
            {
                if (mpvHandle != IntPtr.Zero)
                {
                    mpv_terminate_destroy(mpvHandle);
                }
            }
        }

        public static TimeSpan GetDuration(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new MpvFileNotFoundException(
                    "Unable to extract images. File not found.",
                    filePath);
            }

            var mpvHandle = PreparePropertyHandle(filePath);

            try
            {
                while (true)
                {
                    var eventId = GetEventId(mpvHandle, EventIdTimeout);

                    if (eventId == EventId.FileLoaded)
                    {
                        return TimeSpan.FromSeconds(
                            GetLong(mpvHandle, "duration"));
                    }

                    if (eventId == EventId.EndFile)
                    {
                        throw new MpvOperationException(
                            "Unable to read duration from video.",
                            filePath);
                    }
                    if (eventId == EventId.Shutdown)
                    {
                        throw new MpvOperationException(
                            "Unable to read duration from video.",
                            filePath);
                    }
                    if (eventId == EventId.None)
                    {
                        throw new MpvOperationException(
                            "Unable to read duration from video.",
                            filePath);
                    }
                }
            }
            catch (MpvException exc)
            {
                exc.VideoFilePath = filePath;
                throw;
            }
            finally
            {
                if (mpvHandle != IntPtr.Zero)
                {
                    mpv_terminate_destroy(mpvHandle);
                }
            }
        }

        public IntPtr MpvHandle { get; private set; } = IntPtr.Zero;
        public string FilePath { get; private set; }
        public string OutputPath { get; private set; }
        public TimeSpan Duration
        {
            get
            {
                duration ??= GetDuration(FilePath);
                return duration.Value;
            }
            private set => duration = value;
        }
        private TimeSpan? duration;

        private bool disposedValue;

        public MpvWrapper(
            string filePath,
            TimeSpan? duration = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be" +
                    " null or whitespace", nameof(filePath));
            }

            FilePath = filePath;
            this.duration = duration;

            OutputPath = Path.Combine(
                Path.GetTempPath(),
                "VideoDedup",
                Guid.NewGuid().ToString());

            // Workaround bug in Directory.CreateDirectory by adding a slash at
            // the end. It can happen that CreateDirectory returns without
            // exception but still doesn't create the directory. The slash seems
            // to help but is no guarantee. Maybe we need to loop until the
            // folder exists.
            _ = Directory.CreateDirectory(OutputPath + "/");
        }

        public IEnumerable<byte[]?> GetImages(
            int index,
            int count,
            int divisionCount) =>
            CheckForOutOfMemory(
                () => GetImages(null, index, count, divisionCount),
                FilePath);

        public IEnumerable<byte[]?> GetImages(
            int index,
            int count,
            int divisionCount,
            CancellationToken cancelToken) =>
            CheckForOutOfMemory(
                () => GetImages(cancelToken, index, count, divisionCount),
                FilePath);

        private IEnumerable<byte[]?> GetImages(
            CancellationToken? cancelToken,
            int index,
            int count,
            int divisionCount)
        {
            if (!File.Exists(FilePath))
            {
                throw new MpvFileNotFoundException(
                    "Unable to extract images. File not found.",
                    FilePath);
            }

            if (count <= 0)
            {
                yield break;
            }

            if (divisionCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(divisionCount),
                    $"{nameof(divisionCount)} cannot be less than zero.");
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"{nameof(index)} cannot be less than zero.");
            }

            if (index + count > divisionCount)
            {
                throw new ArgumentOutOfRangeException(nameof(count),
                    $"{nameof(index)} and {nameof(count)} must refer to a " +
                    $"location within {nameof(divisionCount)}.");
            }

            if (Duration == TimeSpan.Zero)
            {
                throw new MpvOperationException(
                    "Extracting images failed. Unable to read video length.",
                    FilePath);
            }
            var stepping = Math.Max(
                Duration.TotalSeconds / (divisionCount + 1),
                1);

            PrepareImageHandle(stepping * ++index);

            while (true)
            {
                var eventId = GetEventId(MpvHandle, EventIdTimeout);

                if (cancelToken is { IsCancellationRequested: true })
                {
                    yield break;
                }

                if (eventId == EventId.PlaybackRestart)
                {
                    yield return GetExtractedImage(OutputPath);

                    // Advance the video until successful or end of video.
                    // For every unsuccessful advance, we yield return null.
                    while (true)
                    {
                        if (--count == 0)
                        {
                            yield break;
                        }
                        if (AdvanceTo(MpvHandle, stepping * ++index))
                        {
                            break;
                        }
                        yield return null;
                    }
                }

                if (eventId == EventId.None)
                {
                    break;
                }
            }

            // If we haven't returned an image for every image index we got,
            // then something went terribly wrong.
            if (count != 0)
            {
                throw new MpvException(
                    "Unrecoverable error. Extracting images failed.",
                    FilePath);
            }
        }

        public IEnumerable<byte[]?> GetImages(
            IEnumerable<ImageIndex> indices) =>
            CheckForOutOfMemory(() => GetImages(null, indices), FilePath);

        public IEnumerable<byte[]?> GetImages(
            IEnumerable<ImageIndex> indices,
            CancellationToken cancelToken) =>
            CheckForOutOfMemory(() => GetImages(cancelToken, indices), FilePath);

        private IEnumerable<byte[]?> GetImages(
            CancellationToken? cancelToken,
            IEnumerable<ImageIndex> indices)
        {
            if (!File.Exists(FilePath))
            {
                throw new MpvFileNotFoundException(
                    "Unable to extract images. File not found.",
                    FilePath);
            }

            if (indices is null)
            {
                throw new ArgumentNullException(nameof(indices));
            }

            var indicesIt = indices.GetEnumerator();
            if (!indicesIt.MoveNext())
            {
                indicesIt.Dispose();
                yield break;
            }

            if (Duration == TimeSpan.Zero)
            {
                throw new MpvOperationException(
                    "Extracting images failed. Unable to read video length.",
                    FilePath);
            }
            var seconds = Duration.TotalSeconds;
            double GetPosition(ImageIndex index) =>
                seconds / index.Denominator * index.Numerator;

            PrepareImageHandle(GetPosition(indicesIt.Current));

            while (true)
            {
                var eventId = GetEventId(MpvHandle, EventIdTimeout);

                if (cancelToken is { IsCancellationRequested: true })
                {
                    yield break;
                }

                if (eventId == EventId.PlaybackRestart)
                {
                    yield return GetExtractedImage(OutputPath);

                    // Advance the video until successful or end of video.
                    // For every unsuccessful advance, we yield return null.
                    while (true)
                    {
                        if (!indicesIt.MoveNext())
                        {
                            yield break;
                        }
                        if (AdvanceTo(MpvHandle, GetPosition(indicesIt.Current)))
                        {
                            break;
                        }
                        yield return null;
                    }
                }

                if (eventId == EventId.None)
                {
                    break;
                }
            }

            // If we haven't returned an image for every image index we got,
            // then something went terribly wrong.
            if (indicesIt.MoveNext())
            {
                throw new MpvException(
                    "Unrecoverable error. Extracting images failed.",
                    FilePath);
            }
        }

        private static IntPtr PreparePropertyHandle(
            string filePath)
        {
            var mpvHandle = mpv_create();
            if (mpvHandle == IntPtr.Zero)
            {
                throw new MpvInitializationException(filePath);
            }

            if (mpv_initialize(mpvHandle) != 0)
            {
                throw new MpvInitializationException(filePath);
            }

            try
            {
                Set(mpvHandle, "aid", "no");
                Set(mpvHandle, "sid", "no");
                Set(mpvHandle, "vo", "null");
                Execute(mpvHandle, "loadfile", filePath);
            }
            catch (MpvException exc)
            {
                exc.VideoFilePath = filePath;
                throw;
            }
            return mpvHandle;
        }

        private void PrepareImageHandle(double position)
        {
            try
            {
                if (MpvHandle == IntPtr.Zero)
                {
                    MpvHandle = mpv_create();
                    if (MpvHandle == IntPtr.Zero)
                    {
                        throw new MpvInitializationException();
                    }

                    if (mpv_initialize(MpvHandle) != 0)
                    {
                        throw new MpvInitializationException();
                    }
                }

                Set(MpvHandle, "aid", "no");
                Set(MpvHandle, "sid", "no");
                Set(MpvHandle, "vo", "image");
                Set(MpvHandle, "vo-image-outdir", OutputPath);

                Set(MpvHandle, "pause", "yes");
                Set(MpvHandle, "start", position);

                Execute(MpvHandle, "loadfile", FilePath);
            }
            catch (MpvException exc)
            {
                exc.VideoFilePath = FilePath;
                throw;
            }
        }

        [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr mpv_create();

        [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern void mpv_terminate_destroy(IntPtr handle);

        [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern int mpv_initialize(IntPtr handle);

        [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern void mpv_free(IntPtr data);

        [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr mpv_wait_event(
            IntPtr handle,
            double timeout);

        [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern int mpv_command(IntPtr handle, IntPtr strings);

        [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern int mpv_get_property(
            IntPtr handle,
            byte[] name,
            int format,
            out long data);

        [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr mpv_get_property_string(
            IntPtr handle,
            byte[] name);

        [DllImport(LibPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern int mpv_set_property_string(
            IntPtr handle,
            byte[] name,
            byte[] data);

        private static void Execute(IntPtr handle, params string[] args)
        {
            var mainPtr = AllocateUtf8IntPtrArrayWithSentinel(
                args,
                out var byteArrayPointers);
            try
            {
                Check(mpv_command(handle, mainPtr),
                    $"Unable to execute command '{string.Join(" ", args)}'.");
            }
            finally
            {
                foreach (var ptr in byteArrayPointers)
                {
                    Marshal.FreeHGlobal(ptr);
                }
                Marshal.FreeCoTaskMem(mainPtr);
            }
        }

        private static void Set(
            IntPtr handle,
            string name,
            string value) =>
            Check(mpv_set_property_string(
                    handle,
                    GetUtf8Bytes(name),
                    GetUtf8Bytes(value)),
                $"Unable to set property '{name}' to '{value}'.");

        private static void Set(
            IntPtr handle,
            string name,
            double value) =>
            Set(handle, name, value.ToString(CultureInfo.InvariantCulture));

        private static long GetLong(IntPtr handle, string name)
        {
            Check(mpv_get_property(
                    handle,
                    GetUtf8Bytes(name),
                    (int)DataFormat.Int64,
                    out var value),
                $"Unable to get property {name}");
            return value;
        }

        private static string? GetString(IntPtr handle, string name)
        {
            var valuePtr = mpv_get_property_string(handle, GetUtf8Bytes(name));
            if (valuePtr == IntPtr.Zero)
            {
                throw new MpvOperationException($"Unable to get property {name}.");
            }
            try
            {
                return Marshal.PtrToStringUTF8(valuePtr);
            }
            finally
            {
                if (valuePtr != IntPtr.Zero)
                {
                    mpv_free(valuePtr);
                }
            }
        }

        private static EventId GetEventId(
            IntPtr handle,
            TimeSpan timeout) =>
            GetEventId(handle, timeout.TotalSeconds);

        private static EventId GetEventId(
            IntPtr handle,
            double timeout = Timeout.Infinite)
        {
            while (true)
            {
                var eventPtr = mpv_wait_event(handle, timeout);
                if (eventPtr == IntPtr.Zero)
                {
                    continue;
                }
                var @event = (Event?)Marshal.PtrToStructure(
                    eventPtr,
                    typeof(Event));
                return (EventId?)@event?.Id ?? EventId.None;
            }
        }

        private static CodecInfo ReadCodecInfo(IntPtr mpvHandle) =>
            new()
            {
                Size = new Size
                {
                    Width = (int)GetLong(mpvHandle, "width"),
                    Height = (int)GetLong(mpvHandle, "height"),
                },
                Name = GetString(mpvHandle, "video-codec"),
                FrameRate = GetLong(mpvHandle, "container-fps"),
            };

        private static bool AdvanceTo(IntPtr mpvHandle, double position)
        {
            try
            {
                Set(mpvHandle, "time-pos", position);
            }
            catch (MpvException)
            {
                return false;
            }
            return true;
        }

        private static void Check(int result, string message)
        {
            if (result != 0)
            {
                throw new MpvOperationException($"{message} result: {result}");
            }
        }

        private static T CheckForOutOfMemory<T>(Func<T> func, string filePath)
        {
            // We ran into issues with some files accessed over the network
            // (SMB shares). When libmpv (Version 0.33 and 0.33.1) loads the
            // file, it allocates a lot of memory (in this case 1.8 GB). Even
            // though the file size is less then 500 MB. Especially in x86 it
            // would cause an OutOfMemoryException. Additionally, we wouldn't
            // get any of the images. So now we try to handle the
            // OutOfMemoryException and throw something that reflects the issue
            // properly.
            try
            {
                return func();
            }
            catch (OutOfMemoryException)
            {
                throw new MpvOutOfMemoryException(filePath);
            }
        }

        private static byte[] GetUtf8Bytes(string s) =>
            Encoding.UTF8.GetBytes(s + "\0");

        private static IntPtr AllocateUtf8IntPtrArrayWithSentinel(
            string[] arr,
            out IntPtr[] byteArrayPointers)
        {
            // add extra element for extra null pointer last (sentinel)
            var numberOfStrings = arr.Length + 1;
            byteArrayPointers = new IntPtr[numberOfStrings];
            var rootPointer = Marshal.AllocCoTaskMem(
                IntPtr.Size * numberOfStrings);
            for (var index = 0; index < arr.Length; index++)
            {
                var bytes = GetUtf8Bytes(arr[index]);
                var unmanagedPointer = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, 0, unmanagedPointer, bytes.Length);
                byteArrayPointers[index] = unmanagedPointer;
            }
            Marshal.Copy(byteArrayPointers, 0, rootPointer, numberOfStrings);
            return rootPointer;
        }

        private static byte[] ReadFileWithRetry(
            string filePath,
            int retryCount = 0)
        {
            try
            {
                return File.ReadAllBytes(filePath);
            }
            catch (IOException)
            {
                if (retryCount > MaxReadRetryCount)
                {
                    throw;
                }
                Thread.Sleep(10);
                return ReadFileWithRetry(filePath, retryCount + 1);
            }
        }

        private static byte[]? GetExtractedImage(string outputPath)
        {
            var filePath = Directory.GetFiles(outputPath).FirstOrDefault();
            if (filePath is null)
            {
                return null;
            }

            var file = ReadFileWithRetry(filePath);
            File.Delete(filePath);
            return file;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (MpvHandle != IntPtr.Zero)
                    {
                        mpv_terminate_destroy(MpvHandle);
                    }

                    if (Directory.Exists(OutputPath))
                    {
                        Directory.Delete(OutputPath, true);
                    }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
