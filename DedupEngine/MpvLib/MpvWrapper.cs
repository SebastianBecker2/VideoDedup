namespace DedupEngine.MpvLib
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using VideoDedupShared;

    public class MpvWrapper : IDisposable
    {
        private const string LibPath = @"mpv-1.dll";
        private static readonly TimeSpan GetEventIdTimeout =
            TimeSpan.FromSeconds(10);
        private static readonly int MaxReadRetryCount = 10;

        public static CodecInfo GetCodecInfo(string filePath)
        {
            var mpvHandle = IntPtr.Zero;

            try
            {
                mpvHandle = mpv_create();
                if (mpvHandle == IntPtr.Zero)
                {
                    throw new MpvException($"Unable to create handle");
                }

                PreparePropertyHandle(mpvHandle, filePath);

                while (true)
                {
                    var eventId = GetEventId(mpvHandle, GetEventIdTimeout);

                    if (eventId == EventId.FileLoaded)
                    {
                        try
                        {
                            return ReadCodecInfo(mpvHandle);
                        }
                        catch (ApplicationException)
                        {
                            // Try again.
                            // It seems, sometimes the hight property (or any
                            // property) isn't available when the event comes
                            // in. So we either wait longer, wait for the next
                            // event or just try again.
                            return ReadCodecInfo(mpvHandle);
                        }
                    }

                    if (eventId == EventId.EndFile)
                    {
                        return null;
                    }
                    if (eventId == EventId.Shutdown)
                    {
                        return null;
                    }
                    if (eventId == EventId.None)
                    {
                        return null;
                    }
                }
            }
            catch (MpvException exc)
            {
                throw new AggregateException(
                    $"Error getting codec info from '{filePath}'", exc);
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
            var mpvHandle = IntPtr.Zero;

            try
            {
                mpvHandle = mpv_create();
                if (mpvHandle == IntPtr.Zero)
                {
                    throw new MpvException(
                        $"Unable to create handle");
                }

                PreparePropertyHandle(mpvHandle, filePath);

                while (true)
                {
                    var eventId = GetEventId(mpvHandle, GetEventIdTimeout);

                    if (eventId == EventId.FileLoaded)
                    {
                        return TimeSpan.FromSeconds(
                            GetLong(mpvHandle, "duration"));
                    }

                    if (eventId == EventId.EndFile)
                    {
                        return TimeSpan.Zero;
                    }
                    if (eventId == EventId.Shutdown)
                    {
                        return TimeSpan.Zero;
                    }
                    if (eventId == EventId.None)
                    {
                        return TimeSpan.Zero;
                    }
                }
            }
            catch (MpvException exc)
            {
                throw new AggregateException(
                    $"Error getting duration from '{filePath}'", exc);
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
        public TimeSpan? Duration { get; private set; }

        private bool disposedValue;

        public MpvWrapper(
            string filePath,
            TimeSpan? duration = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be" +
                    $" null or whitespace", nameof(filePath));
            }

            FilePath = filePath;
            Duration = duration;

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

        public IEnumerable<MemoryStream> GetImages(
            int index,
            int count,
            int partition) =>
            GetImages(null, index, count, partition);

        public IEnumerable<MemoryStream> GetImages(
            int index,
            int count,
            int partition,
            CancellationToken cancelToken) =>
            GetImages(cancelToken, index, count, partition);

        private IEnumerable<MemoryStream> GetImages(
            CancellationToken? cancelToken,
            int index,
            int count,
            int partition)
        {
            // We ran into issues with some files accessed over the network
            // (SMB shares). When libmpv (Version 0.33 and 0.33.1) loads the
            // file, it allocates a lot of memory (in this case 1.8 GB). Even
            // though the file size is less then 500 MB. Especially in x86 it
            // would cause an OutOfMemoryException. Additionally, we wouldn't
            // get any of the images. So now we try to handle the
            // OutOfMemoryException and just return a list with null values.
            try
            {
                return GetImages(index, count, partition, cancelToken);
            }
            catch (OutOfMemoryException)
            {
                return Enumerable.Repeat<MemoryStream>(null, count);
            }
        }

        public IEnumerable<MemoryStream> GetImages(
            int index,
            int count,
            int partition,
            CancellationToken? cancelToken)
        {
            if (count <= 0)
            {
                yield break;
            }

            if (index >= partition || index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                    "Index out of range.");
            }

            if (!Duration.HasValue)
            {
                Duration = GetDuration(FilePath);
            }
            var stepping = Math.Max(
                Duration.Value.TotalSeconds / (partition + 1),
                1);

            PrepareImageHandle(stepping * ++index);

            while (true)
            {
                var eventId = GetEventId(MpvHandle, GetEventIdTimeout);

                if (cancelToken.HasValue
                    && cancelToken.Value.IsCancellationRequested)
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
        }

        public IEnumerable<MemoryStream> GetImages(
            IEnumerable<ImageIndex> indices) =>
            GetImages(null, indices);

        public IEnumerable<MemoryStream> GetImages(
            IEnumerable<ImageIndex> indices,
            CancellationToken cancelToken) =>
            GetImages(cancelToken, indices);

        public IEnumerable<MemoryStream> GetImages(
            CancellationToken? cancelToken,
            IEnumerable<ImageIndex> indices)
        {
            // We ran into issues with some files accessed over the network
            // (SMB shares). When libmpv (Version 0.33 and 0.33.1) loads the
            // file, it allocates a lot of memory (in this case 1.8 GB). Even
            // though the file size is less then 500 MB. Especially in x86 it
            // would cause an OutOfMemoryException. Additionally, we wouldn't
            // get any of the images. So now we try to handle the
            // OutOfMemoryException and just return a list with null values.
            try
            {
                return GetImages(indices, cancelToken);
            }
            catch (OutOfMemoryException)
            {
                return Enumerable
                    .Repeat<MemoryStream>(null, indices.Count())
                    .ToList();
            }
        }

        private IEnumerable<MemoryStream> GetImages(
            IEnumerable<ImageIndex> indices,
            CancellationToken? cancelToken)
        {
            if (indices is null)
            {
                throw new ArgumentNullException(nameof(indices));
            }

            var indicesIt = indices.GetEnumerator();
            if (!indicesIt.MoveNext())
            {
                yield break;
            }

            if (!Duration.HasValue)
            {
                Duration = GetDuration(FilePath);
            }
            var seconds = Duration.Value.TotalSeconds;
            double GetPosition(ImageIndex index) =>
                seconds / index.Denominator * index.Numerator;

            PrepareImageHandle(GetPosition(indicesIt.Current));

            while (true)
            {
                var eventId = GetEventId(MpvHandle, GetEventIdTimeout);

                if (cancelToken.HasValue
                    && cancelToken.Value.IsCancellationRequested)
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
        }

        private static void PreparePropertyHandle(
            IntPtr mpvHandle,
            string filePath)
        {
            if (mpv_initialize(mpvHandle) != 0)
            {
                throw new MpvException("Unable to initialize handle");
            }

            Set(mpvHandle, "aid", "no");
            Set(mpvHandle, "sid", "no");
            Set(mpvHandle, "vo", "null");
            Execute(mpvHandle, "loadfile", filePath);
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
                        throw new MpvException("Unable to create handle");
                    }

                    if (mpv_initialize(MpvHandle) != 0)
                    {
                        throw new MpvException("Unable to initialize handle");
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
                throw new AggregateException(
                    $"Error getting images from '{FilePath}'", exc);
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
                    $"Unable to execute command '{string.Join(" ", args)}'");
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
                $"Unable to set property '{name}' to '{value}'");

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

        private static string GetString(IntPtr handle, string name)
        {
            var valuePtr = mpv_get_property_string(handle, GetUtf8Bytes(name));
            if (valuePtr == IntPtr.Zero)
            {
                throw new MpvException($"Unable to get property {name}");
            }
            try
            {
                // Replace with:
                // Marshal.PtrToStringUTF8(lpBuffer);
                // When converting to .NET 5.0
                return Marshal.PtrToStringAnsi(valuePtr);
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
                var @event = (Event)Marshal.PtrToStructure(
                    eventPtr,
                    typeof(Event));
                return (EventId)@event.Id;
            }
        }

        private static CodecInfo ReadCodecInfo(IntPtr mpvHandle) =>
            new CodecInfo()
            {
                Size = new Size(
                    (int)GetLong(mpvHandle, "width"),
                    (int)GetLong(mpvHandle, "height")),
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
                throw new MpvException(
                    message + " result: " + result.ToString());
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

        private static MemoryStream ReadFileWithRetry(
            string filePath,
            int retryCount = 0)
        {
            try
            {
                return new MemoryStream(File.ReadAllBytes(filePath));
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

        private static MemoryStream GetExtractedImage(string outputPath)
        {
            var filePath = Directory.GetFiles(outputPath).FirstOrDefault();
            if (filePath is null)
            {
                return null;
            }
            else
            {
                var file = ReadFileWithRetry(filePath);
                File.Delete(filePath);
                return file;
            }
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
