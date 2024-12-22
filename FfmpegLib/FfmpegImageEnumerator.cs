namespace FfmpegLib
{
    using System.Collections;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using FFmpeg.AutoGen;
    using FfmpegLib.Exceptions;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    public sealed class FfmpegImageEnumerator : IEnumerable<byte[]?>, IEnumerator<byte[]?>
    {
        private static readonly int DecodeRetryCount = 5;
        private readonly string filePath;
        private readonly CancellationToken? cancelToken;
        private readonly int divisionCount;
        private int streamIndex;
        private readonly int index;
        private readonly int count;
        private List<Task<byte[]?>>? tasks;
        private int currentMoveIndex;

        public FfmpegImageEnumerator(
            string filePath,
            CancellationToken? cancelToken,
            int index,
            int count,
            int divisionCount)
        {
            this.filePath = filePath;
            this.cancelToken = cancelToken;
            this.index = index;
            this.count = count;
            this.divisionCount = divisionCount;

            ffmpeg.avformat_network_init();

            InitializeThreads();
        }

        private unsafe void InitializeThreads()
        {
            var formatContext = AllocateFormatContext();
            double stepping = 0;
            try
            {
                var durationInSeconds =
                    formatContext->duration / (double)ffmpeg.AV_TIME_BASE;
                stepping = CalculateStepping(durationInSeconds, divisionCount);

                streamIndex = FindStreamIndex(formatContext);
            }
            finally
            {
                ffmpeg.avformat_close_input(&formatContext);
                ffmpeg.avformat_free_context(formatContext);
            }

            tasks = Enumerable.Range(0, count)
                .Select(i => Task.Run(() => ExtractImageAtIndex(i, stepping)))
                .ToList();
        }

        private unsafe byte[]? ExtractImageAtIndex(int i, double stepping)
        {
            var formatContext = AllocateFormatContext();

            try
            {
                var stream = formatContext->streams[streamIndex];
                var codec = GetDecoder(stream);
                var streamContext = AllocateStreamContext(stream, codec);
                try
                {
                    //var timestamp = stepping * (i + 1);
                    var timestamp = (stepping * index) + (stepping * (i + 1));
                    timestamp = ffmpeg.av_rescale(
                        (long)timestamp,
                        stream->time_base.den,
                        stream->time_base.num);

                    return ExtractImageAtTimestamp(
                        formatContext,
                        streamContext,
                        (long)timestamp);
                }
                finally
                {
                    ffmpeg.avcodec_free_context(&streamContext);
                }
            }
            catch (FfmpegOperationException)
            {
                return null;
            }
            finally
            {
                ffmpeg.avformat_close_input(&formatContext);
                ffmpeg.avformat_free_context(formatContext);
            }
        }

        public byte[]? Current { get; private set; }

        object IEnumerator.Current => Current!;

        public IEnumerator<byte[]?> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool MoveNext()
        {
            if (count <= 0
                || count <= currentMoveIndex
                || cancelToken?.IsCancellationRequested == true)
            {
                Current = null;
                return false;
            }

            try
            {
                Current = tasks![currentMoveIndex].Result;
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
            foreach (var thread in tasks!)
            {
                thread.Wait();
            }
        }

        private static double CalculateStepping(
            double duration,
            int divisionCount) =>
            Math.Max(duration / (divisionCount + 1), 1);

        private unsafe byte[]? ExtractImageAtTimestamp(
            AVFormatContext* formatContext,
            AVCodecContext* streamContext,
            long timestamp)
        {
            ArgumentNullException.ThrowIfNull(formatContext, nameof(formatContext));
            ArgumentNullException.ThrowIfNull(streamContext, nameof(streamContext));

            if (ffmpeg.av_seek_frame(
                formatContext,
                streamIndex,
                timestamp,
                ffmpeg.AVSEEK_FLAG_BACKWARD | ffmpeg.AVSEEK_FLAG_FRAME)
                < 0)
            {
                return null;
            }

            var frame = ffmpeg.av_frame_alloc();
            var packet = ffmpeg.av_packet_alloc();
            var jpegPacket = ffmpeg.av_packet_alloc();

            AVCodecContext* jpegContext = null;
            try
            {
                if (!GetFrame(formatContext, streamContext, packet, frame, timestamp))
                {
                    throw new FfmpegOperationException(
                        "Unable to extract images. Unable to decode video.",
                        filePath);
                }

                var jpegCodec = ffmpeg.avcodec_find_encoder(AVCodecID.AV_CODEC_ID_MJPEG);
                if (jpegCodec == null)
                {
                    throw new FfmpegOperationException("Jpeg codec not found.", filePath);
                }

                jpegContext = AllocateJpegContext(formatContext, streamContext);

                var result = ffmpeg.avcodec_send_frame(jpegContext, frame);
                if (result != 0)
                {
                    var errorBuffer = new byte[128];
                    fixed (byte* errorPtr = errorBuffer)
                    {
                        ffmpeg.av_strerror(result, errorPtr, (ulong)errorBuffer.Length);
                        var errorMessage = System.Text.Encoding.UTF8.GetString(errorBuffer);
                        throw new FfmpegOperationException(
                            "Unable to extract images. Unable to decode video.",
                            filePath);
                    }
                }

                if (ffmpeg.avcodec_receive_packet(jpegContext, jpegPacket) != 0)
                {
                    throw new FfmpegOperationException(
                        "Unable to extract images. Unable to decode video.",
                        filePath);
                }

                var jpegData = new byte[jpegPacket->size];
                Marshal.Copy((IntPtr)jpegPacket->data, jpegData, 0, jpegPacket->size);

                return jpegData;
            }
            finally
            {
                ffmpeg.av_frame_free(&frame);
                ffmpeg.av_packet_unref(packet);
                ffmpeg.av_packet_free(&packet);
                ffmpeg.av_packet_unref(jpegPacket);
                ffmpeg.av_packet_free(&jpegPacket);
                if (jpegContext is not null)
                {
                    ffmpeg.avcodec_free_context(&jpegContext);
                }
            }

        }

        private unsafe bool GetFrame(
            AVFormatContext* formatContext,
            AVCodecContext* streamContext,
            AVPacket* packet,
            AVFrame* frame,
            long timestamp)
        {
            ArgumentNullException.ThrowIfNull(formatContext, nameof(formatContext));
            ArgumentNullException.ThrowIfNull(streamContext, nameof(streamContext));
            ArgumentNullException.ThrowIfNull(packet, nameof(packet));
            ArgumentNullException.ThrowIfNull(frame, nameof(frame));

            ffmpeg.avcodec_flush_buffers(streamContext);

            while (ffmpeg.av_read_frame(formatContext, packet) >= 0)
            {
                if (packet->stream_index != streamIndex)
                {
                    ffmpeg.av_packet_unref(packet);
                    continue;
                }

                foreach (var _ in Enumerable.Range(0, DecodeRetryCount))
                {
                    if (ffmpeg.avcodec_send_packet(streamContext, packet) != 0)
                    {
                        return false;
                    }

                    var result = ffmpeg.avcodec_receive_frame(streamContext, frame);
                    if (result == ffmpeg.AVERROR_EOF)
                    {
                        ffmpeg.av_packet_unref(packet);
                        return false;
                    }
                    if (result == ffmpeg.AVERROR(ffmpeg.EAGAIN))
                    {
                        break;
                    }
                    if (result < 0)
                    {
                        ffmpeg.av_packet_unref(packet);
                        return false;
                    }

                    if (frame->best_effort_timestamp < timestamp)
                    {
                        ffmpeg.av_packet_unref(packet);
                        ffmpeg.av_frame_unref(frame);
                        break;
                    }
                    return true;
                }
            }
            return false;
        }

        private static unsafe int FindStreamIndex(AVFormatContext* formatContext)
        {
            ArgumentNullException.ThrowIfNull(formatContext, nameof(formatContext));

            AVStream* stream = null;
            for (var index = 0;
                index < formatContext->nb_streams;
                index++)
            {
                if (stream is null
                    && formatContext->streams[index]->codecpar->codec_type
                    == AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    return index;
                }
                formatContext->streams[index]->discard =
                    AVDiscard.AVDISCARD_ALL;
            }

            return -1;
        }

        private unsafe AVCodec* GetDecoder(AVStream* stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            var codec = ffmpeg.avcodec_find_decoder(stream->codecpar->codec_id);
            if (codec is null)
            {
                throw new FfmpegOperationException(
                    "Unable to extract images. Video codec not found.",
                    filePath);
            }
            return codec;
        }

        private unsafe AVFormatContext* AllocateFormatContext()
        {
            var formatContext = ffmpeg.avformat_alloc_context();
            if (formatContext is null)
            {
                throw new FfmpegOperationException(
                    "Unable to extract images. Unable to allocate format context.",
                    filePath);
            }

            if (ffmpeg.avformat_open_input(&formatContext, filePath, null, null) != 0)
            {
                ffmpeg.avformat_free_context(formatContext);
                throw new FfmpegOperationException(
                    "Unable to extract images. Unable to open file.",
                    filePath);
            }

            if (ffmpeg.avformat_find_stream_info(formatContext, null) < 0)
            {
                throw new FfmpegOperationException(
                    "Unable to extract images. Video stream not found.",
                    filePath);
            }

            return formatContext;
        }

        private unsafe AVCodecContext* AllocateStreamContext(
            AVStream* stream, AVCodec* codec)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));
            ArgumentNullException.ThrowIfNull(codec, nameof(codec));

            var streamContext = ffmpeg.avcodec_alloc_context3(codec);
            if (streamContext is null)
            {
                throw new FfmpegOperationException(
                    "Unable to extract images." +
                    "Unable to create stream context.",
                    filePath);
            }

            if (ffmpeg.avcodec_parameters_to_context(
                    streamContext,
                    stream->codecpar)
                < 0)
            {
                ffmpeg.avcodec_free_context(&streamContext);
                throw new FfmpegOperationException(
                    "Unable to extract images." +
                    "Unable to allocate video codec context.",
                    filePath);
            }

            if (ffmpeg.avcodec_open2(streamContext, codec, null) < 0)
            {
                ffmpeg.avcodec_free_context(&streamContext);
                throw new FfmpegOperationException(
                    "Unable to extract images. Could not open video codec.",
                    filePath);
            }

            return streamContext;
        }

        private unsafe AVCodecContext* AllocateJpegContext(
            AVFormatContext* formatContext, AVCodecContext* streamContext)
        {
            ArgumentNullException.ThrowIfNull(formatContext, nameof(formatContext));
            ArgumentNullException.ThrowIfNull(streamContext, nameof(streamContext));

            var jpegCodec = ffmpeg.avcodec_find_encoder(AVCodecID.AV_CODEC_ID_MJPEG);
            if (jpegCodec == null)
            {
                throw new FfmpegOperationException("Jpeg codec not found.", filePath);
            }

            var jpegContext = ffmpeg.avcodec_alloc_context3(jpegCodec);
            if (jpegContext == null)
            {
                throw new FfmpegOperationException("Unable to allocate JPEG codec.", filePath);
            }

            // Configure the new context
            jpegContext->width = streamContext->width;
            jpegContext->height = streamContext->height;
            jpegContext->pix_fmt = AVPixelFormat.AV_PIX_FMT_YUVJ420P;
            jpegContext->global_quality = 12;
            jpegContext->time_base = new AVRational { num = 1, den = 25 };
            jpegContext->bit_rate = 400000;

            if (ffmpeg.avcodec_open2(jpegContext, jpegCodec, null) < 0)
            {
                ffmpeg.avcodec_free_context(&jpegContext);
                throw new FfmpegOperationException("Could not open JPEG codec.", filePath);
            }

            return jpegContext;
        }
    }
}
