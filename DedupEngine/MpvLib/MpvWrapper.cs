namespace DedupEngine.MpvLib
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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
                            Debug.Print("Trying again to codec info");
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
        public int PartitionCount { get; private set; }

        private bool disposedValue;

        public MpvWrapper(
            string filePath,
            int partitionCount,
            TimeSpan? duration = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be" +
                    $" null or whitespace", nameof(filePath));
            }

            FilePath = filePath;
            Duration = duration;
            PartitionCount = partitionCount;

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

        public IList<MemoryStream> GetImages(int index, int count) =>
            GetImages(null, index, count);

        public IList<MemoryStream> GetImages(
            int index,
            int count,
            CancellationToken cancelToken) =>
            GetImages(cancelToken, index, count);

        public IList<MemoryStream> GetImages(
            CancellationToken? cancelToken,
            int index,
            int count)
        {
            // We ran into issues with some files accessed over the network
            // (SMB shares). When libmpv (Version 0.33 and 0.33.1) loads the
            // file, it allocates a lot of memory (in this case 1.8 GB). Even
            // though the file size is less then 500 MB. Especially in x86 it
            // would cause an OutOfMemoryException. Additionally, we wouldn't
            // get the any of the images. So now we try to handle the
            // OutOfMemoryException and just return a list with null values.
            try
            {
                var images = GetImages(
                    index,
                    count,
                    SeekMode.KeyFramesOnly,
                    cancelToken).ToList();
                if (images.Count() == count)
                {
                    return images;
                }

                images = GetImages(
                    index,
                    count,
                    SeekMode.Precise,
                    cancelToken).ToList();
                if (images.Count() == count)
                {
                    return images;
                }

                return Enumerable
                    .Range(index, count)
                    .Select(i =>
                        GetImages(i, 1, SeekMode.Precise, cancelToken)
                        .FirstOrDefault())
                    .ToList();
            }
            catch (OutOfMemoryException)
            {
                return Enumerable.Repeat<MemoryStream>(null, count).ToList();
            }
        }

        private IEnumerable<MemoryStream> GetImages(
            int index,
            int count,
            SeekMode seekMode,
            CancellationToken? cancelToken)
        {
            if (count <= 0)
            {
                yield break;
            }

            if (index >= PartitionCount || index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                    "Index out of range.");
            }

            if (!Duration.HasValue)
            {
                Duration = GetDuration(FilePath);
            }

            try
            {
                PrepareImageHandle();

                Set(MpvHandle, "hr-seek",
                    seekMode == SeekMode.Precise ? "yes" : "no");
                var stepping = Math.Max(
                    (long)(Duration.Value.TotalSeconds / (PartitionCount + 1)),
                    1);
                Set(MpvHandle, "sstep",
                    stepping.ToString(CultureInfo.InvariantCulture));
                var start = stepping * (index + 1);
                Set(MpvHandle, "start",
                    start.ToString(CultureInfo.InvariantCulture));
                var end = stepping * (index + 1 + count);
                Set(MpvHandle, "end",
                    end.ToString(CultureInfo.InvariantCulture));

                Execute(MpvHandle, "loadfile", FilePath);
            }
            catch (MpvException exc)
            {
                throw new AggregateException(
                    $"Error getting images from '{FilePath}'", exc);
            }

            var counter = 0;
            while (true)
            {
                var eventId = GetEventId(MpvHandle, GetEventIdTimeout);

                if (cancelToken.HasValue
                    && cancelToken.Value.IsCancellationRequested)
                {
                    yield break;
                }

                if (eventId == EventId.Seek)
                {
                    foreach (var filePath in Directory.GetFiles(OutputPath))
                    {
                        counter++;
                        yield return ReadFileWithRetry(filePath);
                        File.Delete(filePath);
                        if (counter == count)
                        {
                            yield break;
                        }
                    }
                }

                if (eventId == EventId.EndFile)
                {
                    break;
                }
                if (eventId == EventId.Shutdown)
                {
                    break;
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

        private void PrepareImageHandle()
        {
            if (MpvHandle != IntPtr.Zero)
            {
                return;
            }

            MpvHandle = mpv_create();
            if (MpvHandle == IntPtr.Zero)
            {
                throw new MpvException("Unable to create handle");
            }

            if (mpv_initialize(MpvHandle) != 0)
            {
                throw new MpvException("Unable to initialize handle");
            }

            Set(MpvHandle, "aid", "no");
            Set(MpvHandle, "sid", "no");
            Set(MpvHandle, "vo", "image");
            Set(MpvHandle, "vo-image-outdir", OutputPath);
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

        private MemoryStream ReadFileWithRetry(string filePath, int retryCount = 0)
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
