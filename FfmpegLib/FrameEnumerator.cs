namespace FfmpegLib
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using FFmpeg.AutoGen;
    using FfmpegLib.Exceptions;

    internal sealed class FrameEnumerator :
        IEnumerable<byte[]?>,
        IEnumerator<byte[]?>
    {
        private readonly Task? task;
        private int currentMoveIndex;
        private readonly string path;
        private readonly ParallelOptions options;
        private readonly List<Tuple<FrameIndex, TaskCompletionSource<byte[]?>>>?
            results;
        private readonly List<FrameIndex> indices;

        public FrameEnumerator(
            string filePath,
            IEnumerable<FrameIndex> indices,
            ParallelOptions parallelOptions)
        {
            if (parallelOptions.MaxDegreeOfParallelism == 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(parallelOptions),
                    "MaxDegreeOfParallelism must be greater than zero.");
            }

            path = filePath;
            options = parallelOptions;
            this.indices = [.. indices];

            if (parallelOptions.MaxDegreeOfParallelism == 1)
            {
                return;
            }

            results = [.. indices.Select(index => Tuple.Create(
                index,
                new TaskCompletionSource<byte[]?>()))];

            task = Task.Run(() =>
            {
                try
                {
                    _ = Parallel.ForEach(this.indices, parallelOptions, index =>
                        results.First(x => x.Item1.Equals(index)).Item2
                            .SetResult(ExtractFrameAtIndex(index)));
                }
                catch (OperationCanceledException) { }
            });
        }

        private unsafe byte[]? ExtractFrameAtIndex(FrameIndex index)
        {
            try
            {
                using var formatContext = AllocateFormatContext(path);
                var stream = formatContext.GetVideoStream(true);
                var codec = GetDecoder(stream);
                using var streamContext = AllocateStreamContext(stream, codec);
                var timestamp =
                    stream->duration / index.Denominator * index.Numerator;

                return ExtractFrameAtTimestamp(
                    formatContext,
                    streamContext,
                    stream->index,
                    timestamp);
            }
            catch (FfmpegOperationException)
            {
                return null;
            }
        }

        public byte[]? Current { get; private set; }

        object IEnumerator.Current => Current!;

        public IEnumerator<byte[]?> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool MoveNext()
        {
            try
            {
                if (options.CancellationToken.IsCancellationRequested)
                {
                    Current = null;
                    return false;
                }

                if (options.MaxDegreeOfParallelism == 1)
                {
                    if (currentMoveIndex >= indices.Count)
                    {
                        Current = null;
                        return false;
                    }

                    Current = ExtractFrameAtIndex(indices[currentMoveIndex]);
                    currentMoveIndex++;
                    return true;
                }

                if (results is null || currentMoveIndex >= results!.Count)
                {
                    Current = null;
                    return false;
                }

                var result = results[currentMoveIndex].Item2;
                result.Task.Wait(options.CancellationToken);
                Current = result.Task.Result;

                currentMoveIndex++;
                return true;
            }
            catch
            {
                Current = null;
                return false;
            }
        }

        public void Reset() =>
            throw new NotSupportedException("Reset is not supported.");

        public void Dispose()
        {
            task?.Wait();
            task?.Dispose();
            //foreach (var thread in tasks)
            //{
            //    thread.Wait();
            //    thread.Dispose();
            //}
        }

        private static unsafe byte[]? ExtractFrameAtTimestamp(
            FormatContext formatContext,
            CodecContext streamContext,
            int streamIndex,
            long timestamp)
        {
            ffmpeg.avcodec_flush_buffers(streamContext.GetPointer());

            if (ffmpeg.av_seek_frame(
                formatContext.GetPointer(),
                streamIndex,
                timestamp,
                ffmpeg.AVSEEK_FLAG_BACKWARD | ffmpeg.AVSEEK_FLAG_FRAME)
                < 0)
            {
                return null;
            }

            using var frame = new DoubleBufferedFrame();
            if (frame.GetPointer() is null)
            {
                throw new FfmpegOperationException(
                    "Unable to extract frames. " +
                    "Unable to allocate frame.");
            }
            using var jpegPacket = new Packet();
            if (jpegPacket.GetPointer() is null)
            {
                throw new FfmpegOperationException(
                    "Unable to extract frames. " +
                    "Unable to allocate packet.");
            }

            GetFrame(
                formatContext,
                streamContext,
                streamIndex,
                frame,
                timestamp);

            var jpegCodec = ffmpeg.avcodec_find_encoder(AVCodecID.AV_CODEC_ID_MJPEG);
            if (jpegCodec == null)
            {
                throw new FfmpegOperationException(
                    "Unable to extract frames. " +
                    "Jpeg codec not found.");
            }

            using var jpegContext = AllocateJpegContext(streamContext);

            if (jpegContext.SendFrame(frame) < 0)
            {
                throw new FfmpegOperationException(
                    "Unable to extract frames. " +
                    "Unable to decode frame.");
            }

            if (jpegPacket.ReceivePacket(jpegContext) < 0)
            {
                throw new FfmpegOperationException(
                    "Unable to extract frames. " +
                    "Unable to decode frame.");
            }

            var jpegData = new byte[jpegPacket.Size];
            Marshal.Copy((IntPtr)jpegPacket.Data, jpegData, 0, jpegPacket.Size);

            return jpegData;
        }

        private static unsafe void GetFrame(
            FormatContext formatContext,
            CodecContext streamContext,
            int streamIndex,
            DoubleBufferedFrame frame,
            long timestamp)
        {
            streamContext.FlushBuffers();

            using var packet = new Packet();
            if (packet.GetPointer() is null)
            {
                throw new FfmpegOperationException(
                    "Unable to extract frames. " +
                    "Unable to allocate packet.");
            }

            while (true)
            {
                var result = packet.ReadFrame(formatContext);
                if (result < 0 && result != ffmpeg.AVERROR_EOF)
                {
                    throw new FfmpegOperationException(
                        "Unable to extract frames. " +
                        "Unable to decode frame.");
                }

                if (packet.StreamIndex == streamIndex)
                {
                    if (streamContext.SendPacket(packet) != 0)
                    {
                        throw new FfmpegOperationException(
                            "Unable to extract frames. " +
                            "Unable to decode frame.");
                    }

                    if (DecodePacket(streamContext, frame, timestamp))
                    {
                        return;
                    }
                }

                if (result == ffmpeg.AVERROR_EOF)
                {
                    frame.UseBufferedFrame();
                    if (frame.HasData)
                    {
                        return;
                    }
                    throw new FfmpegOperationException(
                        "Unable to extract frames. " +
                        "Unable to decode frame.");
                }
            }
        }

        private static unsafe bool DecodePacket(
            CodecContext streamContext,
            DoubleBufferedFrame frame,
            long timestamp)
        {
            while (true)
            {
                var result = frame.ReceiveFrame(streamContext);
                if (result == ffmpeg.AVERROR_EOF)
                {
                    frame.UseBufferedFrame();
                    if (frame.HasData)
                    {
                        return true;
                    }
                    throw new FfmpegOperationException(
                        "Unable to extract frames. " +
                        "Unable to decode frame.");
                }
                if (result == ffmpeg.AVERROR(ffmpeg.EAGAIN))
                {
                    return false;
                }
                if (result < 0)
                {
                    throw new FfmpegOperationException(
                        "Unable to extract frames. " +
                        "Unable to decode frame.");
                }

                if (frame.BestEffortTimestamp < timestamp)
                {
                    continue;
                }

                if (frame.BestEffortTimestamp > timestamp)
                {
                    frame.UseBufferedFrame();
                    if (frame.HasData)
                    {
                        return true;
                    }
                    throw new FfmpegOperationException(
                        "Unable to extract frames. " +
                        "Unable to decode frame.");
                }

                return true;
            }
        }

        private static unsafe AVCodec* GetDecoder(AVStream* stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            var codec = ffmpeg.avcodec_find_decoder(stream->codecpar->codec_id);
            if (codec is null)
            {
                throw new FfmpegOperationException(
                    "Unable to extract frames. " +
                    "Decoder not found.");
            }
            return codec;
        }

        private static unsafe FormatContext AllocateFormatContext(
            string filePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));

            var formatContext = new FormatContext();
            if (formatContext.GetPointer() is null)
            {
                throw new FfmpegOperationException(
                    "Unable to extract frames. " +
                    "Unable to allocate format context.");
            }

            if (formatContext.Open(filePath) != 0)
            {
                throw new FfmpegOperationException(
                    "Unable to extract frames. " +
                    "Unable to open file.");
            }

            if (formatContext.FindStreamInfo() < 0)
            {
                throw new FfmpegOperationException(
                    "Unable to extract frames. " +
                    "Video stream not found.");
            }

            return formatContext;
        }

        private static unsafe CodecContext AllocateStreamContext(
            AVStream* stream,
            AVCodec* codec)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));
            ArgumentNullException.ThrowIfNull(codec, nameof(codec));

            try
            {
                var context = new CodecContext(stream, codec);
                if (context.GetPointer() is null)
                {
                    throw new FfmpegOperationException(
                        "Unable to extract frames. " +
                        "Unable to allocate stream codec context.");
                }

                if (context.ParametersToContext(stream->codecpar) < 0)
                {
                    throw new FfmpegOperationException(
                        "Unable to extract frames. " +
                        "Unable to allocate stream codec context.");
                }

                if (context.Open(codec) < 0)
                {
                    throw new FfmpegOperationException(
                        "Unable to extract frames. " +
                        "Could not stream codec context.");
                }

                return context;

            }
            catch (InvalidOperationException)
            {
                throw new FfmpegOperationException(
                    "Unable to extract frames. " +
                    "Unable to allocate stream codec context.");
            }
        }

        private static unsafe CodecContext AllocateJpegContext(
            CodecContext streamContext)
        {
            var jpegCodec =
                ffmpeg.avcodec_find_encoder(AVCodecID.AV_CODEC_ID_MJPEG);
            if (jpegCodec == null)
            {
                throw new FfmpegOperationException(
                    "Unable to extract frames. " +
                    "Jpeg codec not found.");
            }

            try
            {
                var jpegContext = new CodecContext(jpegCodec)
                {
                    Width = streamContext.Width,
                    Height = streamContext.Height,
                    PixFmt = AVPixelFormat.AV_PIX_FMT_YUVJ420P,
                    GlobalQuality = 12,
                    TimeBase = new AVRational { num = 1, den = 25 },
                    BitRate = 400000
                };
                if (jpegContext.GetPointer() is null)
                {
                    throw new FfmpegOperationException(
                        "Unable to extract frames." +
                        " Unable to allocate jpeg codec context.");
                }

                if (jpegContext.Open(jpegCodec) < 0)
                {
                    throw new FfmpegOperationException(
                        "Unable to extract frames. " +
                        "Could not open jpeg codec context.");
                }

                return jpegContext;
            }
            catch (InvalidOperationException)
            {
                throw new FfmpegOperationException(
                    "Unable to extract frames. " +
                    "Unable to allocate jpeg codec context.");
            }
        }
    }
}
